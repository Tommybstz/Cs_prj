
using System.Threading;
using System.Diagnostics;
using System.Text.Json;
using System.Runtime.CompilerServices;
/*
 TODO:
    🔎 Search system
    📊 category breakdown
    💾 backup system
    ✏️ edit transaction
    

     create a folder for the data to keep cleaner
     and use the path combine when saving and loading to ensure it works on all operating systems. for now it just saves in the same directory as the application which is simpler but can get cluttered.
     add the datafile in the addtransaction and cancel transaction
     move the storage logic in a separate class
    and add way to cancel action (ex. you want to leave the transaction adder without adding a transaction or haveing to cancel it after)
    and to cancel a step (ex. im typing at the amount input but i typed the wrong type and i press a key to return to the type)
 */
namespace FinanceTracker
{
    public class FinanceTrackerApp
    {
        private List<Transaction> transactions;//list for the transactions
        private Dictionary<string, (string, Action)> menu;//dictionary for menu options
        private bool running = true;//bool to control menu loop inside Run method
        private int NextId = 1;//integer to assign unique IDs to transactions. starts at 1 and increments with each new transaction
        private readonly string dataFolder="UserData";
        private readonly string dataFile;

        //constructor to set up data file path
        public FinanceTrackerApp()
        {
            dataFile=Path.Combine(dataFolder, "FinanceTrackerData.json");//set the data file path to be inside the UserData folder. this keeps the application directory cleaner and allows for better organization of user data.
            Directory.CreateDirectory(dataFolder);//ensure the data folder exists. if it already exists, this does nothing
        }



        public void Run()
        {
            //l
            LoadData();

            //initialize menu with option number, description, and method to call
            menu = new Dictionary<string, (string, Action)>()
            {
                {"1", ("Add Transaction", AddTransaction)},
                {"2", ("Cancel most recent transaction", CancelTransaction)},
                {"3", ("View all transactions", ViewAll)},
                {"4", ("Filtered View", FilteredView)},
                {"5", ("View summary", ViewSummary)},
                {"6", ("Export & open CSV", ExportCsv)},
                {"7", ("Exit", () =>{ running = false; }) }
            };

            while (running)
            {
                //clear console for a clean menu display each time
                Console.Clear();

                //print menu
                foreach (var option in menu)
                {
                    Console.WriteLine($"{option.Key}. {option.Value.Item1}");
                }

                //check for valid input
                string input = Console.ReadLine().Trim();

                //try to get the selected option from the menu dictionary. if it doesn't exist, show error and continue loop
                if (!menu.TryGetValue(input, out var selection))
                {
                    Ui.Message(ConsoleColor.Red, "[ERROR]", "Invalid option. Please enter 1-7.");
                    Pause();
                    continue;  
                }

                //call the method associated with the selected option
                selection.Item2();

                //wait for user to press a key before showing the menu again(except for exit option which will skip this) so you have time to read any message or transaction.
                if (running)
                {
                    Pause();
                }

            }
            Ui.Message(ConsoleColor.Cyan, "[INFO]", "Exiting application. Goodbye!");
        }

        public void AddTransaction()
        {
            //new transaction object to hold user input
            Transaction transaction = new Transaction();

            //type input with validation
            Console.WriteLine("Enter transaction type (expense/-) or (income/+):");
            string type = Console.ReadLine().ToLower().Trim();

            while (type != "expense" && type != "-" && type != "income" && type != "+")
            {
                Ui.Message(ConsoleColor.Red, "[ERROR]", "Invalid input. Please enter 'expense/-' or 'income/+':");
                type = Console.ReadLine().ToLower().Trim();
            }

            if (type == "-")
            {
                type = "expense";
            }
            else if (type == "+")
            {
                type = "income";
            }

            transaction.Type = type;

            //category input with default to "generic" if left blank
            Console.Write("Enter transaction category: ");
            string category = Console.ReadLine().ToLower();

            if (string.IsNullOrEmpty(category))
            {
                category = "generic";
            }

            transaction.Category = category;

            //amount input with validation to ensure it's a positive decimal number
            Console.Write("Enter transaction amount: ");

            decimal amount;

            while (!decimal.TryParse(Console.ReadLine(), out amount) || amount <= 0)
            {
                Ui.Message(ConsoleColor.Red, "[ERROR]", "Invalid input. Please enter a positive number: ");
            }

            if (transaction.Type == "expense")
            {
                transaction.Amount = -amount; // expenses are negative
            }
            else
            {
                transaction.Amount = amount; // income is positive
            }

            //date input with validation, default to today's date if left blank
            Console.Write("Enter transaction date (yyyy-mm-dd) or press Enter to use today's date: ");

            DateTime date = DateTime.Now;

            string dateInput = Console.ReadLine();
            if (!string.IsNullOrEmpty(dateInput))
            {
                while (!DateTime.TryParse(dateInput, out date))
                {
                    Ui.Message(ConsoleColor.Red, "[ERROR]", "Invalid input. Please enter a valid date (yyyy-mm-dd) or press Enter to use today's date: ");
                    dateInput = Console.ReadLine();

                    if (string.IsNullOrEmpty(dateInput)) break;
                }
            }
            transaction.Date = date;

            //note input (optional)
            Console.WriteLine("Enter a note (optional): ");
            string note = Console.ReadLine();

            if (!string.IsNullOrEmpty(note))
            {
                transaction.Note = note;
            }
            else
            {
                transaction.Note = "";
            }

            //assign unique ID to transaction and increment NextId for the next transaction
            transaction.Id = NextId++;

            //add to list
            transactions.Add(transaction);

            //save to file (JSON)
            SaveData();
            Ui.Message(ConsoleColor.Green, "[SUCCESS]", "Transaction saved!.");

        }

        public void CancelTransaction()
        {
            //exit early if no transactions to remove
            if (transactions.Count == 0)
            {
                Ui.Message(ConsoleColor.Cyan, "[INFO]", "No transactions to remove.");
                return;
            }

            //confirm cancellation with user, showing details of most recent transaction
            Ui.Message(ConsoleColor.Yellow, "[WARN]", "This will remove the most recent transaction.");
            Ui.PrintTransactions(new[] { transactions.Last() });// Converts single transaction into IEnumerable for PrintTransactions compatibility
            Ui.Message(ConsoleColor.Yellow, "", "Are you sure? [y/n]");

            char confirm;

            while (!char.TryParse(Console.ReadLine().ToLower().Trim(), out confirm) || (confirm != 'y' && confirm != 'n')) Ui.Message(ConsoleColor.Red, "[ERROR]", "Invalid input. Please enter 'y' or 'n':");

            if (confirm == 'y')
            {
                transactions.RemoveAt(transactions.Count - 1);
                NextId = transactions.Count > 0 ? transactions.Max(t => t.Id) + 1 : 1; // update NextId after removal
                SaveData();
                Ui.Message(ConsoleColor.Green, "[SUCCESS]", "Most recent transaction removed.");
            }
            else
            {
                Ui.Message(ConsoleColor.Cyan, "[INFO]", "Transaction not removed.");
            }
        }
        public void ViewAll()
        {
            if (transactions.Count == 0)
            {
                Ui.Message(ConsoleColor.Cyan, "[INFO]", "No transactions to view.");
                return;
            }

            //display all transactions in a readable format
            Ui.PrintTransactions(transactions);
        }
        public void FilteredView()
        {
            if (transactions.Count == 0)
            {
                Ui.Message(ConsoleColor.Cyan, "[INFO]", "No transactions to filter.");
                return;
            }

            //date range

            string dateStartInput;
            string dateEndInput;

            DateTime dateStart = DateTime.MinValue;
            DateTime dateEnd = DateTime.MaxValue;

            Console.WriteLine("Enter start date (yyyy-mm-dd) or press Enter to skip: ");
            dateStartInput = Console.ReadLine();

            if (!string.IsNullOrEmpty(dateStartInput))
            {
                while (!DateTime.TryParse(dateStartInput, out dateStart))
                {
                    Ui.Message(ConsoleColor.Red, "[ERROR]", "Invalid input. Please enter a valid date (yyyy-mm-dd):");
                    dateStartInput = Console.ReadLine();
                }
            }

            Console.WriteLine("Enter end date (yyyy-mm-dd) or press Enter to skip: ");
            dateEndInput = Console.ReadLine();

            if (!string.IsNullOrEmpty(dateEndInput))
            {
                while (!DateTime.TryParse(dateEndInput, out dateEnd))
                {
                    Ui.Message(ConsoleColor.Red, "[ERROR]", "Invalid input. Please enter a valid date (yyyy-mm-dd):");
                    dateEndInput = Console.ReadLine();
                }
            }


            //category filter

            Console.WriteLine("Enter category to filter by or press Enter to skip: ");

            string categoryFilter = Console.ReadLine().ToLower().Trim();

            //filter and display transactions
            var filtered = transactions.Where(t => (string.IsNullOrEmpty(categoryFilter) || t.Category.Contains(categoryFilter)) && t.Date >= dateStart && t.Date <= dateEnd);

            if (!filtered.Any())
            {
                Ui.Message(ConsoleColor.Cyan, "[INFO]", "No transactions match the filter criteria.");
                return;
            }

            Ui.PrintTransactions(filtered);

        }
        public void ViewSummary()
        {
            decimal totalIncome = transactions.Where(t => t.Amount >= 0).Sum(t => t.Amount);
            decimal totalExpense = transactions.Where(t => t.Amount < 0).Sum(t => t.Amount);
            decimal balance = totalIncome + totalExpense;

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n================ SUMMARY ================");
            Console.ResetColor();

            Ui.Message(ConsoleColor.Cyan, "Balance", $"{balance:C}");
            Ui.Message(ConsoleColor.Green, "Income", $"{totalIncome:C}");
            Ui.Message(ConsoleColor.Red, "Expenses", $"{totalExpense:C}");
            Ui.BarChart(transactions);
        }
        public void ExportCsv()
        {
            //exit early if no transactions to export
            if (transactions.Count == 0)
            {
                Ui.Message(ConsoleColor.Cyan, "[INFO]", "No transactions to export.");
                return;
            }

            var csvLines = new List<string>() //create a list of strings. the first line will be the header which is what excel uses as column names, then each transaction will be a line in the csv file
            {

            "Type,Category,Amount,Date,Note"
            };

            foreach (var t in transactions)
            {
                csvLines.Add($"{t.Type},{t.Category},{t.Amount},{t.Date:d},\"{t.Note}\"");//format each transaction as a comma separated line. note is wrapped in quotes in case it contains commas
            }

            //write all lines to a file named FinanceTrackerData.csv in the same directory as the application. if the file already exists, it will be overwritten
            File.WriteAllLines("FinanceTrackerData.csv", csvLines);
            Ui.Message(ConsoleColor.Green, "[SUCCESS]", "Transactions exported to FinanceTrackerData.csv");

            //open the csv file with the default associated application (usually Excel)
            Process.Start(new ProcessStartInfo { FileName = "FinanceTrackerData.csv", UseShellExecute = true });

        }
        public void SaveData()
        {
            //save the transactions list to a file named FinanceTrackerData.json in the same directory as the application. if the file already exists, it will be overwritten
            File.WriteAllText("FinanceTrackerData.json", JsonSerializer.Serialize(transactions));
        }
        public void LoadData()
        {
            //load data from file if it exists, otherwise initialize empty list
            try
            {
                string json = File.ReadAllText("FinanceTrackerData.json");//read the contents of the file into a string variable. if the file doesn't exist, this will throw a FileNotFoundException which we catch below
                transactions = JsonSerializer.Deserialize<List<Transaction>>(json) ?? new List<Transaction>();//if deserialization returns null, initialize empty list. the ?? checks if null and if it is, it creates a new List<Transaction>()
                
                NextId= transactions.Any()? transactions.Max(t=>t.Id) + 1 : 1;//set the NextId to be one greater than the maximum existing ID in the loaded transactions, or 1 if there are no transactions. this ensures that new transactions will have unique IDs even after loading from a file.
            }
            catch (FileNotFoundException)
            {
                Ui.Message(ConsoleColor.Cyan, "[INFO]", "No save file found. Starting a new one.");//if the file doesn't exist, we just start with an empty list of transactions. this is not an error condition, so we show an info message and continue
                transactions = new List<Transaction>();
            }
            catch (JsonException)
            {
                Ui.Message(ConsoleColor.Red, "[ERROR]", "Save file is corrupted and cannot be read. Starting fresh.");//if the file is corrupted and can't be deserialized, we show an error message and start with an empty list.
                transactions = new List<Transaction>();

                //might add functionality in the future to backup the corrupted file with a timestamp so the user doesn't lose all their data, but for now we just start fresh if the file can't be read
            }
            catch (Exception)
            {
                Ui.Message(ConsoleColor.Red, "[ERROR]", "Unexpected error loading data. Starting fresh.");
                transactions = new List<Transaction>();
            }

        }
        public void Pause()
        {
            Ui.Message(ConsoleColor.DarkGray, "", "\nPress any key to continue...");
            Console.ReadKey();
        }
    }
}
