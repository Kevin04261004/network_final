using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
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
        byte[] recvBuffer = new byte[MAX_BUF_SIZE];
        byte[] partialBuffer = new byte[MAX_BUF_SIZE];
        int partialSize = 0;

        Int16 packetSize = 0;
        T packetType = (T)Enum.ToObject(typeof(T), 0);

        try
        {
            while (NetworkManager.Instance.DatabaseTcpClient.Connected)
            {
                int recvByteSize = NetworkManager.Instance.NetworkStream.Read(recvBuffer, 0, recvBuffer.Length);

                if (partialSize == 0)
                {
                    if (recvByteSize < PacketDataInfo.HeaderSize)
                    {
                        continue;
                    }

                    int offset = 0;
                    packetSize = BitConverter.ToInt16(recvBuffer, offset);
                    offset += PacketDataInfo.PacketSizeSize;
                    char packetID = BitConverter.ToChar(recvBuffer, offset);
                    offset += PacketDataInfo.PacketIDSize;
                    packetType = (T)Enum.ToObject(typeof(T), BitConverter.ToInt16(recvBuffer, offset));
                    offset += PacketDataInfo.PacketTypeSize;
                    Array.Copy(recvBuffer, offset, partialBuffer, partialSize, recvByteSize - offset);
                    partialSize = recvByteSize - offset;
                }
                else
                {
                    Array.Copy(recvBuffer, 0, partialBuffer, partialSize, recvByteSize);
                    partialSize += recvByteSize;
                }

                if (partialSize >= packetSize)
                {
                    partialSize = 0;

                    byte[] data = new byte[packetSize - PacketDataInfo.HeaderSize];
                    Array.Copy(partialBuffer, 0, data, 0, data.Length);
                    ProcessData(serverEndPoint, packetType, data);
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"ReceiveFromServer Error: {ex.Message}");
        }
        finally
        {
            NetworkManager.Instance.NetworkStream?.Close();
            NetworkManager.Instance.DatabaseTcpClient?.Close();
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
