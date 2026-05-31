using System.Text;
using System.Threading;
using System.Diagnostics;
namespace Falling_Tokens_Game
{
    class Program
    {
        private const int gameWidth = 6, gameHeight = 10;
        private static char[,] gameZone = new char[gameHeight, gameWidth];

        //variables for the partial draw
        private static char[,] lastFrame = new char[gameHeight, gameWidth];
        private static int lastDrawnFps = -1;
        private static int lastDrawnScore = -1;

        private static StringBuilder frame = new StringBuilder();
        private static Random rng = new Random();

        //lock
        private static object lockObject = new object();

        //game variables
        private static int score = 0;
        private static int playerPosition = 4, oldPlayerPosition = 4;//starting position of the player in the middle of the game zone
        public volatile static bool GameOver = false;
        private readonly static char player = '☗', token = '★', bomb = '✸', empty = ' ';
        private static double tokenSpawnTime = 0.5; // spawn a token every 0.5 seconds at normal timing
        private static double tokenMoveTime = 1.5; // move tokens every 1.5 seconds

        //options
        private enum Difficulty
        {
            Easy,
            Normal,
            Hard,
            Nightmare
        }
        private static List<(string option, Action action)> menuOptions = new List<(string option, Action action)>
{
            ("Start",StartGame),
            ("ChangeDiffuculty",ChangeDifficulty),
            ("Quit",()=>Environment.Exit(0)),
        };

        //variables for fps 
        private static Stopwatch? stopwatch;
        private static int frameCounter = 0;
        private static int fps;
        private static Queue<int> fpsHistory = new Queue<int>();//queue to store the fps values for the graph
        private static string graphElements = "▁▂▃▄▅▆▇█";

        static void Main()
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.CursorVisible = false;

            MainMenu();

        }
        static void StartGame()
        {
            GameOver = false;
            score = 0;

            Thread movePlayerInput = new Thread(MovePlayer) { IsBackground = true };
            Thread music = new Thread(() => GameMusic.PlayMusic()) { IsBackground = true };




            for (int row = 0; row < gameZone.GetLength(0); row++)
            {
                for (int col = 0; col < gameZone.GetLength(1); col++)
                {
                    gameZone[row, col] = empty;
                    lastFrame[row, col] = '\0';
                }
            }

            music.Start();
            movePlayerInput.Start();

            stopwatch = Stopwatch.StartNew();

            gameZone[9, playerPosition] = player;

            DrawBorders();

            GameLoop();

            Console.SetCursorPosition(0, gameHeight * 2 + 5);
            Console.WriteLine("Game Over! Press any key to return to menu...");
            Console.ReadKey(true);
        }
        static void GameLoop()
        {
            double lastSpawn = 0,
                lastFall = 0,
                lastFpsTime = 0;
            int lastFpsCounter = 0;

            while (true)
            {
                if (stopwatch.Elapsed.TotalSeconds - lastSpawn > tokenSpawnTime) // spawn tokens at half the frame rate
                {
                    SpawnObjects();
                    lastSpawn = stopwatch.Elapsed.TotalSeconds;
                }
                if (stopwatch.Elapsed.TotalSeconds - lastFall > tokenMoveTime) // spawn tokens at half the frame rate
                {
                    Update();
                    lastFall = stopwatch.Elapsed.TotalSeconds;

                    if (GameOver) break;
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
        }
        static void MovePlayer()
        {
            while (true)
            {
                Thread.Sleep(5);
                if (!Console.KeyAvailable) continue;//check if a key is not pressed

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
                lock (lockObject)
                {
                    gameZone[gameZone.GetLength(0) - 1, oldPlayerPosition] = empty;//clear the old player position
                    gameZone[gameZone.GetLength(0) - 1, playerPosition] = player;
                }

            }
        }
        static void SpawnObjects()
        {
            int elementSpawnPoint;
            elementSpawnPoint = rng.Next(gameZone.GetLength(1));

            if (rng.NextDouble() < 0.8)
                gameZone[0, elementSpawnPoint] = token;//spawn a token at the top of the game zone
            else gameZone[0, elementSpawnPoint] = bomb;//spawn a bomb
        }
        static void DrawBorders()
        {
            Console.SetCursorPosition(0, 0);
            //border creation 
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
            if (score != lastDrawnScore)
            {
                Console.Write(score);
                lastDrawnScore = score;
            }

            for (int row = 0; row < gameZone.GetLength(0); row++)
            {

                for (int col = 0; col < gameZone.GetLength(1); col++)
                {
                    lock (lockObject)
                    {
                        if (gameZone[row, col] != lastFrame[row, col])
                        {

                            Console.SetCursorPosition(col * 4 + 2, row * 2 + 2);
                            Console.Write(gameZone[row, col]);
                            lastFrame[row, col] = gameZone[row, col];
                        }
                    }
                }
            }

            Console.SetCursorPosition(5, gameHeight * 2 + 2);
            if (fps != lastDrawnFps)
            {
                Console.Write(fps);
                lastDrawnFps = fps;
            }

            Console.SetCursorPosition(0, gameHeight * 2 + 4);
            GetFpsGraph();

        }
        static void GetFpsGraph()
        {
            if (fpsHistory.Count == 0) return;

            if (fpsHistory.Count > 20)
            {
                fpsHistory.Dequeue();
            }

            int maxFps = fpsHistory.Max(), indexElement;
            double fpsPerc;

            foreach (int fpsValue in fpsHistory)
            {
                fpsPerc = (double)fpsValue / maxFps;

                indexElement = (int)(fpsPerc * 7);


                Console.Write(graphElements[indexElement]);

            }

        }
        static void Update()//game logic
        {
            for (int row = gameZone.GetLength(0) - 2; row >= 0; row--)
            {
                for (int col = gameZone.GetLength(1) - 1; col >= 0; col--)
                {
                    lock (lockObject)
                    {
                        if (gameZone[row, col] != token && gameZone[row, col] != bomb) continue;//check if there is a token or a bomb at the current position, if not continue to the next position


                        if (row + 1 == gameZone.GetLength(0) - 1 && gameZone[row + 1, col] != player && (gameZone[row, col] == token))//checks if the token reached the bottom without the player catching
                        {
                            GameOver = true;
                        }
                        else if (gameZone[row, col] == bomb && (row + 1 == gameZone.GetLength(0) - 1 && gameZone[row + 1, col] == player))//checks if the player caught the bomb
                        {
                            GameOver = true;
                        }

                        else if (row + 1 == gameZone.GetLength(0) - 1 && gameZone[row + 1, col] == player)
                        {
                            score++;
                            gameZone[row, col] = empty;
                        }
                        else if (row + 1 == gameZone.GetLength(0) - 1 && gameZone[row + 1, col] != player && gameZone[row, col] == bomb)
                        {
                            gameZone[row, col] = empty;
                        }
                        else if (gameZone[row, col] == token)
                        {
                            gameZone[row, col] = empty;
                            gameZone[row + 1, col] = token;
                        }
                        else
                        {

                            gameZone[row, col] = empty;
                            gameZone[row + 1, col] = bomb;
                        }
                    }
                }
            }
        }
        static void MainMenu()
        {
            char selectionIndicator = '➪',
                selectionEmpty = '•';

            while (true)
            {
                Console.Clear();

                bool selected = false;
                int i = 1, iLast = 1;

                Console.WriteLine("MAIN MENU");

                foreach (var item in menuOptions)
                {
                    Console.WriteLine($"{selectionEmpty} {item.option}");
                }

                Console.SetCursorPosition(0, i);
                Console.Write(selectionIndicator);

                while (!selected)
                {
                    var key = Console.ReadKey(true).Key;

                    switch (key)
                    {

                        case ConsoleKey.UpArrow:
                            if (i > 1)
                            {
                                i--;
                            }
                            break;

                        case ConsoleKey.DownArrow:
                            if (i < menuOptions.Count)
                            {
                                i++;
                            }
                            break;

                        case ConsoleKey.Enter:
                            selected = true;
                            break;

                        default:
                            break;
                    }

                    Console.SetCursorPosition(0, iLast);
                    Console.WriteLine(selectionEmpty + " ");
                    Console.SetCursorPosition(0, i);
                    Console.Write(selectionIndicator);
                    iLast = i;

                }

                Console.Clear();
                menuOptions[i - 1].action();
            }

        }
        static void ChangeDifficulty()
        {
            char selectionIndicator = '➪',
                selectionEmpty = '•';
            bool selected = false;
            int i = 1, iLast = 1;

            Console.WriteLine("Change the difficulty with the up and down arrows or press enter to select:");

            foreach (Difficulty d in Enum.GetValues(typeof(Difficulty)))
            {
                Console.WriteLine($"{selectionEmpty} {d}");
            }

            Console.SetCursorPosition(0, i);
            Console.Write(selectionIndicator);

            while (!selected)
            {
                var key = Console.ReadKey(true).Key;

                switch (key)
                {

                    case ConsoleKey.UpArrow:
                        if (i > 1)
                        {
                            i--;
                        }
                        break;

                    case ConsoleKey.DownArrow:
                        if (i < Enum.GetValues(typeof(Difficulty)).Length)
                        {
                            i++;
                        }
                        break;

                    case ConsoleKey.Enter:
                        selected = true;
                        break;

                    default:
                        break;
                }

                Console.SetCursorPosition(0, iLast);
                Console.WriteLine(selectionEmpty + " ");
                Console.SetCursorPosition(0, i);
                Console.Write(selectionIndicator);
                iLast = i;

            }

            Difficulty selectedDifficulty = (Difficulty)(i - 1);

            switch (selectedDifficulty)
            {
                //time in seconds based on the difficulty selected by the player
                case Difficulty.Easy:
                    tokenMoveTime = 0.75;
                    tokenSpawnTime = 1.75;
                    break;

                case Difficulty.Normal:
                    tokenMoveTime = 0.5;
                    tokenSpawnTime = 1.5;
                    break;

                case Difficulty.Hard:
                    tokenMoveTime = 0.3;
                    tokenSpawnTime = 1.3;
                    break;

                case Difficulty.Nightmare:
                    tokenMoveTime = 0.15;
                    tokenSpawnTime = 1.15;
                    break;
            }
        }
    }
}