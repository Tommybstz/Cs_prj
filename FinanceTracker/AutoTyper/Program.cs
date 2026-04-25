// AutoTyper.cs — Drop this file into a new console project alongside your FinanceTracker.
// It launches FinanceTracker.exe as a child process and pipes fake transactions into it.
//
// USAGE:
//   1. Build your FinanceTracker project first (dotnet build)
//   2. Create a new console project: dotnet new console -n AutoTyper
//   3. Drop this file in as Program.cs (replace the default one)
//   4. Set FINANCE_TRACKER_EXE below to the path of your built .exe
//   5. Run: dotnet run
//
// CONFIGURATION — edit these three values:
const string FINANCE_TRACKER_EXE = @"C:\Users\tomma\OneDrive\Desktop\Cs_prj\FinanceTracker\FinanceTracker\bin\Debug\net10.0\FinanceTracker.exe";
const int TRANSACTION_COUNT = 10000;       // how many transactions to inject
const int DELAY_MS = 1;               // ms between lines (increase if app misses input)

// ── Data pools ──────────────────────────────────────────────────────────────
var types = new[] { "income", "+", "expense", "-" };
var incCats = new[] { "salary", "freelance", "bonus", "dividends", "rental", "gift" };
var expCats = new[] { "food", "transport", "rent", "utilities", "entertainment",
                       "clothing", "health", "subscriptions", "travel", "education" };
var notes = new[] { "monthly", "one-time", "quarterly", "reimbursement",
                       "see receipt", "split with partner", "", "", "" }; // blanks = no note

var rng = new Random();

string RandDate()
{
    // random date within the last 2 years
    var start = DateTime.Today.AddYears(-2);
    int span = (DateTime.Today - start).Days;
    return start.AddDays(rng.Next(span)).ToString("yyyy-MM-dd");
}

// ── Build the full input script ──────────────────────────────────────────────
// Each transaction needs these menu interactions:
//   menu → "1" (Add Transaction)
//   type → e.g. "expense"
//   category → e.g. "food"
//   amount → e.g. "42.50"
//   date → e.g. "2024-06-15"
//   note → e.g. "lunch"
//   [pause] → any key… we send Enter
// After all transactions: "8" (Exit) + Enter
var lines = new List<string>();

for (int i = 0; i < TRANSACTION_COUNT; i++)
{
    string rawType = types[rng.Next(types.Length)];
    bool isExpense = rawType == "expense" || rawType == "-";
    string[] catPool = isExpense ? expCats : incCats;

    string type = rawType;
    string category = catPool[rng.Next(catPool.Length)];
    decimal amount = Math.Round((decimal)(rng.NextDouble() * 1990 + 10), 2); // €10–€2000
    string date = RandDate();
    string note = notes[rng.Next(notes.Length)];

    lines.Add("1");        // select "Add Transaction"
    lines.Add(type);
    lines.Add(category);
    lines.Add(amount.ToString("F2"));
    lines.Add(date);
    lines.Add(note);
    lines.Add("");         // press Enter at the "Press any key" pause
}

lines.Add("8");  // Exit
lines.Add("");   // final Enter

// ── Launch & pipe ───────────────────────────────────────────────────────────
Console.ForegroundColor = ConsoleColor.Cyan;
Console.WriteLine($"[AutoTyper] Injecting {TRANSACTION_COUNT} transactions into FinanceTracker...");
Console.WriteLine($"[AutoTyper] Target: {FINANCE_TRACKER_EXE}");
Console.ResetColor();

if (!File.Exists(FINANCE_TRACKER_EXE))
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine($"[AutoTyper] ERROR: Executable not found at:\n  {FINANCE_TRACKER_EXE}");
    Console.WriteLine("Build FinanceTracker first (dotnet build) and verify the path above.");
    Console.ResetColor();
    return;
}

var psi = new System.Diagnostics.ProcessStartInfo
{
    FileName = FINANCE_TRACKER_EXE,
    RedirectStandardInput = true,
    RedirectStandardOutput = true,
    RedirectStandardError = true,
    UseShellExecute = false,
    CreateNoWindow = false,
};

using var process = System.Diagnostics.Process.Start(psi)!;

// Stream output so you can see what's happening
var outputTask = Task.Run(async () =>
{
    string? line;
    while ((line = await process.StandardOutput.ReadLineAsync()) != null)
    {
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.Write("[APP] ");
        Console.ResetColor();
        Console.WriteLine(line);
    }
});

// Also capture stderr
var errorTask = Task.Run(async () =>
{
    string? line;
    while ((line = await process.StandardError.ReadLineAsync()) != null)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Write("[ERR] ");
        Console.ResetColor();
        Console.WriteLine(line);
    }
});

// Feed lines one by one with a small delay
int sent = 0;
foreach (var line in lines)
{
    try
    {
        await Task.Delay(DELAY_MS);
        await process.StandardInput.WriteLineAsync(line);

        if (line == "1") { sent++; Console.ForegroundColor = ConsoleColor.Green; Console.WriteLine($"[AutoTyper] Sending transaction {sent}/{TRANSACTION_COUNT}..."); Console.ResetColor(); }
    }
    catch (IOException)
    {
        // App closed the pipe early (it exited) — this is fine, we're done
        break;
    }
}

process.StandardInput.Close();
await outputTask;
await errorTask;
await process.WaitForExitAsync();

Console.ForegroundColor = ConsoleColor.Green;
Console.WriteLine($"\n[AutoTyper] Done! {TRANSACTION_COUNT} transactions injected. Exit code: {process.ExitCode}");
Console.ResetColor();