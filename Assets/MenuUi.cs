using UnityEngine;
using UnityEngine.SceneManagement;  // Assurez-vous d'ajouter ceci pour utiliser SceneManager

public class MenuUi : MonoBehaviour
{
    public void SetRole(bool isServer) {
        Globals.IsServer = isServer;
    }

    public void StartGame() {
        SceneManager.LoadScene("patatechaude2d");  // Utilisez LoadScene avec la bonne syntaxe
    }
}
