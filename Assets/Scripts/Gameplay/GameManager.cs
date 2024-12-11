using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private Client client;
    private Server server;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        client = Object.FindFirstObjectByType<Client>();
        server = Object.FindFirstObjectByType<Server>();

        if (server == null && client == null)
        {
            Debug.LogError("GameManager requires either a Client or Server script in the scene.");
        }
    }

    public void UpdateBombHolder(PlayerController newHolder)
    {
        if (server != null)
        {
            BombController bombController = Object.FindFirstObjectByType<BombController>();
            if (bombController != null)
            {
                bombController.SetBombHolder(newHolder);

                server.SendMessageToAllClients($"BOMB_UPDATE:{newHolder.playerID}");
            }
        }
        else if (client != null)
        {
            client.SendMessageToServer($"BOMB_HOLDER:{newHolder.playerID}");
        }
    }
}
