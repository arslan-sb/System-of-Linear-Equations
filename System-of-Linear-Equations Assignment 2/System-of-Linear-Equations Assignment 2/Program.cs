using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System_of_Linear_Equations_Assignment_2
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    class LinearEquationSolver
    {
        static void Main()
        {
            Console.WriteLine("Enter the path of the file:");
            string filePath = Console.ReadLine();

            if (File.Exists(filePath))
            {
                

                string[] lines = File.ReadAllLines(filePath);
                int numSystems = int.Parse(lines[0]);

                int lineIndex = 1;
                List<double[,]> equations = new List<double[,]>();
                for (int systemNumber = 1; systemNumber <= numSystems; systemNumber++)
                {
                    int N = int.Parse(lines[lineIndex]);
                    lineIndex++;
                    double[,] coefficients = new double[N, N + 1];
                    for (int i = 0; i < N; i++)
                    {
                        string[] values = lines[lineIndex].Split(' ');
                        for (int j = 0; j <= N; j++)
                        {
                            coefficients[i, j] = double.Parse(values[j]);
                        }
                        lineIndex++;
                    }
                    equations.Add(coefficients);
                }

                for (char c = 'A'; c <= 'E'; c++)
                {
                    if (c == 'B')
                    {
                        var stopwatch = Stopwatch.StartNew();
                        MultiThreadedApplication(equations);
                        stopwatch.Stop();
                        Console.WriteLine($"Execution Time for Multi-Threaded Application: {stopwatch.ElapsedMilliseconds} Milliseconds");
                    }
                    else if (c == 'C')
                    {
                        var stopwatch = Stopwatch.StartNew();
                        ThreadPooledApplication(equations);
                        stopwatch.Stop();
                        Console.WriteLine($"Execution Time for ThreadPool Application: {stopwatch.ElapsedMilliseconds} Milliseconds");
                    }
                    else if (c == 'D')
                    {
                        var stopwatch = Stopwatch.StartNew();
                        ParallelForApplication(equations);
                        stopwatch.Stop();
                        Console.WriteLine($"Execution Time for Parallel.For Application: {stopwatch.ElapsedMilliseconds} Milliseconds");
                    }
                    else if (c == 'E')
                    {
                        var stopwatch = Stopwatch.StartNew();
                        ParallelForEachApplication(equations);
                        stopwatch.Stop();
                        Console.WriteLine($"Execution Time for Parallel.ForEach Application: {stopwatch.ElapsedMilliseconds} Milliseconds");
                    }
                    else
                    {
                        var stopwatch = Stopwatch.StartNew();
                        SingleThreadedApplication(equations);
                        stopwatch.Stop();
                        Console.WriteLine($"Execution Time for Single-Threaded Application: {stopwatch.ElapsedMilliseconds} Milliseconds");
                    }
                }
            }
            else
            {
                Console.WriteLine("File not found.");
            }
        }

        static void MultiThreadedApplication(List<double[,]> equations)
        {
            List<Thread> threads = new List<Thread>();
            int sysNum = 1;
            foreach (var equation in equations)
            {
                Thread thread = new Thread(() => SolveLinearEquation(equation, sysNum));
                threads.Add(thread);
                thread.Start();
                sysNum++;
            }
            foreach (var thread in threads)
            {
                thread.Join();
            }
        }

        static void SingleThreadedApplication(List<double[,]> equations)
        {
            int sysNum = 1;
            foreach (var equation in equations)
            {
                SolveLinearEquation(equation, sysNum);
                sysNum++;
            }
        }

        static void ThreadPooledApplication(List<double[,]> equations)
        {
            ManualResetEvent[] waitHandles = null;
            int numThreadPoolSystems = 0;
            int sysNum = 1;
            foreach (var equation in equations)
            {
                Interlocked.Increment(ref numThreadPoolSystems);
                ThreadPool.QueueUserWorkItem(state =>
                {
                    SolveLinearEquation(equation, sysNum);
                    sysNum++;
                    if (Interlocked.Decrement(ref numThreadPoolSystems) == 0)
                        waitHandles[0].Set();
                });
            }
            waitHandles = new ManualResetEvent[] { new ManualResetEvent(false) };
            WaitHandle.WaitAll(waitHandles);
        }

        static void ParallelForApplication(List<double[,]> equations)
        {
            Parallel.For(0, equations.Count, i =>
            {
                SolveLinearEquation(equations[i], i + 1);
            });
        }

        static void ParallelForEachApplication(List<double[,]> equations)
        {
            int sysNum = 1;
            Parallel.ForEach(equations, equ =>
            {
                SolveLinearEquation(equ, sysNum++);
            });
        }

        static void SolveLinearEquation(double[,] coefficients, int sysNum)
        {
            int n = coefficients.GetLength(0);
            // Perform Gaussian Elimination
            for (int i = 0; i < n - 1; i++)
            {
                for (int j = i + 1; j < n; j++)
                {
                    double factor = coefficients[j, i] / coefficients[i, i];
                    for (int k = i; k < n + 1; k++)
                    {
                        coefficients[j, k] -= factor * coefficients[i, k];
                    }
                }
            }
            // Back substitution
            double[] solutions = new double[n];
            for (int i = n - 1; i >= 0; i--)
            {
                double sum = 0;
                for (int j = i + 1; j < n; j++)
                {
                    sum += coefficients[i, j] * solutions[j];
                }
                solutions[i] = (coefficients[i, n] - sum) / coefficients[i, i];
            }
            // PrintSolution(solutions, sysNum);
        }

        static void PrintSolution(double[] result, int sysNum)
        {
            Console.WriteLine($"\nSolution for system {sysNum}:");
            for (int i = 0; i < result.Length; i++)
            {
                Console.WriteLine($"x{i + 1} = {result[i]}");
            }
        }
    }

}
