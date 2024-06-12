using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLogicServer.Datas.Database
{
    public class DB_UserLoginInfo
    {  
        public string NickName { get; set; }
        public string Id { get; set; }
        public string Password { get; set; }

        public DB_UserLoginInfo(string nickName, string id, string password)
        {
            NickName = nickName;
            Id = id;
            Password = password;
        }
        
    }
}