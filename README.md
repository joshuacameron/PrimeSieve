# About PrimeSieve

PrimeSieve is a C# library that generates primes using a Segmented Sieve of Eratosthenes implementation. The library allows you to calculate all primes up to given value. It calculates all the primes below int.MaxValue (~2.4 billion) in only 51.8 seconds (using single core of Intel Core i7-4700HQ 2.4GHz). I initially wrote the code due to requiring the generation of primes to solve problems at <a href="https://projecteuler.net/about">Project Euler</a>. Currently PrimeSieve can only count all primes to int.MaxValue, however increasing this value is noted on the roadmap below.</br>
</br>
When searching for primes up to int.MaxValue, I found that a bool array of size [int.MaxValue] broke .NET’s 2GB object limit. To solve this, I came up with what I later found to be known as the Segmented Sieve. This approach divides the numbers to check for primality into ranges (segments), and then iterates over each of these returning its primes.</br>
</br>

# Example Usage

The library is produced as a DLL file, so simply add the DLL as a reference to your project and follow the code example below.</br>
The DLL is directly available from inside the solution <a href="https://github.com/JoshuaaMichael/PrimeSieve/blob/master/PrimeSieve.dll?raw=true">here</a>

## Code Example
```C#
using System;
using System.Diagnostics;
using JoshuaaM.PrimeGenerator;

namespace PrimeGeneratorExample
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
```

## Output Example

![PrimeSeive Windows screenshot](https://github.com/JoshuaaMichael/PrimeSieve/blob/master/Images/PrimeSieve.png?raw=true)

# Roadmap

<img src="https://github.com/JoshuaaMichael/PrimeSieve/blob/master/Images/Unchecked.png?raw=true" height="20" align="absmiddle"/>Resolve memory issue with use of List to store result in generator</br>
<img src="https://github.com/JoshuaaMichael/PrimeSieve/blob/master/Images/Unchecked.png?raw=true" height="20" align="absmiddle"/>Create some performance benchmarks and graphs</br>
<img src="https://github.com/JoshuaaMichael/PrimeSieve/blob/master/Images/Unchecked.png?raw=true" height="20" align="absmiddle"/>Add ability to read/write primes to a file</br>
<img src="https://github.com/JoshuaaMichael/PrimeSieve/blob/master/Images/Unchecked.png?raw=true" height="20" align="absmiddle"/>Add ability for MaxNumber to be greater than int.MaxValue long (64 bit)</br>
<img src="https://github.com/JoshuaaMichael/PrimeSieve/blob/master/Images/Unchecked.png?raw=true" height="20" align="absmiddle"/>Add ability to ask for a count of primes, rather than a MaxValue</br>
<img src="https://github.com/JoshuaaMichael/PrimeSieve/blob/master/Images/Unchecked.png?raw=true" height="20" align="absmiddle"/>Add CPU multithreading</br>