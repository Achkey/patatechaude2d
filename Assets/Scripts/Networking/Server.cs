using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;

public class Server : MonoBehaviour
{
    public static Server Instance { get; private set; }

    private TcpListener tcpServer;
    private ConcurrentBag<TcpClient> clients = new ConcurrentBag<TcpClient>();
    private Dictionary<int, Vector2> playerPositions = new Dictionary<int, Vector2>();
    private Dictionary<TcpClient, int> clientToPlayerID = new Dictionary<TcpClient, int>();
    private int nextPlayerID = 0;

    public int port = 7777;
    public GameObject playerPrefab;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public async void StartServer()
    {
        try
        {
            tcpServer = new TcpListener(IPAddress.Any, port);
            tcpServer.Start();
            Debug.Log("Server started!");
            
            while (true)
            {
                try
                {
                    TcpClient client = await tcpServer.AcceptTcpClientAsync();
                    Debug.Log($"New client connected: {client.Client.RemoteEndPoint}");
                    clients.Add(client);
                    SpawnAndAssignPlayer(client);
                }
                catch (ObjectDisposedException)
                {
                    Debug.Log("Server has been stopped.");
                    break;
                }
            }
        }
        catch (SocketException ex)
        {
            Debug.LogError($"Error starting server: {ex.Message}");
        }
    }


    private void OnApplicationQuit()
    {
        StopServer();
    }
    

    void SpawnAndAssignPlayer(TcpClient client)
    {
        if (!clientToPlayerID.TryGetValue(client, out int playerID))
        {
            playerID = nextPlayerID++;
            clientToPlayerID[client] = playerID;
        }

        Vector2 spawnPosition = new Vector2(
            UnityEngine.Random.Range(-16, 16),
            UnityEngine.Random.Range(-9, 9)
        );
        playerPositions[playerID] = spawnPosition;

        SendMessageToClient(client, $"SPAWN:{playerID}:{spawnPosition.x},{spawnPosition.y}\n");
        foreach (var player in playerPositions)
        {
            if (player.Key != playerID)
            {
                SendMessageToClient(client, $"PLAYER_JOINED:{player.Key}:{player.Value.x},{player.Value.y}\n");
            }
        }
    }

    public void UpdatePlayerPosition(int playerID, Vector2 position)
    {
        if (playerPositions.ContainsKey(playerID))
        {
            playerPositions[playerID] = position;
            SendPlayerPositionUpdate(playerID);
        }
    }
    void SendPlayerPositionUpdate(int playerID)
    {
        if (playerPositions.ContainsKey(playerID))
        {
            Vector2 position = playerPositions[playerID];
            string positionMessage = $"PLAYER_POSITION:{playerID}:{position.x},{position.y}\n";
            SendMessageToAllClients(positionMessage);
        }
    }

    void SendMessageToClient(TcpClient client, string message)
    {
        try
        {
            byte[] data = Encoding.UTF8.GetBytes(message);
            client.GetStream().Write(data, 0, data.Length);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error sending message to client: {ex.Message}");
        }
    }
    public void SendMessageToAllClients(string message)
    {
        byte[] data = Encoding.UTF8.GetBytes(message);

        foreach (var client in clients)
        {
            try
            {
                if (client.Connected)
                {
                    client.GetStream().Write(data, 0, data.Length);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error broadcasting message to client: {ex.Message}");
            }
        }
    }

    public void StopServer()
    {
        if (tcpServer != null)
        {
            tcpServer.Stop();
            tcpServer = null;
            Debug.Log("Server stopped!");
        }

        foreach (var client in clients)
        {
            client.Close();
        }

        clients.Clear();
    }
}