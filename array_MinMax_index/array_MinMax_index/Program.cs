class Program
{
    static int MaxIndex(int[] array)
    {
        int maxindex = 0;
        for(int i=1;i< array.Length; i++)
        {
            if (array[i] > array[maxindex])
            {
                maxindex = i;
            }
        }
        return maxindex;

    }
    static int MinIndex(int[] array)
    {
        int minindex = 0;
        for (int i = 1; i < array.Length; i++)
        {
            if (array[i] < array[minindex])
            {
                minindex = i;
            }
        }
        return minindex;
    }
    static void Main()
    {
        int N;
      

        Console.WriteLine("inserire quantita elementi:");
        while(!int.TryParse(Console.ReadLine(), out N) || N <= 0) Console.WriteLine("Errore: valore non valido");

        int[] nums = new int[N];

        for (int i = 0; i < N; i++)
        {
            Console.WriteLine($"inserire elemento {i}:");
            while (!int.TryParse(Console.ReadLine(), out nums[i])) Console.WriteLine("Errore: valore non valido");
        }
        
        Console.WriteLine($"il valore massimo e {nums[MaxIndex(nums)]} che e l'elemento n°{MaxIndex(nums)}");
        Console.WriteLine($"il valore minimo e {nums[MinIndex(nums)]} che e l'elemento n°{MinIndex(nums)}");
    }
}