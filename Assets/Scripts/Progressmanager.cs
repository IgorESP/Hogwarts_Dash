using UnityEngine;
using UnityEngine.SceneManagement;

public class ProgressManager : MonoBehaviour
{
    public static ProgressManager Instance { get; private set; }
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("✅ ProgressManager inicializado");
        }
        else
        {
            Debug.Log("⚠️ ProgressManager ya existe, destruyendo duplicado");
            Destroy(gameObject);
        }
    }
    
    private int mundoActual;
    private int nivelActual = 1;
    
    private int preguntasTotales;
    private int respuestasCorrectas;
    private int respuestasIncorrectas;
    private float tiempoInicio;
    
    private int intentos = 0;
    private int snitchesRecogidos = 0;
    
    private bool siguienteMundoDesbloqueado = false;
    
    void Start()
    {
        mundoActual = PlayerPrefs.GetInt("MundoSeleccionado", 1);
        Debug.Log($"🎮 [ProgressManager] Jugando: Mundo {mundoActual} ({ObtenerNombreMundo()})");
        
        IniciarPartida();
        DesbloquearSiguienteMundoAlEntrar();
    }
    
    private void IniciarPartida()
    {
        preguntasTotales = 0;
        respuestasCorrectas = 0;
        respuestasIncorrectas = 0;
        tiempoInicio = Time.time;
        intentos = 0;
        snitchesRecogidos = 0;
        
        Debug.Log("[ProgressManager] Partida iniciada");
    }
    
    private void DesbloquearSiguienteMundoAlEntrar()
    {
        if (mundoActual < 5)
        {
            int siguienteMundo = mundoActual + 1;
            
            if (NivelesManager.Instance != null)
            {
                if (!NivelesManager.Instance.MundoEstaDesbloqueado(siguienteMundo))
                {
                    NivelesManager.Instance.DesbloquearMundo(siguienteMundo);
                    Debug.Log($"[ProgressManager] Mundo {siguienteMundo} desbloqueado");
                    siguienteMundoDesbloqueado = true;
                }
            }
            else
            {
                PlayerPrefs.SetInt($"Mundo_{siguienteMundo}_Desbloqueado", 1);
                PlayerPrefs.Save();
                Debug.Log($"[ProgressManager] Mundo {siguienteMundo} desbloqueado (legacy)");
                siguienteMundoDesbloqueado = true;
            }
        }
    }
    
    public void RegistrarRespuesta(bool esCorrecta)
    {
        preguntasTotales++;
        
        if (esCorrecta)
        {
            respuestasCorrectas++;
            Debug.Log($"Correcta ({respuestasCorrectas}/{preguntasTotales})");
        }
        else
        {
            respuestasIncorrectas++;
            Debug.Log($"Incorrecta ({respuestasIncorrectas}/{preguntasTotales})");
        }
    }
    
    public void RegistrarIntento()
    {
        intentos++;
        Debug.Log($"Intento #{intentos}");
    }
    
    public void RecogerSnitch()
    {
        snitchesRecogidos++;
        Debug.Log($"Snitch recogido! Total: {snitchesRecogidos}");
    }
    
    public int GetIntentos() => intentos;
    public int GetSnitches() => snitchesRecogidos;
    
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
    
    // ============================================
    // TERMINAR NIVEL (MEJORADO)
    // ============================================
    public void TerminarNivel()
    {
        float tiempoTotal = Time.time - tiempoInicio;
        float porcentaje = CalcularPorcentaje();
        int estrellas = CalcularEstrellas(porcentaje);
        
        Debug.Log("═══════════════════════════════════");
        Debug.Log($"[ProgressManager] NIVEL TERMINADO");
        Debug.Log($"Estadísticas:");
        Debug.Log($"    Mundo: {mundoActual} ({ObtenerNombreMundo()})");
        Debug.Log($"   Preguntas: {preguntasTotales}");
        Debug.Log($"   Correctas: {respuestasCorrectas}");
        Debug.Log($"   Porcentaje: {porcentaje:F1}%");
        Debug.Log($"   Intentos: {intentos}");
        Debug.Log($"   Snitches: {snitchesRecogidos}");
        Debug.Log($"   Estrellas: {estrellas}");
        Debug.Log($"    Tiempo: {tiempoTotal:F1}s");
        Debug.Log("═══════════════════════════════════");
        
        // Actualizar progreso
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
        
        // BUSCAR ResultadosController (MEJORADO)
        Debug.Log("Buscando ResultadosController...");
        
        ResultadosController resultadosController = FindObjectOfType<ResultadosController>();
        
        if (resultadosController != null)
        {
            Debug.Log("ResultadosController encontrado!");
            
            int porcentajeInt = Mathf.RoundToInt(porcentaje);
            bool victoria = estrellas > 0;
            
            resultadosController.MostrarResultados(porcentajeInt, intentos, snitchesRecogidos, victoria);
        }
        else
        {
            Debug.LogError("ResultadosController NO encontrado en la escena");
            Debug.LogError("Verifica que el GameObject UIResultado esté activo y tenga el componente ResultadosController");
            Debug.LogError("Volviendo al menú en 2 segundos...");
            
            Invoke("VolverANiveles", 2f);
        }
    }
    
    private void VolverANiveles()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("PlayScene"); 
    }
    
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
    
    public void SalirSinGuardar()
    {
        Debug.Log("[ProgressManager] Saliendo sin guardar");
        Time.timeScale = 1f;
        SceneManager.LoadScene("PlayScene"); 
    }
}