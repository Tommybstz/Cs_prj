using System.Collections.Generic;
using System.Linq;
class Dipendente
{
    public string Surname { get; set; }
    public string Class { get; set; }
    public double Salary_Y { get; set; }
    public int Age { get; set; }

}
class Program
{
    static void Main(string[] args)
    {
        var dipendenti = new List<Dipendente>();
        
        //input
        while (true)
        {
            //cognome
            Console.WriteLine("Inserire il cognome del dipendente (o 'fine' per terminare):");
            string cognome = Console.ReadLine();
            if (cognome.ToLower() == "fine") break;

            //reparto
            string reparto;
            while (true)
            {
                Console.WriteLine("inserire il reparto del dipendente (Amministrazione, Contabile, Vendite)");
                reparto = Console.ReadLine().ToLower();
                if (reparto == "amministrazione" || reparto == "contabile" || reparto == "vendite") break ;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Reparto non valido. Riprova.");
                Console.ResetColor();
            }

            //salario annuo
            Console.WriteLine("Inserire lo stipendio annuo del dipendente:");
            double salario_anno;
            while (!double.TryParse(Console.ReadLine(), out salario_anno) || salario_anno < 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Stipendio non valido. Riprova.");
                Console.ResetColor();
            }

            //eta
            Console.WriteLine("Inserire l'età del dipendente:");
            int eta;
            while (!int.TryParse(Console.ReadLine(), out eta) || eta < 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Età non valida. Riprova.");
                Console.ResetColor();
            }

            dipendenti.Add(new Dipendente
            {
                Surname = cognome,
                Class = reparto,
                Salary_Y = salario_anno,
                Age = eta
            });
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Dipendente aggiunto con successo!");
            Console.ResetColor();
        }

        //output
        if (dipendenti.Count == 0)
        {
            Console.WriteLine("Nessun dipendente inserito.");
            return;
        }
        Console.WriteLine("\n°=-=-=-=-=-= RISULTATI =-=-=-=-=-=°");

        //dipendenti vendite
        int numVendite = dipendenti.Count(x => x.Class == "vendite");
        Console.WriteLine($"Numero dipendenti nel reparto vendite: {numVendite}");
        
        //minimo amministrazione
        int stipendioMin=dipendenti.Where(x=> x.Class == "amministrazione").Min(x => (int)x.Salary_Y)/12;
        Console.WriteLine($"Stipendio minimo nel reparto amministrazione: {stipendioMin}");

        //dipendenti>€50k
        var Cognome_50k = dipendenti.Where(x => x.Salary_Y > 50000).ToList();
        Console.WriteLine("dipendenti con stipendio > €50.000:");
        if(Cognome_50k.Count > 0)
        {
            Console.WriteLine(string.Join(',',Cognome_50k.Select(x=>x.Surname)));
        }
        else
        {
            Console.WriteLine("Nessuno");
        }

        //media over 50
        var media_over_50 = dipendenti.Where(x => x.Age > 50).ToList();
        if (media_over_50.Count() > 0) 
        {
            Console.WriteLine($"Stipendio medio dipendenti over 50: {media_over_50.Average(x=>x.Salary_Y):F2}");
        }
        else
        {
            Console.WriteLine("nessun dipendente over 50");
        }

        //perc <25
        double perc_u25 = (double)dipendenti.Count(x => x.Age < 25)/dipendenti.Count*100;
        Console.WriteLine($"la percentuale di dipendenti under 25 e: {perc_u25:F1}%");

        Console.WriteLine("°=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=°");
    }
}