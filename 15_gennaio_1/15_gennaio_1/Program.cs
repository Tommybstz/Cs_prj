public class Program
{
    static void Main(string[] args)
    {
        //var
        string name;
        int scatti_P, scatti_A;
        double price,base_price,total_price;

        //input
        Console.WriteLine("Inserire nome utente:");
        name = Console.ReadLine();

        Console.WriteLine("Inserire numero scatti precedenti:");
        scatti_P = int.Parse(Console.ReadLine());
        
        Console.WriteLine("Inserire numero scatti attuali:");
        scatti_A = int.Parse(Console.ReadLine());

        Console.WriteLine("Inserire prezzo per scatto:");
        price = double.Parse(Console.ReadLine());

        Console.WriteLine("Inserire prezzo base:");
        base_price = double.Parse(Console.ReadLine());

        //calcolo prezzo
        total_price = base_price + (scatti_A - scatti_P) * price;

        //output
        Console.WriteLine($"la bolletta dell'utente {name} e' di: {total_price} euro");

    }
}