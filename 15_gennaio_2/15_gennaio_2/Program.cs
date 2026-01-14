using System.Collections.Generic;
class Program {
    static void Main(string[] args){

        //var
        string zona;
        double mq,prezzo_tot;
        Dictionary<string, int> prezzi_mq = new Dictionary<string, int>(){
            {"Centro", 1500},
            {"Zona1", 1200},
            {"Zona2", 1400},
            {"Zona3", 1300},
            {"Periferia", 1000}
        };

        //input
        Console.WriteLine("Inserisci la zona (Centro, Zona1, Zona2, Zona3, Periferia): ");
        zona = Console.ReadLine();
        Console.WriteLine("Inserisci i metri quadrati dell'appartamento: ");
        mq = double.Parse(Console.ReadLine());

        //calcolo prezzo
        prezzo_tot = prezzi_mq[zona] * mq;

        //output
        Console.WriteLine($"Il prezzo totale dell'appartamento in {zona} di {mq} mq è: {prezzo_tot} euro.");
    }
}