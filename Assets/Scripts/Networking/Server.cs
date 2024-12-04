using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class Server : MonoBehaviour
{
    public static Server Instance;

    private TcpListener server;
    private List<TcpClient> clients = new List<TcpClient>();
    private Dictionary<TcpClient, GameObject> connectedPlayers = new Dictionary<TcpClient, GameObject>();

    public int port = 7777;
    public GameObject playerPrefab;

    private List<Vector2> spawnPositions = new List<Vector2>
    {
        new Vector2(-5, 0),
        new Vector2(5, 0),
        new Vector2(0, 5),
        new Vector2(0, -5)
    };

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        StartServer();
    }

    void StartServer()
    {
        try
        {
            if (server != null && server.Server.IsBound)
            {
                Debug.LogError("Server is already running!");
                return;
            }

            server = new TcpListener(IPAddress.Any, port);
            server.Start();
            Debug.Log("Server started!");

            System.Threading.Thread serverThread = new System.Threading.Thread(AcceptClients);
            serverThread.IsBackground = true;
            serverThread.Start();
        }
        catch (SocketException ex)
        {
            Debug.LogError($"Error starting server: {ex.Message}");
        }
    }


    void AcceptClients()
    {
        while (true)
        {
            TcpClient client = server.AcceptTcpClient();
            lock (clients)
            {
                clients.Add(client);
                Debug.Log($"Client connected! Total clients: {clients.Count}");

                StartCoroutine(AssignPlayerCoroutine(client));
            }
        }
    }

    private IEnumerator AssignPlayerCoroutine(TcpClient client)
    {
        yield return null;

        AssignPlayer(client);
    }

    void AssignPlayer(TcpClient client)
    {
        if (clients.Count <= spawnPositions.Count)
        {
        Debug.log("AssignPlayer | <=");
            Vector2 spawnPosition = spawnPositions[clients.Count - 1];
            SpawnPlayerForClient(client, spawnPosition);
        }
        else
        {
        Debug.log("AssignPlayer | more than array pos = random");
            Vector2 spawnPosition = new Vector2(
                Random.Range(-8f, 8f),
                Random.Range(-4.5f, 4.5f)
            );
            SpawnPlayerForClient(client, spawnPosition);
        }
    }

    void SpawnPlayerForClient(TcpClient client, Vector2 spawnPosition)
    {
        GameObject player = Instantiate(playerPrefab, spawnPosition, Quaternion.identity);
        connectedPlayers[client] = player;

        PlayerController playerController = player.GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.playerID = connectedPlayers.Count;
        }

        SendMessageToClient(client, $"SPAWN:{spawnPosition.x},{spawnPosition.y}");
        BroadcastToAll($"PLAYER_JOINED:{connectedPlayers.Count}");
    }

    void HandleMessage(string message, TcpClient client)
    {
        if (message.StartsWith("BOMB_HOLDER"))
        {
            string[] data = message.Split(':');
            int newHolderID = int.Parse(data[1]);

            if (connectedPlayers.TryGetValue(client, out GameObject playerObject))
            {
                PlayerController newHolder = playerObject.GetComponent<PlayerController>();
                if (newHolder != null)
                {
                    GameManager.Instance.UpdateBombHolder(newHolder);
                    Debug.Log($"Bomb holder updated to Player {newHolderID}");
                }
            }
        }
    }


    void SendMessageToClient(TcpClient client, string message)
    {
        byte[] data = Encoding.UTF8.GetBytes(message);
        client.GetStream().Write(data, 0, data.Length);
    }

    public void BroadcastToAll(string message)
    {
        byte[] data = System.Text.Encoding.UTF8.GetBytes(message);
        lock (clients)
        {
            foreach (var client in clients)
            {
                client.GetStream().Write(data, 0, data.Length);
            }
        }
    }


    public bool IsServerReady()
    {
        return server != null && server.Connected;
    }

    private void OnApplicationQuit()
    {
        if (server != null)
        {
            server.Stop();
            Debug.Log("Server stopped.");
        }
    }
}
