/*  Programme pour tester le generateur MRG32k3a          */

using System;

public class test2RngStream {
   public static void  Main ()  {
      int  i;
      double sum = 0.0;
      RngStream g1 = new RngStream ("g1");
      RngStream g2 = new RngStream ("g2");
      RngStream g3 = new RngStream ("g3");

      Console.WriteLine ("Initial states of g1, g2, and g3:\n") ;
      g1.WriteState ();   g2.WriteState ();   g3.WriteState ();
      sum = g2.NextDouble () + g3.NextDouble ();
      for (i = 0;  i < 12345; i++)
         g2.NextDouble ();

      g1.AdvanceState (5, 3);   
      Console.WriteLine ("State of g1 after advancing by 2^5 + 3 = 35 steps:") ;
      g1.WriteState ();
      Console.WriteLine ("" + g1.NextDouble ());

      g1.ResetStartStream ();
      for (i = 0;  i < 35; i++)    g1.AdvanceState (0,1);
      Console.WriteLine ("\nState of g1 after reset and advancing 35 times by 1:") ;
      g1.WriteState ();
      Console.WriteLine (g1.NextDouble ());

      g1.ResetStartStream ();
      int sumi = 0;
      for (i = 0;  i < 35; i++)    sumi += g1.Next (1, 10);
      Console.WriteLine ("\nState of g1 after reset and 35 calls to randInt (1, 10):");
      g1.WriteState ();
      Console.WriteLine ("   sum of 35 integers in [1, 10] = " + sumi);
      sum += sumi / 100.0;
      Console.WriteLine ("\nNextDouble (g1) = " + g1.NextDouble ());

      double sum3 = 0.0;
      g1.ResetStartStream ();
      g1.IncreasedPrecis (true);
      sumi = 0;
      for (i = 0;  i < 17; i++)     sumi += g1.Next (1, 10);
      Console.WriteLine ("\nState of g1 after reset, increasedPrecis (true) and 17 calls to Next (1, 10):");
      g1.WriteState ();
      g1.IncreasedPrecis (false);
      g1.Next (1, 10);
      Console.WriteLine ("State of g1 after increasedPrecis (false) and 1 call to Next");
      g1.WriteState ();
      sum3 = sumi / 10.0;

      g1.ResetStartStream ();
      g1.IncreasedPrecis (true);
      for (i = 0;  i < 17; i++)    sum3 += g1.NextDouble ();
      Console.WriteLine ("\nState of g1 after reset, IncreasedPrecis (true) and 17 calls to NextDouble:");
      g1.WriteState ();
      g1.IncreasedPrecis (false);
      g1.NextDouble ();
      Console.WriteLine ("State of g1 after IncreasedPrecis (false) and 1 call to NextDouble");
      g1.WriteState ();
      sum += sum3 / 10.0;

      sum3 = 0.0;
      Console.WriteLine ("\nSum of first 100 output values from stream g3:");
      for (i=1;  i<=100;  i++) {
	 sum3 += g3.NextDouble ();
      }
      Console.WriteLine ("   sum = " + sum3);
      sum += sum3 / 10.0;

      Console.WriteLine ("\n\nReset stream g3 to its initial seed.");
      g3.ResetStartStream ();
      Console.WriteLine ("First 5 output values from stream g3:");
      for (i=1; i<=5; i++)
	 Console.WriteLine (g3.NextDouble ());
      sum += g3.NextDouble ();

      Console.WriteLine ("\nReset stream g3 to the next SubStream, 4 times.");
      for (i=1; i<=4; i++)
	 g3.ResetNextSubstream ();
      Console.WriteLine ("First 5 output values from stream g3, fourth SubStream:\n");
      for (i=1; i<=5; i++)
	 Console.WriteLine (g3.NextDouble ());
      sum += g3.NextDouble ();

      Console.WriteLine ("\nReset stream g2 to the beginning of SubStream.");
      g2.ResetStartSubstream ();
      Console.Write (" Sum of 100000 values from stream g2 with double precision:   ");
      sum3 = 0.0;
      g2.IncreasedPrecis (true);
      for (i=1; i<=100000; i++)
	 sum3 += g2.NextDouble ();
      Console.WriteLine (sum3);
      sum += sum3 / 10000.0;
      g2.IncreasedPrecis (false);

      g3.SetAntithetic (true);
      Console.Write (" Sum of 100000 antithetic output values from stream g3:   ");
      sum3 = 0.0;
      for (i=1; i<=100000; i++)
	 sum3 += g3.NextDouble ();
      Console.WriteLine (sum3);
      sum += sum3 / 10000.0;

      Console.Write ("\nSetPackageSeed to seed = { 1, 1, 1, 1, 1, 1 }");
      uint[] germe = { 1, 1, 1, 1, 1, 1 };
      RngStream.SetPackageSeed (germe);

      Console.WriteLine ("\nDeclare an array of 4 named streams and write their full state\n");
      RngStream[] gar = { new RngStream ("Poisson"), new RngStream ("Laplace"),
                          new RngStream ("Galois"), new RngStream ("Cantor") };
      for  (i = 0; i < 4; i++)
	 gar[i].WriteStateFull ();

      Console.WriteLine ("Jump stream Galois by 2^127 steps backward");
      gar[2].AdvanceState (-127, 0);
      gar[2].WriteState ();
      gar[2].ResetNextSubstream ();

      for  (i = 0; i < 4; i++)
	 sum += gar[i].NextDouble ();

      Console.WriteLine ("--------------------------------------");
      Console.WriteLine ("Final Sum = " + sum);
   }
}
