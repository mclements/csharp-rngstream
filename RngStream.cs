 
/**
 * Title:          RngStream.cs
 * Description:    Multiple Streams and Substreams of Random Numbers
 * Copyright:      Pierre L'Ecuyer, University of Montreal
 * Notice:         Apache 2.0 license
 * Version         1.1 (Java version)
 * Date:           26 July 2011 (Java version)

 * If you use this software for work leading to publications, 
 * please cite the following relevant articles in which MRG32k3a 
 * and the package with multiple streams were proposed:

 * P. L'Ecuyer, ``Good Parameter Sets for Combined Multiple Recursive Random Number Generators'', 
 * Operations Research, 47, 1 (1999), 159--164.

 * P. L'Ecuyer, R. Simard, E. J. Chen, and W. D. Kelton, 
 * ``An Objected-Oriented Random-Number Package with Many Long Streams and Substreams'', 
 * Operations Research, 50, 6 (2002), 1073--1075
 *

 Adapted for C# by Mark Clements
 Date: 2021-10-25
*/

using System;

public class RngStream : Random
{
    // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
    // Private constants.

    private const double norm   = 2.328306549295727688e-10;
    private const double m1     = 4294967087.0;
    private const double m2     = 4294944443.0;
    private const double a12    =  1403580.0;
    private const double a13n   =   810728.0;
    private const double a21    =   527612.0;
    private const double a23n   =   1370589.0;
    private const double two17    =  131072.0;
    private const double two53    =  9007199254740992.0;
    private const double invtwo24 = 5.9604644775390625e-8;

    private static double[,] InvA1p0 = {   // Inverse of A1p0
	{ 184888585.0, 0.0, 1945170933.0 },
	{         1.0, 0.0,          0.0 },
	{         0.0, 1.0,          0.0 }
    };
    private static double[,] InvA2p0 = {   // Inverse of A2p0
	{ 0.0, 360363334.0, 4225571728.0 },
	{ 1.0,         0.0,          0.0 },
	{ 0.0,         1.0,          0.0 }
    };
    private static double[,] A1p0  =  {
	{       0.0,       1.0,      0.0 },
	{       0.0,       0.0,      1.0 },
	{ -810728.0, 1403580.0,      0.0 }
    };
    private static double[,] A2p0  =  {
	{        0.0,   1.0,         0.0 },
	{        0.0,   0.0,         1.0 },
	{ -1370589.0,   0.0,    527612.0 }
    };
    private static double[,] A1p76 = {
	{      82758667.0, 1871391091.0, 4127413238.0 },
	{    3672831523.0,   69195019.0, 1871391091.0 },
	{    3672091415.0, 3528743235.0,   69195019.0 }
    };
    private static double[,] A2p76 = {
	{    1511326704.0, 3759209742.0, 1610795712.0 },
	{    4292754251.0, 1511326704.0, 3889917532.0 },
	{    3859662829.0, 4292754251.0, 3708466080.0 }
    };
    private static double[,] A1p127 = {
	{    2427906178.0, 3580155704.0,  949770784.0 },
	{     226153695.0, 1230515664.0, 3580155704.0 },
	{    1988835001.0,  986791581.0, 1230515664.0 }
    };
    private static double[,] A2p127 = {
	{    1464411153.0,  277697599.0, 1610723613.0 },
	{      32183930.0, 1464411153.0, 1022607788.0 },
	{    2824425944.0,   32183930.0, 2093834863.0 }
    };
    // new 2D arrays
    private static double[,] InvA1p76 = {
	{   2585822061.0,   2346541846.0,    600781890.0},
	{     42385315.0,   4257896290.0,   2346541846.0},
	{   1248824805.0,   2390631828.0,   4257896290.0}
    };
    private static double[,] InvA2p76 = {
	{    855407695.0,   4134906251.0,    112088500.0},
	{   2897599610.0,    855407695.0,   1987588141.0},
	{    854109890.0,   2897599610.0,   1099731892.0}
    };
    private static double[,] InvA1p127 = {
	{   1737602145.0,   4223429791.0,   3623540388.0},
	{    296220492.0,   2329649265.0,   4223429791.0},
	{   2348335727.0,   4013827785.0,   2329649265.0}
    };
    private static double[,] InvA2p127 = {
	{   3244453311.0,    924928292.0,     95182267.0},
	{   3169310862.0,   3244453311.0,   3740757140.0},
	{   2096686917.0,   3169310862.0,   2895877872.0}
    };

    // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
    // Private variables (fields) for each stream.

    /// <summary>
    ///   Default seed of the package and seed for the next stream to be created.
    /// </summary>
    private static double[] nextSeed = {12345, 12345, 12345, 12345, 12345, 12345};

    /// <summary>
    /// The arrays {\tt Cg}, {\tt Bg}, and {\tt Ig} contain the current state, 
    /// the starting point of the current substream,
    /// and the starting point of the stream, respectively.
    /// </summary>
    private double[] Cg = new double[6];
    private double[] Bg = new double[6];
    private double[] Ig = new double[6];

    /// <summary>
    /// This stream generates antithetic variates if 
    /// and only if {\tt anti = true}.
    /// </summary>
    private bool anti;
    
    /// <summary>
    /// The precision of the output numbers is ``increased'' (see
    /// {\tt increasedPrecis}) if and only if {\tt prec53 = true}.
    /// </summary>
    private bool prec53;

    /// <summary>
    ///   Describes the stream (for writing the state, error messages, etc.).
    /// </summary> 
    private string descriptor;

    // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
    // Private methods

    /// <summary>
    /// Compute (a*s + c) MOD m ; m must be < 2^35
    /// Works also for s, c < 0.                  
    /// </summary>
    private static double multModM 
	(double a, double s, double c, double m)
    {
        double v;
        int a1;
        v = a * s + c;
        if (v >= two53 || v <= -two53 )
	{
	    a1 = (int)(a / two17);  a -= a1 * two17;
	    v  = a1 * s;
	    a1 = (int)(v / m);    v -= a1 * m;
	    v  = v * two17 + a * s + c;
        }
        a1 = (int)(v / m);
        if ((v -= a1 * m) < 0.0) return v += m; else return v;
    }

    /// <summary>
    ///   Returns v = A*s MOD m.  Assumes that -m < s[i] < m.
    ///   Works even if v = s.
    /// </summary>
    private static void matVecModM (double[,] A, double[] s, 
                                    double[] v, double m)
    {
	int i;
	double[] x = new double[3];
	for (i = 0; i < 3;  ++i)
	{
	    x[i] = multModM (A[i,0], s[0], 0.0, m);
	    x[i] = multModM (A[i,1], s[1], x[i], m);
	    x[i] = multModM (A[i,2], s[2], x[i], m);
	}
	for (i = 0; i < 3;  ++i)  v[i] = x[i];
    }

    /// <summary>
    /// Returns C = A*B MOD m
    /// Note: work even if A = C or B = C or A = B = C.
    /// </summary>
    private static void matMatModM (double[,] A, double[,] B, 
                                    double[,] C, double m)
    {
	int i, j;
	double[] V = new double[3];
	double[,] W = new double[3,3];
	for (i = 0; i < 3;  ++i)
	{
	    for (j = 0; j < 3;  ++j)  V[j] = B[j,i];
	    matVecModM (A, V, V, m);
	    for (j = 0; j < 3;  ++j)  W[j,i] = V[j];
	}
	for (i = 0; i < 3;  ++i) {
	    for (j = 0; j < 3;  ++j)
		C[i,j] = W[i,j];
	}
    }

    /// <summary>
    ///   Compute matrix B = (A^(2^e) Mod m);  works even if A = B
    /// </summary> 
    private static void matTwoPowModM (double[,] A, double[,] B, 
                                       double m, int e)
    {
	int i, j;
	/* initialize: B = A */
	if (A != B)
	{
	    for (i = 0; i < 3; i++)
	    {
		for (j = 0; j < 3;  ++j)  B[i,j] = A[i,j];
	    }
	}
	/* Compute B = A^{2^e} */
	for (i = 0; i < e; i++) matMatModM (B, B, B, m);
    }

    /// <summary>
    ///   Compute matrix D = A^c Mod m ;  works even if A = B
    /// </summary> 
    private static void matPowModM (double[,] A, double[,] B, 
                                    double m, int c)
    {
	int i, j;
	int n = c;
	double[,] W = new double[3,3];

	/* initialize: W = A; B = I */
	for (i = 0; i < 3; i++)
	{
	    for (j = 0; j < 3;  ++j)
	    {
		W[i,j] = A[i,j];
		B[i,j] = 0.0;
	    }
	}
	for (j = 0; j < 3;  ++j)   B[j,j] = 1.0;

	/* Compute B = A^c mod m using the binary decomp. of c */
	while (n > 0)
	{
	    if ((n % 2)==1) matMatModM (W, B, B, m);
	    matMatModM (W, W, W, m);
	    n /= 2;
	}
    } 

    /// <summary>
    ///   Generate a uniform random number, with 32 bits of resolution.
    /// </summary>
    private double U01 ()
    {
        int k;
        double p1, p2, u;
        /* Component 1 */
        p1 = a12 * Cg[1] - a13n * Cg[0];
        k = (int)(p1 / m1);
        p1 -= k * m1;
        if (p1 < 0.0) p1 += m1;
        Cg[0] = Cg[1];   Cg[1] = Cg[2];   Cg[2] = p1;
        /* Component 2 */
        p2 = a21 * Cg[5] - a23n * Cg[3];
        k  = (int)(p2 / m2);
        p2 -= k * m2;
        if (p2 < 0.0) p2 += m2;
        Cg[3] = Cg[4];   Cg[4] = Cg[5];   Cg[5] = p2;
        /* Combination */
        u = ((p1 > p2) ? (p1 - p2) * norm : (p1 - p2 + m1) * norm);
        return (anti) ? (1 - u) : u;
    }

    /// <summary>
    ///   Generate a uniform random number, with 52 bits of resolution.
    /// </summary> 
    private double U01d ()
    {
        double u = U01();
        if (anti)
	{
            // Antithetic case: note that U01 already returns 1-u.
            u += (U01() - 1.0) * invtwo24;
            return (u < 0.0) ? u + 1.0 : u;
        } else
	{
            u += U01() * invtwo24;
            return (u < 1.0) ? u : (u - 1.0);
        }
    }

    /// <summary>
    ///   Default constructor. This uses the package seed for the stream's seeds and
    ///   then advances the package seed.
    /// </summary>
    public RngStream ()
    {
	descriptor = "";
	anti = false;
	prec53 = false;
	for (int i = 0; i < 6; ++i)  
	    Bg[i] = Cg[i] = Ig[i] = nextSeed[i];
	GenAdvance (0, 1, A1p127, A2p127, InvA1p127, InvA2p127, nextSeed, nextSeed);
    } 

    /// <summary>
    ///   Constructor which also specifies the stream name
    /// </summary>
    /// <param name="name">The name of the stream</param>
    public RngStream (string name) : this()
    {
	descriptor = name;
    } 

    /// <summary>
    /// Initialise the seed as per R's set.seed() with RNGkind("L'Ecuyer-CMRG")
    /// </summary>
    /// <exception cref="ArgumentException">If seed is less than zero</exception>
    /// <param name="inseed">The seed as an integer</param>
    public RngStream (Int32 inseed) : this()
    {
	if (inseed < 0)
	    throw new ArgumentException("seed must be greater than or equal to 0");
	uint seed = (uint) inseed;
	for(int j = 0; j < 50; j++)
	    seed = (69069 * seed + 1);
	for (int i = 0; i < 6;  ++i) {
	    seed = 69069*seed+1;
	    while (seed >= m2) seed = 69069*seed+1;
	    nextSeed[i] = Cg[i] = Bg[i] = Ig[i] = (double) seed;
	}
	GenAdvance (0, 1, A1p127, A2p127, InvA1p127, InvA2p127, nextSeed, nextSeed);
    }

    /// <summary>
    ///   Set the package seed
    /// </summary>
    /// <param name="seed">The seed as an array</param>
    public static void SetPackageSeed (uint[] seed)
    {
        CheckSeed(seed);
	for (int i = 0; i < 6;  ++i)  nextSeed[i] = seed[i];
    } 

    /// <summary>
    ///   Reset the seeds to the start of the stream
    /// </summary>
    public void ResetStartStream ()
    {
	for (int i = 0; i < 6;  ++i)  Cg[i] = Bg[i] = Ig[i];
    }  

    /// <summary>
    ///   Reset the current seed to the start of the sub-stream
    /// </summary>
    public void ResetStartSubstream ()
    {
	for (int i = 0; i < 6;  ++i)  Cg[i] = Bg[i];
    }  

    /// <summary>
    ///   Reset the current seed to the start of the sub-stream
    /// </summary>
    public void ResetNextSubstream ()
    {
	int i;
	matVecModM (A1p76, Bg, Bg, m1);
	double[] temp = new double[3];
	for (i = 0; i < 3; ++i) temp[i] = Bg[i + 3];
	matVecModM (A2p76, temp, temp, m2);
	for (i = 0; i < 3; ++i) Bg[i + 3] = temp[i];
	for (i = 0; i < 6;  ++i) Cg[i] = Bg[i];
    }  

    /// <summary>
    ///   Reset the seeds to the next stream
    /// </summary> 
    public void ResetNextStream ()
    {
	int i;
	matVecModM (A1p127, Ig, Ig, m1);
	double[] temp = new double[3];
	for (i = 0; i < 3; ++i) temp[i] = Ig[i + 3];
	matVecModM (A2p127, temp, temp, m2);
	for (i = 0; i < 3; ++i) Ig[i + 3] = temp[i];
	for (i = 0; i < 6;  ++i) Cg[i] = Ig[i];
	for (i = 0; i < 6;  ++i) Bg[i] = Ig[i];
    }  
    
    /// <summary>
    ///   Set the anti variable for antithetic sampling.
    /// </summary>
    /// <param name="a">Value for anti</param>
    public void SetAntithetic (bool a)
    {
	anti = a;
    } 

    /// <summary>
    ///   Set the pec53 variable for increased precision.
    /// </summary> 
    /// <param name="incp">Value for prec53</param>
    public void IncreasedPrecis (bool incp)
    {
	prec53 = incp;
    } 

    /// <summary>
    /// Calculate the matrices for the an artitrary number of jumps from single jump
    /// transition matrices.
    /// if e > 0, let n = 2^e + c;
    /// if e < 0, let n = -2^(-e) + c;
    /// if e = 0, let n = c.
    /// Jump n steps forward if n > 0, backwards if n < 0.
    /// </summary>
    /// <param name="e">Calculate for 2^e</param>
    /// <param name="c">Calculate for c</param>
    /// <param name="C1">Output first 3x3 matrix</param>
    /// <param name="C2">Output second 3x3 matrix</param>
    public void CalcMatrix (int e, int c, double[,] C1, double[,] C2)
    {
	double[,] B1 = new double[3,3], B2 = new double[3,3];
	if (e > 0)
	{
	    matTwoPowModM (A1p0, B1, m1, e);
	    matTwoPowModM (A2p0, B2, m2, e);
	}
	else if (e < 0)
	{
	    matTwoPowModM (InvA1p0, B1, m1, -e);
	    matTwoPowModM (InvA2p0, B2, m2, -e);
	}
	if (c >= 0)
	{
	    matPowModM (A1p0, C1, m1, c);
	    matPowModM (A2p0, C2, m2, c);
	}
	else
	{
	    matPowModM (InvA1p0, C1, m1, -c);
	    matPowModM (InvA2p0, C2, m2, -c);
	}
	if (e != 0)
	{
	    matMatModM (B1, C1, C1, m1);
	    matMatModM (B2, C2, C2, m2);
	}
    }

    /// <summary>
    /// Update an output seed based on an artitrary number of jumps from specified
    /// transition matrices.
    /// if e > 0, let n = 2^e + c;
    /// if e < 0, let n = -2^(-e) + c;
    /// if e = 0, let n = c.
    /// Jump n steps forward if n > 0, backwards if n < 0.
    /// </summary>
    /// <param name="e">Calculate for 2^e</param>
    /// <param name="c">Calculate for c</param>
    /// <param name="A1">First positive transition matrix</param>
    /// <param name="A2">Second positive transition matrix</param>
    /// <param name="InvA1">First negative transition matrix</param>
    /// <param name="InvA2">Second negative transition matrix</param>
    /// <param name="CgIn">Input seed array</param>
    /// <param name="CgOut">Output seed array</param>
    public void GenAdvance (int e, int c,
			    double[,] A1, double[,] A2,
			    double[,] InvA1, double[,] InvA2,
			    double[] CgIn, double[] CgOut)
    {
	double[,]
	    B1 = new double[3,3], B2 = new double[3,3],
	    C1 = new double[3,3], C2 = new double[3,3];
	for (int i = 0; i < 6; ++i)  CgOut[i] = CgIn[i];
	if (e > 0)
	{
	    matTwoPowModM (A1, B1, m1, e);
	    matTwoPowModM (A2, B2, m2, e);
	}
	else if (e < 0)
	{
	    matTwoPowModM (InvA1, B1, m1, -e);
	    matTwoPowModM (InvA2, B2, m2, -e);
	}
	if (c >= 0)
	{
	    matPowModM (A1, C1, m1, c);
	    matPowModM (A2, C2, m2, c);
	}
	else
	{
	    matPowModM (InvA1, C1, m1, -c);
	    matPowModM (InvA2, C2, m2, -c);
	}
	if (e != 0)
	{
	    matMatModM (B1, C1, C1, m1);
	    matMatModM (B2, C2, C2, m2);
	}
	matVecModM (C1, CgOut, CgOut, m1);
	double[] cg3 = new double[3];
	for (int i = 0; i < 3; ++i)  cg3[i] = CgOut[i+3];
	matVecModM (C2, cg3, cg3, m2);
	for (int i = 0; i < 3; ++i)  CgOut[i+3] = cg3[i];
    }

    /// <summary>
    ///   Advance the current seed using GenAdvance().
    /// </summary> 
    /// <param name="e">Calculate for 2^e</param>
    /// <param name="c">Calculate for c</param>
    /// <param name="A1">First positive transition matrix</param>
    /// <param name="A2">Second positive transition matrix</param>
    /// <param name="InvA1">First negative transition matrix</param>
    /// <param name="InvA2">Second negative transition matrix</param>
    public void GenAdvanceState (int e, int c,
				 double[,] A1, double[,] A2,
				 double[,] InvA1, double[,] InvA2)
    {
	GenAdvance(e, c, A1, A2, InvA1, InvA2, Cg, Cg);
    }

    /// <summary>
    ///   Advance the current seed using single step transition matrices.
    /// </summary>
    /// <param name="e">Calculate for 2^e</param>
    /// <param name="c">Calculate for c</param>
    public void AdvanceState (int e, int c)
    {
	GenAdvanceState(e, c, A1p0, A2p0, InvA1p0, InvA2p0);
    } 

    /// <summary>
    ///   Advance the current seed using sub-stream steps
    /// </summary>
    /// <param name="e">Calculate for 2^e</param>
    /// <param name="c">Calculate for c</param>
    public void AdvanceSubstream (int e, int c)
    {
	GenAdvanceState(e, c, A1p76, A2p76, InvA1p76, InvA2p76);
	for (int i = 0; i < 6; ++i)
	    Bg[i] = Cg[i];
    }

    /// <summary>
    ///   Advance the current seed using stream steps.
    /// </summary>
    /// <param name="e">Calculate for 2^e</param>
    /// <param name="c">Calculate for c</param>
    public void AdvanceStream (int e, int c)
    {
	GenAdvanceState(e, c, A1p127, A2p127, InvA1p127, InvA2p127);
	for (int i = 0; i < 6; ++i)
	    Ig[i] = Bg[i] = Cg[i];
    }

    /// <summary>
    ///   Check a seed value.
    /// </summary>
    /// <param name="seed">Input seed as an array</param>
    /// <exception cref="ArgumentException">If seed is invalid</exception>
    private static void CheckSeed (uint[] seed)
    {
	/* Check that the seeds are legitimate values */
	int i;
	for (i = 0; i < 3; ++i)
	{
	    if (seed[i] >= m1 || seed[i] < 0)
		throw new ArgumentException("Seed[" + i + "] not valid");
	}
	for (i = 3; i < 6; ++i)
	{
	    if (seed[i] >= m2 || seed[i] < 0)
		throw new ArgumentException("Seed[" + i + "] not valid");
	}
	if (seed[0] == 0 && seed[1] == 0 && seed[2] == 0)
	    throw new ArgumentException("First 3 seeds = 0.");
	if (seed[3] == 0 && seed[4] == 0 && seed[5] == 0)
	    throw new ArgumentException("Last 3 seeds = 0.");
    }

    /// <summary>
    ///   Set all of the seeds.
    /// <param name="seed">Seed as an array</param>
    /// </summary>
    public void SetSeed (uint[] seed)
    {
        CheckSeed (seed);
        for (int i = 0; i < 6;  ++i)
	    Cg[i] = Bg[i] = Ig[i] = seed[i];
    } 

    /// <summary>
    ///   Return the current state.
    /// </summary>
    public double[] GetState()
    {
	return Cg;
    } 

    /// <summary>
    ///   Write a brief description to the console.
    /// </summary>
    public void WriteState ()
    {
	Console.Write ("The current state of the RngStream");
	if (descriptor != null && descriptor != "")
	    Console.Write (" " + descriptor);
	Console.Write (":\n   Cg = { ");
	for (int i = 0; i < 5; i++)
	    Console.Write ((uint) Cg[i] + ", ");
	Console.WriteLine ((uint) Cg[5] + " }\n");
    }  

    /// <summary>
    ///   Write a full description to the console.
    /// </summary>
    public void WriteStateFull ()
    {
	Console.Write ("The RngStream");
	if (descriptor != null && descriptor != "")
	    Console.Write (" " + descriptor);
	Console.WriteLine (":\n   anti = " + (anti ? "true" : "false"));
	Console.WriteLine ("   prec53 = " + (prec53 ? "true" : "false"));
	Console.Write ("   Ig = { ");
	for (int i = 0; i < 5; i++)
	    Console.Write ((uint) Ig[i] + ", ");
	Console.WriteLine ((uint) Ig[5] + " }");
	Console.Write ("   Bg = { ");
	for (int i = 0; i < 5; i++)
	    Console.Write ((uint) Bg[i] + ", ");
	Console.WriteLine ((uint) Bg[5] + " }");
	Console.Write ("   Cg = { ");
	for (int i = 0; i < 5; i++)
	    Console.Write ((uint) Cg[i] + ", ");
	Console.WriteLine ((uint) Cg[5] + " }\n");
    } 

    /// <summary>
    ///   Override the Sample method.
    /// </summary>
    protected override double Sample ()
    {
	if (prec53) return U01d();
	else return U01();
    } 

    /// <summary>
    ///   Override the Next() method
    /// </summary>
    public override int Next()
    {
	return (int)(Sample() / norm);
    }

    /// <summary>
    ///   Override the Next(int,int) method
    /// </summary>
    /// <param name="min">Lower integer value</param>
    /// <param name="max">Upper integer value</param>
    public override int Next(int min, int max)
    {
	if (min > max)
	    throw new ArgumentException("min must be less than or equal to max");
	return min + (int)(Sample() * (max - min + 1));
    }

    /// <summary>
    ///   Override the NextBytes(byte[]) method
    /// </summary>
    /// <param name="buffer">Output array</param>
    public override void NextBytes(byte[] buffer)
    {
	for (int i = 0, length = buffer.Length; i < length; i++)
	    buffer[i] = (byte)Next();
    }
    
}
