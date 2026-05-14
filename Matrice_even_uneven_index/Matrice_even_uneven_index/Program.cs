class Program
{
    static void Main()
    {
        int N,M;

        Console.Write("Inserire altezza matrice: ");
        while (!int.TryParse(Console.ReadLine(), out N) || N <= 0) Console.WriteLine("Errore: valore non valido");

        Console.Write("Inserire larghezza matrice: ");
        while (!int.TryParse(Console.ReadLine(), out M) || M <= 0) Console.WriteLine("Errore: valore non valido");

        var nums = new int[N, M];

        for (int i = 0; i < N; i++)
        {
            for (int j = 0; j < N; j++)
            {
                int sommaInd = i+j;
                nums[i,j]=sommaInd%2==0?1:0;
            }
        }

        foreach (int num in nums) { Console.Write(num); }

    }
}