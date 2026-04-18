class Program
{
    static void Main()
    {
        int N;

        Console.Write("inserire la grandezza dell'array: ");

        while (!int.TryParse(Console.ReadLine(), out N))
            Console.WriteLine("Errore: valore non valido");

        int[] nums = new int[N];
        
        for (int i = 0; i < N; i++)
        {
            Console.Write($"Inserire il {i+1} elemento:");

            while (!int.TryParse(Console.ReadLine(), out nums[i])) 
                Console.WriteLine("Errore: valore non valido");
        }
        for (int i = 0; i < N/2; i++) {
            
            int temp = nums[i];

            nums[i] = nums[N - 1 - i];
            nums[N - 1 - i] = temp;
        }

        Console.WriteLine(string.Join(", ",nums));


    }
}