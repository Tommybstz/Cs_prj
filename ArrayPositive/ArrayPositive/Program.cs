class Program
{
    static void Main()
    {
        int N;
        Console.Write("Inserire la quantita di elementi: ");
        while (!int.TryParse(Console.ReadLine(), out N) || N <= 0)
        {
            Console.Write("Per favore, inserisci un numero intero positivo: ");
        }

        int[] numbers = new int[N];

        for (int i = 0; i < N; i++)
        {
            Console.Write($"Inserire l'elemento {i + 1}: ");
            while (!int.TryParse(Console.ReadLine(), out numbers[i]) || numbers[i] <= 0)
            {
                Console.Write("Errore:inserire un numero intero positivo: ");
            }
        }
    }
}