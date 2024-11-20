using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public int playerID; // Unique identifier for the player
    public float moveSpeed = 5f; // Player movement speed
    public bool hasBomb = false; // Indicates if the player currently holds the bomb

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Player movement using assigned input axes
        float horizontal = Input.GetAxis("Horizontal" + playerID);
        float vertical = Input.GetAxis("Vertical" + playerID);
        Vector2 movement = new Vector2(horizontal, vertical);

        rb.velocity = movement * moveSpeed;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if this player has the bomb
        if (hasBomb)
        {
            // Check if the collision is with another player
            PlayerController otherPlayer = collision.gameObject.GetComponent<PlayerController>();
            if (otherPlayer != null && !otherPlayer.hasBomb)
            {
                PassBombTo(otherPlayer); // Pass the bomb to the other player
            }
        }
    }

    public void PassBombTo(PlayerController targetPlayer)
    {
        if (targetPlayer == null) return;

        hasBomb = false; // Remove the bomb from this player
        targetPlayer.hasBomb = true; // Give the bomb to the target player

        // Notify the GameManager about the change
        GameManager.Instance.UpdateBombHolder(targetPlayer);

        Debug.Log($"Bomb passed from Player {playerID} to Player {targetPlayer.playerID}");
    }
}
