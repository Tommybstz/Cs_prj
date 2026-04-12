
namespace FinanceTracker
{
    public class Ui
    {
        public static void BarChart(List<Transaction> transactions)
        {
            var grouped = transactions
                .Where(t => t.Type == "expense")
                .GroupBy(t => t.Category)//group by category
                .Select(g => new { Category = g.Key, Total = Math.Abs(g.Sum(t => t.Amount)) });//sum expenses in each category and take absolute value

            if (!grouped.Any())
            {

                Ui.Message(ConsoleColor.Cyan, "[INFO]", "No expenses to display.");
                return;
            }

            decimal max = grouped.Max(g => g.Total);

            Console.WriteLine("\n--- Spending by Category ---\n");

            foreach (var group in grouped)
            {
                int barLength = (int)(group.Total / max * 20);
                barLength = Math.Clamp(barLength, 0, 20);
                string bar = new string('█', barLength) + new string('-', 20 - barLength);
                Console.WriteLine($"{group.Category,-15} | {bar} {group.Total:C}");
            }
        }

        public static void Message(ConsoleColor color, string label, string message)
        {
            //Colored message with label. for reporting and user feedback
            Console.ForegroundColor = color;
            Console.WriteLine($"{label.PadRight(12)} {message}");
            Console.ResetColor();
        }
    }
}
