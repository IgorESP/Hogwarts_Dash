using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// Controlador del panel de resultados final al completar un nivel
/// </summary>
public class ResultadosController : MonoBehaviour
{
    private VisualElement root, panelResultados, contenedorResultados;
    private Label titulo, labelPorcentaje, labelIntentos, labelSnitches;
    private Button botonReintentar, botonVolver;
    
    void Start()
    {
        var uiDoc = GetComponent<UIDocument>();
        if (uiDoc == null) return;
        root = uiDoc.rootVisualElement;
        
        // Panel principal y contenedor animado
        panelResultados = root.Q<VisualElement>("PanelResultados");
        contenedorResultados = root.Q<VisualElement>("ContenedorResultados");
        
        // Labels de estadísticas
        titulo = root.Q<Label>("Titulo");
        labelPorcentaje = root.Q<Label>("Label_Porcentaje");
        labelIntentos = root.Q<Label>("Label_Intentos");
        labelSnitches = root.Q<Label>("Label_Snitches");
        
        // Botones de acción
        botonReintentar = root.Q<Button>("Boton_Reintentar");
        botonVolver = root.Q<Button>("Boton_Volver");
        
        // Registrar eventos de los botones
        if (botonReintentar != null) botonReintentar.clicked += Reintentar;
        if (botonVolver != null) botonVolver.clicked += VolverAlMenu;
        
        // Ocultar el panel al iniciar la escena
        OcultarPanel();

        // Añadir sonido a todos los botones del panel
        AñadirSonidoABotones();
    }
    
    /// <summary>
    /// Muestra el panel con las estadísticas finales y pausa el juego
    /// </summary>
    public void MostrarResultados(int porcentaje, int intentos, int snitches, bool victoria = true)
    {
        Debug.Log($"[Resultados] Nivel completado al {porcentaje}%");
        Time.timeScale = 0f; // Pausar físicas y lógica de juego
        
        if (titulo != null) titulo.text = "¡NIVEL COMPLETADO!";
        if (labelPorcentaje != null) labelPorcentaje.text = $"{porcentaje}%";
        if (labelIntentos != null) labelIntentos.text = intentos.ToString();
        if (labelSnitches != null) labelSnitches.text = snitches.ToString();
        
        MostrarPanel();
        
        // Enviar a la base de datos si hay sesión
        StartCoroutine(EnviarResultadosAPI(porcentaje, intentos, snitches));
    }
    
    /// <summary> 
    /// Muestra el panel y ejecuta una animación de escala suave. 
    /// </summary>
    private void MostrarPanel()
    {
        if (panelResultados != null) panelResultados.style.display = DisplayStyle.Flex;
        if (contenedorResultados != null)
        {
            // Animación sencilla de escalado
            contenedorResultados.style.scale = new Scale(new Vector3(0.8f, 0.8f, 1f));
            contenedorResultados.schedule.Execute(() => contenedorResultados.style.scale = new Scale(Vector3.one)).StartingIn(50);
        }
    }
    
    /// <summary> 
    /// Oculta el panel de resultados. 
    /// </summary>
    private void OcultarPanel() 
    { 
        if (panelResultados != null) 
            panelResultados.style.display = DisplayStyle.None; 
    }
    
    /// <summary> 
    /// Reinicia el nivel actual. 
    /// </summary>
    private void Reintentar() 
    { 
        Time.timeScale = 1f; 
        AudioManager.Instance.musicaSource.Stop();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); 
    }

    /// <summary> 
    /// Vuelve al menú principal de selección de niveles. 
    /// </summary>
    private void VolverAlMenu() 
    { 
        Time.timeScale = 1f; 
        AudioManager.Instance.musicaSource.Stop();
        SceneManager.LoadScene("PlayScene"); 
    }
    
    /// <summary>
    /// Sincroniza el progreso del usuario con la API de Hogwarts Dash
    /// </summary>
    private IEnumerator EnviarResultadosAPI(int p, int i, int s)
    {
        string pid = PlayerPrefs.GetString("player_id", "");
        if (string.IsNullOrEmpty(pid)) yield break;
        
        int mundo = PlayerPrefs.GetInt("MundoSeleccionado", 1);
        string url = $"https://hogwartsdashapi.onrender.com/api/v1/jugadores/{pid}/progreso";
        
        // Calcular estrellas según porcentaje
        int estreq = p >= 90 ? 3 : p >= 70 ? 2 : p >= 40 ? 1 : 0;
        
        string json = $"{{\"player_id\":\"{pid}\",\"mundo\":{mundo},\"nivel\":1,\"porcentaje\":{p},\"intentos\":{i},\"snitches\":{s},\"estrellas\":{estreq}}}";
        
        using (var req = new UnityEngine.Networking.UnityWebRequest(url, "POST"))
        {
            byte[] body = System.Text.Encoding.UTF8.GetBytes(json);
            req.uploadHandler = new UnityEngine.Networking.UploadHandlerRaw(body);
            req.downloadHandler = new UnityEngine.Networking.DownloadHandlerBuffer();
            req.SetRequestHeader("Content-Type", "application/json");
            
            string ak = PlayerPrefs.GetString("ApiKey", "hogwarts_dash_2026_admin");
            req.SetRequestHeader("X-API-Key", ak);
            
            yield return req.SendWebRequest();
            if (req.result == UnityEngine.Networking.UnityWebRequest.Result.Success) Debug.Log("[Resultados] API actualizada correctamente");
            else Debug.LogWarning("[Resultados] Fallo al actualizar API");
        }
    }
    
    /// <summary> 
    /// Si el objeto se destruye, reanudar el tiempo. 
    /// </summary>
    void OnDestroy() 
    { 
        Time.timeScale = 1f; 
    }

    /// <summary> 
    /// Añade sonido de clic a todos los botones del panel de resultados. 
    /// </summary>
    private void AñadirSonidoABotones()
    {
        if (root == null) return;

        var botones = root.Query<Button>().ToList();

        foreach (var b in botones)
        {
            b.clicked += () => AudioManager.Instance.PlayBoton();
        }
    }
}