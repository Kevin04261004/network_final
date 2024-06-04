using System;
using System.Collections.Generic;
using GameLogicServer;
using UnityEngine;

public class NetworkObjectManager : MonoBehaviour
{
    public Dictionary<uint, NetworkObjectData> NetworkObjects { get; set; } = new Dictionary<uint, NetworkObjectData>();
    private NetworkManager networkManager;
    private void Awake()
    {
        networkManager = FindAnyObjectByType<NetworkManager>();
    }

    public uint GetIDFromServer(NetworkObjectData networkObjectData)
    {
        return 0;
    }
    
}
