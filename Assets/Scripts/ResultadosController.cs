using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// Controlador del panel de resultados al finalizar un nivel.
/// Muestra estadisticas y permite reintentar o volver al menu.
/// </summary>
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
    private Button botonReintentar;
    private Button botonVolver;
    
    [Header("Estadisticas del Juego")]
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
        
        // Botones
        botonReintentar = root.Q<Button>("Boton_Reintentar");
        botonVolver = root.Q<Button>("Boton_Volver");
        
        // Asignar eventos a los botones
        if (botonReintentar != null) botonReintentar.clicked += Reintentar;
        if (botonVolver != null) botonVolver.clicked += VolverAlMenu;
        
        // Ocultar el panel al inicio
        OcultarPanel();
        
        Debug.Log("[Resultados] ResultadosController inicializado");
    }
    
    // ============================================
    // METODO PRINCIPAL: Mostrar resultados
    // ============================================
    
    /// <summary>
    /// Muestra el panel de resultados con las estadisticas del nivel.
    /// </summary>
    /// <param name="porcentaje">Porcentaje de objetivos completados (0-100)</param>
    /// <param name="intentos">Numero de intentos/muertes</param>
    /// <param name="snitches">Snitches doradas recogidas</param>
    /// <param name="victoria">No se usa, siempre muestra NIVEL COMPLETADO</param>
    public void MostrarResultados(int porcentaje, int intentos, int snitches, bool victoria = true)
    {
        Debug.Log($"[Resultados] Mostrando: {porcentaje}%, {intentos} intentos, {snitches} snitches");
        
        // Pausar el juego
        Time.timeScale = 0f;
        
        // Guardar estadisticas
        porcentajeCompletado = porcentaje;
        totalIntentos = intentos;
        snitchesRecogidas = snitches;
        
        // Titulo siempre es NIVEL COMPLETADO
        if (titulo != null)
        {
            titulo.text = "¡NIVEL COMPLETADO!";
        }
        
        // Actualizar estadisticas en la UI
        if (labelPorcentaje != null) labelPorcentaje.text = porcentaje + "%";
        if (labelIntentos != null) labelIntentos.text = intentos.ToString();
        if (labelSnitches != null) labelSnitches.text = snitches.ToString();
        
        // Mostrar el panel con animacion
        MostrarPanel();
        
        // Guardar progreso en API
        StartCoroutine(GuardarProgresoEnAPI());
    }
    
    // ============================================
    // Calcular numero de estrellas (para API)
    // ============================================
    
    /// <summary>
    /// Calcula el numero de estrellas segun el porcentaje.
    /// Se usa solo para guardar en la API.
    /// </summary>
    private int CalcularNumeroEstrellas(int porcentaje)
    {
        if (porcentaje >= 90) return 3;
        if (porcentaje >= 70) return 2;
        if (porcentaje >= 40) return 1;
        return 0;
    }
    
    // ============================================
    // Mostrar y ocultar panel
    // ============================================
    
    /// <summary>
    /// Muestra el panel de resultados con animacion.
    /// </summary>
    private void MostrarPanel()
    {
        if (panelResultados != null)
        {
            panelResultados.style.display = DisplayStyle.Flex;
            
            // Animacion de entrada (escala)
            if (contenedorResultados != null)
            {
                contenedorResultados.style.scale = new Scale(new Vector3(0.8f, 0.8f, 1f));
                contenedorResultados.schedule.Execute(() => {
                    contenedorResultados.style.scale = new Scale(new Vector3(1f, 1f, 1f));
                }).StartingIn(50);
            }
            
            Debug.Log("[Resultados] Panel mostrado");
        }
        else
        {
            Debug.LogError("[Resultados] panelResultados es null");
        }
    }
    
    /// <summary>
    /// Oculta el panel de resultados.
    /// </summary>
    private void OcultarPanel()
    {
        if (panelResultados != null)
        {
            panelResultados.style.display = DisplayStyle.None;
            Debug.Log("[Resultados] Panel oculto");
        }
    }
    
    // ============================================
    // Eventos de botones
    // ============================================
    
    /// <summary>
    /// Reintentar el nivel actual (recarga la escena).
    /// </summary>
    private void Reintentar()
    {
        Debug.Log("[Resultados] Reintentando nivel");
        
        // Reanudar el tiempo
        Time.timeScale = 1f;
        
        // Reiniciar estadisticas del GameManager si existe
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ReiniciarEstadisticas();
        }
        
        // Recargar la escena actual
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    /// <summary>
    /// Volver al menu de seleccion de mundos (PlayScene).
    /// </summary>
    private void VolverAlMenu()
    {
        Debug.Log("[Resultados] Volviendo a PlayScene");
        
        // Reanudar el tiempo
        Time.timeScale = 1f;
        
        // Cargar la escena de seleccion de mundos
        SceneManager.LoadScene("PlayScene");
    }
    
    // ============================================
    // Guardar progreso en API
    // ============================================
    
    /// <summary>
    /// Envia el progreso del nivel a la API de Hogwarts Dash.
    /// </summary>
    private IEnumerator GuardarProgresoEnAPI()
    {
        // Obtener player_id
        string playerId = PlayerPrefs.GetString("player_id", "");
        if (string.IsNullOrEmpty(playerId))
        {
            Debug.LogWarning("[Resultados] No hay player_id, no se puede enviar a la API");
            yield break;
        }
        
        // Obtener mundo y nivel actual
        int mundoActual = PlayerPrefs.GetInt("MundoSeleccionado", 1);
        int nivelActual = PlayerPrefs.GetInt("NivelSeleccionado", 1);
        
        // Construir URL
        string url = $"https://hogwartsdashapi.onrender.com/api/v1/jugadores/{playerId}/progreso";
        
        // Construir JSON
        string json = $"{{" +
            $"\"player_id\": \"{playerId}\"," +
            $"\"mundo\": {mundoActual}," +
            $"\"nivel\": {nivelActual}," +
            $"\"porcentaje\": {porcentajeCompletado}," +
            $"\"intentos\": {totalIntentos}," +
            $"\"snitches\": {snitchesRecogidas}," +
            $"\"estrellas\": {CalcularNumeroEstrellas(porcentajeCompletado)}" +
            $"}}";
        
        Debug.Log($"[Resultados] Enviando a API: {json}");
        
        // Crear request
        UnityEngine.Networking.UnityWebRequest request = new UnityEngine.Networking.UnityWebRequest(url, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UnityEngine.Networking.UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new UnityEngine.Networking.DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        
        // Agregar API Key
        string apiKey = PlayerPrefs.GetString("ApiKey", "hogwarts_dash_2026_admin");
        if (!string.IsNullOrEmpty(apiKey))
        {
            request.SetRequestHeader("X-API-Key", apiKey);
            request.SetRequestHeader("api-key", apiKey);
        }
        
        // Enviar request
        yield return request.SendWebRequest();
        
        // Verificar respuesta
        if (request.result == UnityEngine.Networking.UnityWebRequest.Result.Success)
        {
            Debug.Log("[Resultados] Progreso enviado exitosamente a la API");
        }
        else
        {
            Debug.LogError($"[Resultados] Error al enviar a API: {request.error}");
            Debug.LogError($"[Resultados] Response: {request.downloadHandler.text}");
        }
    }
    
    // ============================================
    // CLEANUP
    // ============================================
    
    void OnDestroy()
    {
        // Asegurar que el tiempo se reanude
        Time.timeScale = 1f;
    }
}