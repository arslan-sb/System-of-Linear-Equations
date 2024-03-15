//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading;

//namespace System_of_Linear_Equations_Assignment_1
//{

//    class LinearEquationSolver
//    {
//        static void Main()
//        {
//            Console.WriteLine("Enter the number of variables (N):");
//            int N = int.Parse(Console.ReadLine());

//            Console.WriteLine("Enter the coefficients and constants of the equations:");
//            double[,] coefficients = new double[N, N + 1];

//            for (int i = 0; i < N; i++)
//            {
//                for (int j = 0; j <= N; j++)
//                {
//                    Console.Write($"Enter coefficient for variable x{j + 1} in equation {i + 1}: ");
//                    coefficients[i, j] = double.Parse(Console.ReadLine());
//                }
//            }

//            Console.WriteLine("\nEnter the number of iterations:");
//            int iterations = int.Parse(Console.ReadLine());

//            double[] result = SolveLinearEquations(coefficients, iterations);

//            Console.WriteLine("\nSolution:");
//            for (int i = 0; i < N; i++)
//            {
//                Console.WriteLine($"x{i + 1} = {result[i]}");
//            }
//        }

//        static double[] SolveLinearEquations(double[,] coefficients, int iterations)
//        {
//            int N = coefficients.GetLength(0);
//            double[] currentValues = new double[N];
//            double[] newValues = new double[N];

//            for (int iter = 0; iter < iterations; iter++)
//            {
//                Thread[] threads = CreateThreads(coefficients, currentValues, newValues, N);

//                // Start all threads
//                foreach (var thread in threads)
//                {
//                    thread.Start();
//                }

//                // Wait for all threads to finish
//                foreach (var thread in threads)
//                {
//                    thread.Join();
//                }

//                Array.Copy(newValues, currentValues, N);
//            }

//            return currentValues;
//        }

//        static Thread[] CreateThreads(double[,] coefficients, double[] currentValues, double[] newValues, int N)
//        {
//            Thread[] threads = new Thread[N];

//            for (int i = 0; i < N; i++)
//            {
//                int index = i; // Capture the current value of i for the lambda expression
//                threads[i] = new Thread(CalculateAndUpdate(coefficients, currentValues, newValues, N, index));
//            }

//            return threads;
//        }

//        static ParameterizedThreadStart CalculateAndUpdate(double[,] coefficients, double[] currentValues, double[] newValues, int N, int index)
//        {
//            return (object obj) =>
//            {
//                double sum = coefficients[index, N];
//                for (int j = 0; j < N; j++)
//                {
//                    if (j != index)
//                    {
//                        sum -= coefficients[index, j] * currentValues[j];
//                    }
//                }
//                newValues[index] = sum / coefficients[index, index];
//            };
//        }
//    }

//}

