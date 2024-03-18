using System;
using System.Threading;
using System.IO;
using System.Diagnostics;

class LinearEquationSolver
{
    static void Main()
    {
        Console.WriteLine("Enter the path of the file:");
        string filePath = Console.ReadLine();
        Console.WriteLine(filePath);

        if (File.Exists(filePath))
        {
            string[] lines = File.ReadAllLines(filePath);

            int numSystems = int.Parse(lines[0]);

            int lineIndex = 1;

            for (int systemNumber = 1; systemNumber <= numSystems; systemNumber++)
            {
                Console.WriteLine($"Solving system {systemNumber}...");

                int N = int.Parse(lines[lineIndex]);
                lineIndex++;

                double[,] coefficients = new double[N, N + 1];

                for (int i = 0; i < N; i++)
                {
                    string[] values = lines[lineIndex].Split(' ');
                    for (int j = 0; j <= N; j++)
                    {
                        coefficients[i, j] = double.Parse(values[j]);
                        Console.WriteLine(coefficients[i, j]);
                    }
                    lineIndex++;
                }

                int iterations = 19; // You can modify this as needed

                Console.WriteLine("\nHow do you want to execute Application? \na. Single Threaded \nb. Multi-Threaded \nc. ThreadPool");
                char choice = char.Parse(Console.ReadLine().ToUpper());
                
                double[] result;
                if (choice == 'B')
                {
                    var stopwatchSingleThread = Stopwatch.StartNew();
                    result = SolveLinearEquationsWithThreads(coefficients, iterations);
                    stopwatchSingleThread.Stop();
                    Console.WriteLine($"Execution Time for MultiThreaded: { stopwatchSingleThread.Elapsed}");

                }
                else if (choice == 'C')
                {
                    var stopwatchSingleThread = Stopwatch.StartNew();
                    result = SolveLinearEquationsWithThreadPools(coefficients, iterations);
                    Console.WriteLine($"Execution Time for MultiThreaded: { stopwatchSingleThread.Elapsed}");
                }
                else
                {
                    var stopwatchSingleThread = Stopwatch.StartNew();
                    result = SolveLinearEquations(coefficients, iterations);
                    Console.WriteLine($"Execution Time for MultiThreaded: { stopwatchSingleThread.Elapsed}");
                }
                Console.WriteLine($"Solution for system {systemNumber}:");
                PrintSolution(result);
            }
        }
        else
        {
            Console.WriteLine("File not found.");
        }

        //Console.WriteLine("Enter the number of variables (N):");
        //int N = int.Parse(Console.ReadLine());

        //Console.WriteLine("Enter the coefficients and constants of the equations:");
        //double[,] coefficients = new double[N, N + 1];

        //for (int i = 0; i < N; i++)
        //{
        //    for (int j = 0; j <= N; j++)
        //    {
        //        Console.Write($"Enter coefficient for variable x{j + 1} in equation {i + 1}: ");
        //        coefficients[i, j] = double.Parse(Console.ReadLine());
        //    }
        //}

        //Console.WriteLine("\nEnter the number of iterations:");
        //int iterations = int.Parse(Console.ReadLine());

        //Console.WriteLine("\nHow do you want to execute Application? \na. Single Threaded \nb. Multi-Threaded \nc. ThreadPool");
        //char choice = char.Parse(Console.ReadLine().ToUpper());
        //double[] result;
        //if (choice == 'B')
        //{
        //    result = SolveLinearEquationsWithThreads(coefficients, iterations);

        //}
        //else if (choice == 'C')
        //{
        //    result = SolveLinearEquationsWithThreadPools(coefficients, iterations);
        //}
        //else
        //{
        //    result = SolveLinearEquations(coefficients, iterations);

        //}
        //PrintSolution(result);
    }
    //static double LinearEquationSolver(double[,])
    //{

    //}
    static double[] SolveLinearEquationss(double[,] coefficients, int iterations, double tolerance)
    {
        int N = coefficients.GetLength(0);
        double[] currentValues = new double[N];
        double[] newValues = new double[N];

        double maxChange; // Store the maximum change in any component of the solution vector

        for (int iter = 0; iter < iterations; iter++)
        {
            maxChange = 0.0;

            for (int i = 0; i < N; i++)
            {
                double sum = coefficients[i, N];
                for (int j = 0; j < N; j++)
                {
                    if (j != i)
                    {
                        sum -= coefficients[i, j] * currentValues[j];
                    }
                }
                newValues[i] = sum / coefficients[i, i];

                // Calculate the change in the solution vector for this component
                double change = Math.Abs(newValues[i] - currentValues[i]);

                // Update maxChange if necessary
                if (change > maxChange)
                {
                    maxChange = change;
                }
            }

            // Check if the maximum change is below the tolerance level
            if (maxChange < tolerance)
            {
                // Solution has converged, terminate iterations
                Console.WriteLine($"Converged after {iter + 1} iterations.");
                break;
            }

            // Copy newValues to currentValues for the next iteration
            Array.Copy(newValues, currentValues, N);
        }

        return currentValues;
    }


    static double[] SolveLinearEquations(double[,] coefficients, int iterations)
    {
        int N = coefficients.GetLength(0);
        double[] currentValues = new double[N];
        double[] newValues = new double[N];

        for (int iter = 0; iter < iterations; iter++)
        {
            for (int i = 0; i < N; i++)
            {
                double sum = coefficients[i, N];
                for (int j = 0; j < N; j++)
                {
                    if (j != i)
                    {
                        sum -= coefficients[i, j] * currentValues[j];
                    }
                }
                newValues[i] = sum / coefficients[i, i];
            }

            Array.Copy(newValues, currentValues, N);
        }

        return currentValues;
    }

    static double[] SolveLinearEquationsWithThreads(double[,] coefficients, int iterations)
    {
        int N = coefficients.GetLength(0);
        double[] currentValues = new double[N];
        double[] newValues = new double[N];

        for (int iter = 0; iter < iterations; iter++)
        {
            Thread[] threads = new Thread[N];

            for (int i = 0; i < N; i++)
            {
                int index = i; // Capture the current value of i for the lambda expression
                threads[i] = new Thread(() =>
                {
                    double sum = coefficients[index, N];
                    for (int j = 0; j < N; j++)
                    {
                        if (j != index)
                        {
                            sum -= coefficients[index, j] * currentValues[j];
                        }
                    }
                    newValues[index] = sum / coefficients[index, index];
                });
            }

            // Start all threads
            foreach (var thread in threads)
            {
                thread.Start();
            }

            // Wait for all threads to finish
            foreach (var thread in threads)
            {
                thread.Join();
            }

            Array.Copy(newValues, currentValues, N);
        }

        return currentValues;
    }



    static double[] SolveLinearEquationsWithThreadPools(double[,] coefficients, int iterations)
    {
        int N = coefficients.GetLength(0);
        double[] currentValues = new double[N];
        double[] newValues = new double[N];
        int completedThreads = 0;

        for (int i = 0; i < N; i++)
        {
            ThreadPool.QueueUserWorkItem(state =>
            {
                int index = (int)state;
                for (int iter = 0; iter < iterations; iter++)
                {
                    double sum = coefficients[index, N];
                    for (int j = 0; j < N; j++)
                    {
                        if (j != index)
                        {
                            sum -= coefficients[index, j] * currentValues[j];
                        }
                    }
                    newValues[index] = sum / coefficients[index, index];
                }

                if (Interlocked.Increment(ref completedThreads) == N)
                {
                    // All threads have completed their work
                    // Copy new values to current values
                    Array.Copy(newValues, currentValues, N);
                }
            }, i);
        }

        // Wait for all threads to finish
        while (completedThreads < N) ;

        return currentValues;
    }


    static double[] SolveLinearEquationsWithThreadPool(double[,] coefficients, int iterations)
    {
        int N = coefficients.GetLength(0);
        double[] currentValues = new double[N];
        double[] newValues = new double[N];
        AutoResetEvent[] doneEvents = new AutoResetEvent[N];

        for (int i = 0; i < N; i++)
        {
            doneEvents[i] = new AutoResetEvent(false);
            ThreadPool.QueueUserWorkItem(state =>
            {
                int index = (int)state;
                for (int iter = 0; iter < iterations; iter++)
                {
                    double sum = coefficients[index, N];
                    for (int j = 0; j < N; j++)
                    {
                        if (j != index)
                        {
                            sum -= coefficients[index, j] * currentValues[j];
                        }
                    }
                    newValues[index] = sum / coefficients[index, index];
                    doneEvents[index].Set();
                    doneEvents[index].WaitOne();
                }
            }, i);
        }

        // Wait for all threads to finish
        WaitHandle.WaitAll(doneEvents);

        return currentValues;
    }

    static void PrintSolution(double[] result)
    {
        Console.WriteLine("\nSolution:");
        for (int i = 0; i < result.Length; i++)
        {
            Console.WriteLine($"x{i + 1} = {result[i]}");
        }
    }
}
