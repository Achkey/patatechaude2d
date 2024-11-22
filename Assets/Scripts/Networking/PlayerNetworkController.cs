using UnityEngine;

public class PlayerNetworkController : MonoBehaviour
{
    private Vector2 lastPosition;

    void Update()
    {
        if (Server.Instance.isHost)
        {
            // Détecte les mouvements
            Vector2 currentPosition = transform.position;
            if (currentPosition != lastPosition)
            {
                string message = $"MOVE:{currentPosition.x},{currentPosition.y}";
                Server.Instance.BroadcastMessage(message);
                lastPosition = currentPosition;
            }
        }
    }

    public void ApplyMovement(string data)
    {
        string[] splitData = data.Split(',');
        float x = float.Parse(splitData[0]);
        float y = float.Parse(splitData[1]);

        transform.position = new Vector2(x, y);
    }
}
