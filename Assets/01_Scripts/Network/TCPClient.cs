using System;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

public class TCPClient<T> : MonoBehaviour where T : Enum
{
    private readonly string serverIP = "127.0.0.1";
    private static readonly int MAX_BUF_SIZE = 4096;
    private static readonly int PORT_NUM = 10001;
    private Thread receiveThread;
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
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
    }

    private void ReceiveFromServer()
    {
        // byte[] buffer = new byte[MAX_BUF_SIZE];
        // try
        // {
        //     while (NetworkManager.Instance.DatabaseTcpClient.Connected)
        //     {
        //         int bytesRead = NetworkManager.Instance.NetworkStream.Read(buffer, 0, buffer.Length);
        //         if (bytesRead > 0)
        //         {
        //             string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
        //             Debug.Log($"Received: {message}");
        //             // Handle the received message as needed
        //         }
        //     }
        // }
        // catch (Exception ex)
        // {
        //     Console.WriteLine(ex.Message);
        //     throw;
        // }
    }

    private void CloseServer()
    {
        receiveThread?.Abort();
        
        NetworkManager.Instance.DatabaseTcpClient.Close();
    }
}
