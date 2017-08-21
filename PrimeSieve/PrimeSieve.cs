using System;
using System.Collections.Generic;

namespace JoshuaaM.PrimeSieve
{
    public static class PrimeSieve
    {
        /// <summary>
        /// Generate all primes up to maxNumber
        /// </summary>
        /// <param name="maxNumber">The max number to check for primes up until</param>
        /// <returns></returns>
        public static int[] GeneratePrimesToMax(int maxNumber)
        {
            if (maxNumber < 2)
            {
                return new int[] { };
            }

            return GeneratePrimesToMaxSingleThreaded(maxNumber);
        }

        private static int[] GenerateInitialPrimes(long maxNumber)
        {
            long sqrtMaxNumber = (long)Math.Sqrt(maxNumber) + 1;
            bool[] isPrime = new bool[sqrtMaxNumber + 1];

            int countNotPrime = 2; //0,1 are not prime
            for (int i = 2; i < isPrime.Length; i++)
            {
                isPrime[i] = true;
            }

            for (int i = 2; i < isPrime.Length; i++)
            {
                if (isPrime[i])
                {
                    for (int j = i * 2; j < isPrime.Length; j += i)
                    {
                        if (isPrime[j] == true)
                        {
                            isPrime[j] = false;
                            countNotPrime += 1;
                        }
                    }
                }
            }

            return ConvertIsPrimeToPrimes(isPrime, countNotPrime, 0);
        }

        private static int[] GeneratePrimesToMaxSingleThreaded(int maxNumber)
        {
            int sqrtMaxNumber = (int)Math.Sqrt(maxNumber) + 1;
            int[] initialPrimes = GenerateInitialPrimes(maxNumber);
            List<int> primes = new List<int>(500000000); //Will be removed at a later date
            primes.AddRange(initialPrimes);

            int setLength = 10000; //Less than size of CPU L1 cache
            
            //Setting setLength to 10,000,000 got the algorithm to about 1/3 the speed of original
            //It just takes a long freaking time in Array resize/copy

            int[] temp;
            for (int i = sqrtMaxNumber + 1; ; i += setLength)
            {
                int upperLimit = ((long)i + (long)setLength < int.MaxValue) ? i + setLength : int.MaxValue;

                if (upperLimit < maxNumber)
                {
                    temp = CheckForPrimes(i, upperLimit, initialPrimes);
                    primes.AddRange(temp);
                    continue;
                }

                if (upperLimit >= maxNumber)
                {
                    if (i < maxNumber)
                    {
                        temp = CheckForPrimes(i, maxNumber, initialPrimes);
                        primes.AddRange(temp);
                    }
                    break;
                }
            }

            return primes.ToArray();
        }

        private static int[] CheckForPrimes(int min, int max, int[] primeList)
        {
            bool[] isPrime = new bool[max - min + 1];
            for (int i = 0; i < isPrime.Length; i++)
            {
                isPrime[i] = true;
            }

            int countNotPrime = 0;

            for (int i = 0; i < primeList.Length; i++)
            {
                int firstOccurance = ((int)Math.Floor((double)min / (double)primeList[i])) * primeList[i];
                for (long j = firstOccurance; j <= max; j += primeList[i])
                {
                    if (j >= min)
                    {
                        if (isPrime[j - min])
                        {
                            isPrime[j - min] = false;
                            countNotPrime += 1;
                        }
                    }
                }
            }

            return ConvertIsPrimeToPrimes(isPrime, countNotPrime, min);
        }

        private static int[] ConvertIsPrimeToPrimes(bool[] isPrime, int countNotPrime, int offset)
        {
            int[] primes = new int[isPrime.Length - countNotPrime];
            int j = 0;

            for (int i = 0; i < isPrime.Length; i++)
            {
                if (isPrime[i])
                {
                    primes[j++] = i + offset;
                }
            }

            return primes;
        }
    }
}
