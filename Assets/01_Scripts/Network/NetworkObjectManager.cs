using System.Collections.Generic;
using UnityEngine;

public class NetworkObjectManager : MonoBehaviour
{
    public Dictionary<uint, NetworkObject> NetworkObjects { get; set; } = new Dictionary<uint, NetworkObject>();

    public uint GetIDFromServer()
    {
        return 0;
    }
}
