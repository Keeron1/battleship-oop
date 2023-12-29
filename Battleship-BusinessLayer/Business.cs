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
        public Business() { }

        public IQueryable<Players> CheckUsernameExists(string username) 
        {
            //if (dt.UsernameExists(username))
            //{
            //    return true;
            //}
            return dt.UsernameExists(username);
        }
    }
}
