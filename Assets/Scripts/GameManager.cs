using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Gestor principal del juego
/// Lee las estadísticas guardadas en PlayerPrefs por FinishLevel
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    [Header("Estadísticas del Nivel")]
    public int objetivosTotales = 0;
    public int objetivosCompletados = 0;
    public int totalIntentos = 0;
    public int snitchesRecogidas = 0;
    
    [Header("Configuración del Nivel")]
    public int mundoActual = 1;
    public int nivelActual = 1;
    
    private ResultadosController resultadosController;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        mundoActual = PlayerPrefs.GetInt("MundoSeleccionado", 1);
        nivelActual = PlayerPrefs.GetInt("NivelSeleccionado", 1);
        
        Debug.Log($"[GameManager] Iniciado - Mundo {mundoActual}, Nivel {nivelActual}");
    }
    
    void Start()
    {
        resultadosController = FindFirstObjectByType<ResultadosController>();
        
        if (resultadosController == null)
        {
            Debug.LogWarning("[GameManager] No se encontró ResultadosController");
        }
    }
    
    /// <summary>
    /// Finalizar el nivel y mostrar resultados
    /// LEE LOS DATOS DE PLAYERPREFS (guardados por FinishLevel)
    /// </summary>
    public void FinalizarNivel()
    {
        // Leer estadísticas de PlayerPrefs (guardadas por FinishLevel)
        int porcentaje = Mathf.RoundToInt(PlayerPrefs.GetFloat("UltimoPorcentaje", 0f));
        int intentos = PlayerPrefs.GetInt("UltimosIntentos", 0);
        int snitches = PlayerPrefs.GetInt("UltimasSnitches", 0);
        
        Debug.Log($"[GameManager] Finalizando nivel:");
        Debug.Log($"   Porcentaje: {porcentaje}%");
        Debug.Log($"   Intentos: {intentos}");
        Debug.Log($"   Snitches: {snitches}");
        
        // Mostrar panel de resultados CON LOS VALORES CORRECTOS
        if (resultadosController != null)
        {
            bool victoria = porcentaje >= 40;
            resultadosController.MostrarResultados(porcentaje, intentos, snitches, victoria);
            Debug.Log("[GameManager] Panel de resultados llamado con valores correctos");
        }
        else
        {
            Debug.LogError("[GameManager] ResultadosController no encontrado");
        }
    }
    
    /// <summary>
    /// Obtener el porcentaje actual del nivel
    /// </summary>
    public int ObtenerPorcentajeActual()
    {
        if (objetivosTotales == 0) return 0;
        return Mathf.RoundToInt((objetivosCompletados / (float)objetivosTotales) * 100f);
    }
    
    /// <summary>
    /// Reiniciar estadísticas del nivel
    /// </summary>
    public void ReiniciarEstadisticas()
    {
        objetivosCompletados = 0;
        totalIntentos = 0;
        snitchesRecogidas = 0;
        Debug.Log("[GameManager] Estadísticas reiniciadas");
    }
    
    // ============================================
    // MÉTODOS DE COMPATIBILIDAD
    // ============================================
    
    public void PlayerDied()
    {
        totalIntentos++;
        Debug.Log($"[GameManager] PlayerDied llamado - Intento: {totalIntentos}");
    }
    
    public void LevelComplete()
    {
        FinalizarNivel();
    }
    
    public void ActualizarProgreso(int cantidad)
    {
        objetivosCompletados = cantidad;
    }
    
    public void IncrementarProgreso(int cantidad = 1)
    {
        objetivosCompletados += cantidad;
    }
    
    public void RegistrarSnitchRecogida()
    {
        snitchesRecogidas++;
    }
    
    public void RegistrarIntento()
    {
        totalIntentos++;
    }
}