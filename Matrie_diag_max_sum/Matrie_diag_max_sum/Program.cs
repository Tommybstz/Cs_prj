class Program
{
    static void Main()
    {
        int N;

        Console.Write("Inserire grandezza matrice: ");
        while (!int.TryParse(Console.ReadLine(), out N) || N <= 0) Console.WriteLine("Errore: valore non valido");

        var nums = new int[N, N];

        for (int i = 0; i < N; i++)
        {
            for (int j = 0; j < N; j++)
            {
                Console.Write($"Inserire elemento [{i+1},{j+1}] matrice: ");
                while (!int.TryParse(Console.ReadLine(), out nums[i,j])) Console.WriteLine("Errore: valore non valido");
            }
        }

        int maxVal=int.MinValue, maxIndexX=0,maxIndexY=0, diagonalSum=0;

        for (int i = 0, j=0; i < N; i++,j++)
        {
            if (maxVal < nums[i, j])
            {
                maxVal = nums[i, j];
                maxIndexX = i;
                maxIndexY = j;
            }
            diagonalSum += nums[i, j];
        }

        Console.WriteLine($"La somma della diagonale è {diagonalSum}\nIl valore massimo è [{maxIndexX},{maxIndexY}] con un valore di {maxVal}");


    }
}