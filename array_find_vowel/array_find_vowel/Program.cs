class Program
{
    static void Main()
    {
        //variables declaration
        int N;
        bool foundVowel = false;
        char[] vowels= { 'a', 'e', 'i', 'o', 'u', 'A', 'E', 'I', 'O', 'U' };

        //array size input
        Console.WriteLine("inserire quantita caratteri:");
        while (!int.TryParse(Console.ReadLine(), out N) || N <= 0)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("───────────── ERRORE ──────────────");
            Console.WriteLine("Errore: inserire un numero valido");
            Console.ResetColor();
        }

        //array declaration
        char[] arr = new char[N];

        //array elements input
        for (int i = 0; i < N; i++)
        {
            Console.WriteLine($"inserire carattere [{i + 1}/{N}]:");
            while (!char.TryParse(Console.ReadLine(), out arr[i]))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("───────────── ERRORE ──────────────");
                Console.WriteLine("Errore: inserire un carattere valido");
                Console.ResetColor();
            }
        }
        
        foreach (char ch in arr)
        {
            if(vowels.Contains(ch))
            {
                foundVowel=true;
                break;
            }
        }

        //output
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("───────────── OUTPUT ─────────────");
        Console.ResetColor();
        Console.WriteLine(foundVowel ? "Vocale presente":"Vocale non trovata");
    }
}