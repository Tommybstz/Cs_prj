class Program
{
    static void Main()
    {
        int N;

        Console.WriteLine("inserire quantita numeri:");
        while (!int.TryParse(Console.ReadLine(), out N)) Console.WriteLine("Errore: Valore non valido");

        Double[] arr = new double[N];

        for (int i = 0; i < N; i++)
        {
            Console.WriteLine($"inserire numero [{i + 1}/{N}]:");
            while (!double.TryParse(Console.ReadLine(), out arr[i])) Console.WriteLine("Errore: Valore non valido");
        }

        Console.WriteLine($"Media dei numeri dispari in posizioni pari: {AverageUnevenInEven(arr):F2}");
    }

    static double AverageUnevenInEven(Double[] numbers)
    {
        double sum = 0;
        int count = 0;

        for (int i = 0; i < numbers.Length; i++)
        {
            if (i % 2 == 0 && numbers[i] % 2 != 0)
            {
                sum += numbers[i];
                count++;
            }
        }
        if (count == 0) {
            return 0;
        }
        return sum / count;

    }
}