Initial states of g1, g2, and g3:

The current state of the RngStream g1:
   Cg = { 12345, 12345, 12345, 12345, 12345, 12345 }

The current state of the RngStream g2:
   Cg = { 3692455944, 1366884236, 2968912127, 335948734, 4161675175, 475798818 }

The current state of the RngStream g3:
   Cg = { 1015873554, 1310354410, 2249465273, 994084013, 2912484720, 3876682925 }

State of g1 after advancing by 2^5 + 3 = 35 steps:
The current state of the RngStream g1:
   Cg = { 1040577685, 3747037609, 277208355, 1712706441, 627445683, 2408037141 }

0.0794167361498534

State of g1 after reset and advancing 35 times by 1:
The current state of the RngStream g1:
   Cg = { 1040577685, 3747037609, 277208355, 1712706441, 627445683, 2408037141 }

0.0794167361498534

State of g1 after reset and 35 calls to randInt (1, 10):
The current state of the RngStream g1:
   Cg = { 1040577685, 3747037609, 277208355, 1712706441, 627445683, 2408037141 }

   sum of 35 integers in [1, 10] = 186

NextDouble (g1) = 0.0794167361498534

State of g1 after reset, increasedPrecis (true) and 17 calls to Next (1, 10):
The current state of the RngStream g1:
   Cg = { 2633078466, 1040577685, 3747037609, 3152585150, 1712706441, 627445683 }

State of g1 after increasedPrecis (false) and 1 call to Next
The current state of the RngStream g1:
   Cg = { 1040577685, 3747037609, 277208355, 1712706441, 627445683, 2408037141 }


State of g1 after reset, IncreasedPrecis (true) and 17 calls to NextDouble:
The current state of the RngStream g1:
   Cg = { 2633078466, 1040577685, 3747037609, 3152585150, 1712706441, 627445683 }

State of g1 after IncreasedPrecis (false) and 1 call to NextDouble
The current state of the RngStream g1:
   Cg = { 1040577685, 3747037609, 277208355, 1712706441, 627445683, 2408037141 }


Sum of first 100 output values from stream g3:
   sum = 53.9859015923151


Reset stream g3 to its initial seed.
First 5 output values from stream g3:
0.728509786196527
0.965587282283733
0.996184130480117
0.114988416181316
0.973145419129694

Reset stream g3 to the next SubStream, 4 times.
First 5 output values from stream g3, fourth SubStream:

0.173454539635811
0.032863516089416
0.264316912968158
0.305940360211673
0.547630303517707

Reset stream g2 to the beginning of SubStream.
 Sum of 100000 values from stream g2 with double precision:   50098.2241495946
 Sum of 100000 antithetic output values from stream g3:   50017.2231774978

SetPackageSeed to seed = { 1, 1, 1, 1, 1, 1 }
Declare an array of 4 named streams and write their full state

The RngStream Poisson:
   anti = false
   prec53 = false
   Ig = { 1, 1, 1, 1, 1, 1 }
   Bg = { 1, 1, 1, 1, 1, 1 }
   Cg = { 1, 1, 1, 1, 1, 1 }

The RngStream Laplace:
   anti = false
   prec53 = false
   Ig = { 2662865579, 741857976, 4206142246, 3352832365, 2519202871, 655500294 }
   Bg = { 2662865579, 741857976, 4206142246, 3352832365, 2519202871, 655500294 }
   Cg = { 2662865579, 741857976, 4206142246, 3352832365, 2519202871, 655500294 }

The RngStream Galois:
   anti = false
   prec53 = false
   Ig = { 3784663252, 802042081, 160569404, 3391851556, 2150317468, 54240022 }
   Bg = { 3784663252, 802042081, 160569404, 3391851556, 2150317468, 54240022 }
   Cg = { 3784663252, 802042081, 160569404, 3391851556, 2150317468, 54240022 }

The RngStream Cantor:
   anti = false
   prec53 = false
   Ig = { 3276123839, 3170955788, 1470482105, 884064067, 1017894425, 16401881 }
   Bg = { 3276123839, 3170955788, 1470482105, 884064067, 1017894425, 16401881 }
   Cg = { 3276123839, 3170955788, 1470482105, 884064067, 1017894425, 16401881 }

Jump stream Galois by 2^127 steps backward
The current state of the RngStream Galois:
   Cg = { 2662865579, 741857976, 4206142246, 3352832365, 2519202871, 655500294 }

--------------------------------------
Final Sum = 23.7053238628939
