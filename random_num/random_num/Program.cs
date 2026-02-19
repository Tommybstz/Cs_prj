class Program
{
    static void Main(){
        int n, rnd,min,max ;

        Console.WriteLine("inserire il valore minimo per la generazione del numero(>0)");
        min = int.Parse(Console.ReadLine());

        Console.WriteLine("inserire il valore massimo per la generazione del numero(<10^32)");
        max = int.Parse(Console.ReadLine());

        rnd=new Random().Next(min,max);

        do
        {
            Console.WriteLine($"provare a indovinare num da {min} a {max}");
            n=int.Parse(Console.ReadLine());
        } while (n!=rnd);

    }
}