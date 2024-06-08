using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

namespace _01_Scripts.Network
{
    public abstract class UDPClient<T> : MonoBehaviour where T : Enum
    {
        private readonly string serverIP = "127.0.0.1";
        private static readonly int MAX_BUF_SIZE = 4096;
        private static readonly int PORT_NUM = 10000;
        private Thread receiveThread;
        protected virtual void Start()
        {
            Init();
            ConnectToServer();
            receiveThread = new Thread(ReceiveFromServer)
            {
                IsBackground = true
            };
            receiveThread.Start();
        }

        protected abstract void ConnectToServer();
        private void Init()
        {
            try
            {
                NetworkManager.Instance.GameLogicUDPClientSock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                IPAddress ipAddr = IPAddress.Parse(serverIP);
                NetworkManager.Instance.GameLogicServerEndPoint = new IPEndPoint(ipAddr, PORT_NUM);
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
            // Init();
            Debug.Assert(NetworkManager.Instance.GameLogicUDPClientSock != null);
            Debug.Assert(NetworkManager.Instance.GameLogicServerEndPoint != null);

            byte[] recvBuffer = new byte[MAX_BUF_SIZE];
            byte[] partialBuffer = new byte[MAX_BUF_SIZE];
            int partialSize = 0;

            Int16 packetSize = 0;
            T packetType = (T)Enum.ToObject(typeof(T), 0);

            while (true)
            {
                EndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
                int recvByteSize = NetworkManager.Instance.GameLogicUDPClientSock.ReceiveFrom(recvBuffer, ref remoteEndPoint);

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
                    Array.Copy(recvBuffer, offset, partialBuffer, partialSize, recvByteSize);
                    partialSize = recvByteSize;
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
                    ProcessData((IPEndPoint)remoteEndPoint, packetType, data);
                } 
            }
        }

        protected abstract void OnApplicationQuit();

        protected void CloseServer()
        {
            NetworkManager.Instance.GameLogicUDPClientSock?.Close();
            NetworkManager.Instance.GameLogicServerEndPoint = null;
        }
    }
}