using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    public GameObject playerPrefab; // Prefab of the player
    public int numberOfPlayers = 4; // Number of players to spawn
    public float spawnRadius = 5f; // Radius within which players will spawn

    void Start()
    {
        SpawnPlayers();
    }

    private void SpawnPlayers()
    {
        for (int i = 0; i < numberOfPlayers; i++)
        {
            // Generate a random position within the spawn radius
            Vector2 randomPosition = (Vector2)transform.position + Random.insideUnitCircle * spawnRadius;

            // Spawn a player at the random position
            GameObject playerObject = Instantiate(playerPrefab, randomPosition, Quaternion.identity);

            // Assign the player an ID and configure the PlayerController
            PlayerController playerController = playerObject.GetComponent<PlayerController>();
            playerController.playerID = i + 1;

            Debug.Log($"Player {playerController.playerID} spawned at {randomPosition}");
        }
    }
}
