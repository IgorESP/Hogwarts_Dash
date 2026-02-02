using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Gestor principal del juego que rastrea el progreso del nivel
/// Llama al panel de resultados cuando el nivel termina
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    [Header("Estadísticas del Nivel")]
    public int objetivosTotales = 100;         // Meta del nivel (ej: 100m de distancia, 50 monedas, etc.)
    public int objetivosCompletados = 0;       // Progreso actual (ej: 75m recorridos, 40 monedas, etc.)
    public int totalIntentos = 0;              // Número de veces que murió/falló
    public int snitchesRecogidas = 0;          // Snitches doradas recogidas (bonus)
    
    [Header("Configuración del Nivel")]
    public int mundoActual = 1;                // Mundo actual (1-5)
    public int nivelActual = 1;                // Nivel actual (1-10)
    
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
        
        // Obtener mundo y nivel desde PlayerPrefs o nombre de escena
        mundoActual = PlayerPrefs.GetInt("MundoSeleccionado", 1);
        nivelActual = PlayerPrefs.GetInt("NivelSeleccionado", 1);
        
        Debug.Log($"🎮 GameManager iniciado - Mundo {mundoActual}, Nivel {nivelActual}");
    }
    
    void Start()
    {
        // Buscar el ResultadosController en la escena
        resultadosController = FindFirstObjectByType<ResultadosController>();
        
        if (resultadosController == null)
        {
            Debug.LogWarning("⚠️ No se encontró ResultadosController en la escena");
        }
    }
    
    // ============================================
    // MÉTODOS PARA REGISTRAR EVENTOS DEL JUEGO
    // ============================================
    
    /// <summary>
    /// Actualizar progreso del nivel (ej: metros recorridos, monedas recogidas, etc.)
    /// </summary>
    public void ActualizarProgreso(int cantidad)
    {
        objetivosCompletados = cantidad;
        Debug.Log($"📊 Progreso: {objetivosCompletados}/{objetivosTotales} ({ObtenerPorcentajeActual()}%)");
    }
    
    /// <summary>
    /// Incrementar progreso (ej: +1 moneda, +10 metros, etc.)
    /// </summary>
    public void IncrementarProgreso(int cantidad = 1)
    {
        objetivosCompletados += cantidad;
        objetivosCompletados = Mathf.Min(objetivosCompletados, objetivosTotales); // No pasar del máximo
        Debug.Log($"📈 Progreso incrementado: {objetivosCompletados}/{objetivosTotales}");
    }
    
    /// <summary>
    /// Registrar un intento fallido (muerte, caída, etc.)
    /// </summary>
    public void RegistrarIntento()
    {
        totalIntentos++;
        Debug.Log($"💀 Intento registrado: {totalIntentos}");
    }
    
    /// <summary>
    /// Registrar una Snitch dorada recogida
    /// </summary>
    public void RegistrarSnitchRecogida()
    {
        snitchesRecogidas++;
        Debug.Log($"⭐ Snitch recogida: {snitchesRecogidas}");
    }
    
    /// <summary>
    /// Finalizar el nivel y mostrar resultados
    /// </summary>
    public void FinalizarNivel()
    {
        // Calcular porcentaje
        int porcentaje = ObtenerPorcentajeActual();
        
        Debug.Log($"🏁 Nivel finalizado:");
        Debug.Log($"   Progreso: {objetivosCompletados}/{objetivosTotales}");
        Debug.Log($"   Porcentaje: {porcentaje}%");
        Debug.Log($"   Intentos: {totalIntentos}");
        Debug.Log($"   Snitches: {snitchesRecogidas}");
        
        // Guardar progreso en NivelesManager
        if (NivelesManager.Instance != null)
        {
            NivelesManager.Instance.CompletarNivel(mundoActual, nivelActual, porcentaje);
        }
        
        // Guardar en la API
        GuardarProgresoEnAPI(porcentaje);
        
        // Mostrar panel de resultados
        if (resultadosController != null)
        {
            bool victoria = porcentaje >= 40; // 40% mínimo para aprobar
            resultadosController.MostrarResultados(porcentaje, totalIntentos, snitchesRecogidas, victoria);
        }
        else
        {
            Debug.LogError("❌ ResultadosController no encontrado");
        }
    }
    
    /// <summary>
    /// Forzar fin del nivel con valores específicos (para pruebas)
    /// </summary>
    public void FinalizarNivelForzado(int porcentaje, int intentos, int snitches)
    {
        objetivosCompletados = Mathf.RoundToInt(objetivosTotales * porcentaje / 100f);
        totalIntentos = intentos;
        snitchesRecogidas = snitches;
        
        FinalizarNivel();
    }
    
    // ============================================
    // GUARDAR EN LA API
    // ============================================
    
    private void GuardarProgresoEnAPI(int porcentaje)
    {
        if (ApiManager.Instance == null)
        {
            Debug.LogWarning("⚠️ ApiManager no disponible, no se puede guardar en API");
            return;
        }
        
        string playerId = PlayerPrefs.GetString("player_id", "");
        if (string.IsNullOrEmpty(playerId))
        {
            Debug.LogWarning("⚠️ No hay player_id, no se puede guardar en API");
            return;
        }
        
        // Aquí deberías llamar a un método de ApiManager para guardar el progreso
        // Ejemplo: ApiManager.Instance.GuardarProgreso(playerId, mundoActual, nivelActual, porcentaje);
        
        Debug.Log($"💾 Progreso guardado en API - Mundo {mundoActual}, Nivel {nivelActual}, {porcentaje}%");
    }
    
    // ============================================
    // MÉTODOS DE UTILIDAD
    // ============================================
    
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
        Debug.Log("🔄 Estadísticas reiniciadas");
    }
    
    // ============================================
    // MÉTODOS DE COMPATIBILIDAD (para otros scripts)
    // ============================================
    
    /// <summary>
    /// Llamado cuando el jugador muere (compatibilidad con scripts antiguos)
    /// </summary>
    public void PlayerDied()
    {
        RegistrarIntento();
        Debug.Log("💀 PlayerDied llamado - Intento registrado");
    }
    
    /// <summary>
    /// Llamado cuando se completa el nivel (compatibilidad con scripts antiguos)
    /// </summary>
    public void LevelComplete()
    {
        FinalizarNivel();
        Debug.Log("🏁 LevelComplete llamado - Finalizando nivel");
    }
}