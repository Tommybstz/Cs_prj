using System.Collections.Generic;
public class Program
{
    public static void Main()
    {

        //var
        List<char> vocali = new List<char> {'a','e','i','o','u'};
        char lettera;
        //input
        Console.WriteLine("Inserisci una lettera:");
        lettera = char.Parse(Console.ReadLine());

        //logica e output
        if (vocali.Contains(lettera))
        {
            Console.WriteLine($"La lettera '{lettera}' è una vocale");
        }
        else
        {
            Console.WriteLine($"La lettera '{lettera}' è una consonante");
        }
    }
}