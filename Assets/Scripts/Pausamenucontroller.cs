using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class PausaMenuController : MonoBehaviour
{
    private VisualElement panelPausa;
    private Button botonPlay, botonSalir, botonReiniciar, botonPausaEsquina;
    private Slider sliderMusica, sliderSFX;
    private bool estaPausado = false;

    void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        
        // --- APLICAR ACCESIBILIDAD ---
        if (NivelesManager.Instance != null) NivelesManager.Instance.SincronizarAccesibilidad(root);

        panelPausa = root.Q<VisualElement>("PanelPausa");
        botonPlay = root.Q<Button>("Boton_Play");
        botonSalir = root.Q<Button>("Boton_Info");
        botonReiniciar = root.Q<Button>("Boton_Reiniciar");
        botonPausaEsquina = root.Q<Button>("Boton_Pausa_Esquina");
        sliderMusica = root.Q<Slider>("Slider_Musica");
        sliderSFX = root.Q<Slider>("Slider_SFX");

        botonPlay.clicked += Reanudar;
        botonSalir.clicked += VolverAlMenu;
        botonReiniciar.clicked += ReiniciarNivel;
        botonPausaEsquina.clicked += Pausar;

        sliderMusica.RegisterValueChangedCallback(evt => ActualizarVolumen("VolumenMusica", evt.newValue));
        sliderSFX.RegisterValueChangedCallback(evt => ActualizarVolumen("VolumenSFX", evt.newValue));

        sliderMusica.value = PlayerPrefs.GetFloat("VolumenMusica", 80f);
        sliderSFX.value = PlayerPrefs.GetFloat("VolumenSFX", 80f);

        panelPausa.style.display = DisplayStyle.None;
    }

    void Update() { if (Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.Escape)) { if (estaPausado) Reanudar(); else Pausar(); } }
    private void Pausar() { estaPausado = true; Time.timeScale = 0f; panelPausa.style.display = DisplayStyle.Flex; botonPausaEsquina.style.display = DisplayStyle.None; }
    private void Reanudar() { estaPausado = false; Time.timeScale = 1f; panelPausa.style.display = DisplayStyle.None; botonPausaEsquina.style.display = DisplayStyle.Flex; }
    private void ReiniciarNivel() { Time.timeScale = 1f; SceneManager.LoadScene(SceneManager.GetActiveScene().name); }
    private void VolverAlMenu() { Time.timeScale = 1f; SceneManager.LoadScene("MenuPrincipalT"); }
    private void ActualizarVolumen(string clave, float valor) { PlayerPrefs.SetFloat(clave, valor); PlayerPrefs.Save(); }
}