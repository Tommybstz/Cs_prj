class Program
{
    static void Main()
    {
        Console.Write("inserire la parola da controllare: ");

        string word=Console.ReadLine();

        //1 variante
        for( int i = 0;i < word.Length/2;i++)
        {
            if (!(word[i] == word[word.Length -1 - i]))
            {
                Console.WriteLine("questa parola non è palindroma: ");
                return;
            }
        }
        Console.WriteLine("parola palindroma");


        //2 variante
        char[] letters = new char[word.Length];
        for(int i = 0; i < word.Length; i++){
            letters[i] = word[i];
        }

        for (int i = 0; i < word.Length; i++)
        {
            if(!(letters[i] == letters[word.Length -1 - i]))
            {
                Console.WriteLine("parola non palindroma");
                return;
            }
        }
        Console.WriteLine("parola palindroma");
    }
}