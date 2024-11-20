using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    public GameObject playerPrefab; // Prefab for the player
    public int numberOfPlayers = 4; // Number of players to spawn
    public Vector2 spawnAreaMin = new Vector2(-8f, -4.5f); // Bottom-left corner of the spawn area
    public Vector2 spawnAreaMax = new Vector2(8f, 4.5f);   // Top-right corner of the spawn area
    public float minimumDistanceBetweenPlayers = 1.5f; // Minimum distance between players to avoid overlap

    void Start()
    {
        SpawnPlayers();
    }

    private void SpawnPlayers()
    {
        Vector2[] spawnPositions = new Vector2[numberOfPlayers];

        for (int i = 0; i < numberOfPlayers; i++)
        {
            Vector2 spawnPosition;
            int attempts = 0;

            do
            {
                // Generate a random position within the spawn area
                spawnPosition = new Vector2(
                    Random.Range(spawnAreaMin.x, spawnAreaMax.x),
                    Random.Range(spawnAreaMin.y, spawnAreaMax.y)
                );

                attempts++;

                // Limit attempts to prevent infinite loops in small spaces
                if (attempts > 100)
                {
                    Debug.LogError("Could not find a valid spawn position for all players!");
                    break;
                }
            }
            while (!IsPositionValid(spawnPosition, spawnPositions, i));

            spawnPositions[i] = spawnPosition;

            // Spawn the player
            GameObject player = Instantiate(playerPrefab, spawnPosition, Quaternion.identity);

            // Assign a unique ID to the player
            PlayerController playerController = player.GetComponent<PlayerController>();
            playerController.playerID = i + 1;

            Debug.Log($"Player {playerController.playerID} spawned at {spawnPosition}");
        }
    }

    private bool IsPositionValid(Vector2 position, Vector2[] existingPositions, int count)
    {
        for (int i = 0; i < count; i++)
        {
            if (Vector2.Distance(position, existingPositions[i]) < minimumDistanceBetweenPlayers)
            {
                return false; // Position is too close to an existing player
            }
        }
        return true;
    }
}
