/*  Program to test the random number streams file:  RngStream.cs   */

using System;

public class testRngStream {
   public static void  Main ()  {

   double sum;
   int  i;
   RngStream g1 = new RngStream ("g1");
   RngStream g2 = new RngStream ("g2");
   RngStream g3 = new RngStream ("g3");

   sum = g2.NextDouble () + g3.NextDouble ();

   g1.AdvanceState (5, 3);   
   sum += g1.NextDouble ();

   g1.ResetStartStream ();
   for (i = 0;  i < 35; i++)    g1.AdvanceState (0,1);
   sum += g1.NextDouble ();

   g1.ResetStartStream ();
   long sumi = 0;
   for (i = 0;  i < 35; i++)    sumi += g1.Next (1, 10);
   sum += sumi / 100.0;

   double sum3 = 0.0;
   for (i = 0;  i < 100;  i++) {
      sum3 += g3.NextDouble ();
   }
   sum += sum3 / 10.0;

   g3.ResetStartStream ();
   for (i=1; i<=5; i++)
      sum += g3.NextDouble ();

   for (i=0; i<4; i++)
      g3.ResetNextSubstream ();
   for (i=0; i<5; i++)
      sum += g3.NextDouble ();

   g3.ResetStartSubstream ();
   for (i=0; i<5; i++)
      sum += g3.NextDouble ();

   g2.ResetNextSubstream ();
   sum3 = 0.0;
   for (i=1; i<=100000; i++)
      sum3 += g2.NextDouble ();
   sum += sum3 / 10000.0;

   g3.SetAntithetic (true);
   sum3 = 0.0;
   for (i=1; i<=100000; i++)
      sum3 += g3.NextDouble ();
   sum += sum3 / 10000.0;

   uint[] germe = { 1, 1, 1, 1, 1, 1 };
   RngStream.SetPackageSeed (germe);

   RngStream [] gar = { new RngStream ("Poisson"), new RngStream ("Laplace"),
                      new RngStream ("Galois"),  new RngStream ("Cantor") };
   for  (i = 0; i < 4; i++)
      sum += gar[i].NextDouble ();

   gar[2].AdvanceState (-127, 0);
   sum += gar[2].NextDouble ();

   gar[2].IncreasedPrecis (true);
   gar[2].ResetNextSubstream ();
   sum3 = 0.0;
   for  (i = 0; i < 100000; i++)
      sum3 += gar[2].NextDouble ();
   sum += sum3 / 10000.0;

   gar[2].SetAntithetic (true);
   sum3 = 0.0;
   for  (i = 0; i < 100000; i++)
      sum3 += gar[2].NextDouble ();
   sum += sum3 / 10000.0;

   gar[2].SetAntithetic (false);
   gar[2].IncreasedPrecis (false);

   for  (i = 0; i < 4; i++)
      sum += gar[i].NextDouble ();

   Console.WriteLine ("-------------------------------------------");
   Console.WriteLine ("This program should print   39.6975474452511");
   Console.WriteLine ("Actual program result =     "+ sum);
   }
}
