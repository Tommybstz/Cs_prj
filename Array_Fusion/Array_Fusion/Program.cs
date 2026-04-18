class Program
{
    static void Main()
    {
        int N;

        Console.Write("inserire grandezza array: ");

        while (!int.TryParse(Console.ReadLine(), out N))
            Console.WriteLine("Errore: valore non valido");

        string[] A = new string[N];

        for (int i = 0; i < N; i++)
        {
            Console.WriteLine($"inserire elemento n°{i + 1}");
            A[i] = Console.ReadLine();
        }

        int M;

        Console.Write("inserire grandezza array 2: ");

        while (!int.TryParse(Console.ReadLine(), out M))
            Console.WriteLine("Errore: valore non valido");

        string[] B = new string[M];

        for (int i = 0; i < M; i++)
        {
            Console.WriteLine($"inserire elemento n°{i + 1}");
            B[i] = Console.ReadLine();
        }

        string[] C = A.Concat(B).ToArray();
        Console.WriteLine(string.Join(", ", C));
    }
}