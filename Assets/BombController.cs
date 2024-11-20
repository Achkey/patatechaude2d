using UnityEngine;

public class BombController : MonoBehaviour
{
    public PlayerController currentHolder; // Player currently holding the bomb

    void Update()
    {
        // Make the bomb follow the current holder's position
        if (currentHolder != null)
        {
            transform.position = currentHolder.transform.position;
        }
    }
}
