using UnityEngine;

public class Boundary : MonoBehaviour
{
    private void OnTriggerExit2D(Collider2D other)
    {
        // Check if the object leaving the boundary is a player
        PlayerController player = other.GetComponent<PlayerController>();
        if (player != null)
        {
            // Push the player back inside the boundary
            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector3 position = player.transform.position;

                // Use the BoxCollider2D bounds to clamp the player's position
                BoxCollider2D boundary = GetComponent<BoxCollider2D>();
                if (boundary != null)
                {
                    Vector2 boundsMin = boundary.bounds.min;
                    Vector2 boundsMax = boundary.bounds.max;

                    position.x = Mathf.Clamp(position.x, boundsMin.x, boundsMax.x);
                    position.y = Mathf.Clamp(position.y, boundsMin.y, boundsMax.y);

                    player.transform.position = position; // Move player back inside the boundary
                }
            }
        }
    }
}
