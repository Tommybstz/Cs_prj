using System.Text;
using System.Threading;
using System.Diagnostics;
class Program
{
    private readonly static int gameWidth = 6, gameHeight = 10;

    private  static char[,] gameZone = new char[gameHeight, gameWidth];
    private static char[,] lastFrame = new char[gameHeight, gameWidth];
    private static StringBuilder frame = new StringBuilder();

    private static Random Token = new Random();

    public static int score = 0;
    private static int playerPosition = 4, oldPlayerPosition = 4;//starting position of the player in the middle of the game zone
    private volatile static bool  GameOver = false;
    private readonly static char player= '█', token= '☆', empty= ' ';
    private readonly static double tokenSpawnTime = 1.75; // spawn a token every x seconds
    private readonly static double tokenMoveTime = 0.75; // move tokens every x seconds

    private static Stopwatch? stopwatch;
    private static int frameCounter = 0;
    private static int fps;
    private static Queue<int> fpsHistory = new Queue<int>();//queue to store the fps values for the graph
    private static string graphElements = "▁▂▃▄▅▆▇█";

    //"▁▂▃▄▅▆▇█" for the fps graph, add to a queue the fps value every second and if it's too big dequeue the oldest value,


    static void GetFpsGraph()
    {
        if (fpsHistory.Count == 0) return;

        if (fpsHistory.Count > 20)
        {
            fpsHistory.Dequeue();
        }

        int maxFps =fpsHistory.Max(), indexElement;
        double fpsPerc;

        foreach(int fpsValue in fpsHistory)
        {
            fpsPerc = (double)fpsValue / maxFps;

            indexElement = (int)(fpsPerc * 7);


            Console.Write(graphElements[indexElement]);

        }

    }

    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.CursorVisible = false;

        Thread movePlayerInput= new Thread(MovePlayer);

        double lastSpawn=0,
            lastFall=0,
            lastFpsTime=0;
        int lastFpsCounter = 0;


        for (int row = 0; row < gameZone.GetLength(0); row++)
        {
            for (int col = 0; col < gameZone.GetLength(1); col++)
            {
                gameZone[row, col] = empty;
                lastFrame[row, col] = '\0';
            }
        }


        movePlayerInput.Start();

        stopwatch=Stopwatch.StartNew();

        gameZone[9, playerPosition] = player;
        DrawBorders();

        while (true)
        {
            if (stopwatch.Elapsed.TotalSeconds-lastSpawn>tokenSpawnTime) // spawn tokens at half the frame rate
            {
                SpawnTokens();
                lastSpawn = stopwatch.Elapsed.TotalSeconds;
            }
            if (stopwatch.Elapsed.TotalSeconds-lastFall>tokenMoveTime) // spawn tokens at half the frame rate
            {
                Update(); 
                lastFall = stopwatch.Elapsed.TotalSeconds;
                if(GameOver) break;
            }
            if (stopwatch.Elapsed.TotalSeconds - lastFpsTime > 1)
            {
                fps = (int)(frameCounter - lastFpsCounter);

                lastFpsCounter = frameCounter;
                lastFpsTime = stopwatch.Elapsed.TotalSeconds;

                fpsHistory.Enqueue(fps);


            }
            Draw();
            frameCounter++;
        }

        Console.SetCursorPosition(0, gameHeight * 2 + 5);
        Console.WriteLine("game over");


    }

    static void MovePlayer()
    {
        while (true)
        {
            if ( ! Console.KeyAvailable) continue;//check if a key is not pressed

                var key = Console.ReadKey(true).Key;
                if (key == ConsoleKey.LeftArrow && playerPosition > 0)
                {
                    // Move player left
                    oldPlayerPosition = playerPosition--;
                }
                else if (key == ConsoleKey.RightArrow && playerPosition < gameZone.GetLength(1) - 1)
                {
                    // Move player right
                    oldPlayerPosition = playerPosition++;
                }
                else continue;
                gameZone[gameZone.GetLength(0) - 1, oldPlayerPosition] = empty;//clear the old player position
                gameZone[gameZone.GetLength(0) - 1, playerPosition]=player;
            

        }
    }

    static void SpawnTokens()
    {
        int elementSpawnPoint;
        elementSpawnPoint = Token.Next(gameZone.GetLength(1));
        gameZone[0, elementSpawnPoint] = token;//spawn a token at the top of the game zone
    }

    static void DrawBorders()
    {
        Console.SetCursorPosition(0, 0);
        frame.AppendLine($"Score: ");
        for (int row = 0; row < gameZone.GetLength(0); row++)
        {
            frame.AppendLine(new string('─', gameZone.GetLength(1) * 4 + 1));

            for (int col = 0; col < gameZone.GetLength(1); col++)
            {
                frame.Append($"| {empty} ");
            }
            frame.AppendLine("|");
        }
        frame.AppendLine(new string('─', gameZone.GetLength(1) * 4 + 1));
        frame.AppendLine($"FPS: ");

        Console.Write(frame.ToString());
        frame.Clear();
    }

    static void Draw()//draw the game zone on the console, happens every frame
    {
        //Build the frame to be drawn on the console
        Console.SetCursorPosition(7, 0);
        Console.Write(score);
        for (int row = 0; row < gameZone.GetLength(0); row++)
        {

            for (int col = 0; col < gameZone.GetLength(1); col++)
            {
                if(gameZone[row, col] != lastFrame[row, col])
                {
                    Console.SetCursorPosition(col * 4+2, row*2 + 2);
                    Console.Write(gameZone[row, col]);
                    lastFrame[row, col] = gameZone[row, col];
                }
            }
        }

        Console.SetCursorPosition(5, gameHeight * 2 + 2);
        Console.Write(fps);

        Console.SetCursorPosition(0, gameHeight * 2 + 4);
        GetFpsGraph();

    }

    static void Update()//game logic
    {
        for(int row = gameZone.GetLength(0)-2; row >= 0; row--)
        {
            for (int col = gameZone.GetLength(1)-1; col >= 0; col--)
            {
                if (gameZone[row, col] != token) continue;//check if there is a token at the current position, if not continue to the next position
                

                    if(row+1 == gameZone.GetLength(0)-1 && gameZone[row+1,col] != player)
                    {
                        GameOver = true;
                    }

                    else if(row + 1 == gameZone.GetLength(0)-1 && gameZone[row + 1, col] == player)
                    {
                        score++;
                        gameZone[row, col] = empty;
                    }
                    else
                    {

                        gameZone[row, col] = empty;
                        gameZone[row + 1, col] = token;

                    }
                
            }
        }
    }

}