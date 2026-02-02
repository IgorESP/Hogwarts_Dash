using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class Pausamenucontroller : MonoBehaviour
{
    private VisualElement root;
    private VisualElement panelPausa;
    
    // Botones
    private Button botonPausaEsquina;
    private Button botonPlay;
    private Button botonMenu;
    private Button botonReiniciar;
    
    // Sliders
    private Slider sliderMusica;
    private Slider sliderSFX;
    
    private bool juegoEnPausa = false;

    void Start() 
    {
        var uiDoc = GetComponent<UIDocument>();
        root = uiDoc.rootVisualElement;

        // Capturar elementos
        panelPausa = root.Q<VisualElement>("PanelPausa");
        botonPausaEsquina = root.Q<Button>("Boton_Pausa_Esquina");
        botonPlay = root.Q<Button>("Boton_Play");
        botonMenu = root.Q<Button>("Boton_Menu");
        botonReiniciar = root.Q<Button>("Boton_Reiniciar");
        
        sliderMusica = root.Q<Slider>("Slider_Musica");
        sliderSFX = root.Q<Slider>("Slider_SFX");

        // Registrar eventos
        if (botonPausaEsquina != null) botonPausaEsquina.clicked += MostrarPausa;
        if (botonPlay != null) botonPlay.clicked += ReanudarJuego;
        if (botonMenu != null) botonMenu.clicked += VolverAlMenu;
        if (botonReiniciar != null) botonReiniciar.clicked += ReiniciarNivel;
        
        if (sliderMusica != null) sliderMusica.RegisterValueChangedCallback(OnMusicaChanged);
        if (sliderSFX != null) sliderSFX.RegisterValueChangedCallback(OnSFXChanged);

        // Cargar valores de audio
        CargarConfiguracionAudio();
        
        // 🔥 IMPORTANTE: Asegurar que el botón de pausa esté visible
        MostrarBotonPausa();
        
        // 🔥 IMPORTANTE: Ocultar el panel de pausa al inicio
        OcultarPanelPausa();
    }

    void Update() 
    {
        // También se puede pausar con ESC
        if (Input.GetKeyDown(KeyCode.Escape)) 
        {
            if (juegoEnPausa) ReanudarJuego();
            else MostrarPausa();
        }
    }

    // 🔥 NUEVO: Mostrar botón de pausa
    private void MostrarBotonPausa()
    {
        if (botonPausaEsquina != null)
        {
            botonPausaEsquina.style.display = DisplayStyle.Flex;
            botonPausaEsquina.style.visibility = Visibility.Visible;
            Debug.Log("✅ Botón de pausa visible");
        }
        else
        {
            Debug.LogError("❌ No se encontró Boton_Pausa_Esquina");
        }
    }
    
    // 🔥 NUEVO: Ocultar panel de pausa
    private void OcultarPanelPausa()
    {
        if (panelPausa != null)
        {
            panelPausa.style.display = DisplayStyle.None;
            Debug.Log("✅ Panel de pausa oculto");
        }
    }

    private void MostrarPausa() 
    {
        juegoEnPausa = true;
        if (panelPausa != null) panelPausa.style.display = DisplayStyle.Flex;
        Time.timeScale = 0f;
        Debug.Log("⏸️ Juego pausado");
    }

    private void ReanudarJuego() 
    {
        juegoEnPausa = false;
        if (panelPausa != null) panelPausa.style.display = DisplayStyle.None;
        Time.timeScale = 1f;
        Debug.Log("▶️ Juego reanudado");
    }

    private void VolverAlMenu() 
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MenulPrincipalT");
        Debug.Log("🏠 Volviendo al menú principal");
    }

    private void ReiniciarNivel() 
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Debug.Log("🔄 Reiniciando nivel");
    }

    // ====================================
    // AUDIO
    // ====================================
    private void OnMusicaChanged(ChangeEvent<float> evt) 
    {
        float vol = evt.newValue;
        PlayerPrefs.SetFloat("VolumenMusica", vol);
        if (AudioManager.Instance != null) 
        {
            AudioManager.Instance.SetVolumenMusica(vol / 100f);
        }
    }

    private void OnSFXChanged(ChangeEvent<float> evt) 
    {
        float vol = evt.newValue;
        PlayerPrefs.SetFloat("VolumenSFX", vol);
        if (AudioManager.Instance != null) 
        {
            AudioManager.Instance.SetVolumenSFX(vol / 100f);
        }
    }

    private void CargarConfiguracionAudio() 
    {
        float vMus = PlayerPrefs.GetFloat("VolumenMusica", 75f);
        if (sliderMusica != null) sliderMusica.value = vMus;

        float vSFX = PlayerPrefs.GetFloat("VolumenSFX", 75f);
        if (sliderSFX != null) sliderSFX.value = vSFX;
    }
}