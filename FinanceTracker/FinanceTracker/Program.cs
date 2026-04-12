
namespace FinanceTracker
{
    class Program
    {


        static void Main(string[] args)
        {

            Console.Title = "Personal Finance Tracker";
            FinanceTrackerApp app = new FinanceTrackerApp();
            app.Run();
        }
    }
}
