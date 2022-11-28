using System;
using System.Collections.Generic;

namespace jokubui_uzd {
    internal class Program {
        static Complex Zeta;
        static List<int> primes;
        static Random rand;
        static void Main() {
            Zeta = new Complex() { Im = (Math.Sqrt(3) - 1) / 2d, Re = 0d };
            rand = new();
            primes = Primes.GetPrimes(500, 1000);
            for (int i = 0; i < 15; i++) {
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
            int i;
            for (i = 0; i < 2000; i++) {
                int num = 0;
                double d0 = b * b - 3 * c;
                double res = -9 * b * c - 27 * n;
                double d1 = res + 2 * b * b * b;
                double A = d1 * d1 - 4 * d0 * d0 * d0;
                Complex bb = new() { Re = b };
                Complex dd0 = new() { Re = d0 };
                if (A >= 0) {
                    double A1 = Math.Sqrt(A);
                    double B = (d1 - A1) / 2;
                    Complex C = new() { Re = Math.Cbrt(B) };
                    Print(dd0, bb, C, ref num);
                } else {
                    Complex C = Complex.Pow(new(re: d1 / 2d,im:Math.Sqrt(-A) / -2d), 1d/3d);
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
                if (x.Im < 0.001d && x.Im > -0.001d) {
                    if (x.Re - FastFloor(x.Re) < 0.01) num++;
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
    struct Complex {
        public double Re, Im;
        public double SqMag => Re * Re + Im * Im;
        public double Mag => Math.Sqrt(SqMag);
        public Complex(double re = 0d, double im = 0d) {
            Re = re;
            Im = im;
        }
        public static Complex Ln(Complex c) {
            double im = Math.Atan2(c.Im, c.Re);
            return new Complex() {
                Re = Math.Log(c.SqMag) / 2d,
                Im = (im >=0) ? im : im + Math.Tau,
            };
        }
        public static Complex Exp(Complex c) {
            return new Complex() {
                Re = Math.Cos(c.Im) * Math.Exp(c.Re),
                Im = Math.Sin(c.Im) * Math.Exp(c.Re),
            };
        }
        public Complex() { Re = 0d; Im = 0d; }
        public Complex Conjugate => new (Re, -Im );
        public static Complex operator + (Complex left, Complex right) {
            return new Complex { Im = left.Im + right.Im, Re = left.Re + right.Re };
        }
        public static Complex operator - (Complex complex) => new() { Re = -complex.Re, Im = -complex.Im };
        public static Complex operator * (Complex left, Complex right) {
            return new() {
                Re = left.Re * right.Re - left.Im * right.Im,
                Im = left.Im * right.Re + left.Re * right.Im
            };
        }
        public static Complex operator * (Complex left, double right) {
            return new() { Re = left.Re * right, Im = left.Im * right };
        }
        public static Complex operator /(Complex left, double right) => left * (1 / right); 
        public static Complex operator /(Complex left, Complex right) {
            return (left * right.Conjugate) / right.SqMag;
        }
        public static Complex Pow(Complex b, int e) {
            Complex result = new() { Im = 0, Re = 1 };
            for (;e > 0; e >>= 1) {
                if ((e & 1) == 1) result *= b;
                b *= b;
            }
            return result;
        }
        public static Complex Pow(Complex b, double e) {
            return Exp(Ln(b) / e);
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
