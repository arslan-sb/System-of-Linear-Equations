//using System;
//using System.Threading;
//using System.IO;

//class LinearEquationSolver
//{
//    static void Main()
//    {
//        Console.WriteLine("Enter the number of variables (N):");
//        int N = int.Parse(Console.ReadLine());

//        Console.WriteLine("Enter the coefficients and constants of the equations:");
//        double[,] coefficients = new double[N, N + 1];

//        for (int i = 0; i < N; i++)
//        {
//            for (int j = 0; j <= N; j++)
//            {
//                Console.Write($"Enter coefficient for variable x{j + 1} in equation {i + 1}: ");
//                coefficients[i, j] = double.Parse(Console.ReadLine());
//            }
//        }

//        Console.WriteLine("\nEnter the number of iterations:");
//        int iterations = int.Parse(Console.ReadLine());

//        Console.WriteLine("\nHow do you want to execute Application? \na. Single Threaded \nb. Multi-Threaded \nc. ThreadPool");
//        char choice = char.Parse(Console.ReadLine().ToUpper());
//        double[] result;
//        if (choice == 'B')
//        {
//            result = SolveLinearEquationsWithThreads(coefficients, iterations);

//        }
//        else if (choice == 'C')
//        {
//            result = SolveLinearEquationsWithThreadPools(coefficients, iterations);
//        }
//        else
//        {
//            result = SolveLinearEquations(coefficients, iterations);

//        }
//        PrintSolution(result);
//    }

//    static double[] SolveGaussJacobi(double[,] coefficients, double tolerance)
//    {
//        int n = coefficients.GetLength(0);
//        double[] solution = new double[n];
//        double[] prevSolution = new double[n];

//        double maxDifference = tolerance + 1; // Just to enter the loop

//        while (maxDifference > tolerance)
//        {
//            for (int i = 0; i < n; i++)
//            {
//                double sum = 0;
//                for (int j = 0; j < n; j++)
//                {
//                    if (i != j)
//                    {
//                        sum += coefficients[i, j] * prevSolution[j];
//                    }
//                }
//                solution[i] = (coefficients[i, n]-sum) / coefficients[i, i];
//            }

//            maxDifference = Math.Abs(solution[0] - prevSolution[0]); // Compute the maximum difference
//            for (int i = 1; i < n; i++)
//            {
//                double difference = Math.Abs(solution[i] - prevSolution[i]);
//                if (difference > maxDifference)
//                {
//                    maxDifference = difference;
//                }
//            }

//            Array.Copy(solution, prevSolution, n); // Update the previous solution
//        }

//        return solution;
//    }

//    static double[] SolveLinearEquations(double[,] coefficients, int iterations)
//    {
//        int N = coefficients.GetLength(0);
//        double[] currentValues = new double[N];
//        double[] newValues = new double[N];

//        for (int iter = 0; iter < iterations; iter++)
//        {
//            for (int i = 0; i < N; i++)
//            {
//                double sum = 0;
//                for (int j = 0; j < N; j++)
//                {
//                    if (j != i)
//                    {
//                        sum += coefficients[i, j] * currentValues[j];
//                    }
//                }
//                newValues[i] = (coefficients[i, N]- sum) / coefficients[i, i];
//            }

//            Array.Copy(newValues, currentValues, N);
//        }

//        return currentValues;
//    }

//    static double[] SolveLinearEquationsWithThreads(double[,] coefficients, int iterations)
//    {
//        int N = coefficients.GetLength(0);
//        double[] currentValues = new double[N];
//        double[] newValues = new double[N];

//        for (int iter = 0; iter < iterations; iter++)
//        {
//            Thread[] threads = new Thread[N];

//            for (int i = 0; i < N; i++)
//            {
//                int index = i; // Capture the current value of i for the lambda expression
//                threads[i] = new Thread(() =>
//                {
//                    double sum = coefficients[index, N];
//                    for (int j = 0; j < N; j++)
//                    {
//                        if (j != index)
//                        {
//                            sum -= coefficients[index, j] * currentValues[j];
//                        }
//                    }
//                    newValues[index] = sum / coefficients[index, index];
//                });
//            }

//            // Start all threads
//            foreach (var thread in threads)
//            {
//                thread.Start();
//            }

//            // Wait for all threads to finish
//            foreach (var thread in threads)
//            {
//                thread.Join();
//            }

//            Array.Copy(newValues, currentValues, N);
//        }

//        return currentValues;
//    }



//    static double[] SolveLinearEquationsWithThreadPools(double[,] coefficients, int iterations)
//    {
//        int N = coefficients.GetLength(0);
//        double[] currentValues = new double[N];
//        double[] newValues = new double[N];
//        int completedThreads = 0;

//        for (int i = 0; i < N; i++)
//        {
//            ThreadPool.QueueUserWorkItem(state =>
//            {
//                int index = (int)state;
//                for (int iter = 0; iter < iterations; iter++)
//                {
//                    double sum = coefficients[index, N];
//                    for (int j = 0; j < N; j++)
//                    {
//                        if (j != index)
//                        {
//                            sum -= coefficients[index, j] * currentValues[j];
//                        }
//                    }
//                    newValues[index] = sum / coefficients[index, index];
//                }

//                if (Interlocked.Increment(ref completedThreads) == N)
//                {
//                    // All threads have completed their work
//                    // Copy new values to current values
//                    Array.Copy(newValues, currentValues, N);
//                }
//            }, i);
//        }

//        // Wait for all threads to finish
//        while (completedThreads < N) ;

//        return currentValues;
//    }


//    static double[] SolveLinearEquationsWithThreadPool(double[,] coefficients, int iterations)
//    {
//        int N = coefficients.GetLength(0);
//        double[] currentValues = new double[N];
//        double[] newValues = new double[N];
//        AutoResetEvent[] doneEvents = new AutoResetEvent[N];

//        for (int i = 0; i < N; i++)
//        {
//            doneEvents[i] = new AutoResetEvent(false);
//            ThreadPool.QueueUserWorkItem(state =>
//            {
//                int index = (int)state;
//                for (int iter = 0; iter < iterations; iter++)
//                {
//                    double sum = coefficients[index, N];
//                    for (int j = 0; j < N; j++)
//                    {
//                        if (j != index)
//                        {
//                            sum -= coefficients[index, j] * currentValues[j];
//                        }
//                    }
//                    newValues[index] = sum / coefficients[index, index];
//                    doneEvents[index].Set();
//                    doneEvents[index].WaitOne();
//                }
//            }, i);
//        }

//        // Wait for all threads to finish
//        WaitHandle.WaitAll(doneEvents);

//        return currentValues;
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
