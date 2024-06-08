using UnityEngine;
using Debug = System.Diagnostics.Debug;

namespace GameLogicServer
{
    public class NetworkObjectData
    {
        public uint _id;
        public NetworkObjectDataInfo.ENetworkObjectType _netObjectType;
        public NetworkObjectDataInfo.STransform _transform;

        public NetworkObjectData(byte[] data)
        {
            Debug.Assert(data != null);
            Debug.Assert(data.Length != NetworkObjectDataInfo.DataSize);

            NetworkObjectData temp = NetworkObjectDataInfo.Deserialize(data);
            _netObjectType = temp._netObjectType;
            _transform = temp._transform;
        }
        public NetworkObjectData(uint id, NetworkObjectDataInfo.ENetworkObjectType objType, NetworkObjectDataInfo.STransform transform)
        {
            _id = id;
            _netObjectType = objType;
            _transform = transform;
        }

        public NetworkObjectData()
        {
            _id = 0;
            _netObjectType = NetworkObjectDataInfo.ENetworkObjectType.Size;
            _transform = new NetworkObjectDataInfo.STransform();
        }
    }
}