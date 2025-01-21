using Battleship_BusinessLayer;
using Battleship_DataLayer;

using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using static System.Net.Mime.MediaTypeNames;

namespace PresentationLayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    
    // LBTU Windows Programming Home Assignment
    enum GameState
    {
        Login,
        SelectingGame,
        PlacingShips,
        Battling
    }

    public partial class MainWindow : Window
    {
        private Business business = new Business();
        
        private const int gridSizeX = 10;
        private const int gridSizeY = 10;

        private List<Players> players { get; set; } = new List<Players>(2);
        private Players? currentPlayer = null;
        private Games? game = null;
        private GameState? gameState = null;

        // For Battling
        DispatcherTimer? timer = null;
        private int turnTimeLength = 30;
        private int turnTime = 0;

        // For Placing Ships
        private List<Ships>? GameShips { get; set; } = new List<Ships>();
        private Ships? selectedShip = null;
        private List<Ships> shipsPlaced = new List<Ships>();
        private bool placingShipsHorizontally = true;

        public MainWindow()
        {
            InitializeComponent();
            LoadLogin();
        }

        #region Login
        private void LoadLogin()
        {
            gameState = GameState.Login;
            login.Visibility = Visibility.Visible;
            players.Clear();
        }

        private void loginBtn_Click(object sender, RoutedEventArgs e)
        {
            string username = usernameBox.Text;
            string password = passwordBox.Password;
            if (username == "" || password == "")
            {
                MessageBox.Show("Both fields must be filled in!", "Empty Login", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            // Username found
            if (business.CheckUsernameExists(username))
            {
                foreach(Players plr in players)
                {
                    if(plr.Username == username)
                    {
                        MessageBox.Show("This user is already logged in!");
                        return;
                    }
                }
                // Check password, if incorrect then notify user
                if (business.CheckUserPassword(username,password))
                {
                    MessageBox.Show($"Player {players.Count + 1} logged in!");
                    players.Add(business.GetPlayer(username));
                    usernameBox.Text = "";
                    passwordBox.Password = "";
                }
                else
                {
                    MessageBox.Show("Incorrect Password!", "Wrong Login Password", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
            else // User doesn't exist, ask user if they want to create a new user
            {
                MessageBoxResult messageBoxResult = MessageBox.Show("User not found\nWould you like to register this user?", "New User", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    business.CreateNewPlayer(username, password);
                    MessageBox.Show($"Player {players.Count + 1} logged in!");
                    players.Add(business.GetPlayer(username));
                    usernameBox.Text = "";
                    passwordBox.Password = "";
                }
            }

            // Both players logged in
            if (players.Count >= 2)
            {
                login.Visibility = Visibility.Collapsed;
                LoadGameSelector();
            }
        }


        #endregion

        #region GameSelector

        private void LoadGameSelector()
        {
            gameState = GameState.SelectingGame;
            gameSelector.Visibility = Visibility.Visible;
            GetGames();
        }

        private void GetGames()
        {
            ComboBoxItem newGameCbItem = new ComboBoxItem();
            newGameCbItem.Content = "New Game";
            gameSelectCB.Items.Add(newGameCbItem);

            List<Games> activeGames = business.GetActiveGames(players[0].Username, players[1].Username);
            if (activeGames != null)
            {
                foreach(Games game in activeGames)
                {
                    ComboBoxItem comboBoxItem = new ComboBoxItem();
                    comboBoxItem.Content = game.Title;
                    gameSelectCB.Items.Add(comboBoxItem);
                }
            }
            gameSelectCB.SelectedIndex = gameSelectCB.Items.Count-1;
        }

        private string CreateTitle()
        {
            string title = "";
            int matchNo = 0;
            for (int i = 0; i < players.Count(); i++)
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

        private void playBtn_Click(object sender, RoutedEventArgs e)
        {
            if (gameSelectCB.SelectedItem == null)
            {
                MessageBox.Show("Please select a game!", "Invalid Game", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            string gameName = ((ComboBoxItem)gameSelectCB.SelectedItem).Content.ToString();

            if (gameName == "New Game") {
                // Create new game
                string gameTitle = CreateTitle();
                business.CreateNewGame(gameTitle, players[0].Username, players[1].Username);
                gameName = gameTitle;
            }
            game = business.GetActiveGame(players[0].Username, players[1].Username, gameName);
            gameSelectCB.Items.Clear();
            currentPlayer = players[0];
            gameSelector.Visibility = Visibility.Collapsed;
            BuildBoard();
            PlaceShips();
        }

        #endregion

        #region Menu

        private void CleanCurrentState(bool delPlrs = true)
        {
            if(gameState == GameState.PlacingShips)
            {
                CleanPlaceShipsUI();
            }else if(gameState == GameState.Battling)
            {
                CleanBattlingUI();
            }
            gridGame.Children.Clear();
            gridGame.ColumnDefinitions.Clear();
            gridGame.RowDefinitions.Clear();
            GameShips.Clear();
            game = null;
            gameState = null;
            if(delPlrs) players.Clear();
            currentPlayer = null;
        }

        private void menuQuit_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void menuMenu_Click(object sender, RoutedEventArgs e)
        {
            CleanCurrentState();
            LoadLogin();
        }

        private void menuNewGame_Click(object sender, RoutedEventArgs e)
        {
            CleanCurrentState(false);

            string gameTitle = CreateTitle();
            business.CreateNewGame(gameTitle, players[0].Username, players[1].Username);
            game = business.GetActiveGame(players[0].Username, players[1].Username, gameTitle);

            currentPlayer = players[0];
            BuildBoard();
            PlaceShips();
        }

        private void menuRestartGame_Click(object sender, RoutedEventArgs e)
        {
            if (game != null)
            {
                string gameTitle = game.Title;
                business.DeleteGame(game);
                CleanCurrentState(false);
                
                business.CreateNewGame(gameTitle, players[0].Username, players[1].Username);
                game = business.GetActiveGame(players[0].Username, players[1].Username, gameTitle);

                currentPlayer = players[0];
                BuildBoard();
                PlaceShips();
            }
        }

        private void menuDeleteGame_Click(object sender, RoutedEventArgs e)
        {
            if (game != null)
            {
                business.DeleteGame(game);
                CleanCurrentState();
                LoadLogin();
            }
        }

        #endregion

        #region BuildGrid

        // Adds the Column and Row Definitions to the Grid.
        private void BuildBoardGrid()
        {   // + 1 for the numbers and letters
            for (int i = 0; i < gridSizeX + 1; i++)
            {
                ColumnDefinition columnDefinition = new ColumnDefinition();
                columnDefinition.Width = new GridLength(1, GridUnitType.Star);
                gridGame.ColumnDefinitions.Add(columnDefinition);
            }
            for (int i = 0; i < gridSizeY + 1; i++)
            {
                RowDefinition rowDefinition = new RowDefinition();
                rowDefinition.Height = new GridLength(1, GridUnitType.Star);
                gridGame.RowDefinitions.Add(rowDefinition);
            }
        }

        // Adds the numbers into the x axis
        private void BuildBoardNumbers()
        {
            for (int i = 0; i < gridSizeX; i++)
            {
                Border border = new Border();
                border.Background = new SolidColorBrush(Color.FromRgb(32, 49, 70));

                TextBlock textBlock = new TextBlock();
                textBlock.Text = (i + 1).ToString();
                textBlock.Foreground = Brushes.White;
                textBlock.HorizontalAlignment = HorizontalAlignment.Center;
                textBlock.VerticalAlignment = VerticalAlignment.Center;

                border.Child = textBlock;

                Grid.SetRow(border, 0);
                Grid.SetColumn(border, (i + 1));
                gridGame.Children.Add(border);
            }
        }

        // Adds the letters into the y axis
        private void BuildBoardLetters()
        {
            for (int i = 0; i < gridSizeY; i++)
            {
                char letter = (char)(65 + i);

                Border border = new Border();
                border.Background = new SolidColorBrush(Color.FromRgb(32, 49, 70));

                TextBlock textBlock = new TextBlock();
                textBlock.Text = letter.ToString();
                textBlock.Foreground = Brushes.White;
                textBlock.HorizontalAlignment = HorizontalAlignment.Center;
                textBlock.VerticalAlignment = VerticalAlignment.Center;

                border.Child = textBlock;

                Grid.SetRow(border, (i + 1));
                Grid.SetColumn(border, 0);

                gridGame.Children.Add(border);
            }
        }

        // Builds the outer layer of the board
        private void BuildBoardOutside()
        {
            // Add the top left corner block (maybe just set grid background)
            TextBlock textBlock = new TextBlock();
            textBlock.Background = new SolidColorBrush(Color.FromRgb(32, 49, 70));
            Grid.SetRow(textBlock, 0);
            Grid.SetColumn(textBlock, 0);
            gridGame.Children.Add(textBlock);

            BuildBoardNumbers();
            BuildBoardLetters();
        }

        // Builds the slots in each row
        private void BuildBoardSlots()
        {
            for (int y = 1; y <= gridSizeY; y++)
            {
                for (int x = 1; x <= gridSizeX; x++)
                {
                    Border border = new Border();
                    //border.BorderBrush = new SolidColorBrush(Color.FromRgb(144, 144, 144));
                    //border.BorderThickness = new Thickness(1);
                    border.Background = new SolidColorBrush(Color.FromRgb(121, 133, 242));
                    border.Tag = "gameSlot";
                    border.MouseLeftButtonDown += TextBlock_MouseLeftButtonDown;

                    //TextBlock textBlock = new TextBlock();
                    //border.Child = textBlock;

                    Grid.SetRow(border, x);
                    Grid.SetColumn(border, y);
                    gridGame.Children.Add(border);
                }

            }
        }

        // Reset the board slots
        private void ResetBoardSlots()
        {
            foreach (UIElement e in gridGame.Children)
            {
                if (e is Border border && border.Tag as string == "gameSlot")
                {
                    border.Background = new SolidColorBrush(Color.FromRgb(121, 133, 242));
                }
            }
        }

        // Reset specific board slots
        private void ResetBoardSlots(List<string> boardSlots)
        {
            foreach (UIElement e in gridGame.Children)
            {
                int row = Grid.GetRow(e);
                int col = Grid.GetColumn(e);
                char letter = GetRowLetter(row);
                string coord = letter+col.ToString();
                if (boardSlots.Contains(coord) && e is Border border && border.Tag as string == "gameSlot")
                {
                    border.Background = new SolidColorBrush(Color.FromRgb(121, 133, 242));
                }
            }
        }

        // Adds the grid lines
        private void DrawGridLines()
        {
            // Vertical grid lines
            for (int i = 1; i <= gridSizeX; i++)
            {
                Border verticalLine = new Border();
                verticalLine.BorderBrush = Brushes.Gray;
                verticalLine.BorderThickness = new Thickness(1, 0, 0, 0); // 1 px thick left border only
                Grid.SetColumn(verticalLine, i);
                Grid.SetRow(verticalLine, 1);
                Grid.SetRowSpan(verticalLine, gridSizeY + 1); // Span the full height of the grid
                gridGame.Children.Add(verticalLine);
            }

            // Horizontal grid lines
            for (int i = 1; i <= gridSizeY; i++)
            {
                Border horizontalLine = new Border();
                horizontalLine.BorderBrush = Brushes.Gray;
                horizontalLine.BorderThickness = new Thickness(0, 1, 0, 0); // 1 px thick top border only
                Grid.SetRow(horizontalLine, i);
                Grid.SetColumn(horizontalLine, 1);
                Grid.SetColumnSpan(horizontalLine, gridSizeX + 1); // Span the full width of the grid
                gridGame.Children.Add(horizontalLine);
            }
        }

        private char GetRowLetter(int rNo)
        {
            return (char)(64 + rNo);
        }

        private void BuildBoard()
        {
            BuildBoardGrid();
            BuildBoardOutside();
            BuildBoardSlots();
            DrawGridLines();
            gridGame.Visibility = Visibility.Visible;
        }

        #endregion

        #region PlaceShips
        private int[] GetNextCoordinate(int gridRow, int gridCol)
        {
            if (placingShipsHorizontally)
            {
                int tblColNext = gridCol + 1;
                return [gridRow, tblColNext];
            }
            else
            {
                int tblRowNext = gridRow + 1;
                return [tblRowNext, gridCol];
            }
        }

        private int[] GetPreviousCoordinate(int gridRow, int gridCol)
        {
            if (placingShipsHorizontally)
            {
                int tblColNext = gridCol - 1;
                return [gridRow, tblColNext];
            }
            else
            {
                int tblRowNext = gridRow - 1;
                return [tblRowNext, gridCol];
            }
        }

        // Validates the coordinates
        private bool ValidateShipPlacement(int shipSize, int gridRow, int gridCol)
        {
            int shipSizeCounter = shipSize;
            if (placingShipsHorizontally)
            {
                int tblColNext = gridCol + shipSize - 1;
                while (shipSizeCounter > 0)
                {
                    if (tblColNext > gridSizeX) // Ship doesn't fit in the grid
                    {
                        return false;
                    }
                    tblColNext--;
                    shipSizeCounter--;
                };
            }
            else
            {
                int tblRowNext = gridRow - shipSize + 1;
                while (shipSizeCounter > 0)
                {
                    if (tblRowNext < 1) // Ship doesn't fit in the grid
                    {
                        return false;
                    }
                    tblRowNext++;
                    shipSizeCounter--;
                };
            }
            return true;
        }

        // Place the ship on the grid
        private void PlaceShipPart(int gridRow, int gridCol)
        {
            foreach (UIElement e in gridGame.Children)
            {
                int row = Grid.GetRow(e);
                int col = Grid.GetColumn(e);
                if (gridCol == col && gridRow == row && e is Border border)
                {
                    border.Background = Brushes.Black;
                    break;
                }
            }
        }

        // Place the selected ship on the grid
        private void PlaceSelectedShip(int gridRow, int gridCol)
        {
            // 1st coordinate
            business.CreateNewShipCoord(currentPlayer.Username, game.ID, selectedShip.ID, GetRowLetter(gridRow).ToString() + gridCol.ToString());
            PlaceShipPart(gridRow, gridCol);
            
            // Place the ship on the grid
            int shipSizeCounter = selectedShip.Size-1;
            while (shipSizeCounter > 0)
            {
                int[] nextCoord = null;
                if (placingShipsHorizontally)
                {
                    nextCoord = GetNextCoordinate(gridRow, gridCol);
                }
                else
                {   
                    nextCoord = GetPreviousCoordinate(gridRow, gridCol);
                }
                gridRow = nextCoord[0];
                gridCol = nextCoord[1];
                PlaceShipPart(gridRow, gridCol);
                business.CreateNewShipCoord(currentPlayer.Username, game.ID, selectedShip.ID, GetRowLetter(gridRow).ToString() + gridCol.ToString());
                shipSizeCounter--;
            };


            //if (placingShipsHorizontally)
            //{
            //    int tblColNext = gridCol + shipSizeCounter - 1; // -1 for the first iteration
            //    while (shipSizeCounter > 0)
            //    {
            //        PlaceShipPart(gridRow, tblColNext);
            //        tblColNext--;
            //        shipSizeCounter--;
            //    };
            //}
            //else // Vertical
            //{
            //    int tblRowNext = gridRow - shipSizeCounter + 1; // +1 since we're starting from the furthest
            //    while (shipSizeCounter > 0)
            //    {
            //        PlaceShipPart(tblRowNext, gridCol);
            //        tblRowNext++;
            //        shipSizeCounter--;
            //    };
            //}
        }

        // Selects whether the user wants to place the ship horizontally or vertically
        private void RadioButton_Place_Ship_Type_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton rbSelected)
            {
                if (rbSelected.IsChecked == true)
                {
                    string rbValue = rbSelected.Content.ToString();
                    if (rbValue == "Horizontal") placingShipsHorizontally = true;
                    else placingShipsHorizontally = false;
                }
            }
        }

        // Selects the ship the user wants to place
        private void RadioButton_Populate_Ships_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton rbSelected)
            {
                string shipName = rbSelected.Content.ToString();
                foreach (Ships ship in GameShips)
                {
                    if (ship.Title == shipName)
                    {
                        selectedShip = ship;
                        break;
                    }
                }
            }
        }

        // Populate the list with ship name radio buttons
        private void PopulateShipsRB(List<GameShipConfigurations> gameShipsPlaced)
        {
            bool firstShip = true;
            foreach (Ships s in GameShips)
            {
                bool hasBeenPlaced = false;
                foreach (GameShipConfigurations gsc in gameShipsPlaced)
                {
                    if (gsc.ShipFK == s.ID)
                    {
                        hasBeenPlaced = true;
                        break;
                    }
                }

                StackPanel stackPanel = new StackPanel();
                stackPanel.Orientation = Orientation.Horizontal;

                RadioButton radioButton = new RadioButton();
                radioButton.Content = s.Title;
                radioButton.GroupName = "ShipsPlace";
                radioButton.Checked += new RoutedEventHandler(RadioButton_Populate_Ships_Checked);
                if (firstShip)
                {
                    radioButton.IsChecked = true;
                    firstShip = false;
                }
                stackPanel.Children.Add(radioButton);

                TextBlock textBlockSize = new TextBlock();
                textBlockSize.Text = $" | {s.Size.ToString()}";
                stackPanel.Children.Add(textBlockSize);

                TextBlock textBlockStatus = new TextBlock();
                textBlockStatus.Text = hasBeenPlaced ? " | Placed" : " | Not Placed";
                textBlockStatus.Tag = "status";
                stackPanel.Children.Add(textBlockStatus);

                shipsToPlace.Children.Add(stackPanel);
            }
        }

        // Checks if the ship to place overlaps any other ship coordinates
        private bool isShipOverlapping(Ships ship, int gridRow, int gridCol)
        {
            List<GameShipConfigurations> plrShips = business.GetGameShipConfig(game.ID, currentPlayer.Username);
            int shipSizeCounter = ship.Size;
            int[] nextCoord = null;
            do
            {
                string coord = GetRowLetter(gridRow) + gridCol.ToString();
                foreach (GameShipConfigurations sconfig in plrShips)
                {
                    if (sconfig.Coordinate == coord && sconfig.ShipFK != ship.ID) return true;
                }
                if (placingShipsHorizontally) nextCoord = GetNextCoordinate(gridRow, gridCol);
                else nextCoord = GetPreviousCoordinate(gridRow, gridCol);
                gridRow = nextCoord[0];
                gridCol = nextCoord[1];
                shipSizeCounter--;
            }while (shipSizeCounter > 0);
            return false;
        }

        private void DeleteShip(Ships ship)
        {
            DeleteShipOnGrid(ship);

            // Delete the ship from the database
            business.DeleteShip(game.ID, currentPlayer.Username, ship.ID);

            // No need to remove the ship from shipsPlaced list
        }

        // Deletes the ship on the grid
        private void DeleteShipOnGrid(Ships ship)
        {
            List<GameShipConfigurations> plrShips = business.GetGameShipConfig(game.ID, currentPlayer.Username);
            List<string> shipCoords = new List<string>();

            // Get the old ship's coordinates
            foreach (GameShipConfigurations dbShip in plrShips)
            {
                if(dbShip.ShipFK == ship.ID)
                {
                    shipCoords.Add(dbShip.Coordinate);
                }
            }
            // Clean the ship from the grid
            ResetBoardSlots(shipCoords); 
        }

        private void PlaceShipEvent(int tbRow, int tbCol)
        {
            if (selectedShip == null)
            {
                MessageBox.Show("Please select a ship");
                return;
            }

            if (!ValidateShipPlacement(selectedShip.Size, tbRow, tbCol))
            {
                MessageBox.Show("oh no");
            }
            else
            {
                MessageBox.Show($"oh ye {tbCol} {tbRow}");

                if (!isShipOverlapping(selectedShip, tbRow, tbCol)) // Is not overlapping
                {
                    if (shipsPlaced.Contains(selectedShip)) // Ship already placed
                    {
                        MessageBox.Show($"Moving {selectedShip.Title}'s position!", "Battleship", MessageBoxButton.OK, MessageBoxImage.Information);

                        // Delete the old ship
                        DeleteShip(selectedShip);
                    }
                    else // Update the ship's status
                    { 
                        foreach (StackPanel stackPanel in shipsToPlace.Children)
                        {
                            TextBlock? statusFound = null;
                            bool shipFound = false;
                            foreach (UIElement element in stackPanel.Children)
                            {
                                if (element is TextBlock textBlockSt && textBlockSt.Tag as string == "status")
                                {
                                    statusFound = textBlockSt;
                                    if (shipFound != false) break;
                                }
                                if (element is RadioButton radioButton && radioButton.Content == selectedShip.Title)
                                {
                                    shipFound = true;
                                    if (statusFound != null) break;
                                }
                            }
                            if (shipFound && statusFound != null)
                            {
                                if (statusFound.Text == " | Not Placed")
                                {
                                    shipsPlaced.Add(selectedShip);
                                    statusFound.Text = " | Placed";
                                }
                            }
                        }
                    }
                    // Place the ship
                    PlaceSelectedShip(tbRow, tbCol);
                }
                else // Ship is overlapping
                {
                    MessageBox.Show("This ship is overlapping another ship!", "Ship Overlap", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                } 

                // Have all ships been placed?
                if (shipsPlaced.Count >= GameShips.Count)
                {
                    if (players.IndexOf(currentPlayer) == players.Count - 1)
                    {
                        MessageBox.Show("All ships have been placed! Moving to Battle Phase.");
                        CleanPlaceShipsUI();
                        StartBattling();
                    }
                    else
                    {
                        MessageBox.Show($"{players[0].Username}'s ships have been placed. {players[1].Username}'s turn now");
                        currentPlayer = players[1];
                        CleanPlaceShipsUI();
                        PlaceShips();
                    }
                }
                else
                {
                    // Get the next ship and select it
                    RadioButton? checkedRb = null;
                    RadioButton? nextRb = null;
                    foreach (StackPanel stackPanel in shipsToPlace.Children)
                    {
                        if (checkedRb != null & nextRb != null) break;
                        RadioButton? tempNextRb = null;
                        foreach (UIElement element in stackPanel.Children)
                        {
                            if (element is RadioButton radioButton)
                            {
                                if (radioButton.IsChecked == true)
                                {
                                    checkedRb = radioButton;
                                    break;
                                }
                                else tempNextRb = radioButton;
                            }

                            if (element is TextBlock textBlockSt && textBlockSt.Tag as string == "status")
                            {
                                if (textBlockSt.Text == " | Not Placed")
                                {
                                    nextRb = tempNextRb;
                                }
                            }
                        }
                    }
                    if (checkedRb != null && nextRb != null)
                    {
                        nextRb.IsChecked = true;
                    }
                }
            }
        }

        private void PlaceShips()
        {
            gameState = GameState.PlacingShips;
            menuItemGame.IsEnabled = true;
            shipPlacing.Visibility = Visibility.Visible;
            GameShips = business.GetShips();

            // Has user placed all his ships already?
            int gscUnique = business.GetUniqueGameShips(game.ID, currentPlayer.Username);
            if (gscUnique == 5)
            {
                MessageBox.Show($"Player {currentPlayer.Username} has already placed their ships!");
                // Both players have placed all their ships
                if (players.IndexOf(currentPlayer) == players.Count - 1)
                {
                    CleanPlaceShipsUI();
                    StartBattling();
                    return;
                }
                // Player 2's turn
                currentPlayer = players[1];
                CleanPlaceShipsUI();
                PlaceShips();
                return;
            }

            List<GameShipConfigurations> gscShips = business.GetGameShipConfig(game.ID, currentPlayer.Username);
            List<GameShipConfigurations> gscShipsDistinct = gscShips.Distinct().ToList();

            // Place the already placed ships on the grid
            if (gscUnique > 0)
            {
                foreach (Ships ship in GameShips)
                {
                    foreach (GameShipConfigurations gameShip in gscShips)
                    {
                        if (gameShip.ShipFK == ship.ID)
                        {
                            if (!shipsPlaced.Contains(ship))
                            {
                                selectedShip = ship;
                                shipsPlaced.Add(selectedShip);
                            }
                            string coord = gameShip.Coordinate;
                            int letter = coord[0] - 64; // 64 since the playing area starts from the 2nd row
                            int number = int.Parse(coord[1].ToString()); 
                            PlaceShipPart(letter, number);
                        }
                    }
                }
            }
            HorizontalShipType.IsChecked = true; // Set the default placement type
            PopulateShipsRB(gscShipsDistinct);
        }

        private void CleanPlaceShipsUI()
        {
            shipPlacing.Visibility = Visibility.Collapsed;
            menuItemGame.IsEnabled = false;
            shipsToPlace.Children.Clear();
            shipsPlaced.Clear();
            ResetBoardSlots();
        }

        #endregion

        #region Battling

        #region ToolBar
        private void surrender_Click(object sender, RoutedEventArgs e)
        {
            business.SetGameComplete(game);
            MessageBox.Show($"{currentPlayer.Username} has surrendered the game!", "Battleship", MessageBoxButton.OK, MessageBoxImage.Information);
            CleanBattlingUI();
            LoadLogin(); // Go back to menu
        }

        private void skipTurn_Click(object sender, RoutedEventArgs e)
        {
            // Switch to the other player
            if (players[0] == currentPlayer) currentPlayer = players[1];
            else currentPlayer = players[0];
            MessageBox.Show($"Turn Skipped! {currentPlayer.Username}'s turn", "Battleship", MessageBoxButton.OK, MessageBoxImage.Information);
            CleanBattlingUI();
            Battling();
        }
        #endregion

        private void StartBattling()
        {
            // Gets the player whose turn is next
            List<Attacks> allAttacks = business.GetAttacks(game.ID);
            if (allAttacks.Count > 0) // Has battle phase already started?
            {
                int plr1Count = business.GetAttacks(game.ID, players[0].Username).Count;
                int plr2Count = business.GetAttacks(game.ID, players[1].Username).Count;
                if (plr1Count >= plr2Count) currentPlayer = players[0];
                else currentPlayer = players[1];
            }
            else currentPlayer = players[0]; // 1st player starts
            Battling();
        }

        private void Battling()
        {
            gameState = GameState.Battling;
            menuItemGame.IsEnabled = true;
            toolBarGame.Visibility = Visibility.Visible;
            attacksPanel.Visibility = Visibility.Visible;

            // Initalize timer
            timer = new DispatcherTimer();
            turnTime = 0;
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += timer_Tick;
            timer.Start();

            // Set the current player in the textblock
            playerTurnTxt.Text = $"{currentPlayer.Username}'s turn";

            // Load the previous attacks in the listbox and grid
            List<Attacks> allAtks = business.GetAttacks(game.ID, currentPlayer.Username);
            List<Attacks> atks = business.GetAttacks(game.ID, currentPlayer.Username, true);
            PopulateAttacksLb(atks);
            PopulateAttacksGrid(allAtks);
        }

        public void timer_Tick(object sender, EventArgs e)
        {
            // Time passed
            if (turnTime >= turnTimeLength)
            {
                // Switch to the other player
                if (players[0] == currentPlayer) currentPlayer = players[1];
                else currentPlayer = players[0];
                MessageBox.Show($"Time's up! {currentPlayer.Username}'s turn", "Battleship", MessageBoxButton.OK, MessageBoxImage.Information);
                CleanBattlingUI();
                Battling();
            }
            else
            {
                turnTime++;
                turnRemainingTimeTxt.Text = $"Time: {turnTimeLength-turnTime}";
            }
        }

        private void AttackEvent(int tbRow, int tbCol)
        {
            string coord = GetRowLetter(tbRow) + tbCol.ToString();
            List<GameShipConfigurations> opponentShips = business.GetOpponentShips(game.ID, currentPlayer.Username);
            List<Attacks> atks = business.GetAttacks(game.ID, currentPlayer.Username);
            int attacksHit = business.GetAttacks(game.ID, currentPlayer.Username, true).Count();
            
            string attack = isAttackHit(coord, opponentShips, atks);
            switch (attack)
            {
                case "Already Hit":
                    MessageBox.Show("This coordinate has already been hit!", "Attack", MessageBoxButton.OK, MessageBoxImage.Information);
                    break;
                case "Hit":
                    business.CreateAttack(coord, true, game.ID, currentPlayer.Username);
                    attacksHit++;
                    MessageBox.Show("Great Shot!!", "Attack", MessageBoxButton.OK, MessageBoxImage.Information);
                    break;
                case "Miss":
                    business.CreateAttack(coord, false, game.ID, currentPlayer.Username);
                    MessageBox.Show("Shot Missed!", "Attack", MessageBoxButton.OK, MessageBoxImage.Information);
                    break;
            }

            // Player won
            if(attacksHit >= 17)
            {
                business.SetGameComplete(game);
                MessageBox.Show($"{currentPlayer.Username} has won the game!", "Battleship", MessageBoxButton.OK, MessageBoxImage.Information);
                CleanBattlingUI();
                LoadLogin(); // Go back to menu
            }
            else // Game is still ongoing
            {
                // Switch to other player
                if (players[0] == currentPlayer) currentPlayer = players[1];
                else currentPlayer = players[0];
                MessageBox.Show($"{currentPlayer.Username}'s turn", "Battleship", MessageBoxButton.OK, MessageBoxImage.Information);
                CleanBattlingUI();
                Battling();
            }
        }

        private string isAttackHit(string coord, List<GameShipConfigurations> opponentShips, List<Attacks> attacks)
        {
            // Loops through the enemy's ship's coordinates
            foreach (GameShipConfigurations opponent in opponentShips) 
            {
                if (opponent.Coordinate == coord)
                {
                    foreach (Attacks atk in attacks)
                    {
                        if (atk.Coordinate == coord)
                        {
                            return "Already Hit";
                        }
                    }
                    return "Hit";
                }
            }
            return "Miss";
        }

        private void PopulateAttacksLb(List<Attacks> atks)
        {
            foreach (Attacks atk in atks)
            {
                ListBoxItem listBoxItem = new ListBoxItem();
                listBoxItem.Content = atk.Coordinate;
                attacksList.Items.Add(listBoxItem);
            }
        }

        private void PopulateAttacksGrid(List<Attacks> atks)
        {
            foreach (Attacks atk in atks)
            {
                foreach (UIElement e in gridGame.Children)
                {
                    int row = Grid.GetRow(e);
                    int col = Grid.GetColumn(e);
                    char letter = GetRowLetter(row);
                    string coord = letter + col.ToString();
                    if (atk.Coordinate == coord && e is Border border && border.Tag as string == "gameSlot")
                    {
                        if(atk.Hit) border.Background = new SolidColorBrush(Color.FromRgb(173, 28, 28));
                        else border.Background = new SolidColorBrush(Color.FromRgb(28, 173, 30));
                        break;
                    }
                }
            }
        }

        private void CleanBattlingUI()
        {
            turnTime = 0;
            timer.Stop();
            timer = null;
            attacksList.Items.Clear();
            menuItemGame.IsEnabled = false;
            attacksPanel.Visibility = Visibility.Collapsed;
            toolBarGame.Visibility = Visibility.Collapsed;
            ResetBoardSlots();
        }

        #endregion

        // Grid Blocks Event Listener
        private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Border clickedTb = (Border)sender;

            int tbCol = Grid.GetColumn(clickedTb);
            int tbRow = Grid.GetRow(clickedTb);
            char rowLetter = GetRowLetter(tbRow);

            //MessageBox.Show($"You clicked on: {rowLetter}{tbCol}");

            if (gameState == GameState.PlacingShips)
            {
                PlaceShipEvent(tbRow, tbCol);
            }
            else if (gameState == GameState.Battling)
            {
                AttackEvent(tbRow, tbCol);
            }
        }
    }
}