class Program {
    public static double Avg(int[] array)
    {
        int sum = 0;
        foreach (int num in array)
        {
            sum += num;
        }
        return (double)sum/array.Length;
    }

    static void Main(string[] args)
    {
        int N;

        Console.WriteLine("inserire numero elementi:");
        while (!int.TryParse(Console.ReadLine(), out N)) Console.WriteLine("Errore: valore non valido");

        int[] nums = new int[N];

        for (int i = 0; i < N; i++)
        {
            Console.WriteLine($"inserire elemento n: {i+1}");
            while (!int.TryParse(Console.ReadLine(), out nums[i])) Console.WriteLine("Errore: valore non valido");
        }
        Console.WriteLine($"La media e {Avg(nums):2F}");
    }
}