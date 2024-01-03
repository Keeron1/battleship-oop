using Battleship_BusinessLayer;
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
        private char[] GridYaxis = { 'A', 'B', 'C', 'D', 'E', 'F', 'G' };
        private bool[] menuOptsEnabled = { true, true, true };

        private List<Players> players { get; set; } = new List<Players>(2);

        public Presentation()
        {

        }

        private string CreateTitle()
        {
            string title = "";
            int matchNo = 0;
            for (int i = 0; i<players.Count(); i++)
            {
                if (i == 1)
                {
                    title += " VS ";
                }

                if (players[i].Username.Length >= 3)
                {
                    title += players[i].Username.Substring(0, 3);
                }
                else
                {
                    title += players[i].Username;
                }
            }

            string tempTitle;
            do
            {
                tempTitle = title;
                matchNo++;
                tempTitle += " " + matchNo;
            } while (business.ValidTitle(tempTitle));
            return tempTitle;
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

        private bool AskHorizontal()
        {
            bool positionSelected = false;
            bool horizontal = false;
            do
            {
                Console.WriteLine("Would you like to place the ship horizontally or vertically? (h/v)");
                string temp = Console.ReadLine().ToLower();
                if (temp == "h")
                {
                    horizontal = true;
                    positionSelected = true;
                }
                else if (temp == "v")
                {
                    horizontal = false;
                    positionSelected = true;
                }
                else
                {
                    Console.WriteLine("Incorrect input!");
                }
            } while (!positionSelected);
            return horizontal;
        }

        private void PrintGrid()
        {
            //query to get the attacks
            //query to get where the ships are
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

        //menu 1
        private void AddPlrsDtlsUI()
        {
            players.Clear(); //clears the players list
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
                        players.Add(business.GetPlayer(username));
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
                    players.Add(business.GetPlayer(username)); //no need to check for null since we know that the user has just been created
                }
            }

            //create game
            if(players.Count == 2)
            {
                if (business.AreGamesCompleteUsername(players[0].Username, players[1].Username)) //game is complete so create new game
                {
                    business.CreateNewGame(CreateTitle(), players[0].Username, players[1].Username);
                    Console.WriteLine("Game has been created!");
                }
                else
                {
                    Console.WriteLine("A game already exists between both players!");
                    //make option to ask if users would like to start again
                }
                menuOptsEnabled[0] = false; //disables the menu option
            }
        }

        //menu 2
        private void ConfigShipsUI()
        {
            //add loop allow both players to set their ships position
            //add inf loop that once all ships have been placed, it will ask the user if they are happy with their ship positioning
            Console.Clear();

            //current game between both players
            Games game = business.GetActiveGameUsername(players[0].Username, players[1].Username);

            string shipno, shipCoord;
            int shipId, shipSize = 0;
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

            horizontal = AskHorizontal();
            if (horizontal) Console.WriteLine("you chose horizontal!");
            else Console.WriteLine("You chose vertical!");

            Console.WriteLine("Select coordinate to place:");
            shipCoord = Console.ReadLine().ToUpper(); //A1 - G8
            string tempShipCoord = shipCoord;
            string[] shipCoords = new string[5]; //used to store all the coordinates of the ship that is to be placed
            int tempShipSize = 0;
            bool shipSuccessfullyPlaced = false;

            for (int c=0; c<GridYaxis.Length; c++)
            {
                for (int i = 1; i < 9; i++)
                {
                    if (tempShipSize < shipSize) //checking if the whole ship size hasn't been confirmed to fit
                    {
                        if (tempShipCoord == GridYaxis[c].ToString() + i) //checks if the position selected is real
                        {
                            //add check to see if there is a ship already there
                            //^ query gameshipconfig, where playerFK

                            tempShipSize++;
                            shipCoords.Append(GridYaxis[c].ToString() + i);
                            if (horizontal) //horizontal starts counting from the left
                            {
                                tempShipCoord = GridYaxis[c].ToString() + (i + 1); //sets the next ship coordinate that needs checking
                            }
                            else //vertical starts counting from the top
                            {
                                tempShipCoord = GridYaxis[c+1].ToString() + i;
                            }
                        }
                    }

                    if (tempShipSize == shipSize) //checking if ship is confirmed to fit on the grid
                    {
                        shipSuccessfullyPlaced = true;
                        Console.WriteLine("Ship has been placed!");
                        foreach(string coord in shipCoords)//test
                        {
                            Console.WriteLine(coord);
                        }
                        //business.CreateNewShipCoord("plr", 1, shipCoord);
                        return;
                    }
                }
            }

            if(!shipSuccessfullyPlaced)
            {
                Console.WriteLine("Incorrect position!");
            }

        }

        //menu3
        private void LaunchAttackUI()
        {
            Console.WriteLine("Launch Attack");
        }

        //Will repeatedly display the menu to the user, until they select the quit option
        public void Menu()
        {
            int choice;
            while (true)
            {
                try
                {
                    Console.Clear();
                    Console.WriteLine("1. Add Player Details\n2. Configure Ships\n3. Launch Attack\n4. Quit");
                    choice = Convert.ToInt16(Console.ReadLine());

                    switch (choice)
                    {
                        case 1: // Add Player Details
                            if (menuOptsEnabled[0]) AddPlrsDtlsUI();
                            else Console.WriteLine("Players have already been selected!");
                            break;

                        case 2: // Configure Ships
                            if (menuOptsEnabled[1]) ConfigShipsUI();
                            else if (menuOptsEnabled[0]) Console.WriteLine("Please choose your players first!");
                            else Console.WriteLine("Ship configurations have been already set!");
                            break;

                        case 3: // Launch Attack
                            if (menuOptsEnabled[2]) LaunchAttackUI();
                            else if (menuOptsEnabled[0]) Console.WriteLine("Please choose your players first!");
                            else if (menuOptsEnabled[1]) Console.WriteLine("Please set the ship positions first!");
                            break;

                        case 4: // Quit
                            Console.WriteLine("Goodbye...");
                            Environment.Exit(0);
                            break;
                        case 5:
                            Console.WriteLine("testing: ");
                            Console.WriteLine(business.AreGamesCompleteUsername(players[0].Username, players[1].Username)) ;
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
