
using System.Threading;
using System.Diagnostics;
using System.Text.Json;

namespace FinanceTracker
{
    public class FinanceTrackerApp
    {
        private List<Transaction> transactions;

        public void Run()
        {
            

            if (File.Exists("FinanceTrackerData.json"))
            {
                transactions = JsonSerializer.Deserialize<List<Transaction>>(File.ReadAllText("FinanceTrackerData.json"));
            }
            else
            {
                transactions = new List<Transaction>();
            }



            while (true)
            {
                //menu
                Console.WriteLine("\n1. Add Transaction");
                Console.WriteLine("2. View all transactions");
                Console.WriteLine("3. Filtered View");
                Console.WriteLine("4. View summary");
                Console.WriteLine("5. Export & open CSV");
                Console.WriteLine("6. Exit");

                string choice = Console.ReadLine();

                //handle menu choice
                switch (choice)
                {
                    case "1":
                        AddTransaction();
                        break;

                    case "2":
                        ViewAll();
                        break;

                    case "3":
                        FilteredView();
                        break;

                    case "4":
                        ViewSummary();
                        break;

                    case "5":
                        ExportCsv();
                        break;

                    case "6":
                        Console.WriteLine("Goodbye!");
                        return;

                    default:
                        Ui.Message(ConsoleColor.Yellow, "[WARN]", "Invalid option. Please enter 1-6.");
                        break;
                }
            }
        }

        public void AddTransaction()
        {
            Transaction transaction = new Transaction();

            //type
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

            //category
            Console.Write("Enter transaction category: ");
            string category = Console.ReadLine().ToLower();

            if (string.IsNullOrEmpty(category))
            {
                category = "generic";
            }

            transaction.Category = category;

            //amount
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

            //date
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

            //note
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

            //add to list
            transactions.Add(transaction);

            //save to file
            File.WriteAllText("FinanceTrackerData.json", JsonSerializer.Serialize(transactions));
            Ui.Message(ConsoleColor.Green, "[SUCCESS]", "Transaction saved!.");

        }

        public void ViewAll()
        {
            if (transactions.Count == 0)
            {
                Ui.Message(ConsoleColor.Cyan, "[INFO]", "No transactions to view.");
                return;
            }

            foreach (Transaction transaction in transactions)
            {
                Console.WriteLine($"Type: {transaction.Type}\nCategory: {transaction.Category}\nAmount: {transaction.Amount:C}\nDate: {transaction.Date:d}\nNote: {transaction.Note}");
            }
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

            foreach (var transaction in filtered)
            {

                Console.WriteLine($"Type: {transaction.Type}\nCategory: {transaction.Category}\nAmount: {transaction.Amount:C}\nDate: {transaction.Date:d}\nNote: {transaction.Note}");
            }

        }

        public void ViewSummary()
        {
            decimal totalIncome = transactions.Where(t => t.Amount >= 0).Sum(t => t.Amount);
            decimal totalExpense = transactions.Where(t => t.Amount < 0).Sum(t => t.Amount);
            decimal balance = totalIncome + totalExpense;

            Ui.Message(ConsoleColor.Cyan, "Balance:", $"{balance:C}");
            Ui.Message(ConsoleColor.Green, "Income:", $"{totalIncome:C}");
            Ui.Message(ConsoleColor.Red, "Expenses:", $"{totalExpense:C}");
            Ui.BarChart(transactions);
        }

        public void ExportCsv()
        {
            //exit early if no transactions to export
            if (transactions.Count == 0)
            {
                Ui.Message(ConsoleColor.Cyan, "[INFO]", "No transactions to export.");
                return ;
            }

            var csvLines = new List<string>() //create a list of strings. the first line will be the header which is what excel uses as column names, then each transaction will be a line in the csv file
            {

            "Type,Category,Amount,Date,Note"
            };

            foreach (var t in transactions)
            {
                csvLines.Add($"{t.Type},{t.Category},{t.Amount},{t.Date:d},\"{t.Note}\"");//format each transaction as a comma separated line. note is wrapped in quotes in case it contains commas
            }

            File.WriteAllLines("FinanceTrackerData.csv", csvLines);
            Ui.Message(ConsoleColor.Green, "[SUCCESS]", "Transactions exported to FinanceTrackerData.csv");

            Process.Start(new ProcessStartInfo { FileName = "FinanceTrackerData.csv", UseShellExecute = true });
            Environment.Exit(0);

        }
    }
}
