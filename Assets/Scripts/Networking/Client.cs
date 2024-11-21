using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class Client : MonoBehaviour
{
    private TcpClient tcpClient;
    private UdpClient udpClient;
    private NetworkStream stream;
    private string serverIP = "172.20.10.1";
    private int port = 7777;

    void Start()
    {
        try
        {
            // Connect to TCP server
            tcpClient = new TcpClient(serverIP, port);
            stream = tcpClient.GetStream();
            Debug.Log("Connected to TCP server!");

            // Setup UDP client
            udpClient = new UdpClient();
            udpClient.Connect(serverIP, port);

            // Example: Send a test message
            SendTcpMessage("Hello, TCP Server!");
            SendUdpMessage("Hello, UDP Server!");
        }
        catch (Exception ex)
        {
            Debug.LogError("Client error: " + ex.Message);
        }
    }

    public void SendTcpMessage(string message)
    {
        if (tcpClient.Connected)
        {
            byte[] data = Encoding.ASCII.GetBytes(message);
            stream.Write(data, 0, data.Length);
            Debug.Log("TCP message sent: " + message);
        }
    }

    public void SendUdpMessage(string message)
    {
        byte[] data = Encoding.ASCII.GetBytes(message);
        udpClient.Send(data, data.Length);
        Debug.Log("UDP message sent: " + message);
    }

    void OnApplicationQuit()
    {
        stream?.Close();
        tcpClient?.Close();
        udpClient?.Close();
    }
}
