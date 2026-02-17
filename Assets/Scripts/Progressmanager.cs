using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Gestor de progreso durante la partida activa (Stats en tiempo real)
/// </summary>
public class ProgressManager : MonoBehaviour
{
    public static ProgressManager Instance { get; private set; }
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("[Progress] Gestor de partida activa listo");
        }
        else { Destroy(gameObject); }
    }
    
    private int mundoActual;
    private int preguntasTotales, respuestasCorrectas, respuestasIncorrectas;
    private float tiempoInicio;
    private int intentos = 0, snitchesRecogidos = 0;
    
    void Start()
    {
        mundoActual = PlayerPrefs.GetInt("MundoSeleccionado", 1);
        Debug.Log($"[Progress] Iniciando seguimiento: Mundo {mundoActual}");
        
        IniciarPartida();
    }
    
    /// <summary> 
    /// Inicializa todas las estadísticas de la partida. 
    /// </summary>
    public void IniciarPartida()
    {
        preguntasTotales = 0;
        respuestasCorrectas = 0;
        respuestasIncorrectas = 0;
        intentos = 0;
        snitchesRecogidos = 0;

        tiempoInicio = Time.time;
    }
    
    
    /// <summary> 
    /// Registra una respuesta (correcta o incorrecta). 
    /// </summary>
    public void RegistrarRespuesta(bool esCorrecta)
    {
        preguntasTotales++;

        if (esCorrecta) 
            respuestasCorrectas++;
        else 
            respuestasIncorrectas++;

        Debug.Log($"[Progress] Pregunta respondida. Correctas: {respuestasCorrectas}/{preguntasTotales}");
    }
    
    /// <summary> 
    /// Registra un intento fallido. 
    /// </summary>
    public void RegistrarIntento() 
    { 
        intentos++; 
    }

    public int ObtenerIntentos() => intentos;

    /// <summary>
    /// Registra una snitch recogida. 
    /// </summary>
    public void RecogerSnitch() 
    { 
        snitchesRecogidos++; 
        Debug.Log("[Progress] Snitch +1"); 
        }
    
    /// <summary> 
    /// Calcula el porcentaje de aciertos. 
    /// </summary>
    public float CalcularPorcentaje() => 
        preguntasTotales == 0 ? 0f : (respuestasCorrectas / (float)preguntasTotales) * 100f;
    
    /// <summary>
    /// Finaliza el seguimiento de la partida y reporta resultados al NivelesManager y UI
    /// </summary>
    public void TerminarNivel()
    {
        float duracion = Time.time - tiempoInicio;
        float p = CalcularPorcentaje();
        
        Debug.Log($"[Progress] Finalizando Nivel {mundoActual}. Porcentaje: {p:F1}%. Intentos: {intentos}");
        
        if (NivelesManager.Instance != null) 
        {
            // Usar snitches del GameManager si están disponibles para mayor precisión
            int finalSnitches = (GameManager.Instance != null) ? GameManager.Instance.snitchesRecogidas : snitchesRecogidos;
            NivelesManager.Instance.CompletarNivel(mundoActual, 1, p, finalSnitches);
            snitchesRecogidos = finalSnitches; // Sincronizar localmente por si acaso
        }
        
        // Persistir resultados para la escena de fin
        PlayerPrefs.SetFloat("UltimoPorcentaje", p);
        PlayerPrefs.SetInt("UltimosIntentos", intentos);
        PlayerPrefs.SetInt("UltimasSnitches", snitchesRecogidos);
        PlayerPrefs.Save();
        
        ResultadosController ui = FindFirstObjectByType<ResultadosController>();
        if (ui != null) 
            ui.MostrarResultados(Mathf.RoundToInt(p), intentos, snitchesRecogidos, p >= 40f);
        else 
            Debug.LogWarning("[Progress] No se encontró el panel de resultados para mostrar el fin de nivel");
    }

    /// <summary> 
    /// Sale del nivel sin guardar progreso. 
    /// </summary>
    public void SalirSinGuardar() 
    { 
        Time.timeScale = 1f; 
        SceneManager.LoadScene("PlayScene"); 
    }
}