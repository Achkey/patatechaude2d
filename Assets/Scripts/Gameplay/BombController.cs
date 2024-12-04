using UnityEngine;

public class BombController : MonoBehaviour
{
    public PlayerController currentHolder;
    private Client client;

    void Start()
    {
        client = Object.FindFirstObjectByType<Client>();
        if (client == null)
        {
            Debug.LogError("Client script not found in the scene!");
        }
    }

    void Update()
    {
        if (currentHolder != null)
        {
            transform.position = currentHolder.transform.position;
        }
    }

    public void SetBombHolder(PlayerController newHolder)
    {
        currentHolder = newHolder;

        if (client != null)
        {
            string bombMessage = $"BOMB_HOLDER:{currentHolder.playerID}";
            client.SendMessageToServer(bombMessage);
        }
    }
}
