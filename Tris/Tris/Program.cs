using System.Runtime.InteropServices;

class Program
{
    static void Main()
    {
            while (true)
            {
                
                char position;
                Console.Write("premere un tasto sul tastierino numerico per selezionare la posizione: ");
                position=Console.ReadKey().KeyChar;
                //input x axis
                Console.Write("Inserire le coordinate (asse x)[1-3]: ");
                while (!int.TryParse(Console.ReadLine(), out inputX) || inputX > 3 || inputX < 1)
                switch (position)
                {
                    Message(ConsoleColor.Red, "[ERRORE]", "Valore non valido [1-3]");
                    case '1':
                        inputX = 2;
                        inputY = 0;
                        break;

                    case '2':
                        inputX = 2;
                        inputY = 1;
                        break;

                    case '3':
                        inputX = 2;
                        inputY = 2;
                        break;

                    case '4':
                        inputX = 1;
                        inputY = 0;
                        break;

                    case '5':
                        inputX = 1;
                        inputY = 1;
                        break;

                    case '6':
                        inputX = 1;
                        inputY = 2;
                        break;

                    case '7':
                        inputX = 0;
                        inputY = 0;
                        break;

                    case '8':
                        inputX = 0;
                        inputY = 1;
                        break;

                    case '9':
                        inputX = 0;
                        inputY = 2;
                        break;

                    default:
                        Console.WriteLine("posizione non valida");
                        continue;
                }

                //input y axis
                Console.Write("Inserire le coordinate (asse y[1-3]: ");
                while (!int.TryParse(Console.ReadLine(), out inputY) || inputY > 3 || inputY < 1)
                {
                    Message(ConsoleColor.Red,"[ERRORE]", "Valore non valido [1-3]");
                }

                // check if the slot is occupied, if it is it skips to the next loop
                if (trisTable[inputX - 1, inputY - 1] != '\0')
                if (trisTable[inputX , inputY ] != '\0')
                {
                    Message(ConsoleColor.Red, "[ERRORE]", "Casella già occupata!");
                    continue;
                break;
            }

            trisTable[inputX - 1, inputY - 1] = turn;
            trisTable[inputX, inputY] = turn;

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

        //print table again
        Console.Clear();
        Message(ConsoleColor.Cyan, "[RESULT]", "");
        for (x = 0; x < trisTable.GetLength(0); x++)
        {
            Console.WriteLine(new string('─', trisTable.GetLength(1) * 4));
            for (y = 0; y < trisTable.GetLength(1); y++)
                Console.Write($"| {trisTable[x, y]} ");
            Console.WriteLine("|");
        }
        Console.WriteLine(new string('─', trisTable.GetLength(1) * 4));

        Thread.Sleep(2500);

        for (int i = 0,time=0; i < 4; i++,time+=400)
        {
            Console.Clear();
            Message(ConsoleColor.Yellow, "[WINNER]", $"Il vincitore è {new string('.',i)}");
            Thread.Sleep(time);
        }

        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine((winner=='T'?$"Pareggio!":$"{winner}!!").PadLeft(20));
        Console.ResetColor();

    }
    static void Message(ConsoleColor color,string label,string message)
    {
        Console.ForegroundColor = color;
        Console.WriteLine($"{label.PadRight(12)} {message}");
        Console.ResetColor();
    }
