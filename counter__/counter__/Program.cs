
using System.Globalization;

char R;

Console.WriteLine("inserire la lettera del ciclo voluto:");
while(!char.TryParse(Console.ReadLine(),out R));
R=Char.ToLower(R);

if (R=='a')
    for(int i=1; i < 100; i++)
    {
        if(i%2==1) Console.WriteLine(i);
    }

else if (R=='b')
    for(int i=1; i < 100; i++)
    {
        Console.WriteLine(i*i);
    }

else if (R=='c')
    for(int i=1; i < 100; i++)
    {
        if(i%2==0) Console.WriteLine(-i);
        else Console.WriteLine(i);
    }

else if (R=='d')
    for(int i=1; i < 99; i++)
    {

        Console.WriteLine($"d1: {(double) i/(i+1):F2}");
        Console.WriteLine($"d2: {i}/{i+1}");
    }
else
    Console.WriteLine("carattere non valido");