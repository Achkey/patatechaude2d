using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Server : MonoBehaviour
{
    public static Server Instance;

    private TcpListener server;
    private List<TcpClient> clients = new List<TcpClient>();

    public bool isHost; // True si cet ordinateur est l'hôte
    public string hostIP = "127.0.0.1"; // IP de l'hôte
    public int port = 7777;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        if (isHost)
        {
            StartServer();
        }
        else
        {
            ConnectToHost();
        }
    }

    void StartServer()
    {
        server = new TcpListener(IPAddress.Any, port);
        server.Start();
        Debug.Log("Server started!");

        // Lancer un thread pour accepter les connexions
        Thread serverThread = new Thread(AcceptClients);
        serverThread.IsBackground = true;
        serverThread.Start();
    }

    void AcceptClients()
    {
        while (true)
        {
            TcpClient client = server.AcceptTcpClient(); // Bloque jusqu'à ce qu'un client se connecte
            lock (clients)
            {
                clients.Add(client);
            }
            Debug.Log("Client connected!");

            // Lancer un thread pour gérer les communications avec ce client
            Thread clientThread = new Thread(() => HandleClient(client));
            clientThread.IsBackground = true;
            clientThread.Start();
        }
    }

    void HandleClient(TcpClient client)
    {
        NetworkStream stream = client.GetStream();
        byte[] buffer = new byte[1024];
        int bytesRead;

        while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) != 0)
        {
            string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
            Debug.Log($"Received: {message}");

            // Echo back the message
            byte[] response = Encoding.UTF8.GetBytes("Server received: " + message);
            stream.Write(response, 0, response.Length);
        }

        lock (clients)
        {
            clients.Remove(client);
        }

        client.Close();
        Debug.Log("Client disconnected!");
    }

    public void ConnectToHost()
    {
        TcpClient client = new TcpClient();
        client.Connect(hostIP, port);
        Debug.Log("Connected to host!");
    }

    public void SendBroadcastMessage(string message)
    {
        byte[] data = Encoding.UTF8.GetBytes(message);
        lock (clients)
        {
            foreach (var client in clients)
            {
                client.GetStream().Write(data, 0, data.Length);
            }
        }
    }
}
