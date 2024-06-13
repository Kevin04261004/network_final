using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEditor.PackageManager;
using UnityEngine;

public abstract class TCPClient<T> : MonoBehaviour where T : Enum
{
    private readonly string serverIP = "127.0.0.1";
    private static readonly int MAX_BUF_SIZE = 4096;
    private static readonly int PORT_NUM = 10001;
    private Thread receiveThread;
    private IPEndPoint serverEndPoint;
    protected virtual void Start()
    {
        Init();
        receiveThread = new Thread(ReceiveFromServer)
        {
            IsBackground = true
        };
        receiveThread.Start();
        if (NetworkManager.Instance.DatabaseTcpClient != null && NetworkManager.Instance.DatabaseTcpClient.Connected)
        {
            Debug.Log("Connected to Database server.");
        }
        else
        {
            Debug.LogError("Failed to connect to Database server.");
        }
    }

    private void Init()
    {
        try
        {
            NetworkManager.Instance.DatabaseTcpClient = new TcpClient(serverIP, PORT_NUM)
            {
                NoDelay = true
            };
            NetworkManager.Instance.NetworkStream = NetworkManager.Instance.DatabaseTcpClient.GetStream();
            serverEndPoint = NetworkManager.Instance.DatabaseTcpClient.Client.RemoteEndPoint as IPEndPoint;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
    }
    protected abstract void ProcessData(IPEndPoint serverIPEndPoint, T packetType, byte[] buffer);
    private void ReceiveFromServer()
    {
        byte[] receiveData = new byte[MAX_BUF_SIZE];
        byte[] partialData = new byte[MAX_BUF_SIZE];
        int partialLength = 0;
        while (NetworkManager.Instance.DatabaseTcpClient.Connected)
        {
            Int32 receiveLength = 0;
            MemoryStream ms = new MemoryStream();
            try
            {
                if (!NetworkManager.Instance.NetworkStream.CanRead) continue;

                MainThreadWorker.Instance.EnqueueJob(() =>
                {
                    receiveLength = NetworkManager.Instance.NetworkStream.Read(receiveData, 0, receiveData.Length);
                });
                if (receiveLength <= 0) break;
                
                ms.Write(receiveData, 0, receiveLength);
            }
            catch (Exception err)
            {
                Debug.Log(err.Message);
            }
            
            Array.Copy(receiveData, 0, partialData, partialLength, receiveLength);
            partialLength += receiveLength;
            if (partialLength >= PacketDataInfo.HeaderSize)
            {
                int offset = 0;
                int packetSize = BitConverter.ToInt16(partialData, offset);
                offset += PacketDataInfo.PacketSizeSize;
                char packetID = BitConverter.ToChar(partialData, offset);
                offset += PacketDataInfo.PacketIDSize;
                T type = (T)Enum.ToObject(typeof(T), BitConverter.ToInt16(partialData, offset));
                offset += PacketDataInfo.PacketTypeSize;

                if (partialLength < packetSize)
                {
                    continue;
                }
                
                byte[] data = new byte[packetSize - PacketDataInfo.HeaderSize];
                Array.Copy(partialData, PacketDataInfo.HeaderSize, data, 0, data.Length);
                ProcessData(serverEndPoint, type, data);
                
                partialLength -= packetSize;
            }

        }
    }

    protected abstract void OnApplicationQuit();

    protected void CloseServer()
    {
        receiveThread?.Abort();
        NetworkManager.Instance.NetworkStream?.Close();
        NetworkManager.Instance.DatabaseTcpClient?.Close();
    }
}
