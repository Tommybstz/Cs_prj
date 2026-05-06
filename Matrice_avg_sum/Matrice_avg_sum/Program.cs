class Program
{
    static void Main()
    {
        int N, M,somma=0;
        double media;

        Console.Write("Inserire la larghezza: ");
        while (!int.TryParse(Console.ReadLine(),out N)||N<0) Console.WriteLine("Errore: valore non valido");

        Console.Write("Inserire l'altezza: ");
        while (!int.TryParse(Console.ReadLine(), out M) || M < 0) Console.WriteLine("Errore: valore non valido");

        var nums = new int[N,M];

        for (int i = 0; i < N; i++)
        {
            for (int j = 0; j < M; j++)
            {
                Console.Write($"Inserire il valore in posizione [{i+1}, {j+1}]: ");
                while (!int.TryParse(Console.ReadLine(), out nums[i,j]) || M < 0) Console.WriteLine("Errore: valore non valido");
                somma += nums[i, j];
            }
        }

        media = (double) somma/nums.Length;

        Console.WriteLine($"La somma è: {somma}\nLa media è: {media}");
    }
}