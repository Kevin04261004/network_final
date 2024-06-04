using System.Diagnostics;

namespace GameLogicServer
{

    public class NetworkObjectData
    {
        public uint _id;
        public NetworkObjectInfoData.ENetworkObjectType _netObjectType;
        public NetworkObjectInfoData.STransform _transform;

        public NetworkObjectData(byte[] data)
        {
            Debug.Assert(data != null);
            Debug.Assert(data.Length != NetworkObjectInfoData.DataSize);

            NetworkObjectData temp = NetworkObjectInfoData.Deserialize(data);
            _netObjectType = temp._netObjectType;
            _transform = temp._transform;
        }
        public NetworkObjectData(uint id, NetworkObjectInfoData.ENetworkObjectType objType, NetworkObjectInfoData.STransform transform = default)
        {
            _id = id;
            _netObjectType = objType;
            _transform = transform;
        }
    }
}
