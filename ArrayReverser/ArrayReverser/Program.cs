class Program
{
    static void Main()
    {
        int N;
        Console.Write("Inserire il numero di elementi: ");
        while (!int.TryParse(Console.ReadLine(), out N) || N <= 0)
        {
            Console.Write("Per favore, inserisci un numero intero positivo: ");
        }

        int[] normal = new int[N];
        int[] reverse = new int[N];

        for (int i = 0; i < N; i++) {
            Console.WriteLine($"Inserire elemento n°{i+1}: ");
            while (!int.TryParse(Console.ReadLine(), out normal[i]))
            {
                Console.Write($"Per favore, inserisci un numero intero per l'elemento n°{i+1}: ");
            }
            reverse[N-1-i] = normal[i];
        }

        Console.WriteLine(string.Join(", ", normal));
        Console.WriteLine(string.Join(", ", reverse));
    }
}