using Battleship_BusinessLayer;
using Battleship_DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battleship_PresentationLayer
{
    public class Presentation
    {
        private Business business = new Business();
        private IQueryable<Players> myDisplayResult { get; set; } //
        public Presentation()
        {

        }

        public void AddPlrsDtlsUI()
        {
            Console.Clear();

            Console.WriteLine("Input Username:");
            string username = Console.ReadLine();

            //check database to see if already exists

            myDisplayResult = business.CheckUsernameExists(username);

            foreach(Players p in myDisplayResult)
            {
                Console.WriteLine(p.Username,p.Password);
            }

            //Console.WriteLine(myDisplayResult);

            //if (username == null) //username already exists
            //{
            //    Console.WriteLine("Username is already used");
            //    Console.WriteLine("Login: ");
            //    Console.WriteLine("Input password:");
            //    string password = Console.ReadLine();

            //}
            //else
            //{
            //    Console.WriteLine("Input password:");
            //    string password = Console.ReadLine();

            //}

        }

        //Will repeatedly display the menu to the user, until they choose the quit option
        public void Menu()
        {
            int choice = 0;
            while (true)
            {
                try
                {
                    Console.Clear();
                    Console.WriteLine("1. Add Player Details\n2. Configure Ships\n3. Launch Attack\n4. Quit");
                    choice = Convert.ToInt16(Console.ReadLine());

                    switch (choice)
                    {
                        case 1:
                            Console.WriteLine("Add Player Details");
                            AddPlrsDtlsUI();
                            break;
                        case 2:
                            Console.WriteLine("Configure Ships");
                            break;
                        case 3:
                            Console.WriteLine("Launch Attack");
                            break;
                        case 4:
                            Console.WriteLine("Quitting!");
                            Environment.Exit(0);
                            break;
                        default:
                            Console.WriteLine("Incorrect Input!");
                            break;
                    }
                    Console.ReadKey();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                    Console.ReadKey();
                }


            }
        }
    }
}
