class Program
{
    static void Main(string[] args)
    {
        //var
        int citta,abitanti,over70=0;
        string nome;
        double perc;

        //process
        Console.WriteLine("Inserisci il numero di citta da analizzare:");
        citta = int.Parse(Console.ReadLine());
        
        for (int i = 0; i < citta; i++)
        {
            //input nome
            Console.WriteLine($"Inserisci il nome della citta {i + 1}:");
            nome =Console.ReadLine() ?? "";
            while (string.IsNullOrWhiteSpace(nome))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Error.WriteLine("Errore: il nome della citta non puo essere vuoto.");
                Console.ResetColor();
                Console.WriteLine($"Inserisci il nome della citta {i + 1}:");
                nome =Console.ReadLine() ?? "";
            }

            //input abitanti
            while (true)
            {
                Console.WriteLine($"Inserisci il numero di abitanti di {nome}:");
                abitanti = int.Parse(Console.ReadLine());
                if (abitanti <= 0)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Error.WriteLine("Errore: il numero di abitanti deve essere maggiore di zero.");
                    Console.ResetColor();
                    continue;
                }
                break;
            }

            //input over70
            while (true)
            {
                Console.WriteLine($"Inserisci il numero di abitanti over 70 di {nome}:");
                over70 = int.Parse(Console.ReadLine());
                if (over70 < 0 || over70 > abitanti)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Error.WriteLine("Errore: il numero di abitanti over 70 non puo essere maggiore degli abitanti totali");
                    Console.ResetColor();
                    continue;
                }
                break;
            }

            //calcolo percentuale e output
            perc = (double)over70/abitanti*100;
            Console.WriteLine($"La percentuale di abitanti over 70 di {nome} e pari al {perc:F2}%");
        }

    }

}
