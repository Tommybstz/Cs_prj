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
        private static object lockObject = new object();// ensures thread-safe access to gameZone, only one thread can access it at a time

        //game variables
        private static int score = 0;
        private static int playerPosition = 4, oldPlayerPosition = 4;//starting position of the player in the middle of the game zone
        public volatile static bool GameOver = false;
        private readonly static char player = '☗', token = '★', bomb = '✸', empty = ' ';
        private static double tokenSpawnTime = 1.5; // spawn a token every 1.5 seconds at normal timing
        private static double tokenMoveTime = 0.5; // move tokens every 0.5 seconds

        //options
        private static char selectionIndicator = '➪',
            selectionEmpty = '•';
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
            ("ChangeDifficulty",ChangeDifficulty),
            ("Settings",Settings),
            ("Quit",()=>Environment.Exit(0)),
        };
        private enum SettingOptions
        {
            MuteMusic,
            ReturnToMenu
        }
        private static bool musicMute = false;

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
            //reset all the variables
            GameOver = false;
            score = 0;
            frameCounter = 0;
            fps = 0;
            lastDrawnFps = -1;
            lastDrawnScore = -1;
            fpsHistory.Clear();

            //Threads
            Thread movePlayerInput = new Thread(MovePlayer) { IsBackground = true };
            Thread music = new Thread(() => GameMusic.PlayMusic()) { IsBackground = true };

            //initialize the game zone to empty
            for (int row = 0; row < gameZone.GetLength(0); row++)
            {
                for (int col = 0; col < gameZone.GetLength(1); col++)
                {
                    gameZone[row, col] = empty;
                    lastFrame[row, col] = '\0';
                }
            }

            //start threads
            if (!musicMute) music.Start();//if option activated
            movePlayerInput.Start();

            stopwatch = Stopwatch.StartNew();//stopwatch to calculate fps

            gameZone[gameHeight - 1, playerPosition] = player;//initialize player in gamezone

            DrawBorders();//draws the border for the game

            GameLoop();//timings for the spawn, update of the objects and fps check

            Console.SetCursorPosition(0, gameHeight * 2 + 5);
            Console.WriteLine("Game Over! Press any key to return to menu...");
            Console.ReadKey(true);
        }
        static void GameLoop()
        {
            //initialize variables for the timing
            double lastSpawn = 0,
                lastFall = 0,
                lastFpsTime = 0;
            int lastFpsCounter = 0;

            while (true)
            {
                //Thread.Sleep(1);
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
                if (stopwatch.Elapsed.TotalSeconds - lastFpsTime > 1)//checks fps every second
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

                lock (lockObject)//updates the player position
                {
                    gameZone[gameZone.GetLength(0) - 1, oldPlayerPosition] = empty;//clear the old player position
                    gameZone[gameZone.GetLength(0) - 1, playerPosition] = player;
                }

            }
        }
        static void SpawnObjects()
        {
            int elementSpawnPoint = rng.Next(gameZone.GetLength(1));//gets a random position

            lock (lockObject)
            {
                //80% chance to spawn a token and 20 to spawn a bomb
                if (rng.NextDouble() < 0.8) gameZone[0, elementSpawnPoint] = token;//spawn a token at the top of the game zone
                else gameZone[0, elementSpawnPoint] = bomb;//spawn a bomb
            }
        }
        static void DrawBorders()
        {
            Console.SetCursorPosition(0, 0);
            //border frame creation 
            frame.AppendLine($"Score: ");
            for (int row = 0; row < gameZone.GetLength(0); row++)
            {
                frame.AppendLine(new string('─', gameZone.GetLength(1) * 4 + 1));//line border

                //colums borders
                for (int col = 0; col < gameZone.GetLength(1); col++)
                {
                    frame.Append($"| {empty} ");
                }
                frame.AppendLine("|");//right border
            }
            frame.AppendLine(new string('─', gameZone.GetLength(1) * 4 + 1));//bottom border
            frame.AppendLine($"FPS: ");

            Console.Write(frame.ToString());//draws border
            frame.Clear();
        }
        static void Draw()//draw the game zone on the console, happens every frame
        {
            char[,] snapshot;
            lock (lockObject)
            {
                snapshot = (char[,])gameZone.Clone();//takes a snapshot of the gamezone
            }

            //updates the frame only if something changes
            if (score != lastDrawnScore)
            {
                Console.SetCursorPosition(7, 0);
                Console.Write(score);
                lastDrawnScore = score;
            }

            for (int row = 0; row < gameHeight; row++)
            {

                for (int col = 0; col < gameWidth; col++)
                {

                    if (snapshot[row, col] != lastFrame[row, col])
                    {

                        Console.SetCursorPosition(col * 4 + 2, row * 2 + 2);
                        Console.Write(snapshot[row, col]);
                        lastFrame[row, col] = snapshot[row, col];
                    }
                }
            }

            if (fps != lastDrawnFps)
            {
                Console.SetCursorPosition(5, gameHeight * 2 + 2);
                Console.Write(fps);
                lastDrawnFps = fps;
            }

            Console.SetCursorPosition(0, gameHeight * 2 + 4);
            GetFpsGraph();//draws teh graph for the fps

        }
        static void GetFpsGraph()
        {
            if (fpsHistory.Count == 0) return;//if the queue is empty it returns 

            if (fpsHistory.Count > 20)//when the queue is longer than 20 it dequques to make space for new values
            {
                fpsHistory.Dequeue();
            }

            int maxFps = fpsHistory.Max(),
                indexElement;
            double fpsPerc;

            foreach (int fpsValue in fpsHistory)
            {
                fpsPerc = (double)fpsValue / maxFps;//calculate the percent of the fps compared to the max

                indexElement = (int)(fpsPerc * 7);//gets the index for the char to place using percentual


                Console.Write(graphElements[indexElement]);//prints the char 

            }

        }
        static void Update()//game logic
        {
            for (int row = gameZone.GetLength(0) - 2; row >= 0; row--)
            {
                for (int col = gameZone.GetLength(1) - 1; col >= 0; col--)
                {
                    bool isToken, isBomb;
                    //checks if there is a bomb or a token and returns true or false 
                    lock (lockObject)
                    {
                        isToken = gameZone[row, col] == token;
                        isBomb = gameZone[row, col] == bomb;
                    }

                    if (!isToken && !isBomb) continue;//if there is a token or a bomb at the current position, if not continue to the next position

                    bool playerHere;
                    lock (lockObject) { playerHere = gameZone[row + 1, col] == player; }

                    if (row + 1 == gameZone.GetLength(0) - 1)
                    {

                        if (!playerHere && isToken) GameOver = true;//checks if the token reached the bottom without the player catching
                        else if (playerHere && isBomb) GameOver = true;//the player caught the bomb KABOOM!
                        else if (playerHere && isToken)//the player catches the token
                        {
                            score++;
                            lock (lockObject) { gameZone[row, col] = empty; }
                        }
                        else if (!playerHere && isBomb)//the player dodged the bomb 
                        {
                            lock (lockObject) { gameZone[row, col] = empty; }
                        }
                    }
                    else
                    {
                        //makes the item move down (fall)
                        lock (lockObject)
                        {
                            gameZone[row, col] = empty;
                            gameZone[row + 1, col] = isToken ? token : bomb;
                        }

                    }
                }
            }
        }
        static void MainMenu()
        {
            while (true)
            {
                Console.Clear();
                while(Console.KeyAvailable) Console.ReadKey(true);//clear the input buffer to prevent unwanted inputs when returning to the menu

                bool selected = false;
                int ind = 1;
                int indLast = 1;

                Console.WriteLine("MAIN MENU");

                foreach (var item in menuOptions)
                {
                    Console.WriteLine($"{selectionEmpty} {item.option}");
                }

                Console.SetCursorPosition(0, ind);
                Console.Write(selectionIndicator);

                while (!selected)//if an option has been selected exits the loop
                {
                    var key = Console.ReadKey(true).Key;

                    switch (key)
                    {

                        case ConsoleKey.UpArrow:
                            if (ind > 1)
                            {
                                ind--;
                            }
                            break;

                        case ConsoleKey.DownArrow:
                            if (ind < menuOptions.Count)
                            {
                                ind++;
                            }
                            break;

                        case ConsoleKey.Enter:
                            selected = true;
                            break;

                        default:
                            break;
                    }

                    Console.SetCursorPosition(0, indLast);
                    Console.WriteLine(selectionEmpty + " ");
                    Console.SetCursorPosition(0, ind);
                    Console.Write(selectionIndicator);
                    indLast = ind;

                }

                Console.Clear();
                while (Console.KeyAvailable) Console.ReadKey(true);

                menuOptions[ind - 1].action();
            }

        }
        static void ChangeDifficulty()
        {
            bool selected = false;
            int ind = 1;
            int indLast = 1;

            Console.WriteLine("Change the difficulty with the up and down arrows or press enter to select:");

            foreach (Difficulty d in Enum.GetValues(typeof(Difficulty)))
            {
                Console.WriteLine($"{selectionEmpty} {d}");
            }

            Console.SetCursorPosition(0, ind);
            Console.Write(selectionIndicator);

            while (!selected)
            {
                var key = Console.ReadKey(true).Key;

                switch (key)
                {

                    case ConsoleKey.UpArrow:
                        if (ind > 1)
                        {
                            ind--;
                        }
                        break;

                    case ConsoleKey.DownArrow:
                        if (ind < Enum.GetValues(typeof(Difficulty)).Length)
                        {
                            ind++;
                        }
                        break;

                    case ConsoleKey.Enter:
                        selected = true;
                        break;

                    default:
                        break;
                }

                Console.SetCursorPosition(0, indLast);
                Console.WriteLine(selectionEmpty + " ");
                Console.SetCursorPosition(0, ind);
                Console.Write(selectionIndicator);
                indLast = ind;

            }

            Difficulty selectedDifficulty = (Difficulty)(ind - 1);

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
        static void Settings()
        {
            while (true)
            {
                Console.Clear();

                bool selected = false;
                int ind = 1;
                int indLast = 1;

                Console.WriteLine("Settings");

                foreach (SettingOptions option in Enum.GetValues(typeof(SettingOptions)))
                {
                    Console.WriteLine($"{selectionEmpty} {option}");
                }

                Console.SetCursorPosition(0, ind);
                Console.Write(selectionIndicator);

                while (!selected)
                {
                    var key = Console.ReadKey(true).Key;

                    switch (key)
                    {

                        case ConsoleKey.UpArrow:
                            if (ind > 1)
                            {
                                ind--;
                            }
                            break;

                        case ConsoleKey.DownArrow:
                            if (ind < Enum.GetValues(typeof(SettingOptions)).Length)
                            {
                                ind++;
                            }
                            break;

                        case ConsoleKey.Enter:
                            selected = true;
                            break;

                        default:
                            break;
                    }

                    Console.SetCursorPosition(0, indLast);
                    Console.WriteLine(selectionEmpty + " ");
                    Console.SetCursorPosition(0, ind);
                    Console.Write(selectionIndicator);
                    indLast = ind;

                }

                SettingOptions selectedOption = (SettingOptions)(ind - 1);

                switch (selectedOption)
                {
                    //time in seconds based on the difficulty selected by the player
                    case SettingOptions.MuteMusic:
                        musicMute = !musicMute;//switch for music mute if true when selected becomes false
                        break;
                    case SettingOptions.ReturnToMenu:
                        return;
                }
            }
        }
    }
}