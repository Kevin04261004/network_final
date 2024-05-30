using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DYUtil
{
    public static class IPFinder
    {
        public static bool TryGetMyIPv4(out string myIp)
        {
            myIp = "127.0.0.1";
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    myIp = ip.ToString();
                    return true;
                }
            }
            return false;
        }
        public static bool TryGetMyIPv6(out string myIp)
        {
            myIp = "";
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetworkV6)
                {
                    myIp = ip.ToString();
                    return true;
                }
            }
            return false;
        }
    }
}
