using Battleship_PresentationLayer;
using Battleship_DataLayer;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battleship_UI
{
    internal class Program
    {
        /*
         * Presentation layer: code for console application
         * Business Layer: LINQ code must be contained in this layer
         * Data Layer: Database
         */

        static void Main(string[] args)
        {
            Presentation pt = new Presentation();
            //pt.Menu();

            char[] chars = { 'A', 'B', 'C', 'D','E','F','G','H' };
            Console.WriteLine("  | 1 2 3 4 5 6 7 8");
            Console.WriteLine("--|----------------");
            for (int col = 0; col < 8; col++)
            {
                Console.Write(chars[col] + " | ");

                for (int row = 0; row < 8; row++)
                {
                    Console.Write("y ");
                }

                Console.WriteLine();
            }
            Console.ReadKey();

        }
    }
}
