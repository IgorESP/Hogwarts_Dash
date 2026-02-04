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
        DesbloquearSiguienteMundoAlEntrar();
    }
    
    private void IniciarPartida()
    {
        preguntasTotales = respuestasCorrectas = respuestasIncorrectas = intentos = snitchesRecogidos = 0;
        tiempoInicio = Time.time;
    }
    
    /// <summary>
    /// Desbloquea preventivamente el siguiente mundo al entrar en el actual
    /// </summary>
    private void DesbloquearSiguienteMundoAlEntrar()
    {
        if (mundoActual < 5)
        {
            int next = mundoActual + 1;
            if (NivelesManager.Instance != null) NivelesManager.Instance.DesbloquearMundo(next);
            else PlayerPrefs.SetInt($"Mundo_{next}_Desbloqueado", 1);
        }
    }
    
    public void RegistrarRespuesta(bool esCorrecta)
    {
        preguntasTotales++;
        if (esCorrecta) respuestasCorrectas++;
        else respuestasIncorrectas++;
        Debug.Log($"[Progress] Pregunta respondida. Correctas: {respuestasCorrectas}/{preguntasTotales}");
    }
    
    public void RegistrarIntento() { intentos++; }
    public void RecogerSnitch() { snitchesRecogidos++; Debug.Log("[Progress] Snitch +1"); }
    
    public float CalcularPorcentaje() => preguntasTotales == 0 ? 0f : (respuestasCorrectas / (float)preguntasTotales) * 100f;
    
    /// <summary>
    /// Finaliza el seguimiento de la partida y reporta resultados al NivelesManager y UI
    /// </summary>
    public void TerminarNivel()
    {
        float duracion = Time.time - tiempoInicio;
        float p = CalcularPorcentaje();
        
        Debug.Log($"[Progress] Finalizando Nivel {mundoActual}. Porcentaje: {p:F1}%. Intentos: {intentos}");
        
        if (NivelesManager.Instance != null) NivelesManager.Instance.CompletarNivel(mundoActual, 1, p);
        
        // Persistir resultados para la escena de fin
        PlayerPrefs.SetFloat("UltimoPorcentaje", p);
        PlayerPrefs.SetInt("UltimosIntentos", intentos);
        PlayerPrefs.SetInt("UltimasSnitches", snitchesRecogidos);
        PlayerPrefs.Save();
        
        ResultadosController ui = FindFirstObjectByType<ResultadosController>();
        if (ui != null) ui.MostrarResultados(Mathf.RoundToInt(p), intentos, snitchesRecogidos, p >= 40f);
        else Debug.LogWarning("[Progress] No se encontró el panel de resultados para mostrar el fin de nivel");
    }

    public void SalirSinGuardar() { Time.timeScale = 1f; SceneManager.LoadScene("PlayScene"); }
}