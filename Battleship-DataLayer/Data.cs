using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battleship_DataLayer
{
    public class Data 
    {
        public Data() 
        { 

        }

         BattleshipDatabaseEntities db = new BattleshipDatabaseEntities();
        
        public IQueryable<Players> UsernameExists(string username)
        {
            var result = from plr in db.Players select plr; // where plr.Username == username
            return result;
        }
    }
}
