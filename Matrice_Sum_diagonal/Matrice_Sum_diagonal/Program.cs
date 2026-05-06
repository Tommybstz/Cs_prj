class Program
{
    static void Main()
    {
        int L,somma=0;

        Console.Write("Inserire la grandezza della matrice (lato): ");
        while (!int.TryParse(Console.ReadLine(), out L) || L < 0) Console.WriteLine("Errore: valore non valido");

        var nums = new int[L, L];

        for (int i = 0; i < L; i++)
        {
            for (int j = 0; j < L; j++)
            {
                Console.Write($"Inserire l'elemento in posizione [{i + 1},{j + 1}]: ");
                while (!int.TryParse(Console.ReadLine(), out nums[i, j])) Console.WriteLine("Errore: valore non valido");
            }
        }

        for (int i=0,j = 0;j < L; i++,j++)
        {
            somma += nums[i,j];
        }

        Console.WriteLine($"La somma della diagonale principale è {somma}");

    }
}