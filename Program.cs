using System;
using System.Collections.Generic;
using System.Numerics;

namespace jokubui_uzd {
    internal class Program {
        static Complex Zeta;
        static List<int> primes;
        static Random rand;
        static void Main() {
            Zeta = new( (Math.Sqrt(3) - 1) / 2d, 0d);
            rand = new();
            primes = Primes.GetPrimes(500, 1000);
            for (int i = 0; i < 1; i++) {
                DoCalc();
            }
            Console.ReadKey();
        }
        static void DoCalc() {
            long n = 1L;
            int p1 = ChooseRandom(), p2 = ChooseRandom(), p3 = ChooseRandom();
            n *= p1;
            n *= p2;
            n *= p3;
            double ratio = Math.Cbrt(n);
            long b = (long)Math.Round(-3 * ratio);
            long c = (long)Math.Round(3 * ratio * ratio);
            b -= (IsOdd(b) ? 1 : 0);
            c += (IsOdd(c) ? 1 : 0);
            double coef = 2d;
            Console.WriteLine($"{n} = {p1} * {p2} * {p3}");
            Console.WriteLine($"guess is {ratio}");
            Console.WriteLine($"b:{b}; c:{c}");
            int i;
            for (i = 0; i < 2000; i++) {
                int num = 0;
                double d0 = b * b - 3 * c;
                double res = -9 * b * c - 27 * n;
                double d1 = res + 2 * b * b * b;
                double A = d1 * d1 - 4 * d0 * d0 * d0;
                Complex bb = new(b, 0d);
                Complex dd0 = new(d0,0d );
                if (A >= 0) {
                    double A1 = Math.Sqrt(A);
                    double B = (d1 - A1) / 2;
                    Complex C = new( Math.Cbrt(B), 0d );
                    Print(dd0, bb, C, ref num);
                } else {
                    Complex C = Complex.Pow(new(d1 / 2d,Math.Sqrt(-A) / -2d), 1d/3d);
                    Print(dd0, bb, C, ref num);
                }
                double rat_now = Math.Abs(c / (double)b);
                if (Math.Abs(rat_now - ratio) <= coef) {
                    c += 2;
                } else {
                    c = 3 * (long)Math.Round(ratio * ratio);
                    if (IsOdd(c)) c++;
                    b -= 2;
                    rat_now = Math.Abs(c / (double)b);
                    while(Math.Abs(rat_now - ratio) > coef) {
                        c += 2;
                        rat_now = Math.Abs(c / (double)b);
                    }
                }
                Console.WriteLine($"b:{b}; c:{c}");
                if (num == 3) break;
            }
            if (i < 2000) {
                Console.WriteLine($"/{i}/, b: {b}, c: {c}");
            } else {
                Console.WriteLine("insufficient iterations");
            }
        }
        static void Print(Complex dd0, Complex bb, Complex C, ref int num) {
            for (int i = 0; i <= 2; i++) {
                Complex zi = Complex.Pow(Zeta, i);
                Complex x = (bb + zi * C + dd0 / (zi * C)) / -3;
                if (x.Imaginary < 0.001d && x.Imaginary > -0.001d) {
                    if (x.Real - FastFloor(x.Real) < 0.01) num++;
                }
            }
        }
        static bool IsOdd(long n) => (n & 1L) == 1L;
        static long FastFloor(double t) {
            long n = (long)t;
            return n - (t < 0 ? 1 : 0);
        }
        static int ChooseRandom() {
            return primes[rand.Next(primes.Count)];
        }
        private static void Func1() {
            List<int> primes = Primes.GetPrimes(2023);
            int sum = 0;
            foreach (int prime in primes) {
                int f = Function(2023, prime);
                if (f == 0) continue;
                Console.WriteLine($"F(2023/{prime}) == {f}");
                sum += 1;
            }
            Console.WriteLine($"The sum is {sum}");
            Console.ReadKey();
        }

        static int Function(int num, int den) {
            int b = num % den;
            if (b == 0) return num / den;
            if ((b & 1) == 1) return 1;
            return 0;
        }
    }
    static class Primes {
        static readonly List<int> knownPrimes = new();
        static int biggestNum = 1;
        static void GeneratePrimes(int upTo) {
            int check = biggestNum + 1;
            while (check <= upTo) {
                bool indivisible = true;
                for (int i = 0; i < knownPrimes.Count; i++) {
                    if (knownPrimes[i] * knownPrimes[i] > check) break;
                    if (check % knownPrimes[i] == 0) {
                        indivisible = false;
                        break;
                    }
                }
                if (indivisible) knownPrimes.Add(check);
                check++;
            }
            biggestNum = upTo;
        }
        public static List<int> GetPrimes(int upTo) {
            if (upTo > biggestNum) {
                GeneratePrimes(upTo);
            }
            int i;
            for (i = 0; i < knownPrimes.Count && knownPrimes[i] <= upTo; i++) {
            }

            return knownPrimes.GetRange(0, i);
        }
        public static List<int> GetPrimes(int from, int upTo) {
            if (upTo > biggestNum) GeneratePrimes(upTo);
            int i, j;
            for (i = 0; i < knownPrimes.Count && knownPrimes[i] <= from; i++) { }
            for (j = 0; j + i < knownPrimes.Count && knownPrimes[j + i] <= upTo; j++) { }
            return knownPrimes.GetRange(i - 1, j + 1);
        }
    }
}
