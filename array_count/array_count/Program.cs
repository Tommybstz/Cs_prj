class Program
{
    static void Main()
    {
        //variables declaration
        int N;
        char charToCount;
        int counter = 0;

        //array size input
        Console.WriteLine("inserire quantita caratteri:");
        while(!int.TryParse(Console.ReadLine(), out N) || N <= 0)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("───────────── ERRORE ──────────────");
            Console.WriteLine("Errore: inserire un numero valido");
            Console.ResetColor();
        }

        //array declaration
        char[] arr=new char[N];

        //array elements input
        for (int i=0; i<N; i++)
        {
            Console.WriteLine($"inserire carattere [{i+1}/{N}]:");
            while(!char.TryParse(Console.ReadLine(), out arr[i]))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("───────────── ERRORE ──────────────");
                Console.WriteLine("Errore: inserire un carattere valido");
                Console.ResetColor();
            }
        }

        //character to count input
        Console.WriteLine("inserire carattere da contare:");
        while(!char.TryParse(Console.ReadLine(), out charToCount))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("───────────── ERRORE ──────────────");
            Console.WriteLine("Errore: inserire un carattere valido");
            Console.ResetColor();
        }

        //counting
        foreach (char c in arr)
        {
            if (c == charToCount)
            {
                counter++;
            }

        }

        
        //output
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("───────────── OUTPUT ─────────────");
        Console.ResetColor();
        Console.WriteLine($"Il carattere '{charToCount}' appare {counter} volte nell'array.");
    }
}