
/*
 TODO:
    🔎 Search system ...kindof (filtered view)
    📊 category breakdown ...coming later
    💾 backup system  x
    ✏️ edit transaction x
    Delete Stress tester on release

   x  create a folder for the data to keep cleaner
   x  and use the path combine when saving and loading to ensure it works on all operating systems. for now it just saves in the same directory as the application which is simpler but can get cluttered.
   ?  add the datafile in the addtransaction and cancel transaction
   x  move the storage logic in a separate class
  x  and add way to cancel action (ex. you want to leave the transaction adder without adding a transaction or having to cancel it after)
  x  and to cancel a step (ex. im typing at the amount input but i typed the wrong type and i press a key to return to the type)

   x  add a transaction manager (a class that owns the data) so it's safer for the data
 */

using System.Reflection.Metadata.Ecma335;

namespace FinanceTracker
{
    public class FinanceTrackerApp
    {
        private TransactionManager manager = new TransactionManager();
        private Dictionary<string, (string, Action)> menu = new();//dictionary for menu options
        private bool running = true;//bool to control menu loop inside Run method
        private FileStorage storage = new FileStorage();

        public void Run()
        {
            manager.Load(storage.LoadData());

            //initialize menu with option number, description, and method to call
            menu = new Dictionary<string, (string, Action)>()
            {
                {"1", ("Add Transaction", AddTransaction)},
                {"2", ("Cancel most recent transaction", UndoLastTransaction)},
                {"3", ("Edit Transaction",EditTransaction)},
                {"4", ("Remove transaction ",RemoveById)},
                {"5", ("View all transactions", ViewAll)},
                {"6", ("Filtered View", FilteredView)},
                {"7", ("View summary", ViewSummary)},
                {"8", ("Export & open CSV", () => storage.ExportCsv(manager.Transactions.ToList()))},
                {"9", ("Exit", () =>{ running = false; }) }
            };

            while (running)
            {
                //clear console for a clean menu display each time
                Console.Clear(); // for testing:    try { Console.Clear(); } catch (IOException) { }

                //print menu
                foreach (var option in menu)
                {
                    Console.WriteLine($"{option.Key}. {option.Value.Item1}");
                }

                //check for valid input
                string input = Console.ReadLine()?.Trim()??"";

                //try to get the selected option from the menu dictionary. if it doesn't exist, show error and continue loop
                if (!menu.TryGetValue(input, out var selection))
                {
                    Ui.Message(ConsoleColor.Red, "[ERROR]", $"Invalid option. Please enter 1-{menu.Count}.");
                    Pause(); //for testing comment the pauses
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
            // get transaction type
            string? type = PromptType();
            if (type == null) return; // user cancelled

            // get category, defaults to "generic" if left blank
            string category = PromptCategory()??"generic";
            if (category == null) return;

            // get amount, must be a positive number
            decimal? amount = PromptAmount();
            if (amount == null) return; 

            // get date, defaults to today if left blank
            DateTime? date = PromptDate();
            if (date == null) return; 

            // get optional note
            string note = PromptNote();

            // build transaction object
            Transaction transaction = new Transaction
            {
                Type = type,
                Category = category,
                Amount = type == "expense" ? -amount.Value : amount.Value,
                Date = date.Value,
                Note = note
            };

            // add to manager and save to file
            manager.Add(transaction);
            storage.SaveData(manager.Transactions.ToList());
            Ui.Message(ConsoleColor.Green, "[SUCCESS]", "Transaction saved!");
        }
        public void UndoLastTransaction()
        {
            //exit early if no transactions to remove
            if (manager.Transactions.Count == 0)
            {
                Ui.Message(ConsoleColor.Cyan, "[INFO]", "No transactions to remove.");
                return;
            }

            //confirm cancellation with user, showing details of most recent transaction
            Ui.Message(ConsoleColor.Yellow, "[WARN]", "This will remove the most recent transaction.");
            Ui.PrintTransactions(new[] { manager.Transactions.Last() });// Converts single transaction into IEnumerable for PrintTransactions compatibility
            Ui.Message(ConsoleColor.Yellow, "", "Are you sure? [y/n]");

            char confirm;

            while (!char.TryParse(Console.ReadLine()?.ToLower().Trim(), out confirm) || (confirm != 'y' && confirm != 'n')) Ui.Message(ConsoleColor.Red, "[ERROR]", "Invalid input. Please enter 'y' or 'n':");

            if (confirm == 'y')
            {
                manager.Remove(manager.Transactions.Last().Id);

                storage.SaveData(manager.Transactions.ToList());

                Ui.Message(ConsoleColor.Green, "[SUCCESS]", "Most recent transaction removed.");
            }
            else
            {
                Ui.Message(ConsoleColor.Cyan, "[INFO]", "Transaction not removed.");
            }
        }
        public void RemoveById()
        {
            if (manager.Transactions.Count == 0) {
                Ui.Message(ConsoleColor.Cyan, "[INFO]", "No transactions to remove.");
                return;
            }

            int? id = PromptId();

            if (id == null) return;

            Transaction? t=manager.GetById(id.Value);//use .value because it's an int?

            Ui.Message(ConsoleColor.Yellow, "[WARN]", "This will remove the following transaction:");
            Ui.PrintTransactions(new[] { t! });
            Ui.Message(ConsoleColor.Yellow, "", "Are you sure? [y/n]");

            char confirm;
            while (!char.TryParse(Console.ReadLine()?.ToLower().Trim(), out confirm) || (confirm != 'y' && confirm != 'n'))
                Ui.Message(ConsoleColor.Red, "[ERROR]", "Invalid input. Please enter 'y' or 'n':");

            if (confirm == 'n') return;

            manager.Remove(id.Value);//removes the transaction

            //save data
            storage.SaveData(manager.Transactions.ToList());
            Ui.Message(ConsoleColor.Green, "[SUCCESS]", "Transaction removed!");
        }
        public void EditTransaction()
        {
            // exit early if no transactions to edit
            if (manager.Transactions.Count == 0)
            {
                Ui.Message(ConsoleColor.Cyan, "[INFO]", "No transactions to edit.");
                return;
            }

            // get transaction ID to edit
            int? id= PromptId();
            if(id == null) return;

            // get reference to the transaction
            Transaction? t = manager.GetById(id.Value);
            if (t == null)
            {
                Ui.Message(ConsoleColor.Red, "[ERROR]", "Transaction not found.");
                return;
            }

            // get field to edit
            Console.Write("Enter field to edit (type, category, amount, date, note): ");
            string field = Console.ReadLine()?.Trim().ToLower() ?? "";

            switch (field)
            {
                case "type":
                    // edit type and update amount sign accordingly
                    string? type = PromptType();
                    if (type == null) break; // keep old value
                    t.Type = type;
                    t.Amount = t.Type == "expense" ? -Math.Abs(t.Amount) : Math.Abs(t.Amount);
                    break;

                case "category":
                    // edit category, defaults to "generic" if left blank
                    t.Category = PromptCategory()??"generic";
                    break;

                case "amount":
                    // edit amount, keep sign based on type
                    decimal? amount = PromptAmount();
                    if (amount == null) break; // keep old value
                    t.Amount = t.Type == "expense" ? -amount.Value : amount.Value;
                    break;

                case "date":
                    // edit date, keep old value if cancelled
                    DateTime? date = PromptDate();
                    if (date == null) break; // keep old value
                    t.Date = date.Value;
                    break;

                case "note":
                    // edit optional note
                    t.Note = PromptNote();
                    break;

                default:
                    Ui.Message(ConsoleColor.Red, "[ERROR]", "Invalid field.");
                    return;
            }

            // save changes to file
            storage.SaveData(manager.Transactions.ToList());
            Ui.Message(ConsoleColor.Green, "[SUCCESS]", "Transaction saved!");
        }
        public void ViewAll()
        {
            if (manager.Transactions.Count == 0)
            {
                Ui.Message(ConsoleColor.Cyan, "[INFO]", "No transactions to view.");
                return;
            }

            //display all transactions in a readable format
            Ui.PrintTransactions(manager.Transactions);
        }
        public void FilteredView()
        {
            bool filterRunning=true;
            var activeFilters = new List<Func<Transaction,bool>>(); //contains the lambdas/functions
            var filterMenu = new Dictionary<string, (string, Func<Func<Transaction, bool>?>?)>()
            {
                {"1", ("Filter by type", FilterByType)},
                {"2", ("Filter by category", FilterByCategory)},
                {"3", ("Filter by amount", FilterByAmount)},
                {"4", ("Filter by date", FilterByDate)},
                {"5", ("Run", null)},
                {"6", ("Exit", null)}
            };

            if (manager.Transactions.Count == 0)
            {
                Ui.Message(ConsoleColor.Cyan, "[INFO]", "No transactions to filter.");
                return;
            }

            while (filterRunning)
            {
                //clear console for a clean menu display each time
                Console.Clear();

                //print menu
                foreach (var option in filterMenu)
                {
                    Console.WriteLine($"{option.Key}. {option.Value.Item1}");
                }

                //check for valid input
                string input = Console.ReadLine()?.Trim() ?? "";

                if (input == "6") return;
                if (input == "5") filterRunning=false;

                //try to get the selected option from the menu dictionary. if it doesn't exist, show error and continue loop
                if (!filterMenu.TryGetValue(input, out var selection))
                {
                    Ui.Message(ConsoleColor.Red, "[ERROR]", $"Invalid option. Please enter 1-{filterMenu.Count}.");
                    Pause();
                    continue;
                }

                // get the function
                if (selection.Item2 != null)
                {
                    var filter = selection.Item2();
                    if (filter != null)
                        activeFilters.Add(filter);
                }


            }

                //checks if list is empty
                if (activeFilters.Count == 0)
                {
                    Ui.Message(ConsoleColor.Cyan, "[INFO]", "No filters selected.");
                    return;
                }

                //apply the filters
                var filtered = manager.Transactions.AsEnumerable();//filtered list

                foreach (var filter in activeFilters)//loop trough the active filters and apply them
                {
                    filtered = filtered.Where(filter);//apply filter
                }

                Ui.PrintTransactions(filtered);
        }
        public void ViewSummary()
        {
            decimal totalIncome = manager.Transactions.Where(t => t.Amount >= 0).Sum(t => t.Amount);
            decimal totalExpense = manager.Transactions.Where(t => t.Amount < 0).Sum(t => t.Amount);
            decimal balance = totalIncome + totalExpense;
            int year = DateTime.Now.Year; 
            string yearInput;


            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n================ SUMMARY ================");
            Console.ResetColor();

            Ui.Message(ConsoleColor.Cyan, "Balance", $"{balance:C}");
            Ui.Message(ConsoleColor.Green, "Income", $"{totalIncome:C}");
            Ui.Message(ConsoleColor.Red, "Expenses", $"{totalExpense:C}");
            Ui.BarChart(manager.Transactions.ToList());

            Console.WriteLine("Enter the year you want to see or press enter to see this year:");
            yearInput = Console.ReadLine()?.Trim()??"";

            //gets year input and if its empty it leaves it as the current year
            if (!string.IsNullOrEmpty(yearInput)){
                while (!int.TryParse(yearInput, out year))
                {
                    Ui.Message(ConsoleColor.Red, "[ERROR]", "Invalid year. Please enter a valid year:");
                    yearInput = Console.ReadLine()?.Trim() ?? "";

                    if (string.IsNullOrEmpty(yearInput)) break;
                }
            }

            if (!manager.Transactions.Any(t => t.Date.Year == year))//checks if there are transactions for that year
            {
                Ui.Message(ConsoleColor.Cyan, "[INFO]", "No transactions found for that year.");
                return;
            }

            Ui.CategoryBreakDown(manager.Transactions.ToList(),year);
        }
        public void Pause()
        {
            Ui.Message(ConsoleColor.DarkGray, "", "\nPress any key to continue...");
            Console.ReadKey();
        }

        // --- helpers ---
        private int? PromptId()
        {
            Ui.PrintTransactions(manager.Transactions);
            Console.Write("Enter transaction ID: ");
            string input = Console.ReadLine()?.Trim().ToLower() ?? "";
            if (input == "cancel") return null;

            int id;
            while (!int.TryParse(input, out id) || manager.GetById(id) == null)
            {
                Ui.Message(ConsoleColor.Red, "[ERROR]", "Invalid ID. Please enter a valid transaction ID:");
                input = Console.ReadLine()?.Trim().ToLower() ?? "";
                if (input == "cancel") return null;
            }

            return id;
        }
        private string? PromptType()
        {
            Ui.Message(ConsoleColor.Cyan, "[INFO]", "Type 'cancel' at any time to return to the menu.");
            Console.Write("Enter transaction type (expense/-) or (income/+):");

            string input = Console.ReadLine()?.ToLower().Trim() ?? "";
            if (input == "cancel") return null;

            while (input != "expense" && input != "-" && input != "income" && input != "+")
            {
                Ui.Message(ConsoleColor.Red, "[ERROR]", "Invalid input. Please enter 'expense/-' or 'income/+':");
                input = Console.ReadLine()?.ToLower().Trim() ?? "";
                if (input == "cancel") return null;
            }

            return input == "-" ? "expense" : input == "+" ? "income" : input;
        }
        private string? PromptCategory()
        {
            Ui.Message(ConsoleColor.Cyan, "[INFO]", "Type 'cancel' at any time to return to the menu.");
            Console.Write("Enter transaction category: ");

            string input = Console.ReadLine()?.ToLower() ?? "";
            if (input == "cancel") return null;
            return string.IsNullOrEmpty(input) ? "generic" : input;
        }
        private decimal? PromptAmount()
        {
            Ui.Message(ConsoleColor.Cyan, "[INFO]", "Type 'cancel' at any time to return to the menu.");
            Console.Write("Enter transaction amount: ");

            string input = Console.ReadLine()?.Trim() ?? "";
            if (input.ToLower() == "cancel") return null;

            decimal amount;
            while (!decimal.TryParse(input, out amount) || amount <= 0)
            {
                Ui.Message(ConsoleColor.Red, "[ERROR]", "Invalid input. Please enter a positive number:");
                input = Console.ReadLine()?.Trim() ?? "";
                if (input.ToLower() == "cancel") return null;
            }

            return amount;
        }
        private DateTime? PromptDate()
        {
            Ui.Message(ConsoleColor.Cyan, "[INFO]", "Type 'cancel' at any time to return to the menu.");
            Console.Write("Enter transaction date (yyyy-mm-dd) or press Enter to use today's date: ");

            string input = Console.ReadLine()?.Trim() ?? "";
            if (input.ToLower() == "cancel") return null;

            if (string.IsNullOrEmpty(input)) return DateTime.Now;

            DateTime date;
            while (!DateTime.TryParse(input, out date))
            {
                Ui.Message(ConsoleColor.Red, "[ERROR]", "Invalid input. Please enter a valid date (yyyy-mm-dd) or press Enter for today:");
                input = Console.ReadLine()?.Trim() ?? "";
                if (input.ToLower() == "cancel") return null;
                if (string.IsNullOrEmpty(input)) return DateTime.Now;
            }

            return date;
        }
        private string PromptNote()
        {
            Ui.Message(ConsoleColor.Cyan, "[INFO]", "Type 'cancel' at any time to return to the menu.");
            Console.Write("Enter a note (optional): ");

            string input = Console.ReadLine()?.Trim().ToLower() ?? "";
            if (input.Trim().ToLower() == "cancel") return "";

            return input;
        }

        // --- Filtered View Helpers ---
        private Func<Transaction,bool>? FilterByDate()//returns true if the transaction matches the filter (it keeps it)
        {

            string dateStartInput;
            string dateEndInput;

            DateTime dateStart = DateTime.MinValue;
            DateTime dateEnd = DateTime.MaxValue;

            Ui.Message(ConsoleColor.Cyan, "[INFO]", "Type 'cancel' at any time to return to the menu.");
            Console.Write("Enter start date (yyyy-mm-dd) or press Enter to skip: ");
            dateStartInput = Console.ReadLine()?.Trim().ToLower() ?? "";
            if(dateStartInput=="cancel") return null;

            if (!string.IsNullOrEmpty(dateStartInput))
            {
                while (!DateTime.TryParse(dateStartInput, out dateStart))
                {
                    Ui.Message(ConsoleColor.Red, "[ERROR]", "Invalid input. Please enter a valid date (yyyy-mm-dd):");
                    dateStartInput = Console.ReadLine()?.Trim().ToLower() ?? "";
                    if(dateStartInput=="cancel") return null;
                }
            }

            Ui.Message(ConsoleColor.Cyan, "[INFO]", "Type 'cancel' at any time to return to the menu.");
            Console.Write("Enter end date (yyyy-mm-dd) or press Enter to skip: ");
            dateEndInput = Console.ReadLine()?.Trim().ToLower() ?? "";
            if (dateEndInput == "cancel") return null;

            if (!string.IsNullOrEmpty(dateEndInput))
            {
                while (!DateTime.TryParse(dateEndInput, out dateEnd))
                {
                    Ui.Message(ConsoleColor.Red, "[ERROR]", "Invalid input. Please enter a valid date (yyyy-mm-dd):");
                    dateEndInput = Console.ReadLine()?.Trim().ToLower() ?? "";
                    if (dateEndInput == "cancel") return null;
                }
            }
            return t=> t.Date>=dateStart && t.Date<=dateEnd;
        }
        private Func<Transaction, bool>? FilterByCategory()
        {
            Ui.Message(ConsoleColor.Cyan, "[INFO]", "Type 'cancel' at any time to return to the menu.");
            Console.Write("Enter category to filter by or press Enter to skip: ");

            string? categoryFilter = Console.ReadLine()?.ToLower().Trim();

            if(categoryFilter=="cancel") return null;

            if (string.IsNullOrEmpty(categoryFilter)) return t => true;
            return t=> t.Category.Contains(categoryFilter);
        }
        private Func<Transaction, bool>? FilterByAmount()
        {

            string amountInputMin;
            string amountInputMax;

            decimal amountMin = decimal.MinValue;
            decimal amountMax = decimal.MaxValue;

            Ui.Message(ConsoleColor.Cyan, "[INFO]", "Type 'cancel' at any time to return to the menu.");
            Console.Write("Enter the minumum amount: ");

            amountInputMin = Console.ReadLine()?.Trim().ToLower() ?? "";
            if (amountInputMin == "cancel") return null;

            if (!string.IsNullOrEmpty(amountInputMin))
            {
                while (!decimal.TryParse(amountInputMin, out amountMin))
                {
                    Ui.Message(ConsoleColor.Red, "[ERROR]", "Invalid input. Please enter a positive number:");
                    amountInputMin = Console.ReadLine()?.Trim().ToLower() ?? "";
                    if (amountInputMin == "cancel") return null;
                }
            }

            Ui.Message(ConsoleColor.Cyan, "[INFO]", "Type 'cancel' at any time to return to the menu.");
            Console.Write("Enter the maximum amount: ");

            amountInputMax = Console.ReadLine()?.Trim().ToLower() ?? "";
            if (amountInputMax == "cancel") return null;

            if (!string.IsNullOrEmpty(amountInputMax))
            {
                while (!decimal.TryParse(amountInputMax, out amountMax))
                {
                    Ui.Message(ConsoleColor.Red, "[ERROR]", "Invalid input. Please enter a positive number:");
                    amountInputMax = Console.ReadLine()?.Trim().ToLower() ?? "";
                    if (amountInputMin == "cancel") return null;
                }
            }

            return t=> Math.Abs(t.Amount) >= amountMin && Math.Abs(t.Amount)<=amountMax;
        }
        private Func<Transaction, bool>? FilterByType() 
        {
            string? type = PromptType();

            if (type == null) return null;

            return t => t.Type==type;
        }

    }
}
