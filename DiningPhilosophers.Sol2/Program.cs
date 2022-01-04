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

        // maximum amount of time spent eating, in milliseconds
        public const int EAT_TIME = 100;

        // total program runtime in milliseconds
        public const int RUN_TIME = 2000;

        // maximum amount of time until an object is locked
        public const int LOCK_TIMEOUT = 10;

        // the time it needs until philosopher takes the second fork after taking the first one
        public const int FORK_HANDLE_TIME = 5;

        // the forks, implemented as sync objects
        public static object[] forks = new object[NUM_PHILOSOPHERS];

        // the philosophers, implemented as threads
        public static Thread[] philosopher = new Thread[NUM_PHILOSOPHERS];

        // a shared stopwatch to abort the program after a specific timeout
        public static Stopwatch stopwatch = new Stopwatch();

        // a shared dictionary to measure how long each philosopher has eaten
        public static Dictionary<int, int> eatingTime = new Dictionary<int, int>();

        // a sync object to protect access to the eatingTime field
        public static object eatingSync = new object();

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
        public static void DoWork(int index, int fork1Index, int fork2Index)
        {
            var fork1 = forks[fork1Index];
            var fork2 = forks[fork2Index];

            Console.WriteLine($"Philosopher {index} started!");

            bool forkTaken = false;

            // lock the first fork
            Monitor.TryEnter(fork1, ref forkTaken);

            if (forkTaken == false)
            {
                Console.WriteLine("***************************");
                Console.WriteLine("Algorithm Failed");
                Console.WriteLine("***************************");
            }

            Console.WriteLine($"Philosopher {index} got first fork!");

            Takefork2();

            forkTaken = false;

            // lock the second fork
            Monitor.TryEnter(fork2, ref forkTaken);

            if (forkTaken == false)
            {
                Console.WriteLine("***************************");
                Console.WriteLine("Algorithm Failed");
                Console.WriteLine("***************************");
            }

            Console.WriteLine($"Philosopher {index} got second fork!");
            Eat(index);

            Console.WriteLine($"Philosopher {index} released second fork!");
            Monitor.Exit(fork2);

            Monitor.Exit(fork1);
            Console.WriteLine($"Philosopher {index} released first fork!");
        }

        private static void Takefork2()
        {
            Random random = new Random();
            var time = random.Next(FORK_HANDLE_TIME);
            Thread.Sleep(time);
        }

        public static void Main(string[] args)
        {
            // set up the dictionary that measures total eating time
            for (int i = 0; i < NUM_PHILOSOPHERS; i++)
            {
                eatingTime.Add(i, 0);
            }

            // set up the forks
            for (int i = 0; i < NUM_PHILOSOPHERS; i++)
            {
                forks[i] = new object();
            }

            // set up the philosophers
            for (int i = 0; i < NUM_PHILOSOPHERS; i++)
            {
                int index = i;
                object fork1 = forks[i];
                object fork2 = forks[(i + 1) % NUM_PHILOSOPHERS];

            }

            // start the philosophers
            stopwatch.Start();
            Console.WriteLine("Starting philosophers...");

            List<int> eatingPhilosophers = new List<int>();
            int maxNumOfConcurrentEatingPhilosophers = NUM_PHILOSOPHERS / 2;

            while (stopwatch.ElapsedMilliseconds < RUN_TIME)
            {
                if (eatingPhilosophers.Count == maxNumOfConcurrentEatingPhilosophers)
                {
                    for (int philosopherIndex = 0; philosopherIndex < NUM_PHILOSOPHERS; philosopherIndex++)
                    {
                        if (eatingPhilosophers.Contains(philosopherIndex) &&
                            philosopher[philosopherIndex].ThreadState == System.Threading.ThreadState.Stopped)
                        {
                            eatingPhilosophers.Remove(philosopherIndex);
                        }
                    }
                }

                else
                {
                    while (eatingPhilosophers.Count != maxNumOfConcurrentEatingPhilosophers)
                    {
                        int newPhilosopherIndex = SelectPhilosopher(eatingPhilosophers);
                        eatingPhilosophers.Add(newPhilosopherIndex);

                        int fork1Index = (newPhilosopherIndex - 1 + NUM_PHILOSOPHERS) % NUM_PHILOSOPHERS;
                        int fork2Index = newPhilosopherIndex;
                        philosopher[newPhilosopherIndex] = new Thread(
                            _ =>
                            {
                                DoWork(newPhilosopherIndex, fork1Index, fork2Index);
                            }
                        );
                        philosopher[newPhilosopherIndex].Name = "philosopher " + newPhilosopherIndex;

                        philosopher[newPhilosopherIndex].Start();
                    }
                }
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

        private static int SelectPhilosopher(List<int> eatingPhilosophers)
        {
            List<int> validPhilosophers = Enumerable.Range(0, NUM_PHILOSOPHERS).ToList();

            foreach (var eatingPhilosopher in eatingPhilosophers)
            {
                int prevPhilosopher = (eatingPhilosopher - 1 + NUM_PHILOSOPHERS) % NUM_PHILOSOPHERS;
                int nextPhilosopher = (eatingPhilosopher + 1) % NUM_PHILOSOPHERS;

                if (validPhilosophers.Contains(eatingPhilosopher))
                {
                    validPhilosophers.Remove(eatingPhilosopher);
                }

                if (validPhilosophers.Contains(prevPhilosopher))
                {
                    validPhilosophers.Remove(prevPhilosopher);
                }

                if (validPhilosophers.Contains(nextPhilosopher))
                {
                    validPhilosophers.Remove(nextPhilosopher);
                }
            }

            Random rand = new Random();
            var randomPhilIndex = rand.Next(validPhilosophers.Count);
            return validPhilosophers[randomPhilIndex];
        }
    }
}
