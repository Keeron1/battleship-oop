using Battleship_PresentationLayer;
using Battleship_DataLayer;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Diagnostics;
using System.Security;

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
        }

    }
}
