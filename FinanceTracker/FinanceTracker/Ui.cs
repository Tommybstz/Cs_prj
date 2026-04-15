
using System.Diagnostics.Contracts;

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

        public static void PrintTransactions(IEnumerable<Transaction> transactions)
        {
            var list = transactions.ToList();

            if (!list.Any())
            {
                Console.WriteLine("No transactions found.");
                return;
            }

            // calculate dynamic column widths
            int idWidth = Math.Max(2, list.Max(t => t.Id.ToString().Length));
            int typeWidth = Math.Max(4, list.Max(t => t.Type.Length));
            int catWidth = Math.Max(8, list.Max(t => t.Category.Length));
            int amountWidth = Math.Max(6, list.Max(t => t.Amount.ToString("0.00").Length));
            int dateWidth = 10;

            // header
            Console.WriteLine(
                $"{"ID".PadRight(idWidth)} | " +
                $"{"Type".PadRight(typeWidth)} | " +
                $"{"Category".PadRight(catWidth)} | " +
                $"{"Amount".PadRight(amountWidth)} | " +
                $"{"Date".PadRight(dateWidth)} | Note"
            );

            Console.WriteLine(new string('-', idWidth + typeWidth + catWidth + amountWidth + dateWidth + 15));

            // rows
            foreach (var t in list)
            {
                Console.WriteLine(
                    $"{t.Id.ToString().PadRight(idWidth)} | " +
                    $"{t.Type.PadRight(typeWidth)} | " +
                    $"{t.Category.PadRight(catWidth)} | " +
                    $"{t.Amount.ToString("C").PadRight(amountWidth)} | " +
                    $"{t.Date.ToString("yyyy-MM-dd").PadRight(dateWidth)} | {t.Note}"
                );
            }
        }
    }
}
