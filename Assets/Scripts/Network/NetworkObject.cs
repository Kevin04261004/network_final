using System;
using UnityEngine;

public class NetworkObject : MonoBehaviour
{
    private uint id;
    private NetworkObjectManager _manager;

    private void Awake()
    {
        _manager = FindAnyObjectByType<NetworkObjectManager>();
    }

    private void OnEnable()
    {
        _manager.NetworkObjects.Add(id, this);
    }

    private void OnDisable()
    {
        _manager.NetworkObjects.Remove(id);
    }
}
