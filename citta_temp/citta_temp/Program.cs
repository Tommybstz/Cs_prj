using System.Collections.Generic;

class Program
{
    static void Main(string[] args)
    {
        //var
        List<int> temp_max = new List<int> { };
        List<int> temp_min = new List<int> { };
        List<string> citta = new List<string> { };
        int temp, N;
        string nome;

        //input city qt
        while (true)
        {
            Console.WriteLine("inserire il numero di citta");
            if (int.TryParse(Console.ReadLine(), out N) && N > 0)
            {
                break;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Error.WriteLine("Errore: valore non valido");
                Console.ResetColor();
            }


        }
        //input city data
        for (int i = 0; i < N; i++)
        {
            //input name
            while (true)
            {
                Console.WriteLine("Inserire la città:");
                nome = Console.ReadLine();

                if (!string.IsNullOrWhiteSpace(nome))
                {
                    citta.Add(nome);
                    break;
                }

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Errore: nome non valido");
                Console.ResetColor();
            }

            //input max temp
            while (true)
            {
                Console.WriteLine("inserire la temperatura massima");
                if (int.TryParse(Console.ReadLine(), out temp))
                {
                    temp_max.Add(temp);
                    break;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Error.WriteLine("Errore: valore non valido");
                    Console.ResetColor();
                }
            }


            //input min temp
            while (true)
            {
                Console.WriteLine("inserire la temperatura minima");
                if (int.TryParse(Console.ReadLine(), out temp))
                {
                    temp_min.Add(temp);
                    break;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Error.WriteLine("Errore: valore non valido");
                    Console.ResetColor();
                }
            }

        }

        //calc min of max (above zero)
        int above_zero = int.MaxValue;
        int indexAboveZero = -1;

        for (int i = 0; i < temp_max.Count; i++)
        {
            if (temp_max[i] > 0 && temp_max[i] < above_zero)
            {
                above_zero = temp_max[i];
                indexAboveZero = i;
            }
        }

        Console.ForegroundColor = ConsoleColor.Blue;
        //output temp min
        Console.WriteLine($"Città con temperatura più bassa: {citta[temp_min.IndexOf(temp_min.Min())]} con {temp_min.Min()}°C");

        Console.ForegroundColor= ConsoleColor.DarkRed;
        //output temp max
        Console.WriteLine($"Città con temperatura più alta: {citta[temp_max.IndexOf(temp_max.Max())]} con {temp_max.Max()}°C");

        Console.ForegroundColor = ConsoleColor.Green;
        //output temp max_min above zero
        if (indexAboveZero != -1)
        {
            Console.WriteLine($"Città con temperatura massima più bassa sopra lo zero: {citta[indexAboveZero]} con {above_zero}°C");
        }
        else
        {
            Console.WriteLine("Nessuna città ha una temperatura massima sopra lo zero.");
        }
        Console.ResetColor();
    }
}