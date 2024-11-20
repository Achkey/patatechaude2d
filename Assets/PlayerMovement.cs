using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f; // Vitesse de déplacement
    private PlayerController playerController; // Référence au contrôleur du joueur

    void Start()
    {
        playerController = GetComponent<PlayerController>();
    }

    void Update()
    {
        Vector3 movement = Vector3.zero;

        // Contrôles personnalisés en fonction du joueur
        if (playerController.playerID == 1) // Joueur 1
        {
            movement = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0);
        }
        else if (playerController.playerID == 2) // Joueur 2
        {
            movement = new Vector3(Input.GetAxis("Horizontal2"), Input.GetAxis("Vertical2"), 0);
        }

        // Appliquer le mouvement
        transform.position += movement.normalized * speed * Time.deltaTime;
    }
}
