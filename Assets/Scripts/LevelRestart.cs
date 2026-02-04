using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Utilidad para reiniciar el nivel actual
/// </summary>
public class LevelRestart : MonoBehaviour
{
    /// <summary>
    /// Reinicia la escena actual
    /// </summary>
    public void RestartScene()
    {
        Debug.Log("[LevelRestart] Reiniciando nivel...");
        Time.timeScale = 1; // Asegurar que el tiempo se reanude
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    
    /// <summary>
    /// Atajo de teclado para reiniciar (tecla R)
    /// </summary>
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            RestartScene();
        }
    }
}
