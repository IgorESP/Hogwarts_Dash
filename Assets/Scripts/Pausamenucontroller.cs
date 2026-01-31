using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class PausaMenuController : MonoBehaviour
{
    private VisualElement panelPausa;
    private Button botonPlay;
    private Button botonSalir;
    private Button botonReiniciar;
    private Button botonPausaEsquina;
    private Slider sliderMusica;
    private Slider sliderSFX;
    private bool estaPausado = false;

    void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        // Obtener referencias
        panelPausa = root.Q<VisualElement>("PanelPausa");
        botonPlay = root.Q<Button>("Boton_Play");
        botonSalir = root.Q<Button>("Boton_Info"); // ⚠️ Usa el nombre que tengas en UXML
        botonReiniciar = root.Q<Button>("Boton_Reiniciar");
        botonPausaEsquina = root.Q<Button>("Boton_Pausa_Esquina");
        sliderMusica = root.Q<Slider>("Slider_Musica");
        sliderSFX = root.Q<Slider>("Slider_SFX");

        // Verificar que todo se encontró
        if (panelPausa == null) Debug.LogError("No se encontró PanelPausa");
        if (botonPlay == null) Debug.LogError("No se encontró Boton_Play");
        if (botonSalir == null) Debug.LogError("No se encontró Boton_Info/Boton_Salir");
        if (botonReiniciar == null) Debug.LogError("No se encontró Boton_Reiniciar");
        if (botonPausaEsquina == null) Debug.LogError("No se encontró Boton_Pausa_Esquina");

        // Eventos de botones
        botonPlay.clicked += Reanudar;
        botonSalir.clicked += VolverAlMenu;
        botonReiniciar.clicked += ReiniciarNivel;
        botonPausaEsquina.clicked += Pausar;

        // Eventos de sliders
        sliderMusica.RegisterValueChangedCallback(evt => ActualizarVolumen("VolumenMusica", evt.newValue));
        sliderSFX.RegisterValueChangedCallback(evt => ActualizarVolumen("VolumenSFX", evt.newValue));

        // Cargar valores guardados
        sliderMusica.value = PlayerPrefs.GetFloat("VolumenMusica", 80f);
        sliderSFX.value = PlayerPrefs.GetFloat("VolumenSFX", 80f);

        // Iniciar con menú oculto
        panelPausa.style.display = DisplayStyle.None;
        
        Debug.Log("PausaMenuController iniciado correctamente");
    }

    void Update()
    {
        // Presionar P o ESC para pausar/reanudar
        if (Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.Escape))
        {
            if (estaPausado)
                Reanudar();
            else
                Pausar();
        }
    }

    private void Pausar()
    {
        Debug.Log(" Juego pausado");
        estaPausado = true;
        Time.timeScale = 0f;
        panelPausa.style.display = DisplayStyle.Flex;
        botonPausaEsquina.style.display = DisplayStyle.None;
    }

    private void Reanudar()
    {
        Debug.Log("Juego reanudado");
        estaPausado = false;
        Time.timeScale = 1f;
        panelPausa.style.display = DisplayStyle.None;
        botonPausaEsquina.style.display = DisplayStyle.Flex;
    }

    private void ReiniciarNivel()
    {
        Debug.Log("Reiniciando nivel...");
        Time.timeScale = 1f;
        estaPausado = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void VolverAlMenu()
    {
        Debug.Log(" Volviendo al menú principal...");
        Time.timeScale = 1f;
        estaPausado = false;
         SceneManager.LoadScene("MenuPrincipalT"); // ⚠️ Cambia por el nombre de tu menú
    }

  private void ActualizarVolumen(string clave, float valor)
{
    PlayerPrefs.SetFloat(clave, valor);
    PlayerPrefs.Save();
    
    // Aplicar volumen en tiempo real
    if (AudioManager.Instance != null)
    {
        if (clave == "VolumenMusica")
        {
            AudioManager.Instance.SetVolumenMusica(valor / 100f);
        }
        else if (clave == "VolumenSFX")
        {
            AudioManager.Instance.SetVolumenSFX(valor / 100f);
        }
    }
    
    Debug.Log($"🎵 {clave} = {valor}");
}
}