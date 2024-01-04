using Battleship_DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battleship_BusinessLayer
{
    public class Business
    {
        private Data dt = new Data();
        public Business()
        {

        }

        public bool CheckUsernameExists(string username)
        {
            if (dt.GetUser(username).Count() > 0)
            {
                return true;
            }
            return false;
        }

        public bool CheckUserPassword(string password)
        {
            if (dt.ValidUserPassword(password).Count() > 0)
            {
                return true;
            }
            return false;
        }

        public void CreateNewPlayer(string username, string password)
        {

            dt.CreateNewPlayer(username, password);
        }

        public Players GetPlayer(string username)
        {
            IQueryable<Players> dbResponse = dt.GetUser(username);
            if (dbResponse.Count() > 0) //user does not exist
            {
                Players plr = new Players();
                foreach (Players plyr in dt.GetUser(username))
                {
                    plr = new Players(plyr.Username, plyr.Password);
                }
                return plr;
            }
            return null;
        }

        public bool ValidTitle(string title)
        {
            if (dt.ValidTitle(title).Count() > 0)
            {
                return true;
            }
            return false;
        }

        public void CreateNewGame(string title, string creatorFK, string opponentFK)
        {
            dt.CreateNewGame(title, creatorFK, opponentFK);
        }

        //checks if there is an active match between player 1 and player 2
        public bool AreGamesCompleteUsername(string creatorFK, string opponentFK) //using the usernames to check
        {
            IQueryable<Games> games = dt.GetGameUsername(creatorFK, opponentFK);
            if(games == null)
            {
                return true;
            }
            else
            {
                foreach (Games game in games)
                {
                    if (game.Complete == false)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        //gets the current active match between player 1 and player 2
        public Games GetActiveGameUsername(string creatorFK, string opponentFK) //using usernames to check
        {
            IQueryable<Games> games = dt.GetGameUsername(creatorFK, opponentFK);
            if (games == null)
            {
                return null;
            }
            else
            {
                foreach (Games game in games)
                {
                    if (game.Complete == false)
                    {
                        return game;
                    }
                }
            }
            return null;
        }

        public IQueryable<Ships> GetShips()
        {
            return dt.GetShips();
        }

        public void CreateNewShipCoord(string playerFK, int gameFK, int shipFK, string coord)
        {
            dt.CreateShipCoord(playerFK, gameFK, shipFK, coord);
        }

        public IQueryable<GameShipConfigurations> GetGameShipConfig(int gameFK, string playerFK)
        {
            IQueryable<GameShipConfigurations> gsc = dt.GetGameShipConfig(gameFK, playerFK);
            if (gsc != null)
            {
                return gsc;
            }
            return null;
        }
        public IQueryable<GameShipConfigurations> GetGameShipConfig(int gameFK, string playerFK, int shipFk)
        {
            IQueryable<GameShipConfigurations> gsc = dt.GetGameShipConfig(gameFK, playerFK, shipFk);
            if (gsc != null)
            {
                return gsc;
            }
            return null;
        }

    }
}
