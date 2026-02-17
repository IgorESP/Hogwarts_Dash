using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// Gestiona el menú de pausa durante el juego.
/// Se encarga de mostrar/ocultar el panel, controlar el tiempo,
/// y manejar los botones de reanudar, reiniciar y volver al menú.
/// </summary>
public class Pausamenucontroller : MonoBehaviour
{
    // ── Referencias UI ──────────────────────────────────────────
    private VisualElement root;            // Raíz del documento UI
    private VisualElement panelPausa;      // Panel principal del menú de pausa
    private VisualElement barraRelleno;    // Barra de progreso del nivel
    private Label labelPorcentajeHUD;      // Texto del porcentaje de progreso

    // ── Estado ──────────────────────────────────────────────────
    private bool juegoEnPausa = false;     // true = pausado, false = jugando

    // ────────────────────────────────────────────────────────────
    // CICLO DE VIDA
    // ────────────────────────────────────────────────────────────

    /// <summary>
    /// Se ejecuta antes que Start. Resetea el tiempo y el estado de pausa
    /// para asegurarse de que el nivel empieza siempre corriendo.
    /// </summary>
    void Awake()
    {
        Time.timeScale = 1f;
        juegoEnPausa = false;
    }

    /// <summary>
    /// Se ejecuta cuando el objeto se activa.
    /// Lanza la corrutina de inicialización para esperar a que UI Toolkit esté listo.
    /// </summary>
    void OnEnable()
    {
        StartCoroutine(InicializarMenuPausa());
    }

    /// <summary>
    /// Espera un frame para que UI Toolkit cargue completamente
    /// y luego obtiene las referencias de los elementos UI.
    /// </summary>
    IEnumerator InicializarMenuPausa()
    {
        var uiDoc = GetComponent<UIDocument>();
        
        // Esperar un frame para que UI Toolkit esté listo
        yield return null;

        root = uiDoc.rootVisualElement;
        panelPausa = root.Q<VisualElement>("PanelPausa");

        // Ocultar el panel inmediatamente al iniciar el nivel
        // Esto evita que se vea el panel flotando al entrar o salir de una escena
        if (panelPausa != null)
        {
            panelPausa.style.display = DisplayStyle.None;
            panelPausa.visible = false;
            panelPausa.style.opacity = 0f; // Seguridad extra por si acaso
        }

        // Referencias del HUD de progreso
        barraRelleno       = root.Q<VisualElement>("BarraRelleno");
        labelPorcentajeHUD = root.Q<Label>("Label_PorcentajeHUD");

        // Configurar botones y sonidos
        ConfigurarBotones();
        AñadirSonidoABotones();
    }

    // ────────────────────────────────────────────────────────────
    // CONFIGURACIÓN DE BOTONES
    // ────────────────────────────────────────────────────────────

    /// <summary>
    /// Registra los eventos de clic de todos los botones del menú de pausa
    /// y sincroniza los sliders de volumen con los valores guardados.
    /// </summary>
    private void ConfigurarBotones()
    {
        if (root == null) return;

        // Botón de pausa en la esquina superior (abre el menú)
        root.Q<Button>("Boton_Pausa_Esquina")?.RegisterCallback<ClickEvent>(e => AlternarPausa());

        // Botón de reanudar dentro del menú de pausa (cierra el menú)
        root.Q<Button>("Boton_Play")?.RegisterCallback<ClickEvent>(e => AlternarPausa());

        // Botón para volver al menú principal
        root.Q<Button>("Boton_Menu")?.RegisterCallback<ClickEvent>(e => OnMenuClicked());

        // Botón para reiniciar el nivel desde el principio
        root.Q<Button>("Boton_Reiniciar")?.RegisterCallback<ClickEvent>(e => OnReiniciarClicked());

        // ── Slider de volumen de música ──────────────────────────
        var sliderMusica = root.Q<Slider>("Slider_Musica");
        if (sliderMusica != null)
        {
            // Cargar el valor guardado (por defecto 75%)
            sliderMusica.value = PlayerPrefs.GetFloat("VolumenMusica", 75f);

            sliderMusica.RegisterValueChangedCallback(evt =>
            {
                float vol = evt.newValue;
                PlayerPrefs.SetFloat("VolumenMusica", vol);
                PlayerPrefs.Save();
                AudioManager.Instance.SetVolumenMusica(vol / 100f);
            });
        }

        // ── Slider de volumen de efectos de sonido ───────────────
        var sliderSFX = root.Q<Slider>("Slider_SFX");
        if (sliderSFX != null)
        {
            // Cargar el valor guardado (por defecto 75%)
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

    // ────────────────────────────────────────────────────────────
    // LÓGICA DE PAUSA
    // ────────────────────────────────────────────────────────────

    /// <summary>
    /// Alterna entre estado pausado y estado de juego activo.
    /// Muestra u oculta el panel y detiene o reanuda el tiempo del motor.
    /// </summary>
    public void AlternarPausa()
    {
        if (panelPausa == null) return;

        juegoEnPausa = !juegoEnPausa;
        Debug.Log($"[Pausa] Estado: {(juegoEnPausa ? "PAUSA" : "JUGANDO")}");

        // Mostrar u ocultar el panel según el estado
        panelPausa.style.display  = juegoEnPausa ? DisplayStyle.Flex : DisplayStyle.None;
        panelPausa.visible        = juegoEnPausa;
        panelPausa.style.opacity  = juegoEnPausa ? 1f : 0f;

        // Detener o reanudar el tiempo del motor
        Time.timeScale = juegoEnPausa ? 0f : 1f;
    }

    // ────────────────────────────────────────────────────────────
    // ACCIONES DE BOTONES
    // ────────────────────────────────────────────────────────────

    /// <summary>
    /// Oculta el panel, guarda el progreso y vuelve al menú principal.
    /// </summary>
    private void OnMenuClicked()
    {
        Debug.Log("[Pausa] Volviendo al menú principal");

        // Ocultar el panel ANTES de cargar la escena
        // Evita que se vea el panel flotando al hacer la transición
        if (panelPausa != null)
        {
            panelPausa.style.display = DisplayStyle.None;
            panelPausa.visible       = false;
            panelPausa.style.opacity = 0f;
        }

        // Resetear el estado de pausa
        juegoEnPausa = false;

        // Registrar el intento fallido en el GameManager
        if (GameManager.Instance != null)
            GameManager.Instance.PlayerDied();

        // Parar la música antes de cambiar de escena
        if (AudioManager.Instance != null)
            AudioManager.Instance.musicaSource.Stop();

        // Restaurar el tiempo SIEMPRE antes de cambiar de escena
        Time.timeScale = 1f;
        SceneManager.LoadScene("MenuPrincipalT");
    }

    /// <summary>
    /// Reinicia la escena actual desde el principio.
    /// </summary>
    private void OnReiniciarClicked()
    {
        Debug.Log("[Pausa] Reiniciando nivel actual");

        // Ocultar el panel ANTES de reiniciar
        if (panelPausa != null)
        {
            panelPausa.style.display = DisplayStyle.None;
            panelPausa.visible       = false;
            panelPausa.style.opacity = 0f;
        }

        // Resetear el estado de pausa
        juegoEnPausa = false;

        // Parar la música antes de reiniciar
        if (AudioManager.Instance != null)
            AudioManager.Instance.musicaSource.Stop();

        // Restaurar el tiempo SIEMPRE antes de recargar la escena
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // ────────────────────────────────────────────────────────────
    // UPDATE
    // ────────────────────────────────────────────────────────────

    /// <summary>
    /// Se ejecuta cada frame. Actualiza el HUD de progreso
    /// y detecta la tecla de pausa (Escape o P).
    /// </summary>
    void Update()
    {
        // Actualizar barra de progreso siempre (esté pausado o no)
        ActualizarHUDProgreso();

        // Atajo de teclado para pausar/reanudar
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
        {
            AlternarPausa();
        }
    }

    /// <summary>
    /// Actualiza el ancho de la barra de progreso y el texto del porcentaje
    /// en función del progreso actual del GameManager.
    /// </summary>
    private void ActualizarHUDProgreso()
    {
        if (GameManager.Instance == null || barraRelleno == null || labelPorcentajeHUD == null) return;

        int porcentaje = GameManager.Instance.ObtenerPorcentajeActual();

        // Actualizar el ancho de la barra (de 0% a 100%)
        barraRelleno.style.width = new Length(porcentaje, LengthUnit.Percent);

        // Actualizar el texto del porcentaje
        labelPorcentajeHUD.text = $"{porcentaje}%";
    }

    // ────────────────────────────────────────────────────────────
    // SEGURIDAD
    // ────────────────────────────────────────────────────────────

    /// <summary>
    /// Seguridad: si el objeto se desactiva, restaurar el tiempo
    /// para que el juego no quede congelado.
    /// </summary>
    void OnDisable() { Time.timeScale = 1f; }

    /// <summary>
    /// Seguridad: si el objeto se destruye, restaurar el tiempo
    /// para que el juego no quede congelado.
    /// </summary>
    void OnDestroy() { Time.timeScale = 1f; }

    // ────────────────────────────────────────────────────────────
    // SONIDO
    // ────────────────────────────────────────────────────────────

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