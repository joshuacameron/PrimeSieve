using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace JoshuaaM.PrimeSieve
{
    public static class PrimeSieve
    {
        static SemaphoreSlim semForGeneration = new SemaphoreSlim(1);
        static SemaphoreSlim semForWork;

        public static int[] GeneratePrimesToMax(int maxNumber, bool multiThreaded = false)
        {
            if (maxNumber < 2)
            {
                return new int[] { };
            }

            if(multiThreaded)
            {
                semForGeneration.Wait(); //Only one generation of primes can occur at once, otherwise pass semForWork to GeneratePrimesThreadWork
                return GeneratePrimesToMaxMultiThreaded(maxNumber);
            }
            else
            {
                return GeneratePrimesToMaxSingleThreaded(maxNumber);
            }
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
            List<int> primes = new List<int>(); //Will be removed at a later date
            primes.AddRange(initialPrimes);

            int setLength = 10000; //Less than size of CPU L1 cache
            
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

        private static int[] GeneratePrimesToMaxMultiThreaded(int maxNumber)
        {
            int sqrtMaxNumber = (int)Math.Sqrt(maxNumber) + 1;
            int[] initialPrimes = GenerateInitialPrimes(maxNumber);
            List<int> primes = new List<int>();
            primes.AddRange(initialPrimes);

            List<int> threadLowerLimits = new List<int>(); //Used to ordering results of thread work
            List<Tuple<int, int[]>> threadWorkResult = new List<Tuple<int, int[]>>(); //Each thread going to return it's lower limit, and primes it found
            semForWork = new SemaphoreSlim(Environment.ProcessorCount); //Only going to make one thread per logical processor
            List<Task> tasks = new List<Task>();

            int setLength = 10000;

            for (int i = sqrtMaxNumber + 1; ; i += setLength)
            {
                semForWork.Wait(); //Waits for a processor to be available

                int lowerLimit = i;
                int upperLimit = ((long)lowerLimit + (long)setLength < int.MaxValue) ? lowerLimit + setLength : int.MaxValue;

                Task task = Task.Factory.StartNew(() => GeneratePrimesThreadWork(lowerLimit, (upperLimit < maxNumber) ? upperLimit : maxNumber, initialPrimes, threadWorkResult));
                tasks.Add(task);

                if (upperLimit >= maxNumber)
                {
                    break;
                }
            }

            Task.WaitAll(tasks.ToArray());

            //Assemble the result
            threadWorkResult.Sort((a, b) => a.Item1.CompareTo(b.Item1)); //Only order on 
            foreach (Tuple<int, int[]> result in threadWorkResult)
            {
                primes.AddRange(result.Item2);
            }

            semForGeneration.Release();
            return primes.ToArray();
        }

        private static void GeneratePrimesThreadWork(int lowerLimit, int upperLimit, int[] primeList, List<Tuple<int, int[]>> results)
        {
            int[] primeResults = CheckForPrimes(lowerLimit, upperLimit, primeList);

            Tuple<int, int[]> result = new Tuple<int, int[]>(lowerLimit, primeResults);

            lock (results)
            {
                results.Add(result);
                semForWork.Release();
            }
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
