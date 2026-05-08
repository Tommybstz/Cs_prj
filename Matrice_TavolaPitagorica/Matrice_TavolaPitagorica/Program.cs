using Microsoft.VisualBasic;

class Program
{
    static void Main()
    {
        var tavolaPitagorica = new int[10, 10];

        //valori sopra e a destra 
        for (int i = 1; i < tavolaPitagorica.GetLength(0)+1; i++)
        {
            tavolaPitagorica[i-1,0] = i;
        }

        for (int i = 1; i < tavolaPitagorica.GetLength(0)+1; i++)
        {
            tavolaPitagorica[0,i-1] = i;
        }

        //riempimento e calcolo
        for(int i = 1;i < tavolaPitagorica.GetLength(0)+1; i++)
        {
            for (int j = 1; j < tavolaPitagorica.GetLength(0)+1; j++)
            {


        for (int x = 0; x < tavolaPitagorica.GetLength(0); x++)
                tavolaPitagorica[i-1, j-1] = tavolaPitagorica[0, j-1] * tavolaPitagorica[i-1, 0];
            }
        }

        for (int x = 0; x < tavolaPitagorica.GetLength(0); x++)
        {
            Console.WriteLine(new string('─', 10 * 5));

            for (int y = 0; y < tavolaPitagorica.GetLength(0); y++)
                Console.Write($"| {tavolaPitagorica[x, y]} ");

            Console.WriteLine("|");
        }
        Console.WriteLine(new string('─', 10 * 5));
    }
}