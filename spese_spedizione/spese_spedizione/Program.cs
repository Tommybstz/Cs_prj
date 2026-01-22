class Program
{
    static void Main(string[] args)
    {
        //var
        int N;
        double weight,shippingCost=0;

        //input
        Console.WriteLine("inserire il numero di pacchi da spedire");
        N = int.Parse(Console.ReadLine());

        //cost calculation
        for (int i = 0; i<N; i++)
        {
            //input 2
            Console.WriteLine($"inserire il peso del pacco numero: {i + 1}");
            weight = double.Parse(Console.ReadLine());

            //calculation
            if (weight<=3)
            {
                shippingCost += 5;
            }
            else if(weight<=10)
            {
                shippingCost += 8;
            }
            else
            {
                shippingCost += 10;
            }
        }
        shippingCost += shippingCost * 0.04;

        //output
        Console.WriteLine($"il costo totale di spedizione è: {shippingCost} euro");


    }
}