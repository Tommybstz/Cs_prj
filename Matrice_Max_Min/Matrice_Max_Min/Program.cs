class Program
{
    static void Main()
    {
        int N, M;

        Console.Write("Inserire la larghezza: ");
        while (!int.TryParse(Console.ReadLine(), out N) || N < 0) Console.WriteLine("Errore: valore non valido");

        Console.Write("Inserire l'altezza: ");
        while (!int.TryParse(Console.ReadLine(), out M) || M < 0) Console.WriteLine("Errore: valore non valido");

        var nums = new int[N, M];

        for (int i = 0; i < N; i++)
        {
            for (int j = 0; j < M; j++)
            {
                Console.Write($"Inserire il valore in posizione [{i + 1}, {j + 1}]: ");
                while (!int.TryParse(Console.ReadLine(), out nums[i, j]) || M < 0) Console.WriteLine("Errore: valore non valido");
            }
        }

        int xMax=0
            , yMax=0
            , xMin=0
            , yMin=0;

        for (int i = 0; i < N; i++)
        {
            for (int j = 0; j < M; j++)
            {
                if (nums[i, j] > nums[xMax, yMax])
                {
                    xMax = i;
                    yMax= j;
                }

                if (nums[i, j] < nums[xMin, yMin])
                {
                    xMin = i;
                    yMin = j;
                }
            }
        }



        Console.WriteLine($"Il numero maggiore si trova in posizione: [{xMax+1},{yMax+1}]\nIl numero minore si trova in posizione: [{xMin+1},{yMin+1}]");
    }
}