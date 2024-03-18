//using System;
//using System.Threading;
//using System.IO;
//using System.Diagnostics;
//using System.Collections.Generic;

//class LinearEquationSolver
//{
//    static void Main()
//    {
//        Console.WriteLine("Enter the path of the file:");
//        string filePath = Console.ReadLine();



//        if (File.Exists(filePath))
//        {
//            Console.WriteLine("\nHow do you want to execute Application? \na. Single Threaded \nb. Multi-Threaded \nc. ThreadPool");
//            char choice = char.Parse(Console.ReadLine().ToUpper());
//            var stopwatchSingleThread = Stopwatch.StartNew();
//            string[] lines = File.ReadAllLines(filePath);

//            int numSystems = int.Parse(lines[0]);

//            List<Thread> threads = new List<Thread>();
//            ManualResetEvent[] waitHandles = null;
//            int numThreadPoolSystems = 0;

//            int lineIndex = 1;

//            for (int systemNumber = 1; systemNumber <= numSystems; systemNumber++)
//            {
//                Console.WriteLine($"Solving system {systemNumber}...");

//                int N = int.Parse(lines[lineIndex]);
//                lineIndex++;

//                double[,] coefficients = new double[N, N + 1];

//                for (int i = 0; i < N; i++)
//                {
//                    string[] values = lines[lineIndex].Split(' ');
//                    for (int j = 0; j <= N; j++)
//                    {
//                        coefficients[i, j] = double.Parse(values[j]);
//                        if (j == N)
//                        {
//                            Console.Write($"={coefficients[i, j]}\n");
//                        }
//                        else
//                        {
//                            Console.Write($"{coefficients[i, j]}x[{j + 1}]+");
//                        }
//                    }

//                    lineIndex++;
//                }

//                if (choice == 'B')
//                {
//                    Thread thread = new Thread(() => SolveLinearEquation(coefficients, systemNumber));
//                    threads.Add(thread);
//                    thread.Start();

//                }
//                else if (choice == 'C')
//                {
//                    waitHandles = new ManualResetEvent[] { new ManualResetEvent(false) };

//                    Interlocked.Increment(ref numThreadPoolSystems);
//                    ThreadPool.QueueUserWorkItem(state =>
//                    {
//                        SolveLinearEquation(coefficients, systemNumber);
//                        if (Interlocked.Decrement(ref numThreadPoolSystems) == 0)
//                            waitHandles[0].Set();
//                    });
//                }
//                else
//                {
//                    SolveLinearEquation(coefficients, systemNumber);

//                }


//            }

//            // Join all threads if multi-threaded option is chosen
//            if (choice == 'B')
//            {
//                foreach (var thread in threads)
//                {
//                    thread.Join();
//                }
//            }
//            else if (choice == 'C')
//            {
//                waitHandles = new ManualResetEvent[] { new ManualResetEvent(false) };
//                WaitHandle.WaitAll(waitHandles);
//            }

//            stopwatchSingleThread.Stop();
//            Console.WriteLine($"Execution Time: { stopwatchSingleThread.Elapsed}");
//        }
//        else
//        {
//            Console.WriteLine("File not found.");
//        }


//    }
//    static void SolveLinearEquation(double[,] coefficients, int systemNumber)
//    {

//        int n = coefficients.GetLength(0);

//        // Perform Gaussian Elimination
//        for (int i = 0; i < n - 1; i++)
//        {
//            for (int j = i + 1; j < n; j++)
//            {
//                double factor = coefficients[j, i] / coefficients[i, i];
//                for (int k = i; k < n + 1; k++)
//                {
//                    coefficients[j, k] -= factor * coefficients[i, k];
//                }
//            }
//        }

//        // Back substitution
//        double[] solutions = new double[n];
//        for (int i = n - 1; i >= 0; i--)
//        {
//            double sum = 0;
//            for (int j = i + 1; j < n; j++)
//            {
//                sum += coefficients[i, j] * solutions[j];
//            }
//            solutions[i] = (coefficients[i, n] - sum) / coefficients[i, i];
//        }
//        Console.WriteLine($"Solution for system {systemNumber}:");
//        PrintSolution(solutions);

//    }




//    static void PrintSolution(double[] result)
//    {
//        Console.WriteLine("\nSolution:");
//        for (int i = 0; i < result.Length; i++)
//        {
//            Console.WriteLine($"x{i + 1} = {result[i]}");
//        }
//    }
//}
