
namespace FinanceTracker
{
    class Program
    {

        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.Title = "Personal Finance Tracker";
            FinanceTrackerApp app = new FinanceTrackerApp();
            app.Run();
        }
    }
}
