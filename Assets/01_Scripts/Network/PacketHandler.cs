using System.Collections.Generic;
using System.Net;
using UnityEngine;

public abstract class PacketHandler<T> : MonoBehaviour
{
    public static PacketHandler<T> Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    protected Dictionary<T, PacketHandlerEvent> packetHandlerEvents = new Dictionary<T, PacketHandlerEvent>();
    public delegate void PacketHandlerEvent(IPEndPoint endPoint, byte[] data);
    public abstract void SetHandler(T packetType, PacketHandlerEvent handler);
    public void ProcessPacket(IPEndPoint serverIPEndPoint, T type, byte[] packetBuffer)
    {
        Debug.Log($"[{serverIPEndPoint.Address}] packetType: {type}");
        if (packetHandlerEvents[type] == null)
        {
            Debug.LogError("[Process Packet] No Function for process {type} packet");
        }
        else
        {
            packetHandlerEvents[type](serverIPEndPoint, packetBuffer);
        }
    }
}
