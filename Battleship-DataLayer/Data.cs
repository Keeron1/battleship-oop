using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battleship_DataLayer
{
    public class Data
    {
        private BattleshipDatabaseEntities db = new BattleshipDatabaseEntities();
        public Data()
        {

        }

        public IQueryable<Players> GetUser(string username)
        {
            var result = from plr in db.Players where plr.Username == username select plr;
            return result;
        }

        public IQueryable<Players> ValidUserPassword(string password)
        {
            var result = from plr in db.Players where plr.Password == password select plr;
            return result;
        }

        public void CreateNewPlayer(string username, string password)
        {
            Players player = new Players(username, password);
            db.Players.Add(player);
            db.SaveChanges();
        }

        public IQueryable<Games> ValidTitle(string title)
        {
            var result = from game in db.Games where game.Title == title select game;
            return result;
        }

        public void CreateNewGame(string title, string creatorFK, string opponentFK)
        {
            Games game = new Games(title, creatorFK, opponentFK, false);
            db.Games.Add(game);
            db.SaveChanges();
        }

        public IQueryable<Games> GetGameUsername(string creatorFK, string opponentFK)
        {
            var result = from game in db.Games where game.CreatorFK == creatorFK && game.OpponentFK == opponentFK select game;
            if (result.Count() > 0)
            {
                return result;
            }
            return null;
            
        }

        public IQueryable<Ships> GetShips()
        {
            var result = from ship in db.Ships select ship;
            return result;
        }

        public IQueryable<Ships> GetShip(int id)
        {
            var result = from ship in db.Ships where ship.ID == id select ship;
            return result;
        }

        public IQueryable<GameShipConfigurations> GetGameShipCID(int id)
        {
            var result = from ship in db.GameShipConfigurations where ship.ShipFK == id select ship;
            return result;
        }

        public void CreateShipCoord(string playerFK, int gameFK, int shipFK, string coord)
        {
            GameShipConfigurations gsc = new GameShipConfigurations(playerFK, gameFK, shipFK, coord);
            db.GameShipConfigurations.Add(gsc);
            db.SaveChanges();
        }

    }

    partial class Players
    {
        public Players(string Username, string Password)
        {
            this.Username = Username;
            this.Password = Password;
        }
    }

    partial class Games
    {
        public Games(string title, string creatorFK, string opponetFK, bool complete)
        {
            this.Title = title;
            this.CreatorFK = creatorFK;
            this.OpponentFK = opponetFK;
            this.Complete = complete;
        }

        public Games(int id, string title, string creatorFK, string opponetFK, bool complete)
        {
            this.ID = id;
            this.Title = title;
            this.CreatorFK = creatorFK;
            this.OpponentFK = opponetFK;
            this.Complete = complete;
        }
    }

    partial class GameShipConfigurations
    {
        public GameShipConfigurations() { }
        public GameShipConfigurations(string playerFK, int gameFK, int shipFK, string coord) 
        {
            this.PlayerFK = playerFK;
            this.GameFk = gameFK; 
            this.ShipFK = shipFK;
            this.Coordinate = coord;
        }
    }
}
