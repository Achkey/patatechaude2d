using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public int playerID;
    public float moveSpeed = 5f;

    private Rigidbody2D rb;
    private BoxCollider2D boundary;

    public bool hasBomb = false;

    private Client client;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        boundary = GameObject.Find("Boundary")?.GetComponent<BoxCollider2D>();
        if (boundary == null)
        {
            Debug.LogWarning("Boundary not found!");
        }

        client = Object.FindFirstObjectByType<Client>();
        if (client == null)
        {
            Debug.LogError("Client script not found in the scene!");
        }
    }

    void Update()
    {
        if (client == null || client.playerPrefab != gameObject) return;

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

        rb.velocity = movement * moveSpeed;

        SendPositionToServer(transform.position);
    }

    private void ClampPositionWithinBoundary()
    {
        if (boundary != null)
        {
            Vector3 position = transform.position;
            Vector2 boundsMin = boundary.bounds.min;
            Vector2 boundsMax = boundary.bounds.max;

            position.x = Mathf.Clamp(position.x, boundsMin.x, boundsMax.x);
            position.y = Mathf.Clamp(position.y, boundsMin.y, boundsMax.y);

            transform.position = position;
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

    private void SendPositionToServer(Vector3 position)
    {
        if (client != null)
        {
            string positionMessage = $"POSITION_UPDATE:{playerID}:{position.x}:{position.y}";
            client.SendMessageToServer(positionMessage);
        }
    }

    public void UpdateBombStatus(bool newStatus)
    {
        hasBomb = newStatus;
        Debug.Log($"Player {playerID} bomb status updated: {hasBomb}");
    }
}
