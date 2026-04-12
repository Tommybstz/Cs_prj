
namespace FinanceTracker
{
    public class Transaction
    {
        public string Type { get; set; }//expense or income
        public string Category { get; set; }//food, transport, salary, etc.
        public decimal Amount { get; set; }//positive for income, negative for expenses
        public DateTime Date { get; set; }//date of transaction
        public string Note { get; set; }//optional note about the transaction
    }
}
