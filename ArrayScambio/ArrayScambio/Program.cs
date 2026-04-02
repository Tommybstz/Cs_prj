class Program
{
    static void Main()
    {
        int N,index1,index2,temp;

        Console.WriteLine("Inserisci la dimensione dell'array: ");
        while (!int.TryParse(Console.ReadLine(), out N) || N <= 0) Console.WriteLine("Errore: Valore non valido");

        int[] array = new int[N];

        Console.WriteLine("Inserisci gli elementi dell'array:");
        for (int i = 0; i < N; i++)
        {
            Console.Write($"Elemento {i+1}: ");
            while (!int.TryParse(Console.ReadLine(), out array[i])) Console.WriteLine("Errore: Valore non valido");
        }


        Console.WriteLine("Inserisci l'indice degli elementi da scambiare:");

        Console.Write("Indice 1: ");
        while (!int.TryParse(Console.ReadLine(), out index1) || index1 < 0 || index1 >= N) Console.WriteLine("Errore: Valore non valido");

        Console.Write("Indice 2: ");
        while (!int.TryParse(Console.ReadLine(), out index2) || index2 < 0 || index2 >= N) Console.WriteLine("Errore: Valore non valido");

        temp=array[index1];
        array[index1] = array[index2];
        array[index2] = temp;

        Console.WriteLine($"Array dopo lo scambio: {string.Join(',',array)}");

    }
}