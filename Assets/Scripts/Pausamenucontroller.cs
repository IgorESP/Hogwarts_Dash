using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// Gestiona el menú de pausa durante el juego
/// </summary>
public class Pausamenucontroller : MonoBehaviour
{
    private VisualElement root;
    private VisualElement panelPausa;
    private bool juegoEnPausa = false;

    void Awake()
    {
        // Resetear estado del tiempo al entrar en la escena
        Time.timeScale = 1f;
        juegoEnPausa = false;
    }

    void OnEnable()
    {
        StartCoroutine(InicializarMenuPausa());
    }

    IEnumerator InicializarMenuPausa()
    {
        var uiDoc = GetComponent<UIDocument>();
        yield return null; // Dar tiempo al motor de UI

        root = uiDoc.rootVisualElement;
        panelPausa = root.Q<VisualElement>("PanelPausa");

        // Ocultar por defecto al iniciar el nivel
        if (panelPausa != null)
        {
            panelPausa.style.display = DisplayStyle.None;
            panelPausa.visible = false;
        }

        ConfigurarBotones();
    }

    private void ConfigurarBotones()
    {
        if (root == null) return;

        // Botones de interfaz
        root.Q<Button>("Boton_Pausa_Esquina")?.RegisterCallback<ClickEvent>(e => AlternarPausa());
        root.Q<Button>("Boton_Play")?.RegisterCallback<ClickEvent>(e => AlternarPausa());
        root.Q<Button>("Boton_Menu")?.RegisterCallback<ClickEvent>(e => OnMenuClicked());
        root.Q<Button>("Boton_Reiniciar")?.RegisterCallback<ClickEvent>(e => OnReiniciarClicked());
        
        // Cargar volumen de música actual en el slider de pausa
        var sliderMusica = root.Q<Slider>("Slider_Musica");
        if (sliderMusica != null) sliderMusica.value = PlayerPrefs.GetFloat("VolumenMusica", 75f);
    }

    /// <summary>
    /// Cambia entre estado de juego activo y pausado
    /// </summary>
    public void AlternarPausa()
    {
        if (panelPausa == null) return;

        juegoEnPausa = !juegoEnPausa;
        Debug.Log($"[Pausa] Estado: {(juegoEnPausa ? "PAUSA" : "JUGANDO")}");
        
        panelPausa.style.display = juegoEnPausa ? DisplayStyle.Flex : DisplayStyle.None;
        panelPausa.visible = juegoEnPausa;
        
        // Detener o reanudar el tiempo del motor
        Time.timeScale = juegoEnPausa ? 0f : 1f;
    }

    private void OnMenuClicked()
    {
        Debug.Log("[Pausa] Volviendo al menú principal");
        Time.timeScale = 1f;
        SceneManager.LoadScene("MenuPrincipalT");
    }

    private void OnReiniciarClicked()
    {
        Debug.Log("[Pausa] Reiniciando nivel actual");
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void OnDisable() { Time.timeScale = 1f; }
    void OnDestroy() { Time.timeScale = 1f; }
}