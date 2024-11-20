using UnityEngine;

public class Boundary : MonoBehaviour
{
    private void OnTriggerExit2D(Collider2D other)
    {
        // joueur??
        PlayerController player = other.GetComponent<PlayerController>();
        if (player != null)
        {
            // impossibilité de sortir du RIGID
            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector3 position = player.transform.position;

                // retrieving player position by BoxCollider2D
                BoxCollider2D boundary = GetComponent<BoxCollider2D>();
                if (boundary != null)
                {
                    Vector2 boundsMin = boundary.bounds.min;
                    Vector2 boundsMax = boundary.bounds.max;

                    position.x = Mathf.Clamp(position.x, boundsMin.x, boundsMax.x);
                    position.y = Mathf.Clamp(position.y, boundsMin.y, boundsMax.y);

                    player.transform.position = position; // Push player inside again
                }
            }
        }
    }
}
