using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using System.Collections;

public class ResultadosController : MonoBehaviour
{
    [Header("Referencias UI")]
    private UIDocument uiDocument;
    private VisualElement panelResultados;
    private VisualElement contenedorResultados;
    
    [Header("Elementos UI")]
    private Label titulo;
    private Label labelPorcentaje;
    private Label labelIntentos;
    private Label labelSnitches;
    private VisualElement estrella1, estrella2, estrella3;
    private Button botonReintentar;
    private Button botonVolver;
    
    [Header("Estadísticas del Juego")]
    public int porcentajeCompletado = 0;
    public int totalIntentos = 0;
    public int snitchesRecogidas = 0;
    
    void Start()
    {
        // Obtener el UIDocument
        uiDocument = GetComponent<UIDocument>();
        VisualElement root = uiDocument.rootVisualElement;
        
        // Obtener referencias a los elementos
        panelResultados = root.Q<VisualElement>("PanelResultados");
        contenedorResultados = root.Q<VisualElement>("ContenedorResultados");
        
        // Labels
        titulo = root.Q<Label>("Titulo");
        labelPorcentaje = root.Q<Label>("Label_Porcentaje");
        labelIntentos = root.Q<Label>("Label_Intentos");
        labelSnitches = root.Q<Label>("Label_Snitches");
        
        // Estrellas
        estrella1 = root.Q<VisualElement>("Estrella1");
        estrella2 = root.Q<VisualElement>("Estrella2");
        estrella3 = root.Q<VisualElement>("Estrella3");
        
        // Botones
        botonReintentar = root.Q<Button>("Boton_Reintentar");
        botonVolver = root.Q<Button>("Boton_Volver");
        
        // Asignar eventos a los botones
        if (botonReintentar != null) botonReintentar.clicked += Reintentar;
        if (botonVolver != null) botonVolver.clicked += VolverAlMenu;
        
        // IMPORTANTE: Ocultar el panel al inicio
        OcultarPanel();
        
        Debug.Log("ResultadosController inicializado");
    }
    
    // ============================================
    // MÉTODO PRINCIPAL: Mostrar resultados
    // ============================================
    public void MostrarResultados(int porcentaje, int intentos, int snitches, bool victoria = true)
    {
        Debug.Log($"Mostrando resultados: {porcentaje}%, {intentos} intentos, {snitches} snitches");
        
        // Pausar el juego
        Time.timeScale = 0f;
        
        // Guardar estadísticas
        porcentajeCompletado = porcentaje;
        totalIntentos = intentos;
        snitchesRecogidas = snitches;
        
        // Actualizar título según victoria o derrota
        if (victoria)
            titulo.text = "¡NIVEL COMPLETADO!";
        else
            titulo.text = "¡NIVEL FALLIDO!";
        
        // Actualizar estadísticas en la UI
        labelPorcentaje.text = porcentaje + "%";
        labelIntentos.text = intentos.ToString();
        labelSnitches.text = snitches.ToString();
        
        // Calcular y mostrar estrellas
        ActualizarEstrellas(porcentaje);
        
        // Mostrar el panel con animación
        MostrarPanel();
        
        // Guardar progreso
        GuardarProgreso();
    }
    
    // ============================================
    // Sistema de estrellas según porcentaje
    // ============================================
    private void ActualizarEstrellas(int porcentaje)
    {
        // Resetear opacidad de todas las estrellas
        estrella1.style.opacity = 0.3f;
        estrella2.style.opacity = 0.3f;
        estrella3.style.opacity = 0.3f;
        
        // Activar estrellas según porcentaje
        if (porcentaje >= 33) // 1 estrella
        {
            estrella1.style.opacity = 1f;
        }
        if (porcentaje >= 66) // 2 estrellas
        {
            estrella2.style.opacity = 1f;
        }
        if (porcentaje >= 90) // 3 estrellas
        {
            estrella3.style.opacity = 1f;
        }
    }
    
    // ============================================
    // Mostrar y ocultar panel
    // ============================================
    private void MostrarPanel()
    {
        if (panelResultados != null)
        {
            panelResultados.style.display = DisplayStyle.Flex;
            
            // Animación de entrada
            if (contenedorResultados != null)
            {
                contenedorResultados.style.scale = new Scale(new Vector3(0.8f, 0.8f, 1f));
                contenedorResultados.schedule.Execute(() => {
                    contenedorResultados.style.scale = new Scale(new Vector3(1f, 1f, 1f));
                }).StartingIn(50);
            }
            
            Debug.Log("Panel de resultados mostrado");
        }
        else
        {
            Debug.LogError("panelResultados es null");
        }
    }
    
    private void OcultarPanel()
    {
        if (panelResultados != null)
        {
            panelResultados.style.display = DisplayStyle.None;
            Debug.Log("Panel de resultados oculto");
        }
    }
    
    // ============================================
    // Eventos de botones
    // ============================================
    private void Reintentar()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    private void VolverAlMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MenulPrincipalT");
    }
    
    // ============================================
    // Guardar progreso (PlayerPrefs y API)
    // ============================================
    private void GuardarProgreso()
    {
        string nombreNivel = SceneManager.GetActiveScene().name;
        
        // Guardar en PlayerPrefs
        PlayerPrefs.SetInt(nombreNivel + "_Porcentaje", porcentajeCompletado);
        PlayerPrefs.SetInt(nombreNivel + "_Intentos", totalIntentos);
        PlayerPrefs.SetInt(nombreNivel + "_Snitches", snitchesRecogidas);
        PlayerPrefs.SetInt(nombreNivel + "_Completado", 1);
        PlayerPrefs.Save();
        
        Debug.Log("Progreso guardado localmente: " + nombreNivel);
        
        // Guardar en la API
        StartCoroutine(EnviarProgresoAPI());
    }
    
    private IEnumerator EnviarProgresoAPI()
    {
        string playerId = PlayerPrefs.GetString("player_id", "");
        if (string.IsNullOrEmpty(playerId))
        {
            Debug.LogWarning("No hay player_id guardado, no se puede enviar a la API");
            yield break;
        }
        
        string url = $"https://hogwartsdashapi.onrender.com/api/v1/jugadores/{playerId}/progreso";
        
        string json = $"{{" +
            $"\"player_id\": \"{playerId}\"," +
            $"\"nivel\": \"{SceneManager.GetActiveScene().name}\"," +
            $"\"porcentaje\": {porcentajeCompletado}" +
            $"}}";
        
        UnityEngine.Networking.UnityWebRequest request = new UnityEngine.Networking.UnityWebRequest(url, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UnityEngine.Networking.UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new UnityEngine.Networking.DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        
        yield return request.SendWebRequest();
        
        if (request.result == UnityEngine.Networking.UnityWebRequest.Result.Success)
        {
            Debug.Log("Progreso enviado a la API exitosamente");
        }
        else
        {
            Debug.LogError($"Error al enviar progreso a la API: {request.error}");
        }
    }
}