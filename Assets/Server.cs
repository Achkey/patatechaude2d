using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public class Server : MonoBehaviour
{
    private TcpListener server;
    private bool isRunning;

    void Start()
    {
        StartServer(7777); // Démarre le serveur sur le port 7777
    }

    public void StartServer(int port)
    {
        server = new TcpListener(IPAddress.Any, port);
        server.Start();
        isRunning = true;

        Debug.Log($"Server started on port {port}.");

        Thread thread = new Thread(ListenForClients);
        thread.Start();
    }

    private void ListenForClients()
    {
        while (isRunning)
        {
            try
            {
                TcpClient client = server.AcceptTcpClient();
                Debug.Log("New client connected!");

                Thread thread = new Thread(() => HandleClient(client));
                thread.Start();
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Error accepting client: {ex.Message}");
            }
        }
    }

    private void HandleClient(TcpClient client)
    {
        NetworkStream stream = client.GetStream();
        byte[] buffer = new byte[1024];
        int bytesRead;

        while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) != 0)
        {
            string message = System.Text.Encoding.ASCII.GetString(buffer, 0, bytesRead);
            Debug.Log($"Received: {message}");
        }

        Debug.Log("Client disconnected.");
        client.Close();
    }

    public void StopServer()
    {
        isRunning = false;
        server.Stop();
        Debug.Log("Server stopped.");
    }
}
