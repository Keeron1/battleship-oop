using System;
using System.Collections.Generic;
using System.Data.Entity;
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
            var result = from plr in db.Players select plr; // where plr.Username == username
            return result;
        }
    }
}
