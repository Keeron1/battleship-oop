﻿using Battleship_BusinessLayer;
using Battleship_DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Battleship_PresentationLayer
{
    public class Presentation
    {
        private Business business = new Business();
        private IQueryable<Ships> GameShips { get; set; }
        char[] GridYaxis = { 'A', 'B', 'C', 'D', 'E', 'F', 'G' };

        public Presentation()
        {

        }

        private string MaskPassword()
        {
            string pass = "";
            ConsoleKeyInfo key;
            do
            {
                key = Console.ReadKey(true);

                if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter &&
                    (((int)key.Key) >= 48 && ((int)key.Key <= 90)) || //numbers and letters
                    (((int)key.Key) >= 96 && ((int)key.Key <= 106)) || //numpad
                    (((int)key.Key) >= 186 && ((int)key.Key <= 223)) //symbols
                   )
                {
                    pass += key.KeyChar;
                    Console.Write("*");
                }
                else if (key.Key == ConsoleKey.Backspace && pass.Length > 0)
                {
                    pass = pass.Substring(0, (pass.Length - 1));
                    Console.Write("\b \b");
                }
            }
            while (key.Key != ConsoleKey.Enter || pass.Length == 0); //if pass length is 0 or enter not pressed then repeat
            Console.WriteLine();
            return pass;
        }

        private void PrintGrid()
        {
            Console.WriteLine("  | 1 2 3 4 5 6 7 8");
            Console.WriteLine("--|----------------");
            for (int col = 1; col < 8; col++) //grid is 8 by 7
            {
                Console.Write(GridYaxis[col - 1] + " | ");
                for (int row = 1; row < 9; row++)
                {
                    if (col == 2 && row == 2)
                    {
                        Console.Write("L ");
                    }
                    else
                    {
                        Console.Write("y ");
                    }
                }
                Console.WriteLine();
            }
        }

        public void AddPlrsDtlsUI()
        {
            for (int i = 1; i < 3; i++) //will loop twice to get player information
            {
                Console.Clear();
                Console.WriteLine($"Input details player {i}:");

                Console.WriteLine("Input Username:");
                string username = Console.ReadLine();
                string password;

                if (business.CheckUsernameExists(username)) //username already exists
                {
                    Console.WriteLine("Username is already used!");
                    Console.WriteLine($"Login to {username}: ");
                    Console.WriteLine("Input password:");
                    password = MaskPassword();

                    if (business.CheckUserPassword(password)) //while loop until user quits or gets correct password?
                    {
                        Console.WriteLine("Logged in");
                    }
                    else
                    {
                        Console.WriteLine("Wrong password");
                    }
                }
                else
                {
                    Console.WriteLine("Username not used");
                    Console.WriteLine("Input password:");
                    password = MaskPassword();
                    business.CreateNewPlayer(username, password);
                    Console.WriteLine($"New Player {username} created!");
                }
            }
        }

        public void ConfigShipsUI()
        {
            //add loop allow both players to set their ships position
            //add inf loop that once all ships have been placed, it will ask the user if they are happy with their ship positioning
            Console.Clear();
            string shipno, shipCoord, temp;
            int shipId, shipSize;
            bool horizontal = false;

            PrintGrid();
            Console.WriteLine();
            GameShips = business.GetShips();

            foreach (Ships s in GameShips)
            {
                Console.WriteLine($"Ship: {s.ID} Title: {s.Title} Size: {s.Size} Not Placed");
            }

            bool shipFound = false;
            do //add validation later on to see if user has already placed the ship, if they try then allow them to move the position
            {
                Console.WriteLine("Select ship no to place:");
                shipno = Console.ReadLine();

                //check if valid ship has been selected
                if (int.TryParse(shipno, out shipId))
                {
                    foreach (Ships s in GameShips)
                    {
                        if (s.ID == shipId)
                        {
                            shipFound = true;
                            shipSize = s.Size;
                        }
                    }
                }
                if (!shipFound)
                {
                    Console.WriteLine("Please select a ship!");
                    Console.WriteLine("To select a ship write it's number");
                }
            }while(!shipFound);

            bool positionSelected = false;
            do
            {
                Console.WriteLine("Would you like to place the ship horizontally or vertically? (h/v)");
                temp = Console.ReadLine();
                if (temp == "h")
                {
                    horizontal = true;
                    positionSelected = true;
                }
                else if(temp == "v")
                {
                    horizontal = false;
                    positionSelected = true;
                }
                else
                {
                    Console.WriteLine("Incorrect input!");
                }
            } while (!positionSelected);

            if (horizontal) 
            {
                Console.WriteLine("you chose horizontal!"); 
            }
            else
            {
                Console.WriteLine("You chose vertical!");
            }


            Console.WriteLine("Select coordinate to place:");
            shipCoord = Console.ReadLine().ToUpper(); //A1 - G8

            //check if it there is enough space for selected ship
            foreach (char c in GridYaxis)
            {
                for (int i = 1; i < 9; i++)
                {
                    Console.WriteLine(c.ToString() + i);
                    if (shipCoord == c.ToString() + i)
                    {//checks if the grid selected is empty
                        Console.WriteLine("Position good!");
                        return; //used to break from the for loop
                    }
                }
            }

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
                            ConfigShipsUI();
                            break;
                        case 3:
                            Console.WriteLine("Launch Attack");
                            break;
                        case 4:
                            Console.WriteLine("Goodbye...");
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
