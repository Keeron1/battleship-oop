﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
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

        public IQueryable<Players> ValidUserPassword(string username, string password)
        {
            var result = from plr in db.Players where plr.Password == password && plr.Username == username select plr;
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

        public IQueryable<Games> GetGame(string creatorFK, string opponentFK)
        {
            var result = from game in db.Games where game.CreatorFK == creatorFK && game.OpponentFK == opponentFK select game;
            if (result.Count() > 0)
            {
                return result;
            }
            return null;
        }

        public IQueryable<Games> GetGame(string creatorFK, string opponentFK, string gameTitle)
        {
            var result = from game in db.Games where game.CreatorFK == creatorFK && game.OpponentFK == opponentFK && game.Title == gameTitle select game;
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

        public IQueryable<GameShipConfigurations> GetGameShipConfig(int gameFK, string playerFK)
        {
            var result = from ship in db.GameShipConfigurations where ship.GameFk == gameFK && ship.PlayerFK == playerFK select ship;
            return result;
        }

        public IQueryable<int> GetUniqueGameShips(int gameFK, string playerFK)
        {
            var result = (from ship in db.GameShipConfigurations
                         where ship.GameFk == gameFK && ship.PlayerFK == playerFK
                         select ship.ShipFK).Distinct();
            return result;
        }

        public void CreateShipCoord(string playerFK, int gameFK, int shipFK, string coord)
        {
            GameShipConfigurations gsc = new GameShipConfigurations(playerFK, gameFK, shipFK, coord);
            db.GameShipConfigurations.Add(gsc);
            db.SaveChanges();
        }

        public void CreateAttack(string coord, bool hit, int gameFK, string playerFK)
        {
            Attacks atk = new Attacks(coord, hit, gameFK, playerFK);
            db.Attacks.Add(atk);
            db.SaveChanges();
        }
        
        public IQueryable<Attacks> GetAttacks(int gameFK)
        {
            var result = from atk in db.Attacks where atk.GameFK == gameFK select atk;
            return result;
        }
        public IQueryable<Attacks> GetAttacks(int gameFK, string playerFK)
        {
            var result = from atk in db.Attacks where atk.GameFK == gameFK && atk.PlayerFK == playerFK select atk;
            return result;
        }
        public IQueryable<Attacks> GetAttacks(int gameFK, string playerFK, bool hit) //will either retrive attacks that are a hit or a miss
        {
            var result = from atk in db.Attacks where atk.GameFK == gameFK && atk.PlayerFK == playerFK && atk.Hit == hit select atk;
            return result;
        }

        public IQueryable<GameShipConfigurations>GetOpponentShips(int gameFK, string playerFK)
        {
            var result = from gsc in db.GameShipConfigurations where gsc.GameFk == gameFK && gsc.PlayerFK != playerFK select gsc;
            return result;
        }

        public void SetGameComplete(Games game)
        {
            game.Complete = true;
            db.SaveChanges();
        }

        public void DeleteShip(int gameFK, string playerFK, int shipFK)
        {
            var rowsToDelete = db.GameShipConfigurations.Where(gsc => gsc.GameFk == gameFK &&
                                                               gsc.PlayerFK == playerFK &&
                                                               gsc.ShipFK == shipFK);
            if (rowsToDelete.Any())
            {
                db.GameShipConfigurations.RemoveRange(rowsToDelete);
                db.SaveChanges();
            }
        }

        public void DeleteGame(Games game)
        {
            // Remove attacks
            var attacks = db.Attacks.Where(a => a.GameFK == game.ID).ToList();
            db.Attacks.RemoveRange(attacks);

            // Remove player's ships
            var gameShips = db.GameShipConfigurations.Where(gsc => gsc.GameFk == game.ID).ToList();
            db.GameShipConfigurations.RemoveRange(gameShips);

            // Remove the game
            db.Games.Remove(game);
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

    partial class Attacks
    {
        public Attacks() { }
        public Attacks(string Coordinate, bool Hit, int GameFK, string playerFk)
        {
            this.Coordinate = Coordinate;
            this.Hit = Hit;
            this.GameFK = GameFK;
            this.PlayerFK = playerFk;
        }
        public Attacks(int ID, string Coordinate, bool Hit, int GameFK, string playerFK)
        {
            this.ID = ID;
            this.Coordinate = Coordinate;
            this.Hit = Hit;
            this.GameFK = GameFK;
            this.PlayerFK = playerFK;
        }
    }

}
