using UnityEngine;

public class BombController : MonoBehaviour
{
    public PlayerController currentHolder; // Joueur qui a BOMB

    void Update()
    {
        // Bomb stick au joueur
        if (currentHolder != null)
        {
            transform.position = currentHolder.transform.position;
        }
    }
}
