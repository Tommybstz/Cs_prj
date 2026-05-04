using System.ComponentModel;

class Program
{
    static void Main()
    {
        var trisTable = new char[3, 3];
        int inputX, inputY,x,y;
        char winner='T', turn;
        bool won=false;

        for(int match = 0; match < trisTable.Length; match++)
        {
            Console.Clear();

            turn = match % 2 == 0 ? 'X' : 'Y';//assign the turn in an alternated way

            //print the trisTable matrix as a table
            for ( x = 0; x < trisTable.GetLength(0); x++)
            {
                Console.WriteLine(new string('─', trisTable.GetLength(1) * 4));

                for (y = 0; y < trisTable.GetLength(1); y++) //iterazione colonne 
                {
                    Console.Write($"| {trisTable[x, y]} ");
                }
                Console.WriteLine("|");
            }
            Console.WriteLine(new string('─', trisTable.GetLength(1) * 4));

            while (true)
            {
                //input x axis
                Console.Write("Inserire le coordinate (asse x)[1-3]: ");
                while (!int.TryParse(Console.ReadLine(), out inputX) || inputX > 3 || inputX < 1)
                {
                    Message(ConsoleColor.Red, "[ERRORE]", "Valore non valido [1-3]");
                }

                //input y axis
                Console.Write("Inserire le coordinate (asse y[1-3]: ");
                while (!int.TryParse(Console.ReadLine(), out inputY) || inputY > 3 || inputY < 1)
                {
                    Message(ConsoleColor.Red,"[ERRORE]", "Valore non valido [1-3]");
                }

                // check if the slot is occupied, if it is it skips to the next loop
                if (trisTable[inputX - 1, inputY - 1] != '\0')
                {
                    Message(ConsoleColor.Red, "[ERRORE]", "Casella già occupata!");
                    continue;
                }

                break;
            }

            trisTable[inputX - 1, inputY - 1] = turn;



            //checks diagonals
            if (trisTable[0, 0] == trisTable[1, 1] && trisTable[0, 0] == trisTable[2, 2] && trisTable[0, 0] ==turn) //top-left -> bottom-right, and assign the winner X or O. 
            { 
                winner = turn;
                break;
            }
            if (trisTable[0, 2] == trisTable[1, 1] && trisTable[0, 2] == trisTable[2, 0] && trisTable[0, 2] == turn)//top-right ->bottom-left.
            {
                winner = turn;
                break;
            }

            //checks rows
            for (x = 0; x < trisTable.GetLength(0); x++)
            {
                if (trisTable[x, 0] == trisTable[x, 1] && trisTable[x, 0] == trisTable[x, 2] && trisTable[x, 0] == turn) 
                {
                    winner = turn;
                    won = true;
                    break;
                }
            }
            if (won) break;

            //checks columns
            for (y = 0; y < trisTable.GetLength(1); y++)
            {
                if (trisTable[0, y] == trisTable[1, y] && trisTable[0, y] == trisTable[2, y] && trisTable[0, y] == turn)
                {
                    winner = turn;
                    won = true;
                    break;
                }
            }
            if (won) break;

        }

        for(int i = 0,time=0; i < 4; i++,time+=400)
        {
            Console.Clear();
            Message(ConsoleColor.Yellow, "[WINNER]", $"Il vincitore è {new string('.',i)}");
            Thread.Sleep(time);
        }

        
        Console.WriteLine((winner=='T'?$"Pareggio!":$"{winner}!!").PadLeft(20));

    }
    static void Message(ConsoleColor color,string label,string message)
    {
        Console.ForegroundColor = color;
        Console.WriteLine($"{label.PadRight(12)} {message}");
        Console.ResetColor();
    }
}