
class Program
{
    static void Main(string[] args)
    {
        //var
        string nome;
        double perc, prod, tot=0;

        //input
        while (true)
        {
            Console.WriteLine("inserire il nome del prodotto (inserire stop per fermare)");
            nome = Console.ReadLine();
            if (nome == null || nome == "")
            {
                continue;

            }
            if (nome.ToLower() == "stop")
            {
                break;
            }

            Console.WriteLine("inserire il prezzo del prodotto");
            if (!double.TryParse(Console.ReadLine(), out prod))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("valore non valido");
                Console.ResetColor();
                continue;
            }

            Console.WriteLine("inserire la percentuale di sconto");
            if (!double.TryParse(Console.ReadLine(), out perc))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("valore non valido");
                Console.ResetColor();
                continue;
            }
            prod-= prod * perc / 100;
            Console.WriteLine($"il prezzo del prodotto: {nome} e di {prod:F2} Euro");
            tot += prod;
        }
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"il totale e di {tot:F2} Euro");
        Console.ResetColor();
    }
}