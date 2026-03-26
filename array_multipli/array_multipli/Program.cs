class Program
{
    static int SommaPari(int[] array) {
        int somma = 0;
        for (int i = 0; i < array.Length; i++) {
            if (array[i] % 2 == 0) {
                somma += array[i];
            }
        }
        return somma;
    }

    static int SommaDispari(int[] array)
    {
        int somma = 0;
        for (int i = 0; i < array.Length; i++)
        {
            if (i % 2 == 1)
            {
                somma += array[i];
            }
        }
        return somma;
    }

    static int SommaM3(int[] array)
    {
        int somma = 0;
        for (int i = 0; i < array.Length; i++)
        {
            if (array[i] % 3 == 0)
            {
                somma += array[i];
            }
        }
        return somma;
    }
    static void Main()
    {
        int N;

        Console.WriteLine("Inserisci quantita elementi:");
        while (!int.TryParse(Console.ReadLine(), out N) || N <= 0) Console.WriteLine("Inserisci un numero intero positivo:");

        int[] array = new int[N];

        for (int i = 0; i < N; i++)
        {
            Console.WriteLine($"Inserisci elemento {i + 1}:");
            while (!int.TryParse(Console.ReadLine(), out array[i])) Console.WriteLine("Inserisci un numero intero:");
        }

        Console.WriteLine($"La somma dei numeri pari è: {SommaPari(array)}");
        Console.WriteLine($"La somma dei numeri dispari è: {SommaDispari(array)}");
        Console.WriteLine($"La somma dei numeri multipli di 3 è: {SommaM3(array)}");
    }
}
