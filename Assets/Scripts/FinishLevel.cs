using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Detecta cuando el jugador llega a la meta y finaliza el nivel
/// </summary>
public class FinishLevel : MonoBehaviour
{
    [Header("Configuración")]
    [Tooltip("Tiempo de espera antes de mostrar resultados")]
    public float tiempoEspera = 0.5f;
    
    private bool nivelCompletado = false;
    
    /// <summary>
    /// Detecta cuando el jugador alcanza la meta
    /// </summary>
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !nivelCompletado)
        {
            nivelCompletado = true;
            if (AudioManager.Instance != null)
                AudioManager.Instance.PlayVictoria();
            Debug.Log("[FinishLevel] ¡Meta alcanzada! Finalizando nivel...");
            Invoke("FinalizarNivel", tiempoEspera);
        }
    }
    
    /// <summary>
    /// Guarda las estadísticas del nivel y muestra resultados
    /// </summary>
    private void FinalizarNivel()
    {
        // Llamar al GameManager para que calcule y guarde todo
        if (GameManager.Instance != null)
        {
            GameManager.Instance.FinalizarNivel();
        }
        else
        {
            Debug.LogWarning("[FinishLevel] GameManager no encontrado");
            SceneManager.LoadScene("PlayScene");
        }
    }
    
    /// <summary>
    /// Cuenta cuántas Snitches fueron recogidas en el nivel
    /// </summary>
    private int ContarMonedasRecogidas()
    {
        // Buscar todos los scripts de Snitch (incluyendo desactivados)
        AnimacionSnitch[] todasLasSnitches = Resources.FindObjectsOfTypeAll<AnimacionSnitch>();
        int recogidas = 0;
        
        foreach (AnimacionSnitch snitch in todasLasSnitches)
        {
            // Solo contar las que pertenecen a la escena y fueron recogidas
            if (snitch.gameObject.scene.name != null && snitch.recogida)
            {
                recogidas++;
            }
        }
        
        Debug.Log($"[FinishLevel] Snitches recogidas: {recogidas}");
        return recogidas;
    }
    
    /// <summary>
    /// Obtiene el número de intentos del nivel actual
    /// </summary>
    private int ObtenerIntentos()
    {
        string escenaActual = SceneManager.GetActiveScene().name;
        return PlayerPrefs.GetInt("Intentos_" + escenaActual, 0);
    }
}