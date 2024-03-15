using System;
using System.Threading;

class LinearEquationSolver
{
    static void Main()
    {
        Console.WriteLine("Enter the number of variables (N):");
        int N = int.Parse(Console.ReadLine());

        Console.WriteLine("Enter the coefficients and constants of the equations:");
        double[,] coefficients = new double[N, N + 1];

        for (int i = 0; i < N; i++)
        {
            for (int j = 0; j <= N; j++)
            {
                Console.Write($"Enter coefficient for variable x{j + 1} in equation {i + 1}: ");
                coefficients[i, j] = double.Parse(Console.ReadLine());
            }
        }

        Console.WriteLine("\nEnter the number of iterations:");
        int iterations = int.Parse(Console.ReadLine());

        Console.WriteLine("\nHow do you want to execute Application? \na. Single Threaded \nb. Multi-Threaded \nc. ThreadPool");
        char choice = char.Parse(Console.ReadLine().ToUpper());
        double[] result;
        if (choice == 'B')
        {
            result = SolveLinearEquationsWithThreads(coefficients, iterations);
            
        }
        else if (choice=='C')
        {
            result= SolveLinearEquationsWithThreadPools(coefficients, iterations);
        }
        else
        {
            result = SolveLinearEquations(coefficients, iterations);
            
        }
        PrintSolution(result);
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
