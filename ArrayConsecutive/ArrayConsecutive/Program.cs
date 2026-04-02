class Program
{
    static void Main()
    {
        int N;
        bool found=false;

        Console.WriteLine("Inserire quantità elementi:");
        while (!int.TryParse(Console.ReadLine(), out N) || N <= 0) Console.WriteLine("Errore: inserire un numero valido");

        int[] arr = new int[N];

        for (int i = 0; i < N; i++)
        {
            Console.WriteLine($"Inserire elemento [{i + 1}/{N}]:");
            while (!int.TryParse(Console.ReadLine(), out arr[i])) Console.WriteLine("Errore: inserire un valore valido");
        }

        for (int i = 0; i < N - 2; i++)
        {
            if (arr[i] == arr[i + 1] && arr[i]== arr[i + 2])
            {
                found = true;
                break;
            }
            
        }

        if (!found) Console.WriteLine("Non sono presenti 3 elementi consecutivi uguali");
        else Console.WriteLine("Sono presenti 3 elementi consecutivi uguali");
    }
}
