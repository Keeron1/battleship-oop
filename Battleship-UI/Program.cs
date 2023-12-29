using Battleship_PresentationLayer;

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
            pt.Menu();

        }
    }
}
