using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLogicServer.Datas.Database
{
    public class DB_UserLoginInfo
    {  
        public string Id { get; set; }
        public string Password { get; set; }
        public string NickName { get; set; }

        public DB_UserLoginInfo()
        {
            Id = string.Empty;
            Password = string.Empty;
            NickName = string.Empty;
        }
        public DB_UserLoginInfo(string id, string password, string nickName)
        {
            Id = id;
            Password = password;
            NickName = nickName;
        }

    }
}
