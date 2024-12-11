using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class Client : MonoBehaviour
{
    private Server server;
    private TcpClient tcpClient;
    private NetworkStream stream;
    private byte[] buffer = new byte[1024];
    private bool isConnected = false;

    private ConcurrentQueue<System.Action> mainThreadActions = new ConcurrentQueue<System.Action>();

    public string serverAddress = "127.0.0.1";
    public int port = 7777;

    public GameObject playerPrefab;

    private void Start()
    {
        server = UnityEngine.Object.FindAnyObjectByType<Server>();
        
        StartCoroutine(ConnectToServer(serverAddress, port));
    }

    private void Update()
    {
        int actionsProcessed = 0;

        while (mainThreadActions.Count > 0  && actionsProcessed < 10)
        {
            if (mainThreadActions.TryDequeue(out var action))
            {
                action.Invoke();
            }
            actionsProcessed++;
        }
    }
    
    private void OnApplicationQuit()
    {
        Disconnect();
    }


    private void OnMessageReceived(IAsyncResult ar)
    {
        try
        {
            int bytesRead = stream.EndRead(ar);
            if (bytesRead > 0)
            {
                string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                HandleMessage(message);

                stream.BeginRead(buffer, 0, buffer.Length, OnMessageReceived, null);
            }
            else
            {
                Debug.LogWarning("Disconnected from server.");
                Disconnect();
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error reading from server: {ex.Message}");
            Disconnect();
        }
    }

    void HandleMessage(string message)
    {
        string[] messages = message.Split('\n');
        foreach (string msg in messages)
        {
            if (string.IsNullOrEmpty(msg)) continue;
            
            if (msg.StartsWith("SPAWN"))
            {
                Debug.Log($"You have been spawned: {msg.Substring(6)}");

                string[] data = msg.Substring(6).Split(':');
                int playerID = int.Parse(data[0]);

                string[] positionData = data[1].Split(',');
                float x = float.Parse(positionData[0]);
                float y = float.Parse(positionData[1]);
                
                mainThreadActions.Enqueue(() => InstantiatePlayerOnClient(playerID, new Vector2(x, y)));
            }
            else if (msg.StartsWith("PLAYER_JOINED"))
            {
                Debug.Log($"New player joined: {msg.Substring(14)}");
                
                string[] data = msg.Substring(14).Split(':');
                int playerID = int.Parse(data[0]);

                string[] positionData = data[1].Split(',');
                float x = float.Parse(positionData[0]);
                float y = float.Parse(positionData[1]);

                mainThreadActions.Enqueue(() => InstantiatePlayerOnClient(playerID, new Vector2(x, y)));
            }
            else if (msg.StartsWith("PLAYER_POSITION"))
            {
                Debug.Log($"Player position update: {msg.Substring(16)}");

                string[] data = msg.Substring(16).Split(':');
                int playerID = int.Parse(data[0]);

                string[] positionData = data[1].Split(',');
                float x = float.Parse(positionData[0]);
                float y = float.Parse(positionData[1]);

                // mainThreadActions.Enqueue(() => UpdatePlayerPosition(playerID, new Vector2(x, y)));
            }
            else {
                Debug.Log($"Message received: {msg}");
            }
        }
    }
    
    public void SendMessageToServer(string message)
    {
        try
        {
            if (isConnected && stream != null)
            {
                byte[] data = Encoding.UTF8.GetBytes(message);
                stream.Write(data, 0, data.Length);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error sending message: {ex.Message}");
        }
    }

    void InstantiatePlayerOnClient(int playerID, Vector2 position)
    {
        GameObject player = Instantiate(playerPrefab, position, Quaternion.identity);
        PlayerController playerController = player.GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.playerID = playerID;
        }
    }
    
    // void UpdatePlayerPosition(int playerID, Vector2 newPosition)
    // {
    //     PlayerController[] players = FindObjectsOfType<PlayerController>();
    //     foreach (var player in players)
    //     {
    //         if (player.playerID == playerID)
    //         {
    //             player.transform.position = newPosition;
    //             break;
    //         }
    //     }
    // }

    private IEnumerator ConnectToServer(string address, int port)
    {
        while (!isConnected)
        {
            try
            {
                if (tcpClient != null && tcpClient.Connected)
                {
                    Debug.Log("Already connected.");
                    yield break;
                }

                tcpClient = new TcpClient();
                tcpClient.Connect(address, port);
                stream = tcpClient.GetStream();
                isConnected = true;
                Debug.Log("Connected to server!");

                stream.BeginRead(buffer, 0, buffer.Length, OnMessageReceived, null);
                yield break;
            }
            catch (SocketException e)
            {
                Debug.LogError($"Failed to connect: {e.Message}");
                server.StartServer();
            }

            yield return new WaitForSeconds(2f);
        }
    }

    private void Disconnect()
    {
        isConnected = false;

        if (stream != null)
        {
            stream.Close();
            stream = null;
        }

        if (tcpClient != null)
        {
            tcpClient.Close();
            tcpClient = null;
        }
    }
}
