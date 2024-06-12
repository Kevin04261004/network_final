using GameLogicServer.Datas;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace GameLogicServer
{
    public static class NetworkObjectDataInfo
    {
        public static int IDSize
        {
            get => sizeof(uint);
        }

        public static int NetworkObjectTypeSize
        {
            get => sizeof(Int32);
        }

        public static int STransformSize
        {
            get => Marshal.SizeOf<STransform>();
        }
        public enum ENetworkObjectType
        {
            None,
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct STransform
        {
            [MarshalAs(UnmanagedType.R4)]
            public float posX;
            [MarshalAs(UnmanagedType.R4)]
            public float posY;
            [MarshalAs(UnmanagedType.R4)]
            public float posZ;

            [MarshalAs(UnmanagedType.R4)]
            public float rotX;
            [MarshalAs(UnmanagedType.R4)]
            public float rotY;
            [MarshalAs(UnmanagedType.R4)]
            public float rotZ;

            [MarshalAs(UnmanagedType.R4)]
            public float scaleX;
            [MarshalAs(UnmanagedType.R4)]
            public float scaleY;
            [MarshalAs(UnmanagedType.R4)]
            public float scaleZ;
        }
        public static int DataSize { get => (IDSize + NetworkObjectTypeSize + STransformSize); }

        public static byte[] Serialize(NetworkObjectData networkObj)
        {
            Debug.Assert(networkObj != null);

            byte[] objTypeBytes = BitConverter.GetBytes((int)networkObj._netObjectType);
            byte[] transformBytes = MarshalingTool.StructToByte(networkObj._transform);
            byte[] data = new byte[DataSize];

            int offset = 0;
            Array.Copy(objTypeBytes, 0, data, offset, objTypeBytes.Length);
            offset += objTypeBytes.Length;
            Array.Copy(transformBytes, 0, data, offset, transformBytes.Length);
            offset += transformBytes.Length;

            return data;
        }

        public static uint GetID(byte[] data)
        {
            return Convert.ToUInt32(data);
        }

        public static ENetworkObjectType GetNetworkObjectType(byte[] data)
        {
            byte[] objTypeData = new byte[NetworkObjectTypeSize];
            Array.Copy(data, IDSize, objTypeData, 0, NetworkObjectTypeSize);
            ENetworkObjectType objType = (ENetworkObjectType)Convert.ToUInt32(objTypeData);
            return objType;
        }

        public static STransform GetTransformData(byte[] data)
        {
            byte[] transformBytes = new byte[Marshal.SizeOf<STransform>()];
            Array.Copy(data, IDSize + NetworkObjectTypeSize, transformBytes, 0, transformBytes.Length);
            STransform transform = MarshalingTool.ByteToStruct<STransform>(transformBytes);

            return transform;
        }
        public static NetworkObjectData Deserialize(byte[] data)
        {
            Debug.Assert(data != null);

            uint id = GetID(data);
            ENetworkObjectType objType = GetNetworkObjectType(data);
            STransform transform = GetTransformData(data);

            return new NetworkObjectData(id, objType, transform);
        }
    }
}
