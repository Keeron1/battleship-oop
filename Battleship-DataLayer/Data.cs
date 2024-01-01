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

        public IQueryable<Players> UsernameExists(string username)
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

        public IQueryable<Ships> GetShips()
        {
            var result = from ship in db.Ships select ship;
            return result;
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
}
