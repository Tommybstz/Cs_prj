using Microsoft.VisualBasic;

class Program
{
    static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        //
        Console.WriteLine("🪙");//🪙 is the coin emoji
        var gameZone = new char[10, 10];

        for (int x = 0; x < gameZone.GetLength(0); x++)
        {
            for (int y = 0; y < gameZone.GetLength(1); y++)
            {
                gameZone[x, y] = ' ';
            }
        }

        int elementSpawn;
        Random Token = new Random();
        elementSpawn=Token.Next(gameZone.GetLength(0));
        Console.WriteLine(elementSpawn);

        for (int x = 0; x < gameZone.GetLength(0); x++)
        {
            Console.WriteLine(new string('─', gameZone.GetLength(1) * 4 + 1));

            for (int y = 0; y < gameZone.GetLength(1); y++) //iterazione colonne 
            {
                Console.Write($"| {gameZone[x, y]} ");
            }
            Console.WriteLine("|");
        }
        Console.WriteLine(new string('─', gameZone.GetLength(1) * 4+1));




        //provare con async per lo spostamento con caduta
        //


    }
}