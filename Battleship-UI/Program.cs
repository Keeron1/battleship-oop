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
         * Business Layer: Communication between both layers
         * Data Layer: Database & LINQ CODE
         */

        static void Main(string[] args)
        {
            Presentation pt = new Presentation();

            pt.Menu();

            //char[] chars = { 'A', 'B', 'C', 'D','E','F','G','H' }; //starts from 0
            //Console.WriteLine("  | 1 2 3 4 5 6 7 8");
            //Console.WriteLine("--|----------------");
            //for (int col = 1; col < 9; col++)
            //{
            //    Console.Write(chars[col-1] + " | ");

            //    for (int row = 1; row < 9; row++)
            //    {
            //        if (col==2 && row == 2)
            //        {
            //            Console.Write("L ");
            //        }
            //        else
            //        {
            //            Console.Write("y ");
            //        }

            //    }

            //    Console.WriteLine();
            //}
            //Console.ReadKey();
        }

    }
}
