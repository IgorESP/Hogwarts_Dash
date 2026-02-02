using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

/// <summary>
/// Controlador del menu de pausa.
/// Maneja pausa, reanudacion, reinicio, volver al menu y controles de audio.
/// </summary>
public class Pausamenucontroller : MonoBehaviour
{
    private VisualElement root;
    private VisualElement panelPausa;
    
    // Botones
    private Button botonPausaEsquina;
    private Button botonPlay;
    private Button botonMenu;
    private Button botonReiniciar;
    
    // Sliders de audio
    private Slider sliderMusica;
    private Slider sliderSFX;
    
    // Estado de pausa
    private bool juegoEnPausa = false;

    void Start() 
    {
        var uiDoc = GetComponent<UIDocument>();
        root = uiDoc.rootVisualElement;

        // Capturar elementos de la UI
        panelPausa = root.Q<VisualElement>("PanelPausa");
        botonPausaEsquina = root.Q<Button>("Boton_Pausa_Esquina");
        botonPlay = root.Q<Button>("Boton_Play");
        botonMenu = root.Q<Button>("Boton_Menu");
        botonReiniciar = root.Q<Button>("Boton_Reiniciar");
        
        sliderMusica = root.Q<Slider>("Slider_Musica");
        sliderSFX = root.Q<Slider>("Slider_SFX");

        // Registrar eventos de botones
        if (botonPausaEsquina != null) botonPausaEsquina.clicked += MostrarPausa;
        if (botonPlay != null) botonPlay.clicked += ReanudarJuego;
        if (botonMenu != null) botonMenu.clicked += VolverAlMenu;
        if (botonReiniciar != null) botonReiniciar.clicked += ReiniciarNivel;
        
        // Registrar eventos de sliders
        if (sliderMusica != null) sliderMusica.RegisterValueChangedCallback(OnMusicaChanged);
        if (sliderSFX != null) sliderSFX.RegisterValueChangedCallback(OnSFXChanged);

        // Cargar valores guardados de audio
        CargarConfiguracionAudio();
        
        // Asegurar que el boton de pausa este visible
        MostrarBotonPausa();
        
        // Ocultar el panel de pausa al inicio
        OcultarPanelPausa();
    }

    void Update() 
    {
        // Pausar/Reanudar con tecla ESC
        if (Input.GetKeyDown(KeyCode.Escape)) 
        {
            if (juegoEnPausa) 
                ReanudarJuego();
            else 
                MostrarPausa();
        }
    }

    // ====================================
    // MOSTRAR/OCULTAR ELEMENTOS
    // ====================================
    
    /// <summary>
    /// Mostrar el boton de pausa en la esquina superior derecha.
    /// </summary>
    private void MostrarBotonPausa()
    {
        if (botonPausaEsquina != null)
        {
            botonPausaEsquina.style.display = DisplayStyle.Flex;
            botonPausaEsquina.style.visibility = Visibility.Visible;
            Debug.Log("[Pausa] Boton de pausa visible");
        }
        else
        {
            Debug.LogError("[Pausa] No se encontro Boton_Pausa_Esquina");
        }
    }
    
    /// <summary>
    /// Ocultar el panel de pausa al inicio.
    /// </summary>
    private void OcultarPanelPausa()
    {
        if (panelPausa != null)
        {
            panelPausa.style.display = DisplayStyle.None;
            Debug.Log("[Pausa] Panel de pausa oculto");
        }
    }

    // ====================================
    // FUNCIONES DE PAUSA
    // ====================================
    
    /// <summary>
    /// Mostrar el menu de pausa y congelar el juego.
    /// </summary>
    private void MostrarPausa() 
    {
        juegoEnPausa = true;
        
        // Mostrar panel de pausa
        if (panelPausa != null) 
            panelPausa.style.display = DisplayStyle.Flex;
        
        // Congelar el tiempo del juego
        Time.timeScale = 0f;
        
        Debug.Log("[Pausa] Juego pausado");
    }

    /// <summary>
    /// Reanudar el juego y ocultar el menu de pausa.
    /// </summary>
    private void ReanudarJuego() 
    {
        juegoEnPausa = false;
        
        // Ocultar panel de pausa
        if (panelPausa != null) 
            panelPausa.style.display = DisplayStyle.None;
        
        // Reanudar el tiempo del juego
        Time.timeScale = 1f;
        
        Debug.Log("[Pausa] Juego reanudado");
    }

    /// <summary>
    /// Volver al menu principal (PlayScene -> MenuPrincipalT).
    /// </summary>
    private void VolverAlMenu() 
    {
        // Reanudar tiempo antes de cambiar de escena
        Time.timeScale = 1f;
        
        Debug.Log("[Pausa] Volviendo al menu principal");
        
        // Cargar el menu principal
        SceneManager.LoadScene("MenuPrincipalT");
    }

    /// <summary>
    /// Reiniciar el nivel actual.
    /// </summary>
    private void ReiniciarNivel() 
    {
        // Ocultar el panel de pausa
        juegoEnPausa = false;
        if (panelPausa != null) 
            panelPausa.style.display = DisplayStyle.None;
        
        // Reanudar tiempo antes de reiniciar
        Time.timeScale = 1f;
        
        Debug.Log("[Pausa] Reiniciando nivel");
        
        // Reiniciar estadisticas del GameManager si existe
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ReiniciarEstadisticas();
        }
        
        // Recargar la escena actual
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // ====================================
    // CONTROLES DE AUDIO
    // ====================================
    
    /// <summary>
    /// Callback cuando cambia el slider de musica.
    /// </summary>
    private void OnMusicaChanged(ChangeEvent<float> evt) 
    {
        float volumen = evt.newValue;
        
        // Guardar en PlayerPrefs
        PlayerPrefs.SetFloat("VolumenMusica", volumen);
        PlayerPrefs.Save();
        
        // Aplicar al AudioManager si existe
        if (AudioManager.Instance != null) 
        {
            AudioManager.Instance.SetVolumenMusica(volumen / 100f);
        }
        
        Debug.Log($"[Pausa] Volumen musica: {volumen}%");
    }

    /// <summary>
    /// Callback cuando cambia el slider de SFX.
    /// </summary>
    private void OnSFXChanged(ChangeEvent<float> evt) 
    {
        float volumen = evt.newValue;
        
        // Guardar en PlayerPrefs
        PlayerPrefs.SetFloat("VolumenSFX", volumen);
        PlayerPrefs.Save();
        
        // Aplicar al AudioManager si existe
        if (AudioManager.Instance != null) 
        {
            AudioManager.Instance.SetVolumenSFX(volumen / 100f);
        }
        
        Debug.Log($"[Pausa] Volumen SFX: {volumen}%");
    }

    /// <summary>
    /// Cargar la configuracion de audio guardada.
    /// </summary>
    private void CargarConfiguracionAudio() 
    {
        // Cargar volumen de musica (default 75%)
        float volumenMusica = PlayerPrefs.GetFloat("VolumenMusica", 75f);
        if (sliderMusica != null) 
        {
            sliderMusica.value = volumenMusica;
        }

        // Cargar volumen de SFX (default 75%)
        float volumenSFX = PlayerPrefs.GetFloat("VolumenSFX", 75f);
        if (sliderSFX != null) 
        {
            sliderSFX.value = volumenSFX;
        }
        
        Debug.Log($"[Pausa] Audio cargado - Musica: {volumenMusica}%, SFX: {volumenSFX}%");
    }
    
    // ====================================
    // CLEANUP
    // ====================================
    
    void OnDestroy()
    {
        // Asegurar que el tiempo se reanude al destruir el objeto
        Time.timeScale = 1f;
    }
}