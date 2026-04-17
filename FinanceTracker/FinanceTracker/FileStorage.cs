using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json;

namespace FinanceTracker
{

    internal class FileStorage
    {
        private List<Transaction> transactions;
        private readonly string dataFolder = "UserData";
        private readonly string dataFile;
        private readonly string backupFile;

        public FileStorage(List<Transaction> transactions)
        {
            this.transactions = transactions;
            dataFile = Path.Combine(dataFolder, "FinanceTrackerData.json");//set the data file path to be inside the UserData folder. this keeps the application directory cleaner and allows for better organization of user data.
            backupFile = Path.Combine(dataFolder, $"FTD_backup_{DateTime.Today:yyyy-MM-dd}.json");
            Directory.CreateDirectory(dataFolder);//ensure the data folder exists. if it already exists, this does nothing
        }
        public void ExportCsv()
        {
            //exit early if no transactions to export
            if (transactions.Count == 0)
            {
                Ui.Message(ConsoleColor.Cyan, "[INFO]", "No transactions to export.");
                return;
            }

            var csvLines = new List<string>() //create a list of strings. the first line will be the header which is what excel uses as column names, then each transaction will be a line in the csv file
            {

            "Type,Category,Amount,Date,Note"
            };

            foreach (var t in transactions)
            {
                csvLines.Add($"{t.Type},{t.Category},{t.Amount},{t.Date:d},\"{t.Note}\"");//format each transaction as a comma separated line. note is wrapped in quotes in case it contains commas
            }

            //write all lines to a file named FinanceTrackerData.csv in the same directory as the application. if the file already exists, it will be overwritten
            File.WriteAllLines("FinanceTrackerData.csv", csvLines);
            Ui.Message(ConsoleColor.Green, "[SUCCESS]", "Transactions exported to FinanceTrackerData.csv");

            //open the csv file with the default associated application (usually Excel)
            Process.Start(new ProcessStartInfo { FileName = "FinanceTrackerData.csv", UseShellExecute = true });

        }
        public void SaveData()
        {
            //save the transactions list to a file named FinanceTrackerData.json in the same directory as the application. if the file already exists, it will be overwritten
            File.WriteAllText(dataFile, JsonSerializer.Serialize(transactions));
            BackupData();
        }
        public void LoadData()
        {
            //load data from file if it exists, otherwise initialize empty list
            try
            {
                string json = File.ReadAllText(dataFile);//read the contents of the file into a string variable. if the file doesn't exist, this will throw a FileNotFoundException which we catch below
                var loaded = JsonSerializer.Deserialize<List<Transaction>>(json) ?? new List<Transaction>();//if deserialization returns null, initialize empty list. the ?? checks if null and if it is, it creates a new List<Transaction>()
                transactions.Clear();
                transactions.AddRange(loaded);

                //NextId = transactions.Any() ? transactions.Max(t => t.Id) + 1 : 1;//set the NextId to be one greater than the maximum existing ID in the loaded transactions, or 1 if there are no transactions. this ensures that new transactions will have unique IDs even after loading from a file.
            }
            catch (FileNotFoundException)
            {
                Ui.Message(ConsoleColor.Cyan, "[INFO]", "No save file found. Starting a new one.");//if the file doesn't exist, we just start with an empty list of transactions. this is not an error condition, so we show an info message and continue
                transactions.Clear();
            }
            catch (JsonException)
            {
                Ui.Message(ConsoleColor.Red, "[ERROR]", "Save file is corrupted and cannot be read. Starting fresh.");//if the file is corrupted and can't be deserialized, we show an error message and start with an empty list.
                transactions.Clear();

                //might add functionality in the future to backup the corrupted file with a timestamp so the user doesn't lose all their data, but for now we just start fresh if the file can't be read
            }
            catch (Exception)
            {
                Ui.Message(ConsoleColor.Red, "[ERROR]", "Unexpected error loading data. Starting fresh.");
                transactions.Clear();
            }

        }
        public void BackupData()
        {
            try
            {

                File.Copy(dataFile, backupFile, overwrite: true);
                CleanOldBackups();
            }
            catch
            {
                Ui.Message(ConsoleColor.Red, "[ERROR]", "Failed to create a backup.");
            }

        }
        public void CleanOldBackups()
        {

            var backups = Directory.GetFiles(dataFolder, "FTD_backup_*.json");

            if (!backups.Any())
            {
                return;
            }

            foreach (var backup in backups)
            {
                //checks if the file is older then 30 days 
                if ((DateTime.Today - File.GetLastWriteTime(backup)).TotalDays > 30)
                {
                    File.Delete(backup);
                }

            }

        }
    }
}
