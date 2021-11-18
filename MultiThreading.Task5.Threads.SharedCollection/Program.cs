/*
 * 5. Write a program which creates two threads and a shared collection:
 * the first one should add 10 elements into the collection and the second should print all elements
 * in the collection after each adding.
 * Use Thread, ThreadPool or Task classes for thread creation and any kind of synchronization constructions.
 */
using System;
using System.Collections.Generic;
using System.Threading;

namespace MultiThreading.Task5.Threads.SharedCollection
{
    class Program
    {
        private static readonly List<int> _sharedCollection = new List<int>();
        private static readonly AutoResetEvent _printEvent = new AutoResetEvent(true);
        private static readonly AutoResetEvent _addEvent = new AutoResetEvent(false);
        private static readonly CancellationTokenSource cts = new CancellationTokenSource();

        static void Main(string[] args)
        {
            Console.WriteLine("5. Write a program which creates two threads and a shared collection:");
            Console.WriteLine("the first one should add 10 elements into the collection and the second should print all elements in the collection after each adding.");
            Console.WriteLine("Use Thread, ThreadPool or Task classes for thread creation and any kind of synchronization constructions.");
            Console.WriteLine();

            var addThread = new Thread(AddItems);
            var printThread = new Thread(PrintItems);
            addThread.Start();
            printThread.Start();

            addThread.Join();

            Console.ReadLine();
        }
        private static void AddItems()
        {
            for (int i = 0; i < 10; i++)
            {
                _printEvent.WaitOne();
                _sharedCollection.Add(i);
                _addEvent.Set();
            }

            cts.Cancel();
        }

        private static void PrintItems()
        {
            while (!cts.Token.IsCancellationRequested)
            {
                _addEvent.WaitOne();
                Console.WriteLine($"[{string.Join(", ", _sharedCollection)}]");
                _printEvent.Set();
            }
        }
    }
}
