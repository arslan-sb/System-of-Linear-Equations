

using System;

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

        double[] result = SolveLinearEquations(coefficients);

        Console.WriteLine("\nSolution:");
        for (int i = 0; i < N; i++)
        {
            Console.WriteLine($"x{i + 1} = {result[i]}");
        }
        Console.ReadKey();
    }

    static double[] SolveLinearEquations(double[,] coefficients)
    {
        int N = coefficients.GetLength(0);

        for (int i = 0; i < N; i++)
        {
            // Make the diagonal element 1
            double divisor = coefficients[i, i];
            for (int j = 0; j <= N; j++)
            {
                coefficients[i, j] /= divisor;
            }

            // Make other elements in the column zero
            for (int k = 0; k < N; k++)
            {
                if (k != i)
                {
                    double factor = coefficients[k, i];
                    for (int j = 0; j <= N; j++)
                    {
                        coefficients[k, j] -= factor * coefficients[i, j];
                    }
                }
            }
        }

        // Extract the solution
        double[] result = new double[N];
        for (int i = 0; i < N; i++)
        {
            result[i] = coefficients[i, N];
        }

        return result;
    }
}
