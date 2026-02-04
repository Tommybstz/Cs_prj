using System;

class Program
{
    //function for factorial
    static long Factorial(int n)
    {
        long result = 1;
        for (int i = 2; i <= n; i++)
        {
            result *= i;
        }
        return result;
    }

    static void Main()
    {
        while (true)
        {
            //input
            Console.Write("Enter a non-negative integer (0-20): ");

            //validity
            if (int.TryParse(Console.ReadLine(), out int number))
            {
                if (number < 0 || number > 20)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Numero non valido! Inserire un numero tra 0 e 20.");
                    Console.ResetColor();
                    continue;
                }
                //output
                long result = Factorial(number);
                Console.WriteLine($"The factorial of {number} is {result}");
                break;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Input non valido. Inserire un numero intero.");
                Console.ResetColor();
            }
        }
    }
}
