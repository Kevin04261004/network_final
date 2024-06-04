using GameLogicServer;
using UnityEngine;

namespace Util
{
    public class NetworkObject : MonoBehaviour
    {
        [field: SerializeField] public NetworkObjectData NetworkObjectData { get; private set; }
    }
}