using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelRestart : MonoBehaviour
{
    // Script de utilidad para reiniciar el nivel desde UI o Debug
    
    public void RestartScene()
    {
        Time.timeScale = 1; // Asegurar que el tiempo se reanude
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    
    // Acceso rápido para testear
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            RestartScene();
        }
    }
}
