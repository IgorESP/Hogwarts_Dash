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
        // Inicializar UI tras un frame para asegurar que UI Toolkit está listo
        StartCoroutine(InicializarMenuPausa());
    }

    /// <summary> 
    /// Espera un frame para que UI Toolkit cargue y luego obtiene referencias. 
    /// </summary>
    IEnumerator InicializarMenuPausa()
    {
        var uiDoc = GetComponent<UIDocument>();
        yield return null;

        root = uiDoc.rootVisualElement;
        panelPausa = root.Q<VisualElement>("PanelPausa");

        // Ocultar por defecto al iniciar el nivel
        if (panelPausa != null)
        {
            panelPausa.style.display = DisplayStyle.None;
            panelPausa.visible = false;
        }

        ConfigurarBotones();

        AñadirSonidoABotones();
    }

    /// <summary> 
    /// Registra los eventos de los botones del menú de pausa. 
    /// </summary>
    private void ConfigurarBotones()
    {
        if (root == null) return;

        // Botón de pausa en la esquina
        root.Q<Button>("Boton_Pausa_Esquina")?.RegisterCallback<ClickEvent>(e => AlternarPausa());

        // Botón de reanudar dentro del menú
        root.Q<Button>("Boton_Play")?.RegisterCallback<ClickEvent>(e => AlternarPausa());

        // Botón para volver al menú principal
        root.Q<Button>("Boton_Menu")?.RegisterCallback<ClickEvent>(e => OnMenuClicked());

        // Botón para reiniciar el nivel
        root.Q<Button>("Boton_Reiniciar")?.RegisterCallback<ClickEvent>(e => OnReiniciarClicked());
        
        // Sincronizar slider de música con el volumen actual
        var sliderMusica = root.Q<Slider>("Slider_Musica");
        if (sliderMusica != null)
        {
            sliderMusica.value = PlayerPrefs.GetFloat("VolumenMusica", 75f);

            sliderMusica.RegisterValueChangedCallback(evt =>
            {
                float vol = evt.newValue;
                PlayerPrefs.SetFloat("VolumenMusica", vol);
                PlayerPrefs.Save();

                AudioManager.Instance.SetVolumenMusica(vol / 100f);
            });
        }

        var sliderSFX = root.Q<Slider>("Slider_SFX");
        if (sliderSFX != null)
        {
            sliderSFX.value = PlayerPrefs.GetFloat("VolumenSFX", 75f);

            sliderSFX.RegisterValueChangedCallback(evt =>
            {
                float vol = evt.newValue;
                PlayerPrefs.SetFloat("VolumenSFX", vol);
                PlayerPrefs.Save();

                AudioManager.Instance.SetVolumenSFX(vol / 100f);
            });
        }
    }

    /// <summary>
    /// Cambia entre estado de juego activo y pausado
    /// </summary>
    public void AlternarPausa()
    {
        if (panelPausa == null) return;

        juegoEnPausa = !juegoEnPausa;
        Debug.Log($"[Pausa] Estado: {(juegoEnPausa ? "PAUSA" : "JUGANDO")}");
        
        // Mostrar u ocultar el panel
        panelPausa.style.display = juegoEnPausa ? DisplayStyle.Flex : DisplayStyle.None;
        panelPausa.visible = juegoEnPausa;
        
        // Detener o reanudar el tiempo del motor
        Time.timeScale = juegoEnPausa ? 0f : 1f;
    }

    /// <summary> 
    /// Vuelve al menú principal. 
    /// </summary>
    private void OnMenuClicked()
    {
        Debug.Log("[Pausa] Volviendo al menú principal");
        
        AudioManager.Instance.musicaSource.Stop();

        Time.timeScale = 1f;
        SceneManager.LoadScene("MenuPrincipalT");
    }

    /// <summary> 
    /// Reinicia la escena actual. 
    /// </summary>
    private void OnReiniciarClicked()
    {
        Debug.Log("[Pausa] Reiniciando nivel actual");
        Time.timeScale = 1f;
    
        AudioManager.Instance.musicaSource.Stop();

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Seguridad adicional: si el objeto se desactiva o destruye, restaurar el tiempo
    void OnDisable() { Time.timeScale = 1f; }
    void OnDestroy() { Time.timeScale = 1f; }

    /// <summary> 
    /// Añade sonido de clic a todos los botones del menú de pausa. 
    /// </summary>
    private void AñadirSonidoABotones()
    {
        if (root == null) return;

        var botones = root.Query<Button>().ToList();

        foreach (var b in botones)
        {
            if (b == null) continue;

            b.clicked += () => AudioManager.Instance.PlayBoton();
        }
    }
}