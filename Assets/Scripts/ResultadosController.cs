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
        
        // Registrar tiempo localmente
        float tiempoSesion = (GameManager.Instance != null) ? Time.timeSinceLevelLoad : 0f;
        if (NivelesManager.Instance != null) 
            NivelesManager.Instance.RegistrarTiempoNivel(PlayerPrefs.GetInt("MundoSeleccionado", 1), 1, tiempoSesion);

        // Enviar a la base de datos si hay sesión
        StartCoroutine(EnviarResultadosAPI(porcentaje, 1, snitches));
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
        if (AudioManager.Instance != null)
            AudioManager.Instance.musicaSource.Stop();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); 
    }

    /// <summary> 
    /// Vuelve al menú principal de selección de niveles. 
    /// </summary>
    private void VolverAlMenu() 
    { 
        Time.timeScale = 1f; 
        if (AudioManager.Instance != null)
            AudioManager.Instance.musicaSource.Stop();
        SceneManager.LoadScene("PlayScene"); 
    }
    
    /// <summary>
    /// Sincroniza el progreso del usuario con la API de Hogwarts Dash
    /// </summary>
    private IEnumerator EnviarResultadosAPI(int porcentaje, int intentos, int snitches)
    {
        string playerId = PlayerPrefs.GetString("player_id", "");
        if (string.IsNullOrEmpty(playerId)) 
        {
            Debug.LogWarning("[API] Sin player_id logueado, no se sincronizará el progreso.");
            yield break;
        }

        // Obtener el nombre del nivel correctamente (coincidiendo con GameManager y la DB)
        int mundoNum = PlayerPrefs.GetInt("MundoSeleccionado", 1);
        string nivel = mundoNum switch
        {
            1 => "comedor",
            2 => "gryffindor",
            3 => "slytherin",
            4 => "hufflepuff",
            5 => "ravenclaw",
            _ => "comedor"
        };

        float tiempoJugado = (GameManager.Instance != null) ? Time.timeSinceLevelLoad : 0f;

        Debug.Log($"[API] Enviando victoria: Nivel={nivel}, %={porcentaje}, Snitches={snitches}");

        // Enviamos 1 intento para que el backend lo sume o actualice según su lógica
        ApiManager.Instance.ActualizarProgresoNivel(
            playerId, nivel, porcentaje, snitches, 1, tiempoJugado,
            (exito, mensaje) => {
                if (exito) Debug.Log($"[API] Progreso guardado exitosamente para {nivel}");
                else Debug.LogWarning($"[API] Error al guardar progreso: {mensaje}");
            }
        );

        yield return null;
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