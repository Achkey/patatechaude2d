using System.Net.Sockets;
using System.Text;
using UnityEngine;
using System.Collections;

public class Client : MonoBehaviour
{
    private TcpClient client;
    public string serverAddress = "127.0.0.1";
    public int port = 7777;
    private bool isConnected = false;

    public GameObject playerPrefab;

    private void Start()
    {
        StartCoroutine(ConnectToServerWithRetry());
    }

    private IEnumerator ConnectToServerWithRetry()
    {
        while (!isConnected)
        {
            if (Server.Instance != null && Server.Instance.IsServerReady())
            {
                ConnectToServer(serverAddress, port);
            }
            else
            {
                Debug.Log("Waiting for server to be ready...");
            }

            yield return new WaitForSeconds(2f);
        }
    }

    void ConnectToServer(string address, int port)
    {
        if (client != null && client.Connected)
        {
            Debug.Log("Already connected.");
            return;
        }

        client = new TcpClient();
        try
        {
            client.Connect(address, port);
            isConnected = true;
            Debug.Log("Connected to server!");

//            SpawnPlayer(new Vector2(0, 0));
        }
        catch (SocketException e)
        {
            Debug.LogError($"Failed to connect: {e.Message}");
        }
    }

    private void Update()
    {
        if (isConnected && client.Available > 0)
        {
            byte[] buffer = new byte[1024];
            int bytesRead = client.GetStream().Read(buffer, 0, buffer.Length);

            string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
            HandleMessage(message);
        }
    }

    void HandleMessage(string message)
    {
        if (message.StartsWith("SPAWN"))
        {
            string[] data = message.Substring(6).Split(',');
            float x = float.Parse(data[0]);
            float y = float.Parse(data[1]);

            Vector2 spawnPosition = new Vector2(x, y);
            SpawnPlayer(spawnPosition);
        }
    }

    void SpawnPlayer(Vector2 position)
    {
        GameObject player = Instantiate(playerPrefab, position, Quaternion.identity);
        PlayerController playerController = player.GetComponent<PlayerController>();
        if (playerController != null)
        {
            // Assign unique player properties if necessary
        }
    }

    public void SendMessageToServer(string message)
    {
        if (isConnected)
        {
            byte[] data = Encoding.UTF8.GetBytes(message);
            client.GetStream().Write(data, 0, data.Length);
        }
    }
}
