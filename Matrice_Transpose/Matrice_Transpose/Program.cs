using System.Diagnostics;
using System.Xml.Linq;

class Program
{
    static void Main()
    {
        /*3.Data una matrice quadrata di dimensioni scelte dall'utente, controlla se la matrice è simmetrica
        Una matrice simmetrica è una matrice quadrata che ha la proprietà di essere la trasposta di se stessa. 
        Suggerimento: controlla le caratteristiche degli elementi nella matrice*/

        int size;

        Console.Write("Inserire grandezza: ");
        while (!int.TryParse(Console.ReadLine(), out size) || size < 0) Console.WriteLine("Errore: valore non valido");


        var nums = new int[size,size];
        var transposed = new int[size,size];

        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                Console.Write($"Inserire elemento [{i},{j}]: ");
                while (!int.TryParse(Console.ReadLine(), out nums[i,j])) Console.WriteLine("Errore: valore non valido");

            }
        }
        bool flag = true;

        for (int i = 0; i < size && flag; i++)
        {
            for (int j = 0; j < size && flag; j++)
            {

                if (nums[i,j] != nums[i, j]){
                    Console.WriteLine("La matrice non è simmetrica");
                    flag = false;
                }
            }
        }
        if(flag) Console.WriteLine("La matrice è simmetrica");
       
        Console.ReadKey();

    }
}