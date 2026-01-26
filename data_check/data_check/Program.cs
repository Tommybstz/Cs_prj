using System.Collections.Generic;
public class Program
{
    public static void Main()
    {
        //var
        int day, month, year;
        bool giorno_valido, mese_valido, anno_valido;
        List<int> mesi_31 = new List<int> { 1, 3, 5, 7, 8, 10, 12 };
        List<int> mesi_30 = new List<int> { 4, 6, 9, 11 };
        
        //values
        giorno_valido = false;  
        mese_valido = false;
        anno_valido = false;


        //input
        Console.WriteLine("inserire il giorno:");
        day = int.Parse(Console.ReadLine());

        Console.WriteLine("inserire il mese:");
        month = int.Parse(Console.ReadLine());

        Console.WriteLine("inserire l'anno(da 0 a 2026):");
        year = int.Parse(Console.ReadLine());

        //logica
        if (year >= 0 && year <= 2026)
        {
            anno_valido = true;
        }
        if (month > 0 && month <= 12)
        {

            mese_valido = true;
            if (mesi_31.Contains(month))
            {
                if (day >= 0 && day <= 31)
                {
                    giorno_valido = true;
                }
            }

            else if (mesi_30.Contains(month))
            {
                if (day >= 0 && day <= 30)
                    giorno_valido = true;
            }

            else
            {
                if (day >= 0 && day <= 28)
                {
                    giorno_valido = true;
                }

            }
        }

        //output
        if (!giorno_valido)
        {
            Console.WriteLine("il giorno inserito non è valido");
        }

        if (!mese_valido)
        {
            Console.WriteLine("il mese inserito non è valido");
        }

        if (!anno_valido)
        {
            Console.WriteLine("l'anno inserito non è valido");
        }

    }
}