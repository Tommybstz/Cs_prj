class Program
{
    static void Main()
    {
        /* matrici -> array bi-dimensionale(2D) formato da un numero determinato di righe(N) e colonne(M)
         * elementi omogenei -> TUTTI stesso tipo (int,char,string, etc...)
         * per accedere ad un solo elemento sono necessarii 2 indici, il primo (N) è il numero di righe, il secondo è il numero di colonne(M)
         * gli indici partono sempre da 0. per l'indice della riga(i) e per la colonna(j), gli indici vanno da 0 a N-1(righe) e 0 a M-1(col)
         * 
         * esempio: una matrice 2x2 quindi [2,2]
         * 
         * colonne | |
         * _____       _____
         * | | | righe _____
         * -----
         * | | |
         * -----
         */

        //operazioni fondamentali
        int row, col;//dimensioni matrice
        int i, j;//indici matrice

        //input righe con validazione
        Console.Write("inserire num righe: ");
        while (!int.TryParse(Console.ReadLine(), out row))
        {
            Console.WriteLine("errore: valore non valido");
        }

        //input colonne con validazione
        Console.Write("inserire num colonne: ");
        while (!int.TryParse(Console.ReadLine(), out col))
        {
            Console.WriteLine("errore: valore non valido");
        }

        //creazione matrice
        var matrice = new int[row, col];

        //caricamento dati
        for (i = 0; i < matrice.GetLength(0); i++)//iterazione righe. GetLength al posto di Length dato che ci sono 2 indici (row,col)->(0,1)
        {
            //i aumenta alla fine del for interno

            for (j = 0; j < matrice.GetLength(1); j++) //iterazione colonne 
            {
                //input valore
                Console.Write($"riga {i + 1}. inserire {j + 1} numero: ");
                while (!int.TryParse(Console.ReadLine(), out matrice[i, j]))
                {
                    Console.WriteLine("errore: valore non valido");
                }
            }
        }

        // UI : stampa matrice
        for (i = 0; i < matrice.GetLength(0); i++)
        {
            Console.WriteLine(new string('-',col*5));
             
            for (j = 0; j < matrice.GetLength(1); j++) //iterazione colonne 
            {
                Console.Write($"| {matrice[i, j]} ");
            }
            Console.WriteLine("|");
        }

        //esercizio
        //calcolare e stampare la somma degli elementi riga per riga e colonna per colonna
        int sommaRiga;
        int sommaColonna;
        var sommeRighe=new int[row];
        var sommeColonne = new int[col];

        //somma righe
        for (i = 0;i < matrice.GetLength(0); i++)
        {
            sommaRiga = 0;

            for (j = 0;j < matrice.GetLength(1); j++)
            {
                sommaRiga += matrice[i,j];
            }
            sommeRighe[i]=sommaRiga;
        }

        //somma colonne
        for (j = 0; j < matrice.GetLength(1); j++)
        {
            sommaColonna = 0;

            for (i = 0; i < matrice.GetLength(0); i++)
            {
                sommaColonna += matrice[i, j];
            }
            sommeColonne[j] = sommaColonna;
        }

        for (i = 0; i < sommeRighe.Length; i++)
        {
            Console.WriteLine($"\nLa somma della riga n°{i + 1} è {sommeRighe[i]}");

            for(j = 0;j<sommeColonne.Length; j++)
            {
                Console.WriteLine($"La somma della colonna n°{i + 1} è {sommeColonne[j]}");
            }
        }



    }
}