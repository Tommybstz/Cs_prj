using System.Text.Json;

namespace FinaceTrackerEx
{
    class Transaction
    {
        public string Type { get; set; }//expense or income
        public string Category { get; set; }//food, transport, salary, etc.
        public double Amount { get; set; }//positive for income, negative for expenses
        public DateTime Date { get; set; }//date of transaction
        public string Note { get; set; }//optional note about the transaction
    }
    class Program
    {


        static void Main(string[] args)
        {
            List<Transaction> transactions;

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
                Console.WriteLine("5. Exit");

                string choice = Console.ReadLine();

                //handle menu choice
                switch (choice)
                {
                    case "1":
                        AddTransaction(transactions);
                        break;

                    case "2":
                        ViewAll(transactions);
                        break;

                    case "3":
                        FilteredView(transactions);
                        break;

                    case "4":
                        ViewSummary(transactions);
                        break;

                    case "5":
                        Console.WriteLine("Goodbye!");
                        return;
                }
            }
        }

        static void AddTransaction(List<Transaction> transactions)
        {
            Transaction transaction = new Transaction();

            //type
            Console.WriteLine("Enter transaction type (expense/-) or (income/+):");
            string type = Console.ReadLine().ToLower().Trim();

            while (type != "expense" && type != "-" && type != "income" && type != "+")
            {
                Console.WriteLine("Invalid input. Please enter 'expense/-' or 'income/+':");
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

            double amount;

            while (!double.TryParse(Console.ReadLine(), out amount) || amount <= 0)
            {
                Console.Write("\nInvalid input. Please enter a positive number: ");
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
                    Console.Write("Invalid input. Please enter a valid date (yyyy-mm-dd) or press Enter to use today's date: ");
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
            Console.WriteLine("Data saved successfully!.");

        }

        static void ViewAll(List<Transaction> transactions)
        {
            if (transactions.Count == 0)
            {
                Console.WriteLine("No transactions to yet.");
                return;
            }

            foreach (Transaction transaction in transactions)
            {
                Console.WriteLine($"Type: {transaction.Type}\nCategory: {transaction.Category}\nAmount: {transaction.Amount:C}\nDate: {transaction.Date}\nNote: {transaction.Note}");
            }
        }

        static void FilteredView(List<Transaction> transactions)
        {
            if (transactions.Count == 0)
            {
                Console.WriteLine("No transactions to filter.");
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
                    Console.WriteLine("Invalid input. Please enter a valid date (yyyy-mm-dd):");
                    dateStartInput = Console.ReadLine();
                }
            }

            Console.WriteLine("Enter end date (yyyy-mm-dd) or press Enter to skip: ");
            dateEndInput = Console.ReadLine();

            if (!string.IsNullOrEmpty(dateEndInput))
            {
                while (!DateTime.TryParse(dateEndInput, out dateEnd))
                {
                    Console.WriteLine("Invalid input. Please enter a valid date (yyyy-mm-dd):");
                    dateEndInput = Console.ReadLine();
                }
            }


            //category filter

            Console.WriteLine("Enter category to filter by or press Enter to skip: ");

            string categoryFilter = Console.ReadLine().ToLower().Trim();

            //filter and display transactions
            foreach (Transaction transaction in transactions.Where(t => string.IsNullOrEmpty(categoryFilter) || t.Category.Contains(categoryFilter) && t.Date >= dateStart && t.Date <= dateEnd))
            {

                Console.WriteLine($"Type: {transaction.Type}\nCategory: {transaction.Category}\nAmount: {transaction.Amount:C}\nDate: {transaction.Date}\nNote: {transaction.Note}");
            }

        }

        static void ViewSummary(List<Transaction> transactions)
        {
            double totalIncome = transactions.Where(t => t.Amount >= 0).Sum(t => t.Amount);
            double totalExpense = transactions.Where(t => t.Amount < 0).Sum(t => t.Amount);
            double balance = totalIncome + totalExpense;

            Console.WriteLine($"Your balance is: {balance:C}\nTotal income: {totalIncome:C}\nTotal expenses: {totalExpense:C}");
            BarChart(transactions);
        }

        static void BarChart(List<Transaction> transactions)
        {
            var grouped = transactions
                .Where(t => t.Type == "expense")
                .GroupBy(t => t.Category)//group by category
                .Select(g => new { Category = g.Key, Total = Math.Abs(g.Sum(t => t.Amount)) });//sum expenses in each category and take absolute value

            if (!grouped.Any())
            {
                Console.WriteLine("No expenses to display.");
                return;
            }

            double max = grouped.Max(g => g.Total);

            Console.WriteLine("\n--- Spending by Category ---\n");

            foreach (var group in grouped)
            {
                int barLength = (int)(group.Total / max * 20);
                barLength = Math.Clamp(barLength, 0, 20);
                string bar = new string('█', barLength) + new string('-', 20 - barLength);
                Console.WriteLine($"{group.Category,-15} | {bar} {group.Total:C}");
            }
        }
    }
}
