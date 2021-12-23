using System;
using System.Threading;
using System.Diagnostics;
using System.Collections.Generic;

namespace Dining_Philosophers
{
    class MainClass
    {
        // the number of philosophers
        public const int NUM_PHILOSOPHERS = 5;

        // maximum amount of time spent thinking, in milliseconds
        public const int THINK_TIME = 10;

        // maximum amount of time spent eating, in milliseconds
        public const int EAT_TIME = 10;

        // total program runtime in milliseconds
        public const int RUN_TIME = 2000;

        // maximum amount of time until an object is locked
        public const int LOCK_TIMEOUT = 10;

        // maximum amount of time 
        public const int CHOPSTICK_HANDLE_TIME = 3;

        // the chopsticks, implemented as sync objects
        public static object[] chopstick = new object[NUM_PHILOSOPHERS];

        // the philosophers, implemented as threads
        public static Thread[] philosopher = new Thread[NUM_PHILOSOPHERS];

        // a shared stopwatch to abort the program after a specific timeout
        public static Stopwatch stopwatch = new Stopwatch();

        // a shared dictionary to measure how long each philosopher has eaten
        public static Dictionary<int, int> eatingTime = new Dictionary<int, int>();

        // a sync object to protect access to the eatingTime field
        public static object eatingSync = new object();

        // this method lets the philosopher think
        public static void Think()
        {
            Random random = new Random();
            Thread.Sleep(random.Next(THINK_TIME));
        }

        // this method lets the philosopher eat
        public static void Eat(int index)
        {
            Random random = new Random();
            int time_spent_eating = random.Next(EAT_TIME);
            Thread.Sleep(time_spent_eating);

            // log our total eating time
            lock (eatingSync)
            {
                eatingTime[index] += time_spent_eating;
            }
        }

        // the philosopher work method - think and eat until timeout
        public static void DoWork(int index, object chopstick1, object chopstick2)
        {
            Console.WriteLine($"Philosopher {index} started!");

            // lock the first chopstick
            Monitor.Enter(chopstick1);

            Console.WriteLine($"Philosopher {index} got first chopstick!");

            TakeChopstick2();

            // lock the second chopstick
            Monitor.Enter(chopstick2);

            Console.WriteLine($"Philosopher {index} got second chopstick!");
            Eat(index);

            Console.WriteLine($"Philosopher {index} released second chopstick!");
            Monitor.Exit(chopstick2);

            Monitor.Exit(chopstick1);
            Console.WriteLine($"Philosopher {index} released first chopstick!");

            Think();
        }

        private static void TakeChopstick2()
        {
            Random random = new Random();
            var time = random.Next(CHOPSTICK_HANDLE_TIME);
            Thread.Sleep(time);
        }

        public static void Main(string[] args)
        {
            // set up the dictionary that measures total eating time
            for (int i = 0; i < NUM_PHILOSOPHERS; i++)
            {
                eatingTime.Add(i, 0);
            }

            // set up the chopsticks
            for (int i = 0; i < NUM_PHILOSOPHERS; i++)
            {
                chopstick[i] = new object();
            }

            // set up the philosophers
            for (int i = 0; i < NUM_PHILOSOPHERS; i++)
            {
                int index = i;
                object chopstick1 = chopstick[i];
                object chopstick2 = chopstick[(i + 1) % NUM_PHILOSOPHERS];

            }

            // start the philosophers
            stopwatch.Start();
            Console.WriteLine("Starting philosophers...");

            List<int> runningThreads = new List<int>();
            int maxNumOfConcurrentThreads = NUM_PHILOSOPHERS / 2;

            while (stopwatch.ElapsedMilliseconds < RUN_TIME)
            {
                if (runningThreads.Count == maxNumOfConcurrentThreads)
                {
                    for (int philosopherIndex = 0; philosopherIndex < NUM_PHILOSOPHERS; philosopherIndex++)
                    {
                        if (runningThreads.Contains(philosopherIndex) &&
                            philosopher[philosopherIndex].ThreadState == System.Threading.ThreadState.Stopped)
                        {
                            runningThreads.Remove(philosopherIndex);
                        }
                    }
                }

                else
                {
                    int newPhilosopherIndex = SelectPhilosopher(runningThreads);
                    runningThreads.Add(newPhilosopherIndex);

                    object chopstick1 = chopstick[newPhilosopherIndex];
                    object chopstick2 = chopstick[(newPhilosopherIndex + 1) % NUM_PHILOSOPHERS];

                    philosopher[newPhilosopherIndex] = new Thread(
                    _ =>
                    {
                        DoWork(newPhilosopherIndex, chopstick1, chopstick2);
                    });

                    philosopher[newPhilosopherIndex].Start();
                }
            }


            for (int i = 0; i < NUM_PHILOSOPHERS; i++)
            {
                philosopher[i].Start();
            }

            // wait for all philosophers to complete
            for (int i = 0; i < NUM_PHILOSOPHERS; i++)
            {
                philosopher[i].Join();
            }
            Console.WriteLine("All philosophers have finished");

            // report the total time spent eating
            int total_eating_time = 0;
            for (int i = 0; i < NUM_PHILOSOPHERS; i++)
            {
                Console.WriteLine("Philosopher {0} has eaten for {1}ms", i, eatingTime[i]);
                total_eating_time += eatingTime[i];
            }
            Console.WriteLine("Total time spent eating: {0}ms", total_eating_time);
            Console.WriteLine("Optimal time spent eating: {0}ms", stopwatch.ElapsedMilliseconds * 2);
        }

        private static int SelectPhilosopher(List<int> runningThreads)
        {
            List<int> 
        }
    }
}
