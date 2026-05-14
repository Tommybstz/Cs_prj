class Program
{
    static void Main()
    {
        int size;
        Console.Write("inserire la grandezza della matrice: ");
        while(!int.TryParse(Console.ReadLine(), out size)||size<0) Console.WriteLine("Errore: inserire un numero positivo");

        var nums= new int[size,size];

        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                Console.Write($"Inserire il valore in posizione [{i+1},{j+1}]: ");
                while (!int.TryParse(Console.ReadLine(), out nums[i,j])) Console.WriteLine("Errore: inserire un numero intero");

            }
        }

        bool flag = true;
        for (int i = 0; i < size&& flag; i++)
        {
            for (int j = 0; j < size&& flag; j++)
            {
                if (i == j) continue;
                else if (!(nums[i, j] == 0)) flag = false;
            }
        }
        Console.WriteLine(!flag?"la matrice non è diagonale":"la matrice è diagonale");

    }
}