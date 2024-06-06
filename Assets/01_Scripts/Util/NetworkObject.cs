using GameLogicServer;
using UnityEngine;

namespace Util
{
    public class NetworkObject : MonoBehaviour
    {
        public NetworkObjectData NetworkObjectData { get; private set; } = new NetworkObjectData();
    }
}