using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public int playerID;
    public float moveSpeed = 10f;

    private Rigidbody2D rigidBody;
    private BoxCollider2D boundary;

    public bool hasBomb = false;

    private Server server;
    private Client client;

    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        boundary = GameObject.Find("Boundary")?.GetComponent<BoxCollider2D>();

        if (boundary == null)
        {
            Debug.LogWarning("Boundary not found!");
        }

        client = UnityEngine.Object.FindFirstObjectByType<Client>();
        if (client == null)
        {
            Debug.LogError("Client script not found in the scene!");
        }

        server = UnityEngine.Object.FindFirstObjectByType<Server>();
        if (server == null)
        {
            Debug.LogError("Server script not found in the scene!");
        }
    }

    void Update()
    {
        HandleMovement();
        ClampPositionWithinBoundary();

        if (Input.GetKeyDown(KeyCode.Space) && hasBomb)
        {
            AttemptToPassBomb();
        }
    }

    private void HandleMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector2 movement = new Vector2(horizontal, vertical);

        rigidBody.linearVelocity = movement * moveSpeed;
    }

    private void ClampPositionWithinBoundary()
    {
        if (boundary != null)
        {
            Vector2 position = transform.position;
            Vector2 boundsMin = boundary.bounds.min;
            Vector2 boundsMax = boundary.bounds.max;

            position.x = Mathf.Clamp(position.x, boundsMin.x, boundsMax.x);
            position.y = Mathf.Clamp(position.y, boundsMin.y, boundsMax.y);

            transform.position = position;
        
            // server.UpdatePlayerPosition(playerID, new Vector2(position.x, position.y));
        }
    }

    private void AttemptToPassBomb()
    {
        Debug.Log($"Player {playerID} is trying to pass the bomb!");

        if (client != null)
        {
            string bombMessage = $"PASS_BOMB:{playerID}";
            client.SendMessageToServer(bombMessage);
        }
    }

    public void UpdateBombStatus(bool newStatus)
    {
        hasBomb = newStatus;
        Debug.Log($"Player {playerID} bomb status updated: {hasBomb}");
    }
}
