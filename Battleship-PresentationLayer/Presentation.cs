using Battleship_BusinessLayer;
using Battleship_DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
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
        private bool[] menuOptsEnabled = { true, true, true }; //add players, ship config, attack

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

        private Ships AskShipId()
        {
            Ships ship = new Ships();
            bool shipFound = false;
            int shipId;
            do //add validation later on to see if user has already placed the ship, if they try then allow them to move the position
            {
                Console.WriteLine("Select ship no to place:");
                string shipNoTemp = Console.ReadLine();

                //check if valid ship has been selected
                if (int.TryParse(shipNoTemp, out shipId))
                {
                    foreach (Ships s in GameShips)
                    {
                        if (s.ID == shipId)
                        {
                            shipFound = true;
                            ship = s;
                        }
                    }
                }
                if (!shipFound)
                {
                    Console.WriteLine("Please select a ship!");
                    Console.WriteLine("To select a ship write it's number");
                }
            } while (!shipFound);
            return ship;
        } //still requires little work

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

        private void PrintGrid() //still requires work
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
        private void AddPlrsDtlsUI() //still requires little work
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
                    Console.WriteLine("A game has been created!");
                }
                else
                {
                    Console.WriteLine("A game already exists between both players!");
                    //make option to ask if users would like to start again
                }
                menuOptsEnabled[0] = false; //disables the add players option
            }
        }

        //menu 2
        private void ConfigShipsUI()
        {
            Console.Clear();

            //current game between both players
            Games game = business.GetActiveGameUsername(players[0].Username, players[1].Username);
            GameShips = business.GetShips();

            for (int p=0; p<2; p++)
            {
                for (int y = 0; y < 5; y++)
                {
                    Ships ship; //to store the ship chosen by the user
                    int shipSize; //stores the ship's size

                    bool horizontal; //whether the user selected for the ship to be placed horizontally or vertically

                    List<string> shipCoords = new List<string>(); //used to store all the coordinates of the ship that is to be placed
                    string shipCoord; //stores the original position the user chose

                    Console.Clear();
                    PrintGrid();
                    Console.WriteLine();
                    Console.WriteLine($"{players[p].Username}'s turn:");
                    foreach (Ships s in GameShips)
                    {
                        Console.WriteLine($"Ship: {s.ID} Title: {s.Title} Size: {s.Size} Not Placed");
                    }

                    ship = AskShipId();
                    shipSize = ship.Size;

                    horizontal = AskHorizontal();
                    if (horizontal) Console.WriteLine("you chose horizontal!");
                    else Console.WriteLine("You chose vertical!");

                    bool shipSuccessfullyPlaced = false;
                    do //inf loop so that the user can choose a grid position even if they mess up
                    {
                        Console.Clear();
                        PrintGrid();
                        Console.WriteLine();

                        Console.WriteLine($"{players[p].Username}'s turn:");
                        Console.WriteLine($"Ship selected: {ship.ID} Title: {ship.Title} Size: {ship.Size}");
                        if(horizontal) Console.WriteLine("Placing ship horizontally");
                        else Console.WriteLine("Placing ship vertically");
                        Console.WriteLine();
                        Console.WriteLine("Select coordinate to place:");
                        shipCoord = Console.ReadLine().ToUpper(); //A1 - G8

                        int tempShipSize = 0; //used to determine if all ship coords have been found
                        
                        for (int c = 0; c < GridYaxis.Length; c++) //grid starts generating from a1 > a2 > a3...
                        {
                            for (int i = 1; i < 9; i++)
                            {
                                if (tempShipSize < shipSize) //checking if the whole ship size hasn't been confirmed to fit
                                {
                                    if (shipCoord == GridYaxis[c].ToString() + i) //checks if the position selected is a real grid position
                                    {
                                        //add check to see if there is a ship already there
                                        //^ query gameshipconfig, where playerFK

                                        tempShipSize++;
                                        shipCoords.Add(GridYaxis[c].ToString() + i); //appends the ship coordinate to an array

                                        if (horizontal) //horizontal starts counting from the left
                                        {
                                            shipCoord = GridYaxis[c].ToString() + (i + 1); //sets the next ship coordinate that needs checking
                                        }
                                        else //vertical starts counting from the top
                                        {
                                            shipCoord = GridYaxis[c + 1].ToString() + i;
                                        }
                                    }
                                }

                                if (tempShipSize == shipSize && !shipSuccessfullyPlaced) //checking if ship is confirmed to fit on the grid
                                { //checking if the ship sizes are the same and if the ship hasn't been saved yet
                                  //need to test return but I think it will quit all loops
                                    shipSuccessfullyPlaced = true;
                                    foreach (string coord in shipCoords)
                                    {   
                                        business.CreateNewShipCoord(players[p].Username, game.ID, ship.ID, coord);
                                    }
                                }
                            }
                        }

                        if (!shipSuccessfullyPlaced)
                        {
                            Console.WriteLine("Incorrect position!");
                        }
                        else Console.WriteLine("Ship has been placed!");
                        Console.ReadKey();
                    } while (!shipSuccessfullyPlaced);
                }
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
