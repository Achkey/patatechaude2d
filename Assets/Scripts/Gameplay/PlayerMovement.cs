using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    private Client client;

    void Start()
    {
        client = Object.FindObjectOfType<Client>();
        if (client == null)
        {
            Debug.LogError("Client script not found in the scene!");
        }
    }

    void Update()
    {
        HandleMovement();
        SendPositionToServer();
    }

    private void HandleMovement()
    {
        Vector3 movement = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0);
        transform.Translate(movement * speed * Time.deltaTime);
    }

    private void SendPositionToServer()
    {
        if (client != null)
        {
            client.SendMessageToServer($"POSITION:{transform.position.x},{transform.position.y}");
        }
    }
}
