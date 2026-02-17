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

    [Header("Marcadores de Progreso (Opcional)")]
    [Tooltip("Objeto que marca el inicio real del nivel (0%)")]
    public Transform ptoInicio;
    
    [Tooltip("Objeto que marca la pared final del nivel (100%)")]
    public Transform ptoFinal;
    
    [Header("Configuración del Nivel")]
    [Tooltip("Mundo que se está jugando actualmente")]
    public int mundoActual = 1;
    
    [Tooltip("Nivel que se está jugando actualmente")]
    public int nivelActual = 1;
    
    private ResultadosController resultadosController;
    private Transform playerTransform;
    private float startX;
    private float endX;
    private bool nivelFinalizado = false;
    
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
        ReiniciarEstadisticas();  

        // Reiniciar estadísticas de esta sesión/intento
        //snitchesRecogidas = 0;
        if (NivelesManager.Instance != null) NivelesManager.Instance.RegistrarIntentoGlobal();
        if (ProgressManager.Instance != null) ProgressManager.Instance.RegistrarIntento();

        // Buscar el controlador de resultados en la escena
        resultadosController = FindFirstObjectByType<ResultadosController>();

        // Configurar puntos de inicio y fin para el porcentaje
        if (ptoInicio != null)
        {
            startX = ptoInicio.position.x;
        }
        else
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerTransform = player.transform;
                startX = playerTransform.position.x;
            }
        }

        // Si el playerTransform no se asignó arriba (porque usamos ptoInicio), buscarlo ahora
        if (playerTransform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) playerTransform = player.transform;
        }

        // Configurar punto final
        if (ptoFinal != null)
        {
            endX = ptoFinal.position.x;
        }
        else
        {
            // Fallback: Buscar meta (FinishLevel o tag Win)
            GameObject meta = GameObject.FindWithTag("Win");
            if (meta == null)
            {
                FinishLevel finishScript = FindFirstObjectByType<FinishLevel>();
                if (finishScript != null) meta = finishScript.gameObject;
            }

            if (meta != null)
            {
                endX = meta.transform.position.x;
            }
            else
            {
                Debug.LogWarning("[GameManager] No se encontró meta para calcular porcentaje. Usando valor por defecto.");
                endX = startX + 500f; // Valor más largo por defecto
            }
        }
        
        Debug.Log($"[GameManager] Configuración de progreso: Inicio={startX}, Fin={endX}, Distancia Total={endX - startX}");
    }
    
    /// <summary>
    /// Finaliza el nivel, lee las estadísticas guardadas y muestra el panel de resultados
    /// </summary>
    public void FinalizarNivel()
    {
        if (nivelFinalizado) return;
        nivelFinalizado = true;

        Debug.Log("[GameManager] Iniciando proceso de finalización de nivel...");
        
        // Calcular porcentaje real al ganar (debería ser 100 o muy cerca)
        int porcentaje = ObtenerPorcentajeActual();
        int intentos = (ProgressManager.Instance != null) ? ProgressManager.Instance.ObtenerIntentos() : PlayerPrefs.GetInt("IntentosActuales", totalIntentos); 
        int snitches = snitchesRecogidas;
        
        Debug.Log($"[GameManager] Resultados - Porcentaje: {porcentaje}%, Intentos: {intentos}, Snitches: {snitches}");
        
        // Mostrar la interfaz de resultados
        if (resultadosController != null)
        {
            // Se considera victoria si se llega a la meta (100%) o según lógica de porcentaje
            bool victoria = porcentaje >= 40; 
            resultadosController.MostrarResultados(porcentaje, intentos, snitches, victoria);
            
            // Actualizar sistema de progresión global
            if (NivelesManager.Instance != null)
                NivelesManager.Instance.CompletarNivel(mundoActual, nivelActual, porcentaje, snitches);

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

    /// <summary> 
    /// Se llama cuando el jugador muere. 
    /// </summary>
    public void PlayerDied()
    {
        if (nivelFinalizado) return;

        totalIntentos++;
        
        int intentosEsteIntento = 1;  
        int porcentajeMuerte = ObtenerPorcentajeActual();
        int snitchEsteIntento = snitchesRecogidas;
        
        EnviarProgresoAlMorir(porcentajeMuerte, snitchEsteIntento, intentosEsteIntento);
        
        PlayerPrefs.SetFloat("UltimoPorcentaje", porcentajeMuerte);
        PlayerPrefs.SetInt("UltimasSnitches", snitchesRecogidas);
        PlayerPrefs.Save();

        if (NivelesManager.Instance != null)
        {
            float tiempoSesion = Time.timeSinceLevelLoad;
            NivelesManager.Instance.CompletarNivel(mundoActual, nivelActual, porcentajeMuerte, snitchesRecogidas);
            NivelesManager.Instance.RegistrarTiempoNivel(mundoActual, nivelActual, tiempoSesion);
        }
        
        Debug.Log($"[GameManager] Jugador muerto al {porcentajeMuerte}%. Intento #{totalIntentos}");
    }

    private void EnviarProgresoAlMorir(int porcentaje, int snitchesEsteIntento, int intentosEsteIntento)
    {
        string playerId = PlayerPrefs.GetString("player_id", "");
        if (string.IsNullOrEmpty(playerId)) return;
        
        string nivel = GetNombreNivelActual();
        float tiempo = Time.timeSinceLevelLoad;
        
        ApiManager.Instance.ActualizarProgresoNivel(
            playerId, nivel, porcentaje, snitchesEsteIntento, intentosEsteIntento, tiempo,
            (exito, msg) => Debug.Log(exito ? "[API] Progreso muerte OK" : "[API] Error muerte")
        );
    }

     private string GetNombreNivelActual()
    {
        return mundoActual switch
        {
            1 => "comedor",
            2 => "gryffindor", 
            3 => "slytherin",
            4 => "hufflepuff",
            5 => "ravenclaw",
            _ => "comedor"
        };
    }
    
    /// <summary> 
    /// Se llama cuando el jugador completa el nivel (por trigger o tag). 
    /// </summary>
    public void LevelComplete()
    {
        Debug.Log("[GameManager] Nivel completado detectado por tag.");
        FinalizarNivel();
    }
    
    /// <summary> 
    /// Métodos reservados para futuros sistemas de progreso. 
    /// </summary>
    public void ActualizarProgreso(int cantidad)
    {
        
    }
    public void IncrementarProgreso(int cantidad = 1)
    {
        
    }
    
    /// <summary> 
    /// Registra una snitch recogida durante la sesión. 
    /// </summary>
    public void RegistrarSnitchRecogida() 
    { 
        snitchesRecogidas++; 
        Debug.Log($"[GameManager] Snitch registrada. Total: {snitchesRecogidas}");
    }
    
    /// <summary> 
    /// Registra un intento adicional. 
    /// </summary>
    public void RegistrarIntento() 
    { 
        totalIntentos++; 
    }
    
    /// <summary> 
    /// Devuelve el porcentaje actual del nivel basado en la posición X. 
    /// </summary>
    public int ObtenerPorcentajeActual()
    {
        if (playerTransform == null) return 0;

        float totalDist = endX - startX;
        if (totalDist <= 0.1f) return 0;

        float distRecorrida = playerTransform.position.x - startX;
        float progreso = distRecorrida / totalDist;

        return Mathf.Clamp(Mathf.RoundToInt(progreso * 100), 0, 100);
    }
    
    /// <summary> 
    /// Reinicia estadísticas de la sesión actual. 
    /// </summary>
    public void ReiniciarEstadisticas()
    {
        Debug.Log("[GameManager] Reiniciando estadísticas de sesión");
        totalIntentos = 0;
        snitchesRecogidas = 0;
    }
}