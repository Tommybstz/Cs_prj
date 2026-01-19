using System.Collections.Generic;
class Program
{
    static void Main(string[] args)
    {
        //var
        int N,dvd;
        List<int> vendite = new List<int>();

        //input
        Console.WriteLine("inserisci il numero di giorni:");
        N = int.Parse(Console.ReadLine());

        //calcolo
        for (int i =1; i <= N; i++)
        {
            Console.WriteLine($"inserisci il numero di dvd venduti nel giorno {i}:");
            dvd=int.Parse(Console.ReadLine());

            if (dvd > 10)
            {
                vendite.Add(i);
            }
        }

        //output
        Console.WriteLine($"i giorni con piu di 10 vendite sono: {string.Join(", ",vendite)}");


    }
}