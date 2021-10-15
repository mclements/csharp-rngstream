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
      g1.writeState ();   g2.writeState ();   g3.writeState ();
      sum = g2.randU01 () + g3.randU01 ();
      for (i = 0;  i < 12345; i++)
         g2.randU01 ();

      g1.advanceState (5, 3);   
      Console.WriteLine ("State of g1 after advancing by 2^5 + 3 = 35 steps:") ;
      g1.writeState ();
      Console.WriteLine ("" + g1.randU01 ());

      g1.resetStartStream ();
      for (i = 0;  i < 35; i++)    g1.advanceState (0,1);
      Console.WriteLine ("\nState of g1 after reset and advancing 35 times by 1:") ;
      g1.writeState ();
      Console.WriteLine (g1.randU01 ());

      g1.resetStartStream ();
      int sumi = 0;
      for (i = 0;  i < 35; i++)    sumi += g1.randInt (1, 10);
      Console.WriteLine ("\nState of g1 after reset and 35 calls to randInt (1, 10):");
      g1.writeState ();
      Console.WriteLine ("   sum of 35 integers in [1, 10] = " + sumi);
      sum += sumi / 100.0;
      Console.WriteLine ("\nrandU01 (g1) = " + g1.randU01 ());

      double sum3 = 0.0;
      g1.resetStartStream ();
      g1.increasedPrecis (true);
      sumi = 0;
      for (i = 0;  i < 17; i++)     sumi += g1.randInt (1, 10);
      Console.WriteLine ("\nState of g1 after reset, increasedPrecis (true) and 17 calls to randInt (1, 10):");
      g1.writeState ();
      g1.increasedPrecis (false);
      g1.randInt (1, 10);
      Console.WriteLine ("State of g1 after increasedPrecis (false) and 1 call to randInt");
      g1.writeState ();
      sum3 = sumi / 10.0;

      g1.resetStartStream ();
      g1.increasedPrecis (true);
      for (i = 0;  i < 17; i++)    sum3 += g1.randU01 ();
      Console.WriteLine ("\nState of g1 after reset, IncreasedPrecis (true) and 17 calls to RandU01:");
      g1.writeState ();
      g1.increasedPrecis (false);
      g1.randU01 ();
      Console.WriteLine ("State of g1 after IncreasedPrecis (false) and 1 call to RandU01");
      g1.writeState ();
      sum += sum3 / 10.0;

      sum3 = 0.0;
      Console.WriteLine ("\nSum of first 100 output values from stream g3:");
      for (i=1;  i<=100;  i++) {
	 sum3 += g3.randU01 ();
      }
      Console.WriteLine ("   sum = " + sum3);
      sum += sum3 / 10.0;

      Console.WriteLine ("\n\nReset stream g3 to its initial seed.");
      g3.resetStartStream ();
      Console.WriteLine ("First 5 output values from stream g3:");
      for (i=1; i<=5; i++)
	 Console.WriteLine (g3.randU01 ());
      sum += g3.randU01 ();

      Console.WriteLine ("\nReset stream g3 to the next SubStream, 4 times.");
      for (i=1; i<=4; i++)
	 g3.resetNextSubstream ();
      Console.WriteLine ("First 5 output values from stream g3, fourth SubStream:\n");
      for (i=1; i<=5; i++)
	 Console.WriteLine (g3.randU01 ());
      sum += g3.randU01 ();

      Console.WriteLine ("\nReset stream g2 to the beginning of SubStream.");
      g2.resetStartSubstream ();
      Console.Write (" Sum of 100000 values from stream g2 with double precision:   ");
      sum3 = 0.0;
      g2.increasedPrecis (true);
      for (i=1; i<=100000; i++)
	 sum3 += g2.randU01 ();
      Console.WriteLine (sum3);
      sum += sum3 / 10000.0;
      g2.increasedPrecis (false);

      g3.setAntithetic (true);
      Console.Write (" Sum of 100000 antithetic output values from stream g3:   ");
      sum3 = 0.0;
      for (i=1; i<=100000; i++)
	 sum3 += g3.randU01 ();
      Console.WriteLine (sum3);
      sum += sum3 / 10000.0;

      Console.Write ("\nSetPackageSeed to seed = { 1, 1, 1, 1, 1, 1 }");
      uint[] germe = { 1, 1, 1, 1, 1, 1 };
      RngStream.setPackageSeed (germe);

      Console.WriteLine ("\nDeclare an array of 4 named streams and write their full state\n");
      RngStream[] gar = { new RngStream ("Poisson"), new RngStream ("Laplace"),
                          new RngStream ("Galois"), new RngStream ("Cantor") };
      for  (i = 0; i < 4; i++)
	 gar[i].writeStateFull ();

      Console.WriteLine ("Jump stream Galois by 2^127 steps backward");
      gar[2].advanceState (-127, 0);
      gar[2].writeState ();
      gar[2].resetNextSubstream ();

      for  (i = 0; i < 4; i++)
	 sum += gar[i].randU01 ();

      Console.WriteLine ("--------------------------------------");
      Console.WriteLine ("Final Sum = " + sum);
   }
}
