using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace GameLogicServer
{
    public class NetworkObjectManager
    {
        public enum ENetworkObjectType
        {

        }
        public struct STransform
        {

        }
        [StructLayout(LayoutKind.Explicit)]
        public struct SNetworkObject
        {
            [FieldOffset(0)]
            ENetworkObjectType netObjectType;

            [FieldOffset(sizeof(ENetworkObjectType))]
            STransform transform;

            // 여기서 NetworkType에 따른 무수히 많은 struct들을 만들어볼까? --> 쓰읍 이건 좀...
        }
        uint networkID = 0;
        Dictionary<uint, SNetworkObject> networkObjects = new Dictionary<uint, SNetworkObject>();
        public uint GetNetworkID(SNetworkObject networkObj)
        {
            networkObjects.Add(networkID, networkObj);
            return networkID++;
        }
        public void UpdateNetworkObject(uint id, SNetworkObject networkObj)
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
