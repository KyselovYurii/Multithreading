/*
 * 4.	Write a program which recursively creates 10 threads.
 * Each thread should be with the same body and receive a state with integer number, decrement it,
 * print and pass as a state into the newly created thread.
 * Use Thread class for this task and Join for waiting threads.
 * 
 * Implement all of the following options:
 * - a) Use Thread class for this task and Join for waiting threads.
 * - b) ThreadPool class for this task and Semaphore for waiting threads.
 */

using System;
using System.Threading;

namespace MultiThreading.Task4.Threads.Join
{
    class Program
    {
        private static Semaphore semaphore = new Semaphore(0, 1);

        static void Main(string[] args)
        {
            Console.WriteLine("4.	Write a program which recursively creates 10 threads.");
            Console.WriteLine("Each thread should be with the same body and receive a state with integer number, decrement it, print and pass as a state into the newly created thread.");
            Console.WriteLine("Implement all of the following options:");
            Console.WriteLine();
            Console.WriteLine("- a) Use Thread class for this task and Join for waiting threads.");
            Console.WriteLine("- b) ThreadPool class for this task and Semaphore for waiting threads.");

            Console.WriteLine();

            // option a
            var thread = new Thread(UseThreadClass);
            thread.Start(10);
            thread.Join();
            
            // option b
            ThreadPool.QueueUserWorkItem(UseThreadPoolClass, 10);
            semaphore.WaitOne();
            semaphore.Release();

            Console.ReadLine();
        }

        private static void UseThreadClass(object state)
        {
            var count = (int)state;
            if(count > 0)
            {
                count--;
                Console.WriteLine("Thread " + count);
                var thread = new Thread(UseThreadClass);
                thread.Start(count);
                thread.Join();
            }
        }

        private static void UseThreadPoolClass(object state)
        {
            var count = (int)state;
            if (count > 0)
            {
                count--;
                Console.WriteLine("ThreadPool " + count);
                ThreadPool.QueueUserWorkItem(UseThreadPoolClass, count);
                semaphore.WaitOne();
            }
            semaphore.Release();
        }
    }
}
