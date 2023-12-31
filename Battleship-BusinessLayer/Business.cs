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
            if (dt.UsernameExists(username).Count() > 0)
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
    }
}
