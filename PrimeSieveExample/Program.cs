using System;
using System.Diagnostics;
using JoshuaaM.PrimeSieve;

namespace PrimeSieveExample
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "PrimeSieve";
            Stopwatch sw = new Stopwatch();
            Console.WriteLine("Starting to search for primes");
            sw.Start();

            int[] primes = PrimeSieve.GeneratePrimesToMax(int.MaxValue);

            sw.Stop();
            Console.WriteLine("Completed");
            Console.WriteLine("Time: {0}", sw.Elapsed);
            Console.WriteLine("Primes found: {0}", primes.Length);
            Console.ReadLine();
        }
    }
}
