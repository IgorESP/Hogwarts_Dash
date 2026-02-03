using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using System.Collections;

public class Pausamenucontroller : MonoBehaviour
{
    private VisualElement root;
    private VisualElement panelPausa;
    private bool juegoEnPausa = false;

    void Awake()
    {
        // FORZAR tiempo normal SIEMPRE
        Time.timeScale = 1f;
        juegoEnPausa = false;
    }

    void OnEnable()
    {
        StartCoroutine(OcultarMenuAlInicio());
    }

    IEnumerator OcultarMenuAlInicio()
    {
        var uiDoc = GetComponent<UIDocument>();
        yield return null; // Esperar 1 frame

        root = uiDoc.rootVisualElement;
        panelPausa = root.Q<VisualElement>("PanelPausa");

        // FORZAR ocultación SIEMPRE
        if (panelPausa != null)
        {
            panelPausa.style.display = DisplayStyle.None;
            panelPausa.visible = false;
        }

        ConfigurarBotones();
    }

    void Start()
    {
        // Segunda verificación en Start
        StartCoroutine(VerificarOcultacion());
    }

    IEnumerator VerificarOcultacion()
    {
        yield return new WaitForSeconds(0.1f);
        
        if (panelPausa != null)
        {
            panelPausa.style.display = DisplayStyle.None;
            panelPausa.visible = false;
            Debug.Log("[Pausa] Panel oculto en Start");
        }
    }

    private void ConfigurarBotones()
    {
        if (root == null) return;

        var botonPausaEsquina = root.Q<Button>("Boton_Pausa_Esquina");
        var botonPlay = root.Q<Button>("Boton_Play");
        var botonMenu = root.Q<Button>("Boton_Menu");
        var botonReiniciar = root.Q<Button>("Boton_Reiniciar");

        if (botonPausaEsquina != null) botonPausaEsquina.clicked += AlternarPausa;
        if (botonPlay != null) botonPlay.clicked += AlternarPausa;
        if (botonMenu != null) botonMenu.clicked += OnMenuClicked;
        if (botonReiniciar != null) botonReiniciar.clicked += OnReiniciarClicked;
        
        var sliderMusica = root.Q<Slider>("Slider_Musica");
        if (sliderMusica != null) sliderMusica.value = PlayerPrefs.GetFloat("VolumenMusica", 75f);
    }

    private void AlternarPausa()
    {
        if (panelPausa == null) return;

        juegoEnPausa = !juegoEnPausa;
        panelPausa.style.display = juegoEnPausa ? DisplayStyle.Flex : DisplayStyle.None;
        panelPausa.visible = juegoEnPausa;
        Time.timeScale = juegoEnPausa ? 0f : 1f;
    }

    private void OnMenuClicked()
    {
        // Resetear estado ANTES de salir
        Time.timeScale = 1f;
        juegoEnPausa = false;
        
        if (panelPausa != null)
        {
            panelPausa.style.display = DisplayStyle.None;
            panelPausa.visible = false;
        }
        
        SceneManager.LoadScene("MenuPrincipalT");
    }

    private void OnReiniciarClicked()
    {
        // Resetear estado ANTES de reiniciar
        Time.timeScale = 1f;
        juegoEnPausa = false;
        
        if (panelPausa != null)
        {
            panelPausa.style.display = DisplayStyle.None;
            panelPausa.visible = false;
        }
        
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void OnDisable()
    {
        // Resetear al desactivar el objeto
        Time.timeScale = 1f;
    }

    void OnDestroy()
    {
        // Resetear al destruir el objeto
        Time.timeScale = 1f;
    }
}