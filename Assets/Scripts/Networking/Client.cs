using UnityEngine;
using System.Net.Sockets;
using System.Text;

public class Client : MonoBehaviour
{
    private TcpClient client;

    private void Start()
    {
        if (!Server.Instance.isHost)
        {
            client = new TcpClient();
            try
            {
                client.Connect(Server.Instance.hostIP, Server.Instance.port);
                Debug.Log("Client connected to server.");
            }
            catch (SocketException e)
            {
                Debug.LogError($"Failed to connect to server: {e.Message}");
            }
        }
    }

    void Update()
    {
        if (client != null && client.Connected && client.Available > 0)
        {
            byte[] buffer = new byte[1024];
            int bytesRead = client.GetStream().Read(buffer, 0, buffer.Length);

            string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
            if (message.StartsWith("MOVE"))
            {
                string positionData = message.Substring(5);

                // Remplacement de FindObjectOfType
                PlayerNetworkController playerController = Object.FindFirstObjectByType<PlayerNetworkController>();
                if (playerController != null)
                {
                    playerController.ApplyMovement(positionData);
                }
                else
                {
                    Debug.LogWarning("PlayerNetworkController not found.");
                }
            }
        }
    }

    private void OnApplicationQuit()
    {
        if (client != null && client.Connected)
        {
            client.Close();
            Debug.Log("Client disconnected.");
        }
    }
}
