/*
*  Create a Task and attach continuations to it according to the following criteria:
   a.    Continuation task should be executed regardless of the result of the parent task.
   b.    Continuation task should be executed when the parent task finished without success.
   c.    Continuation task should be executed when the parent task would be finished with fail and parent task thread should be reused for continuation
   d.    Continuation task should be executed outside of the thread pool when the parent task would be cancelled
   Demonstrate the work of the each case with console utility.
*/
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MultiThreading.Task6.Continuation
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Create a Task and attach continuations to it according to the following criteria:");
            Console.WriteLine("a.    Continuation task should be executed regardless of the result of the parent task.");
            Console.WriteLine("b.    Continuation task should be executed when the parent task finished without success.");
            Console.WriteLine("c.    Continuation task should be executed when the parent task would be finished with fail and parent task thread should be reused for continuation.");
            Console.WriteLine("d.    Continuation task should be executed outside of the thread pool when the parent task would be cancelled.");
            Console.WriteLine("Demonstrate the work of the each case with console utility.");
            Console.WriteLine();

            // feel free to add your code
            TaskA();
            Console.WriteLine("================================");
            TaskB();
            Console.WriteLine("================================");
            TaskC();
            Console.WriteLine("================================");
            TaskD();

            Console.ReadLine();
        }

        private static void TaskA()
        {
            Task.Run(() =>
            {
                Console.WriteLine("TaskA. Success!");

            }).ContinueWith((task) =>
            {
                // Should be executed
                Console.WriteLine("Executed regardless of the result");
            }).Wait();

            Task.Run(() =>
            {
                Console.WriteLine("TaskA. Exception!");
                throw new InvalidOperationException();

            }).ContinueWith((task) =>
            {
                // Should be executed
                Console.WriteLine("Executed regardless of the result");
            }, TaskContinuationOptions.None).Wait();
        }

        private static void TaskB()
        {
            var task1 = Task.Run(() =>
            {
                Console.WriteLine("TaskB. Success!");

            });
            var task2 = task1.ContinueWith((task) =>
            {
                // Should not be executed
                Console.WriteLine("Executed when the parent task finished without success");
            }, TaskContinuationOptions.OnlyOnFaulted);

            Task.WaitAny(task1, task2);

            Task.Run(() =>
            {
                Console.WriteLine("TaskB. Exception!");
                throw new InvalidOperationException();

            }).ContinueWith((task) =>
            {
                // Should be executed
                Console.WriteLine("Executed when the parent task finished without success");
            }, TaskContinuationOptions.OnlyOnFaulted).Wait();
        }

        private static void TaskC()
        {
            int threadId = -1;
            var task1 = Task.Run(() =>
            {
                threadId = Thread.CurrentThread.ManagedThreadId;
                Console.WriteLine("TaskC. Exception!");
                throw new InvalidOperationException();

            });
            var task2 = task1.ContinueWith((task) =>
            {
                if (threadId == Thread.CurrentThread.ManagedThreadId)
                {
                    // Should not be executed
                    Console.WriteLine("Executed when the parent task would be finished with fail and parent task thread should be reused for continuation");
                }
            }, TaskContinuationOptions.OnlyOnFaulted);

            Task.WaitAny(task1, task2);

            Task.Run(() =>
            {
                threadId = Thread.CurrentThread.ManagedThreadId;
                Console.WriteLine("TaskC. Exception!");
                throw new InvalidOperationException();

            }).ContinueWith((task) =>
            {
                if (threadId == Thread.CurrentThread.ManagedThreadId)
                {
                    // Should be executed
                    Console.WriteLine("Executed when the parent task would be finished with fail and parent task thread should be reused for continuation");
                }
            }, TaskContinuationOptions.OnlyOnFaulted | TaskContinuationOptions.ExecuteSynchronously).Wait();
        }

        private static void TaskD()
        {
            var task1 = Task.Run(() =>
            {
                Console.WriteLine("TaskD. Success!");

            });
            var task2 = task1.ContinueWith((task) =>
            {
                // Should not be executed
                if (!Thread.CurrentThread.IsThreadPoolThread)
                {
                    Console.WriteLine("Executed outside of the thread pool when the parent task would be cancelled");
                }
            });

            Task.WaitAny(task1, task2);

            CancellationTokenSource cts = new CancellationTokenSource();
            cts.Cancel();

            Task.Run(() =>
            {
                Console.WriteLine("TaskD. Cancelled!");

            }, cts.Token).ContinueWith((task) =>
            {
                // Should be executed
                if (!Thread.CurrentThread.IsThreadPoolThread)
                {
                    Console.WriteLine("Executed outside of the thread pool when the parent task would be cancelled");
                }
            }, TaskContinuationOptions.OnlyOnCanceled | TaskContinuationOptions.LongRunning).Wait();
        }
    }
}
