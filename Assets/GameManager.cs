using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance; // Singleton instance

    public GameObject bombPrefab; // Prefab of the bomb
    private BombController bombInstance; // Instance of the bomb

    void Awake()
    {
        // Ensure there's only one GameManager instance
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        AssignBombToRandomPlayer(); // Assign the bomb at the start of the game
    }

    public void AssignBombToRandomPlayer()
    {
        PlayerController[] players = FindObjectsOfType<PlayerController>();

        if (players.Length == 0)
        {
            Debug.LogError("No players found in the scene!");
            return;
        }

        // Select a random player to hold the bomb
        int randomIndex = Random.Range(0, players.Length);
        PlayerController randomPlayer = players[randomIndex];

        // Create the bomb if it doesn't exist
        if (bombInstance == null)
        {
            GameObject bombObject = Instantiate(bombPrefab, randomPlayer.transform.position, Quaternion.identity);
            bombInstance = bombObject.GetComponent<BombController>();
        }

        // Assign the bomb to the selected player
        randomPlayer.hasBomb = true;
        bombInstance.currentHolder = randomPlayer;

        Debug.Log($"Bomb assigned to Player {randomPlayer.playerID}");
    }

    public void UpdateBombHolder(PlayerController newHolder)
    {
        if (bombInstance != null)
        {
            bombInstance.currentHolder = newHolder; // Update the bomb's holder
            Debug.Log($"Bomb is now held by Player {newHolder.playerID}");
        }
    }
}
