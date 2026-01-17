using System.Collections.Generic;
using System;
using System.Linq;
class Program
{
    static void Main()
    {
        //var
        double num;
        List<double> numbers = new List<double>();

        //input and storing
        for (int i = 0; i < 3; i++)
        {
            Console.WriteLine("inserire un numero");
            num=double.Parse(Console.ReadLine());
            numbers.Add(num);
        }
        //output max
        Console.WriteLine($"il valore più grande è: {numbers.Max()}");

    }
}