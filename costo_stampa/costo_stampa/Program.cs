class Program
{
    static void Main()
    {
        //var
        int pics;
        double tariff_1;
        double tariff_2;
        string result;
        //input
        Console.WriteLine("Inserisci il numero di foto da stampare:");
        pics = int.Parse(Console.ReadLine());

        //price
        tariff_1 = (pics * 0.5) + 10;
        tariff_2 = pics * 0.65;
        
        if(tariff_1 < tariff_2)
        {
            result = "tariffa 1";
        }
        else
        {
            result = "tariffa 2";
        }

        //output
        Console.WriteLine($"La tariffa più conveniente è: {result}");
    }
}