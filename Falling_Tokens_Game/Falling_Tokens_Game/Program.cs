using System.Text;
using System.Threading;
using System.Diagnostics;
class Program
{
    private readonly static int gameWidth = 6, gameHeight = 10;

    private static char[,] gameZone = new char[gameHeight, gameWidth];
    private static Random Token = new Random();
    public static int score = 0;
    private static StringBuilder frame = new StringBuilder();
    private static int playerPosition = 4, oldPlayerPosition = 4;//starting position of the player in the middle of the game zone
    private static bool GameOver = false;
    private readonly static char player= '█', token= '☆', empty= ' ';
    private static int frameCounter = 0;
    private static Stopwatch stopwatch;
    private static int fps;


    //add a game loop that runs at a fixed frame rate and updates the game state and draws the game zone on the console
    //add a warm up phase so that the fps target does +10 until it reaches the target fps, for exapmle if the target fps is 60, the game would run at 34 fps but every frame it increases by 10 or 1 until the actual fps reaches the target fps, at the end the fpsIncreaser would be like 600 or something like that, and the game would run at 60 fps, which would be like hardtyping 600 in the target fps
    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.CursorVisible = false;

        Thread movePlayerInput= new Thread(MovePlayer);

        int fpsTarget = 600;
        int frameTime = 1000 / fpsTarget;

        double tokenSpawnTime = 1.75; // spawn a token every x seconds
        double tokenMoveTime = 0.75; // move tokens every x seconds
        double lastSpawn=0, lastFall=0;


        for (int row = 0; row < gameZone.GetLength(0); row++)
        {
            for (int col = 0; col < gameZone.GetLength(1); col++)
            {
                gameZone[row, col] = empty;
            }
        }

        movePlayerInput.Start();

        stopwatch=Stopwatch.StartNew();

        while (true)
        {
            long frameStart = stopwatch.ElapsedMilliseconds;
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
            fps = (int)(frameCounter / stopwatch.Elapsed.TotalSeconds);
            Draw();
            Console.WriteLine();
            frameCounter++;

            //Thread.Sleep(frameTime);
        }
        Console.WriteLine("game over");


    }

    static void MovePlayer()
    {
        gameZone[9, playerPosition] = player;
        while (true)
        {
            if (Console.KeyAvailable)//check if a key is pressed
            {
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
    }

    static void SpawnTokens()
    {
        int elementSpawnPoint;
        elementSpawnPoint = Token.Next(gameZone.GetLength(1));
        gameZone[0, elementSpawnPoint] = token;//spawn a token at the top of the game zone
    }   

    static void Draw()
    {
        //Build the frame to be drawn on the console
        Console.SetCursorPosition(0, 0);
        frame.AppendLine($"Score: {score}");
        for (int row = 0; row < gameZone.GetLength(0); row++)
        {
            frame.AppendLine(new string('─', gameZone.GetLength(1) * 4 + 1));

            for (int col = 0; col < gameZone.GetLength(1); col++)
            {
                frame.Append($"| {gameZone[row, col]} ");
            }
            frame.AppendLine("|");
        }
        frame.AppendLine(new string('─', gameZone.GetLength(1) * 4 + 1));
        frame.AppendLine($"FPS: {fps}");

        Console.Write(frame.ToString());
        frame.Clear();
    }
    static void Update()
    {
        for(int row = gameZone.GetLength(0)-2; row >= 0; row--)
        {
            for (int col = gameZone.GetLength(1)-1; col >= 0; col--)
            {
                if (gameZone[row, col] == token)
                {

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

}