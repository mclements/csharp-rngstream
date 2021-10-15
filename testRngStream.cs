/*  Program to test the random number streams file:  RngStream.cs   */

using System;

public class testRngStream {
   public static void  Main ()  {

   double sum;
   int  i;
   RngStream g1 = new RngStream ("g1");
   RngStream g2 = new RngStream ("g2");
   RngStream g3 = new RngStream ("g3");

   sum = g2.randU01 () + g3.randU01 ();

   g1.advanceState (5, 3);   
   sum += g1.randU01 ();

   g1.resetStartStream ();
   for (i = 0;  i < 35; i++)    g1.advanceState (0,1);
   sum += g1.randU01 ();

   g1.resetStartStream ();
   long sumi = 0;
   for (i = 0;  i < 35; i++)    sumi += g1.randInt (1, 10);
   sum += sumi / 100.0;

   double sum3 = 0.0;
   for (i = 0;  i < 100;  i++) {
      sum3 += g3.randU01 ();
   }
   sum += sum3 / 10.0;

   g3.resetStartStream ();
   for (i=1; i<=5; i++)
      sum += g3.randU01 ();

   for (i=0; i<4; i++)
      g3.resetNextSubstream ();
   for (i=0; i<5; i++)
      sum += g3.randU01 ();

   g3.resetStartSubstream ();
   for (i=0; i<5; i++)
      sum += g3.randU01 ();

   g2.resetNextSubstream ();
   sum3 = 0.0;
   for (i=1; i<=100000; i++)
      sum3 += g2.randU01 ();
   sum += sum3 / 10000.0;

   g3.setAntithetic (true);
   sum3 = 0.0;
   for (i=1; i<=100000; i++)
      sum3 += g3.randU01 ();
   sum += sum3 / 10000.0;

   uint[] germe = { 1, 1, 1, 1, 1, 1 };
   RngStream.setPackageSeed (germe);

   RngStream [] gar = { new RngStream ("Poisson"), new RngStream ("Laplace"),
                      new RngStream ("Galois"),  new RngStream ("Cantor") };
   for  (i = 0; i < 4; i++)
      sum += gar[i].randU01 ();

   gar[2].advanceState (-127, 0);
   sum += gar[2].randU01 ();

   gar[2].increasedPrecis (true);
   gar[2].resetNextSubstream ();
   sum3 = 0.0;
   for  (i = 0; i < 100000; i++)
      sum3 += gar[2].randU01 ();
   sum += sum3 / 10000.0;

   gar[2].setAntithetic (true);
   sum3 = 0.0;
   for  (i = 0; i < 100000; i++)
      sum3 += gar[2].randU01 ();
   sum += sum3 / 10000.0;

   gar[2].setAntithetic (false);
   gar[2].increasedPrecis (false);

   for  (i = 0; i < 4; i++)
      sum += gar[i].randU01 ();

   Console.WriteLine ("-------------------------------------------");
   Console.WriteLine ("This program should print   39.6975474452511");
   Console.WriteLine ("Actual program result =     "+ sum);
   }
}
