using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// ProgressManager para Hogwarts Dash
/// Registra progreso y desbloquea mundos
/// </summary>
public class ProgressManager : MonoBehaviour
{
    // ====================================
    // SINGLETON
    // ====================================
    public static ProgressManager Instance { get; private set; }
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("ProgressManager inicializado");
        }
        else
        {
            Debug.Log(" ProgressManager ya existe, destruyendo duplicado");
            Destroy(gameObject);
        }
    }
    
    // ====================================
    // DATOS DEL JUEGO ACTUAL
    // ====================================
    private int mundoActual;
    private int nivelActual = 1;
    
    // Estadísticas
    private int preguntasTotales;
    private int respuestasCorrectas;
    private int respuestasIncorrectas;
    private float tiempoInicio;
    
    // NUEVAS ESTADÍSTICAS
    private int intentos = 0;
    private int snitchesRecogidos = 0;
    
    private bool siguienteMundoDesbloqueado = false;
    
    // ====================================
    // INICIAR
    // ====================================
    void Start()
    {
        // Obtener mundo seleccionado
        mundoActual = PlayerPrefs.GetInt("MundoSeleccionado", 1);
        
        Debug.Log($"🎮 [ProgressManager] Jugando: Mundo {mundoActual} ({ObtenerNombreMundo()})");
        
        IniciarPartida();
        
        // DESBLOQUEAR SIGUIENTE MUNDO AL ENTRAR
        DesbloquearSiguienteMundoAlEntrar();
    }
    
    private void IniciarPartida()
    {
        preguntasTotales = 0;
        respuestasCorrectas = 0;
        respuestasIncorrectas = 0;
        tiempoInicio = Time.time;
        
        // INICIALIZAR NUEVAS ESTADÍSTICAS
        intentos = 0;
        snitchesRecogidos = 0;
        
        Debug.Log(" [ProgressManager] Partida iniciada");
    }
    
    // ====================================
    //  DESBLOQUEAR SIGUIENTE MUNDO AL ENTRAR
    // ====================================
    private void DesbloquearSiguienteMundoAlEntrar()
    {
        // Si no es el último mundo, desbloquear el siguiente
        if (mundoActual < 5)
        {
            int siguienteMundo = mundoActual + 1;
            
            if (NivelesManager.Instance != null)
            {
                // Verificar si ya está desbloqueado
                if (!NivelesManager.Instance.MundoEstaDesbloqueado(siguienteMundo))
                {
                    NivelesManager.Instance.DesbloquearMundo(siguienteMundo);
                    Debug.Log($"[ProgressManager] Mundo {siguienteMundo} desbloqueado (solo por entrar)");
                    siguienteMundoDesbloqueado = true;
                }
                else
                {
                    Debug.Log($"[ProgressManager] Mundo {siguienteMundo} ya estaba desbloqueado");
                }
            }
            else
            {
                // Sistema legacy
                PlayerPrefs.SetInt($"Mundo_{siguienteMundo}_Desbloqueado", 1);
                PlayerPrefs.Save();
                Debug.Log($"[ProgressManager] Mundo {siguienteMundo} desbloqueado (legacy)");
                siguienteMundoDesbloqueado = true;
            }
        }
        else
        {
            Debug.Log("[ProgressManager] ¡Último mundo! No hay más para desbloquear");
        }
    }
    
    // ====================================
    // REGISTRAR RESPUESTA
    // ====================================
    public void RegistrarRespuesta(bool esCorrecta)
    {
        preguntasTotales++;
        
        if (esCorrecta)
        {
            respuestasCorrectas++;
            Debug.Log($"[ProgressManager] Correcta ({respuestasCorrectas}/{preguntasTotales})");
        }
        else
        {
            respuestasIncorrectas++;
            Debug.Log($" [ProgressManager] Incorrecta ({respuestasIncorrectas}/{preguntasTotales})");
        }
    }
    
    // ====================================
    // NUEVOS MÉTODOS PARA ESTADÍSTICAS
    // ====================================
    
    /// <summary>
    /// Registra un intento fallido (muerte, caída, etc.)
    /// </summary>
    public void RegistrarIntento()
    {
        intentos++;
        Debug.Log($"Intento #{intentos}");
    }
    
    /// <summary>
    /// Registra un snitch recogido
    /// </summary>
    public void RecogerSnitch()
    {
        snitchesRecogidos++;
        Debug.Log($"Snitch recogido! Total: {snitchesRecogidos}");
    }
    
    public int GetIntentos() => intentos;
    public int GetSnitches() => snitchesRecogidos;
    
    // ====================================
    // CALCULAR PORCENTAJE
    // ====================================
    public float CalcularPorcentaje()
    {
        if (preguntasTotales == 0) return 0f;
        return (respuestasCorrectas / (float)preguntasTotales) * 100f;
    }
    
    private int CalcularEstrellas(float porcentaje)
    {
        if (porcentaje >= 90f) return 3;
        if (porcentaje >= 70f) return 2;
        if (porcentaje >= 40f) return 1;
        return 0;
    }
    
    // ====================================
    // TERMINAR NIVEL (CORREGIDO)
    // ====================================
    public void TerminarNivel()
    {
        float tiempoTotal = Time.time - tiempoInicio;
        float porcentaje = CalcularPorcentaje();
        int estrellas = CalcularEstrellas(porcentaje);
        
        Debug.Log("═══════════════════════════════════");
        Debug.Log($"🏁 [ProgressManager] NIVEL TERMINADO");
        Debug.Log($" Estadísticas:");
        Debug.Log($"    Mundo: {mundoActual} ({ObtenerNombreMundo()})");
        Debug.Log($"    Preguntas: {preguntasTotales}");
        Debug.Log($"    Correctas: {respuestasCorrectas}");
        Debug.Log($"   Porcentaje: {porcentaje:F1}%");
        Debug.Log($"   Intentos: {intentos}");
        Debug.Log($"   Snitches: {snitchesRecogidos}");
        Debug.Log($"   Estrellas: {estrellas}");
        Debug.Log($"   Tiempo: {tiempoTotal:F1}s");
        
        if (siguienteMundoDesbloqueado)
        {
            Debug.Log($"Mundo {mundoActual + 1} fue desbloqueado al entrar");
        }
        
        Debug.Log("═══════════════════════════════════");
        
        // Actualizar progreso con NivelesManager
        if (NivelesManager.Instance != null)
        {
            NivelesManager.Instance.CompletarNivel(mundoActual, nivelActual, porcentaje);
        }
        
        // Guardar estadísticas
        PlayerPrefs.SetInt("UltimoMundo", mundoActual);
        PlayerPrefs.SetFloat("UltimoPorcentaje", porcentaje);
        PlayerPrefs.SetInt("UltimasCorrectas", respuestasCorrectas);
        PlayerPrefs.SetInt("UltimasTotales", preguntasTotales);
        PlayerPrefs.SetFloat("UltimoTiempo", tiempoTotal);
        PlayerPrefs.Save();
        
        // MOSTRAR PANTALLA DE RESULTADOS (CORREGIDO)
        var resultadosController = FindObjectOfType<ResultadosController>();
        if (resultadosController != null)
        {
            // CORRECCIÓN: Convertir float a int y usar bool en lugar de int
            int porcentajeInt = Mathf.RoundToInt(porcentaje); // float → int
            bool victoria = estrellas > 0; // Determinar si ganó basado en estrellas
            
            resultadosController.MostrarResultados(porcentajeInt, intentos, snitchesRecogidos, victoria);
        }
        else
        {
            Debug.LogError("No se encontró ResultadosController - Volviendo al menú en 2s");
            Invoke("VolverANiveles", 2f);
        }
    }
    
    private void VolverANiveles()
    {
        SceneManager.LoadScene("PlayScene"); 
    }
    
    // ====================================
    // OBTENER ESTADÍSTICAS (Para UI)
    // ====================================
    public int GetPreguntasTotales() => preguntasTotales;
    public int GetRespuestasCorrectas() => respuestasCorrectas;
    public int GetRespuestasIncorrectas() => respuestasIncorrectas;
    public float GetTiempoTranscurrido() => Time.time - tiempoInicio;
    
    private string ObtenerNombreMundo()
    {
        return mundoActual switch
        {
            1 => "Gryffindor",
            2 => "Slytherin",
            3 => "Ravenclaw",
            4 => "Hufflepuff",
            5 => "Final",
            _ => "Desconocido"
        };
    }
    
    // ====================================
    // SALIR SIN GUARDAR
    // ====================================
    public void SalirSinGuardar()
    {
        Debug.Log("[ProgressManager] Saliendo sin guardar");
        SceneManager.LoadScene("PlayScene"); 
    }
}