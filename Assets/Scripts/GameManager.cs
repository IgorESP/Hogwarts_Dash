using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Gestor principal del flujo de juego y estadísticas de sesión
/// </summary>
public class GameManager : MonoBehaviour
{
    // Singleton para acceso global
    public static GameManager Instance { get; private set; }
    
    [Header("Estadísticas del Nivel")]
    [Tooltip("Número total de intentos en la sesión actual")]
    public int totalIntentos = 0;
    
    [Tooltip("Número de snitches recogidas en la sesión actual")]
    public int snitchesRecogidas = 0;
    
    [Header("Configuración del Nivel")]
    [Tooltip("Mundo que se está jugando actualmente")]
    public int mundoActual = 1;
    
    [Tooltip("Nivel que se está jugando actualmente")]
    public int nivelActual = 1;
    
    private ResultadosController resultadosController;
    
    void Awake()
    {
        // Implementación de Singleton
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        // Cargar selección actual de PlayerPrefs
        mundoActual = PlayerPrefs.GetInt("MundoSeleccionado", 1);
        nivelActual = PlayerPrefs.GetInt("NivelSeleccionado", 1);
        
        Debug.Log($"[GameManager] Iniciado - Escena: {SceneManager.GetActiveScene().name}, Mundo: {mundoActual}, Nivel: {nivelActual}");
    }
    
    void Start()
    {
        // Buscar el controlador de resultados en la escena
        resultadosController = FindFirstObjectByType<ResultadosController>();
    }
    
    /// <summary>
    /// Finaliza el nivel, lee las estadísticas guardadas y muestra el panel de resultados
    /// </summary>
    public void FinalizarNivel()
    {
        Debug.Log("[GameManager] Iniciando proceso de finalización de nivel...");
        
        // Recuperar datos guardados temporalmente en PlayerPrefs por FinishLevel
        int porcentaje = Mathf.RoundToInt(PlayerPrefs.GetFloat("UltimoPorcentaje", 0f));
        int intentos = PlayerPrefs.GetInt("UltimosIntentos", 0);
        int snitches = PlayerPrefs.GetInt("UltimasSnitches", 0);
        
        Debug.Log($"[GameManager] Resultados - Porcentaje: {porcentaje}%, Intentos: {intentos}, Snitches: {snitches}");
        
        // Mostrar la interfaz de resultados
        if (resultadosController != null)
        {
            // Se considera victoria si se llega a la meta (100%) o según lógica de porcentaje
            bool victoria = porcentaje >= 40; 
            resultadosController.MostrarResultados(porcentaje, intentos, snitches, victoria);
            Debug.Log("[GameManager] Panel de resultados desplegado");
        }
        else
        {
            Debug.LogError("[GameManager] No se encontró ResultadosController en la escena para mostrar el panel.");
        }
    }
    
    // ====================================
    // MÉTODOS DE COMPATIBILIDAD Y CONTROL
    // ====================================

    public void PlayerDied()
    {
        totalIntentos++;
        Debug.Log($"[GameManager] Jugador muerto. Total intentos sesión: {totalIntentos}");
    }
    
    public void LevelComplete()
    {
        Debug.Log("[GameManager] Nivel completado detectado por tag.");
        FinalizarNivel();
    }
    
    public void ActualizarProgreso(int cantidad) { /* Implementar si es necesario */ }
    public void IncrementarProgreso(int cantidad = 1) { /* Implementar si es necesario */ }
    
    public void RegistrarSnitchRecogida() 
    { 
        snitchesRecogidas++; 
        Debug.Log($"[GameManager] Snitch registrada. Total: {snitchesRecogidas}");
    }
    
    public void RegistrarIntento() 
    { 
        totalIntentos++; 
    }
    
    public int ObtenerPorcentajeActual()
    {
        return 0; // Implementar cálculo de porcentaje en tiempo real si se requiere
    }
    
    public void ReiniciarEstadisticas()
    {
        Debug.Log("[GameManager] Reiniciando estadísticas de sesión");
        totalIntentos = 0;
        snitchesRecogidas = 0;
    }
}