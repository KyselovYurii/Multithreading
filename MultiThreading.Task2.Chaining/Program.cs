/*
 * 2.	Write a program, which creates a chain of four Tasks.
 * First Task – creates an array of 10 random integer.
 * Second Task – multiplies this array with another random integer.
 * Third Task – sorts this array by ascending.
 * Fourth Task – calculates the average value. All this tasks should print the values to console.
 */
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MultiThreading.Task2.Chaining
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(".Net Mentoring Program. MultiThreading V1 ");
            Console.WriteLine("2.	Write a program, which creates a chain of four Tasks.");
            Console.WriteLine("First Task – creates an array of 10 random integer.");
            Console.WriteLine("Second Task – multiplies this array with another random integer.");
            Console.WriteLine("Third Task – sorts this array by ascending.");
            Console.WriteLine("Fourth Task – calculates the average value. All this tasks should print the values to console");
            Console.WriteLine();

            var firstTask = Task.Run(() =>
            {
                var array = new int[10];
                var random = new Random();
                ForAll(array, (v) => random.Next(0, 100));

                Console.WriteLine($"First Task - [{string.Join(", ", array)}]");
                return array;
            });
            firstTask.Wait();

            var secondTask = Task.Run(() =>
            {
                var array = firstTask.Result;
                var rndInt = new Random().Next(0, 100);
                ForAll(array, (v) => v * rndInt);

                Console.WriteLine($"Second Task (mul by {rndInt}) – [{string.Join(", ", array)}]");
                return array;
            });
            secondTask.Wait();

            var thirdTask = Task.Run(() =>
            {
                var array = secondTask.Result;
                Array.Sort(array);

                Console.WriteLine($"Third Task – [{string.Join(", ", array)}]");
                return array;
            });
            thirdTask.Wait();

            var fourthTask = Task.Run(() =>
            {
                var array = thirdTask.Result;
                var avg = array.Average();

                Console.WriteLine($"Fourth Task – {avg}");
            });
            fourthTask.Wait();

            Console.ReadLine();
        }

        private static void ForAll(int[] array, Func<int, int> func)
        {
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = func(array[i]);
            }
        }
    }
}
