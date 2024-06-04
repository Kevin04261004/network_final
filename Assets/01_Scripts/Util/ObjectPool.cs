using System;
using System.Collections.Generic;
using GameLogicServer;
using UnityEngine;
using Util;

public class ObjectPool : MonoBehaviour
{
    [field: SerializeField] public GameObject[] _objectPrefab { get; private set; }
    [SerializeField] private int _defaultCount;
    [SerializeField] private int _addCount;

    private Dictionary<NetworkObjectDataInfo.ENetworkObjectType, Queue<GameObject>> _objectPoolQueue =
        new Dictionary<NetworkObjectDataInfo.ENetworkObjectType, Queue<GameObject>>();
    private NetworkManager networkManager;

    private void Awake()
    {
        networkManager = FindAnyObjectByType<NetworkManager>();
        
        InitPool();
    }

    public void InitPool()
    {
        foreach (var pool in _objectPoolQueue)
        {
            ExtendPool(pool.Key, _defaultCount);
        }
    }

    private void ExtendPool(NetworkObjectDataInfo.ENetworkObjectType objectType, int count)
    {
        InstantiateNetworkObject(objectType, count);
    }

    private void InstantiateNetworkObject(NetworkObjectDataInfo.ENetworkObjectType objectType, int count)
    {
        CreateNetworkObjectData networkObjectData;
        networkObjectData._count = count;
        networkObjectData.networkObjectType = objectType;
        
        byte[] networkObjBytes = MarshalingTool.StructToByte(networkObjectData);
        PacketData packetData = new PacketData(PacketDataInfo.EGameLogicPacketType.Client_RequireCreateNetworkObject, networkObjBytes);
        networkManager.SendToServer(ESendServerType.GameLogic, packetData.ToPacket());
    }
    
    public GameObject Get(NetworkObjectDataInfo.ENetworkObjectType objectType, Vector3 pos = default, Quaternion rotation = default, Transform parent = null)
    {
        Debug.Assert(objectType >= NetworkObjectDataInfo.ENetworkObjectType.Size);
        if (_objectPoolQueue[objectType].Count <= 0)
        {
            ExtendPool(_addCount);
        }

        GameObject go = _objectPoolQueue[objectType].Dequeue();

        if (parent == null)
        {
            go.transform.position = pos;
            go.transform.rotation = rotation;
        }
        else
        {
            go.transform.parent = parent;
            go.transform.localPosition = pos;
            go.transform.localRotation = rotation;
        }
        
        go.SetActive(true);

        return go;
    }

    public void Return(GameObject go)
    {
        NetworkObjectDataInfo.ENetworkObjectType networkObjectType = NetworkObjectDataInfo.ENetworkObjectType.Size;
        if (go.TryGetComponent(out NetworkObject networkObject))
        {
            networkObjectType = networkObject.NetworkObjectData._netObjectType;
        }
        else
        {
            Debug.Assert(false, $"Network Object 스크립트가 붙어 있어야 합니다.");
        }
        Debug.Assert(networkObjectType >= NetworkObjectDataInfo.ENetworkObjectType.Size);

        if (_objectPoolQueue[networkObjectType].Contains(go))
        {
            return;
        }
        
        go.SetActive(false);
        go.transform.SetParent(this.transform);
        go.transform.localPosition = Vector3.zero;
        go.transform.localRotation = Quaternion.identity;
        
        _objectPoolQueue[networkObjectType].Enqueue(go);
    }
}
