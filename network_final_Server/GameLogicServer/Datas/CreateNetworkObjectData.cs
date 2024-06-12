using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace GameLogicServer
{
    [StructLayout(LayoutKind.Sequential)]
    public struct CreateNetworkObjectData
    {
        [MarshalAs(UnmanagedType.I4)]
        public Int32 _count;
        [MarshalAs(UnmanagedType.I4)]
        public NetworkObjectDataInfo.ENetworkObjectType networkObjectType;
    }
}
