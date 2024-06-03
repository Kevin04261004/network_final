using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLogicServer
{
    public class NetworkObjectManager
    {
        uint networkID = 0;

        public uint GetNetworkID()
        {
            return networkID++;
        }
    }
}
