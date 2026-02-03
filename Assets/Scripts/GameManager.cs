using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    [Header("Estadísticas del Nivel")]
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
        
        Debug.Log($"[GameManager] Iniciado - Mundo {mundoActual}");
    }
    
    void Start()
    {
        resultadosController = FindFirstObjectByType<ResultadosController>();
    }
    
    /// <summary>
    /// Finalizar el nivel - LEE de PlayerPrefs y muestra panel
    /// </summary>
    public void FinalizarNivel()
    {
        // LEER de PlayerPrefs (guardado por FinishLevel)
        int porcentaje = Mathf.RoundToInt(PlayerPrefs.GetFloat("UltimoPorcentaje", 0f));
        int intentos = PlayerPrefs.GetInt("UltimosIntentos", 0);
        int snitches = PlayerPrefs.GetInt("UltimasSnitches", 0);
        
        Debug.Log($"[GameManager] ===== FINALIZANDO NIVEL =====");
        Debug.Log($"[GameManager] Porcentaje: {porcentaje}%");
        Debug.Log($"[GameManager] Intentos: {intentos}");
        Debug.Log($"[GameManager] Snitches: {snitches}");
        
        // Mostrar panel
        if (resultadosController != null)
        {
            bool victoria = porcentaje >= 40;
            resultadosController.MostrarResultados(porcentaje, intentos, snitches, victoria);
            Debug.Log($"[GameManager] ✅ Panel llamado con valores: {porcentaje}%, {intentos} intentos, {snitches} snitches");
        }
        else
        {
            Debug.LogError("[GameManager] ❌ ResultadosController es NULL!");
        }
    }
    
    // Métodos de compatibilidad
    public void PlayerDied()
    {
        totalIntentos++;
        Debug.Log($"[GameManager] PlayerDied - Intentos: {totalIntentos}");
    }
    
    public void LevelComplete()
    {
        FinalizarNivel();
    }
    
    public void ActualizarProgreso(int cantidad) { }
    public void IncrementarProgreso(int cantidad = 1) { }
    public void RegistrarSnitchRecogida() { snitchesRecogidas++; }
    public void RegistrarIntento() { totalIntentos++; }
    
    public int ObtenerPorcentajeActual()
    {
        return 0;
    }
    
    public void ReiniciarEstadisticas()
    {
        totalIntentos = 0;
        snitchesRecogidas = 0;
    }
}