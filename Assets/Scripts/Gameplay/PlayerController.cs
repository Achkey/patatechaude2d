using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public int playerID; // ID unique
    public float moveSpeed = 5f; // Speed
    public bool hasBomb = false; // a la bomb?

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

void Update()
{
    // Mouvement
    float horizontal = Input.GetAxis("Horizontal" + playerID);
    float vertical = Input.GetAxis("Vertical" + playerID);
    Vector2 movement = new Vector2(horizontal, vertical);
    rb.linearVelocity = movement * moveSpeed;

    // can't leave boundary
    BoxCollider2D boundary = GameObject.Find("Boundary").GetComponent<BoxCollider2D>();
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


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (hasBomb)
        {
            PlayerController otherPlayer = collision.gameObject.GetComponent<PlayerController>();
            if (otherPlayer != null && !otherPlayer.hasBomb)
            {
                PassBombTo(otherPlayer);
            }
        }
        else
        {
            PlayerController bombHolder = collision.gameObject.GetComponent<PlayerController>();
            if (bombHolder != null && bombHolder.hasBomb)
            {
                bombHolder.PassBombTo(this);
            }
        }
    }

    public void PassBombTo(PlayerController targetPlayer)
    {
        if (targetPlayer == null) return;

        hasBomb = false;
        targetPlayer.hasBomb = true;

        GameManager.Instance.UpdateBombHolder(targetPlayer);

        Debug.Log($"Bomb passed from Player {playerID} to Player {targetPlayer.playerID}");
    }
}
