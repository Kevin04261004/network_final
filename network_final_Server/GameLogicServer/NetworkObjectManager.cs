using GameLogicServer.Datas;

namespace GameLogicServer
{
    public class NetworkObjectManager
    {
        private uint networkID = 1;
        private Dictionary<uint, NetworkObjectData> networkObjects = new Dictionary<uint, NetworkObjectData>();
        
        public uint CreateNetworkObject(CreateNetworkObjectData createNetworkObjectData)
        {
            uint startID = networkID;
            for(int i = 0; i < networkObjects.Count; ++i)
            {
                uint id = GetNetworkID();
                NetworkObjectData newObjData = new NetworkObjectData(id, createNetworkObjectData.networkObjectType);
                networkObjects.Add(id, newObjData);
            }
            return startID;
        }

        private uint GetNetworkID()
        {
            return networkID++;
        }
        public void UpdateNetworkObject(uint id, NetworkObjectData networkObj)
        {
            if(!networkObjects.ContainsKey(id))
            {
                return;
            }
            networkObjects[networkID] = networkObj;
        }
        public void RemoveNetworkObjectFromDictionary(uint id)
        {
            networkObjects.Remove(id);
        }
    }
}
