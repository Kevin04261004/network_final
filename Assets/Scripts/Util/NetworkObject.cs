using System;
using UnityEngine;

public class NetworkObject : MonoBehaviour
{
    private static uint s_id = 0;
    [field: SerializeField] public uint id { get; private set; }

    public NetworkObject()
    {
        id = s_id;
        s_id++;
    }
}
