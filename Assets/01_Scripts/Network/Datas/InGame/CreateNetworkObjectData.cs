using System;
using System.Runtime.InteropServices;

namespace GameLogicServer
{
    [StructLayout(LayoutKind.Sequential)]
    public struct CreateNetworkObjectData
    {
        [MarshalAs(UnmanagedType.I4)]
        public Int32 _count;
        [MarshalAs(UnmanagedType.I4)]
        public NetworkObjectDataInfo.ENetworkObjectType _networkObjectType;
    }
}