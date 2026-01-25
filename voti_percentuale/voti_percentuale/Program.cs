using System.Collections.Generic;
class Program
{
       static void Main(string[] args)
    {
        //var
        Dictionary<string, int> voti = new Dictionary<string, int>
        {
            {"insufficiente"    ,0 },
            {"sufficiente"      ,0 },
            {"buono"            ,0 },
            {"ottimo"           ,0 },
        };
        int N;
        int voto;

        //input
        System.Console.Write("Quanti voti vuoi inserire?\n");
        N = int.Parse(System.Console.ReadLine());

        //process
        for (int i = 0; i < N; i++)
        {
            Console.Write($"\ninserire il voto numero {i + 1} :");
            voto = int.Parse(System.Console.ReadLine());
            if (voto < 0 || voto > 10)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Error.WriteLine("Errore: voto non valido (deve essere tra 0 e 10)");
                Console.ResetColor();
                i--;
            }
            else if (voto < 6)
            {
                voti["insufficiente"] += 1;
                Console.WriteLine("voto insufficiente aggiunto");
            }
            else if (voto == 6)
            {
                voti["sufficiente"] += 1;
                Console.WriteLine("voto sufficiente aggiunto");
            }
            else if (voto < 9)
            {
                voti["buono"] += 1;
                Console.WriteLine("voto buono aggiunto");
            }
            else
            {
                voti["ottimo"] += 1;
                Console.WriteLine("voto ottimo aggiunto");
            }

        }

        //output
        Console.WriteLine($"\nEcco la percentuale dei voti inseriti:");
        foreach(var item in voti)
        {
            double perc = (double)item.Value / N * 100;
            Console.WriteLine($"{item.Key} : {perc:F2}%");
        }
    }

}