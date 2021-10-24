/*  Program to test the random number streams file:  RngStream.cs   */

using System;

public class test3RngStream {
    
    public static void WriteMatrix(double[,] mat)
    {
	Console.Write("{");
	for (int i=0; i<3; ++i) {
	    Console.Write("{");
	    for (int j=0; j<3; ++j)
		Console.Write(mat[i,j]+(j<2 ? ", " : ""));
	    Console.WriteLine("}"+(i<2 ? ", " : "}"));
	}
    }

   public static void  Main ()
    {
	RngStream g = new RngStream (12345);
	g.WriteState();
	Console.WriteLine (g.NextDouble());

	// double[,] C1 = new double[3,3], C2=new double[3,3];
	// g.CalcMatrix(76,0,C1,C2);
	// WriteMatrix(C1);
	// WriteMatrix(C2);
	// g.CalcMatrix(127,0,C1,C2);
	// WriteMatrix(C1);
	// WriteMatrix(C2);
	// g.CalcMatrix(-76,0,C1,C2);
	// WriteMatrix(C1);
	// WriteMatrix(C2);
	// g.CalcMatrix(-127,0,C1,C2);
	// WriteMatrix(C1);
	// WriteMatrix(C2);
    }
}
