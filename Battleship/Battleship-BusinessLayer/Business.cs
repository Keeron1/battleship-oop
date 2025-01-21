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

        public bool CheckUserPassword(string username, string password)
        {
            if (dt.ValidUserPassword(username, password).Count() > 0)
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
            IQueryable<Games> games = dt.GetGame(creatorFK, opponentFK);
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
        public Games GetActiveGame(string creatorFK, string opponentFK) //using usernames to check
        {
            IQueryable<Games> games = dt.GetGame(creatorFK, opponentFK);
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

        public Games GetActiveGame(string creatorFK, string opponentFK, string gameTitle) //using usernames to check
        {
            IQueryable<Games> game = dt.GetGame(creatorFK, opponentFK, gameTitle);
            return game.FirstOrDefault(); // returns the game or null by default
        }

        public List<Games> GetActiveGames(string creatorFK, string opponentFK) //using usernames to check
        {
            IQueryable<Games> games = dt.GetGame(creatorFK, opponentFK);
            if (games == null)
            {
                return null;
            }

            List<Games> activeGames = new List<Games>();
            foreach (Games game in games)
            {
                if (game.Complete == false)
                {
                    activeGames.Add(game);
                }
            }
            if (activeGames.Count > 0) return activeGames;
            return null;
        }

        public List<Ships> GetShips()
        {
            return dt.GetShips().ToList();
        }

        public void CreateNewShipCoord(string playerFK, int gameFK, int shipFK, string coord)
        {
            dt.CreateShipCoord(playerFK, gameFK, shipFK, coord);
        }

        public List<GameShipConfigurations> GetGameShipConfig(int gameFK, string playerFK)
        {
            IQueryable<GameShipConfigurations> gsc = dt.GetGameShipConfig(gameFK, playerFK);
            if (gsc != null)
            {
                return gsc.ToList();
            }
            return null;
        }
        public int GetUniqueGameShips(int gameFK, string playerFK)
        {
            IQueryable<int> gsc = dt.GetUniqueGameShips(gameFK, playerFK);
            if (gsc != null)
            {
                foreach(int i in gsc) //running the query
                {
                    return gsc.Count();
                }
               
            }
            return 0;
        }
        public void CreateAttack(string coord, bool hit, int gameFK, string playerFK)
        {
            dt.CreateAttack(coord, hit, gameFK, playerFK);
        }

        public List<Attacks> GetAttacks(int gameFK)
        {
            IQueryable<Attacks> atk = dt.GetAttacks(gameFK);
            if (atk != null)
            {
                return atk.ToList();
            }
            return null;
        }

        public List<Attacks> GetAttacks(int gameFK, string playerFK)
        {
            IQueryable<Attacks> atk = dt.GetAttacks(gameFK, playerFK);
            if (atk != null)
            {
                return atk.ToList();
            }
            return null;
        }

        public List<Attacks> GetAttacks(int gameFK, string playerFK, bool hit)
        {
            IQueryable<Attacks> atk = dt.GetAttacks(gameFK, playerFK, hit);
            if (atk != null)
            {
                return atk.ToList();
            }
            return null;
        }

        public List<GameShipConfigurations> GetOpponentShips(int gameFK, string playerFK)
        {
            IQueryable<GameShipConfigurations> gsc = dt.GetOpponentShips(gameFK, playerFK);
            if(gsc != null)
            {
                return gsc.ToList();
            }
            return null;
        }

        public void SetGameComplete(Games game)
        {
            dt.SetGameComplete(game);
        }

        public void DeleteShip(int gameFK, string playerFK, int shipFK)
        {
            dt.DeleteShip(gameFK, playerFK, shipFK);
        }

        public void DeleteGame(Games game)
        {
            dt.DeleteGame(game);
        }
    }
}
