
/* chiesto in input un orario nel formato ore e minuti usando 2 numeri, dire se l'orario e in un formato correto o meno, se non indicare la componente da correggere*/

//var
class Program
{
    public static void Main(String[] args)
    {
        //var

        int ora, min;

        //input
        Console.WriteLine("inserire ora");
        ora = int.Parse(Console.ReadLine());

        Console.WriteLine("inserire i minuti");
        min = int.Parse(Console.ReadLine());

        //logica

        if (ora > 23 || ora < 0)
        {
            Console.WriteLine("formato dell'ora non corretto");
        }
        if (min > 59 || min < 0)
        {
            Console.WriteLine("formato dei minuti non corretto");
        }
        else {
            Console.WriteLine($"formato corretto.\nsono le {ora}:{min}");
        }
    }
}