using System;

namespace OurDatabase
{
    class Program
    {
        public static string PrintRead(string output)
        {
            Console.Write(output);
            return Console.ReadLine();
        }
        
        static void Main(string[] args)
        {

            var magicTable = new PersonTable();
            while (true)
            {
                Console.WriteLine("1. Add Item");
                Console.WriteLine("2. List Item");
                Console.WriteLine("3. Search Id");
                Console.WriteLine("4. Search Name");
                Console.WriteLine("5. Search Age");
                
                if (int.TryParse(Console.ReadLine(), out var choice))
                {
                    switch (choice)
                    {
                        case 1:
                            Console.WriteLine("Type your data and press enter");
                            magicTable.AddData(new Person(int.Parse(PrintRead("Id:")), PrintRead("Name:"), int.Parse(PrintRead("Age:"))));
                            // try
                            // {
                            //     magicTable.AddData(new Person(int.Parse(PrintRead("Id:")), PrintRead("Name:"), int.Parse(PrintRead("Age:"))));
                            // }
                            // catch (Exception e)
                            // {
                            //     Console.WriteLine(e.Message);
                            // }
                            
                            break;
                        case 2:
                            Console.WriteLine(magicTable.ToString());
                            break;
                        case 3:
                            Console.WriteLine(magicTable.SearchById(int.Parse(PrintRead("Search Id with Value:")))?.ToString() ?? "Not found");
                            break;
                        
                        case 4:
                            Console.WriteLine(magicTable.SearchByName(PrintRead("Search Name with Value:"))?.ToString() ?? "Not found");
                            break;
                        
                        case 5:
                            Console.WriteLine(magicTable.SearchByAge(int.Parse(PrintRead("Search Age with Value:")))?.ToString() ?? "Not found");
                            break;
                        
                        default:
                            break;
                    }
                }
            }
        }
    }
}