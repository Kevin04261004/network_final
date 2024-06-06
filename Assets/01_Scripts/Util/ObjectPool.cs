using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Runtime.InteropServices;
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

    private IEnumerator Start()
    {
        GameLogicPacketHandler.Instance.SetHandler(PacketDataInfo.EGameLogicPacketType.Server_CreateNetworkObjectSuccess, CreateNetworkObject);
        yield return new WaitForSeconds(1);
        InitPool();
    }

    private void InitPool()
    {
        for (int i = 0; i < _objectPrefab.Length; ++i)
        {
            _objectPoolQueue.Add((NetworkObjectDataInfo.ENetworkObjectType)i, new Queue<GameObject>(_defaultCount));
        }
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
        NetworkManager.Instance.SendToServer(ESendServerType.GameLogic, packetData.ToPacket());
    }
    private void CreateNetworkObject(IPEndPoint endPoint, byte[] data)
    {
        MainThreadWorker.Instance.EnqueueJob(() =>
        {
            int structSize = Marshal.SizeOf<CreateNetworkObjectData>();
            if (data.Length < structSize + sizeof(Int32))
            {
                Debug.LogError("Data array is too short to contain expected structures.");
                return;
            }
            byte[] createNetworkObjectDataBytes = new byte[structSize];
            byte[] startIDBytes = new byte[sizeof(UInt32)];
            int offset = 0;
            Array.Copy(data, 0, createNetworkObjectDataBytes, offset, createNetworkObjectDataBytes.Length);
            offset += createNetworkObjectDataBytes.Length;
            Array.Copy(data, offset, startIDBytes, 0, startIDBytes.Length);
            offset += startIDBytes.Length;

            uint startID = BitConverter.ToUInt32(startIDBytes);
            CreateNetworkObjectData networkObjectData = MarshalingTool.ByteToStruct<CreateNetworkObjectData>(createNetworkObjectDataBytes);

            uint max = (uint)startID + (uint)networkObjectData._count;
            for (; startID < max; ++startID)
            {
                GameObject go = Instantiate(_objectPrefab[(int)networkObjectData.networkObjectType], Vector3.zero,
                    Quaternion.identity, transform);
                go.SetActive(false);
                if (go.TryGetComponent(out NetworkObject networkObject))
                {
                    networkObject.NetworkObjectData._id = startID;
                    networkObject.NetworkObjectData._netObjectType = networkObjectData.networkObjectType;
                }
                _objectPoolQueue[networkObjectData.networkObjectType].Enqueue(go);
            } 
        });
    }
    public GameObject Get(NetworkObjectDataInfo.ENetworkObjectType objectType, Vector3 pos = default, Quaternion rotation = default, Transform parent = null)
    {
        Debug.Assert(objectType >= NetworkObjectDataInfo.ENetworkObjectType.Size);
        if (_objectPoolQueue[objectType].Count <= 0)
        {
            ExtendPool(objectType, _addCount);
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
