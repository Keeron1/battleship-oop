using Battleship_PresentationLayer;
using PresentationLayer;
//using Battleship_DataLayer;

using System;
using System.Windows;

namespace Battleship_UI
{
    internal class Program
    {
        /*
         * Presentation layer: code for console application
         * Business Layer: Communication between both layers
         * Data Layer: Database & LINQ CODE
         */

        [STAThread]
        static void Main(string[] args)
        {
            PresentationLayer.MainWindow presentationLayer = new PresentationLayer.MainWindow();
            Application app = new Application();
            app.Run(presentationLayer);

            //Presentation pt = new Presentation();
            //pt.Menu();
        }
    }
}
