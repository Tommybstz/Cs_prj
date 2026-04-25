
/*
 TODO:
    🔎 Search system ...coming soon
    📊 category breakdown ...coming later
    💾 backup system  x
    ✏️ edit transaction x
    

   x  create a folder for the data to keep cleaner
   x  and use the path combine when saving and loading to ensure it works on all operating systems. for now it just saves in the same directory as the application which is simpler but can get cluttered.
   ?  add the datafile in the addtransaction and cancel transaction
   x  move the storage logic in a separate class
  x  and add way to cancel action (ex. you want to leave the transaction adder without adding a transaction or having to cancel it after)
  x  and to cancel a step (ex. im typing at the amount input but i typed the wrong type and i press a key to return to the type)

     add a transaction manager (a class that owns the data) so it's safer for the data
 */
using System.Transactions;

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
                {"4", ("View all transactions", ViewAll)},
                {"5", ("Filtered View", FilteredView)},
                {"6", ("View summary", ViewSummary)},
                {"7", ("Export & open CSV", () => storage.ExportCsv(manager.Transactions.ToList()))},
                {"8", ("Exit", () =>{ running = false; }) }
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
                string input = Console.ReadLine()?.Trim()??"";

                //try to get the selected option from the menu dictionary. if it doesn't exist, show error and continue loop
                if (!menu.TryGetValue(input, out var selection))
                {
                    Ui.Message(ConsoleColor.Red, "[ERROR]", $"Invalid option. Please enter 1-{menu.Count}.");
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
            Ui.Message(ConsoleColor.Cyan, "[INFO]", "Type 'cancel' at any time to return to the menu.");

            string type = Console.ReadLine()?.ToLower().Trim()??"";

            if (type == "cancel") return;

            while (type != "expense" && type != "-" && type != "income" && type != "+")
            {
                Ui.Message(ConsoleColor.Red, "[ERROR]", "Invalid input. Please enter 'expense/-' or 'income/+':");

                type = Console.ReadLine()?.ToLower().Trim()??"";

                if (type == "cancel") return;
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
            Ui.Message(ConsoleColor.Cyan, "[INFO]", "Type 'cancel' at any time to return to the menu.");

            string category = Console.ReadLine()?.ToLower()??"";

            if (string.IsNullOrEmpty(category))
            {
                category = "generic";
            }

            transaction.Category = category;

            //amount input with validation to ensure it's a positive decimal number
            Console.Write("Enter transaction amount: ");
            Ui.Message(ConsoleColor.Cyan, "[INFO]", "Type 'cancel' at any time to return to the menu.");

            decimal amount;
            
            string amountInput = Console.ReadLine()??"";

            if (amountInput?.Trim().ToLower() == "cancel") return;

            while (!decimal.TryParse(amountInput, out amount) || amount <= 0)
            {
                Ui.Message(ConsoleColor.Red, "[ERROR]", "Invalid input. Please enter a positive number: ");

                amountInput = Console.ReadLine()??"";

                if (amountInput?.Trim().ToLower() == "cancel") return;
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
            Ui.Message(ConsoleColor.Cyan, "[INFO]", "Type 'cancel' at any time to return to the menu.");

            DateTime date = DateTime.Now;

            string dateInput = Console.ReadLine()??"";
            if (!string.IsNullOrEmpty(dateInput))
            {
                if (dateInput.Trim().ToLower() == "cancel") return;

                while (!DateTime.TryParse(dateInput, out date))
                {
                    Ui.Message(ConsoleColor.Red, "[ERROR]", "Invalid input. Please enter a valid date (yyyy-mm-dd) or press Enter to use today's date: ");
                    dateInput = Console.ReadLine()??"";

                    if (string.IsNullOrEmpty(dateInput))
                    {
                        date = DateTime.Now;
                        break;
                    }

                    if (dateInput.Trim().ToLower() == "cancel") return;

                }
            }
            transaction.Date = date;

            //note input (optional)
            Console.WriteLine("Enter a note (optional): ");
            Ui.Message(ConsoleColor.Cyan, "[INFO]", "Type 'cancel' at any time to return to the menu.");

            string note = Console.ReadLine()??"";

            if (!string.IsNullOrEmpty(note))
            {
                if (note.Trim().ToLower() == "cancel") return;
                transaction.Note = note ?? "";
            }

            //add to list
            manager.Add(transaction);

            //save to file (JSON)
            storage.SaveData(manager.Transactions.ToList());
            Ui.Message(ConsoleColor.Green, "[SUCCESS]", "Transaction saved!.");

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
        public void EditTransaction()
        {
            int id;
            string idInput;

            if (manager.Transactions.Count == 0)
            {
                Ui.Message(ConsoleColor.Cyan, "[INFO]", "No transactions to edit.");
                return;
            }

            Console.Write("Enter Id to edit: ");
            idInput = Console.ReadLine()?.Trim().ToLower()??"";

            if (idInput == "cancel") return;

            while (!int.TryParse(idInput, out id)|| manager.GetById(id)==null){

                Ui.Message(ConsoleColor.Red, "[ERROR]", "Invalid ID. Please enter a valid transaction ID:");
                idInput = Console.ReadLine()?.Trim().ToLower()??"";
                if (idInput == "cancel") return;
            }

            Transaction? t=manager.GetById(id);//reference to the transaction 

            if (t == null)
            {
                Ui.Message(ConsoleColor.Red, "[ERROR]", "Transaction not found.");
                return;
            }

            Console.Write("Enter field to edit (type, category, amount, date, note): ");
            string field = Console.ReadLine()?.Trim().ToLower()??"";

            switch (field)
            {
                case "type":
                    //edit type
                    Console.WriteLine("Enter transaction type (expense/-) or (income/+):");
                    Ui.Message(ConsoleColor.Cyan, "[INFO]", "Type 'cancel' at any time to return to the menu.");

                    string type = Console.ReadLine()?.ToLower().Trim()??"";

                    if (type == "cancel") return;

                    while (type != "expense" && type != "-" && type != "income" && type != "+")
                    {
                        Ui.Message(ConsoleColor.Red, "[ERROR]", "Invalid input. Please enter 'expense/-' or 'income/+':");

                        type = Console.ReadLine()?.ToLower().Trim()??"";

                        if (type == "cancel") return;
                    }

                    t.Type = (type == "-") ? "expense" : (type == "+") ? "income":type;

                    t.Amount= (t.Type =="expense")? -Math.Abs(t.Amount):Math.Abs(t.Amount);//update the value 

                    break;

                case "category":
                    //edit category
                    Console.Write("Enter transaction category: ");
                    Ui.Message(ConsoleColor.Cyan, "[INFO]", "Type 'cancel' at any time to return to the menu.");

                    string category = Console.ReadLine()?.ToLower()??"";

                    if (string.IsNullOrEmpty(category))
                    {
                        category = "generic";
                    }

                    t.Category = category;

                    break;

                case "amount":
                    //edit amount
                    decimal amount;

                    Console.Write("Enter transaction amount: ");
                    Ui.Message(ConsoleColor.Cyan, "[INFO]", "Type 'cancel' at any time to return to the menu.");

                    string amountInput = Console.ReadLine()??"";

                    if (amountInput?.Trim().ToLower() == "cancel") return;

                    while (!decimal.TryParse(amountInput, out amount) || amount <= 0)
                    {
                        Ui.Message(ConsoleColor.Red, "[ERROR]", "Invalid input. Please enter a positive number: ");

                        amountInput = Console.ReadLine()??"";

                        if (amountInput?.Trim().ToLower() == "cancel") return;
                    }

                    t.Amount = (t.Type == "expense") ? -amount : amount;

                    break;
                   


                case "date":
                    //edit date
                    Console.Write("Enter transaction date (yyyy-mm-dd) or press Enter to use today's date: ");
                    Ui.Message(ConsoleColor.Cyan, "[INFO]", "Type 'cancel' at any time to return to the menu.");

                    string dateInput = Console.ReadLine()?.Trim()??"";
                    if (dateInput.ToLower() == "cancel") return;

                    DateTime date;

                    while (!DateTime.TryParse(dateInput, out date))
                    {
                        Ui.Message(ConsoleColor.Red, "[ERROR]", "Invalid input. Please enter a valid date (yyyy-mm-dd):");
                        dateInput = Console.ReadLine()?.Trim()??"";
                        if (dateInput.ToLower() == "cancel") return;//it returns early so the value stays the same
                    }

                    t.Date = date;

                    break;


                case "note":
                    //edit note
                    Console.WriteLine("Enter a note (optional): ");
                    Ui.Message(ConsoleColor.Cyan, "[INFO]", "Type 'cancel' at any time to return to the menu.");

                    string note = Console.ReadLine()??"";

                    if (!string.IsNullOrEmpty(note))
                    {
                        if (note.Trim().ToLower() == "cancel") return;//returns early so it stays the same
                        t.Note = note ?? "";
                    }
                    break;

                default:
                    Ui.Message(ConsoleColor.Red, "[ERROR]", "Invalid field.");
                    return;
            }
            storage.SaveData(manager.Transactions.ToList());
            Ui.Message(ConsoleColor.Green, "[SUCCESS]", "Transaction saved!.");

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
            if (manager.Transactions.Count == 0)
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
            dateStartInput = Console.ReadLine()??"";

            if (!string.IsNullOrEmpty(dateStartInput))
            {
                while (!DateTime.TryParse(dateStartInput, out dateStart))
                {
                    Ui.Message(ConsoleColor.Red, "[ERROR]", "Invalid input. Please enter a valid date (yyyy-mm-dd):");
                    dateStartInput = Console.ReadLine()??"";
                }
            }

            Console.WriteLine("Enter end date (yyyy-mm-dd) or press Enter to skip: ");
            dateEndInput = Console.ReadLine() ?? "";

            if (!string.IsNullOrEmpty(dateEndInput))
            {
                while (!DateTime.TryParse(dateEndInput, out dateEnd))
                {
                    Ui.Message(ConsoleColor.Red, "[ERROR]", "Invalid input. Please enter a valid date (yyyy-mm-dd):");
                    dateEndInput = Console.ReadLine() ?? "";
                }
            }


            //category filter

            Console.WriteLine("Enter category to filter by or press Enter to skip: ");

            string? categoryFilter = Console.ReadLine()?.ToLower().Trim();

            //filter and display transactions
            var filtered = manager.Transactions.Where(t => (string.IsNullOrEmpty(categoryFilter) || t.Category.Contains(categoryFilter)) && t.Date >= dateStart && t.Date <= dateEnd);

            if (!filtered.Any())
            {
                Ui.Message(ConsoleColor.Cyan, "[INFO]", "No transactions match the filter criteria.");
                return;
            }

            Ui.PrintTransactions(filtered);

        }
        public void ViewSummary()
        {
            decimal totalIncome = manager.Transactions.Where(t => t.Amount >= 0).Sum(t => t.Amount);
            decimal totalExpense = manager.Transactions.Where(t => t.Amount < 0).Sum(t => t.Amount);
            decimal balance = totalIncome + totalExpense;

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n================ SUMMARY ================");
            Console.ResetColor();

            Ui.Message(ConsoleColor.Cyan, "Balance", $"{balance:C}");
            Ui.Message(ConsoleColor.Green, "Income", $"{totalIncome:C}");
            Ui.Message(ConsoleColor.Red, "Expenses", $"{totalExpense:C}");
            Ui.BarChart(manager.Transactions.ToList());
        }
        public void Pause()
        {
            Ui.Message(ConsoleColor.DarkGray, "", "\nPress any key to continue...");
            Console.ReadKey();
        }
    }
}
