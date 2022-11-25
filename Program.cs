using System;
using System.Collections.Generic;

namespace jokubui_uzd {
    internal class Program {
        static void Main() {
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
    }
}
