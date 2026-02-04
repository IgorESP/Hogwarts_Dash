using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

/// <summary>
/// Controlador de la interfaz de ajustes (Opciones)
/// </summary>
public class AjustesController : MonoBehaviour
{
    private VisualElement root;
    
    // Referencias a elementos de UI
    private Slider sliderMusica, sliderSFX, sliderTamanoLetra;
    private Label labelMusica, labelSFX, labelTamanoLetra, mensajeVacio;
    private Button botonRanking, botonAtras, botonCerrarSesion, botonCreditos, botonTutorial, botonManual, botonCerrarRanking, botonCerrarCreditos;
    private VisualElement panelRankingOverlay, panelCreditosOverlay;
    private ScrollView listaRanking;

    void Start() 
    {
        Debug.Log("[Ajustes] Iniciando controlador de ajustes");
        var uiDoc = GetComponent<UIDocument>();
        if (uiDoc != null) root = uiDoc.rootVisualElement;

        ObtenerReferencias();
        RegistrarEventos();
        CargarConfiguracion();
    }

    private void ObtenerReferencias()
    {
        if (root == null) return;

        // Audio y Accesibilidad
        sliderMusica = root.Q<Slider>("Slider_Musica");
        sliderSFX = root.Q<Slider>("Slider_SFX");
        labelMusica = root.Q<Label>("Label_Musica");
        labelSFX = root.Q<Label>("Label_SFX");
        sliderTamanoLetra = root.Q<Slider>("Slider_TamanoLetra");
        labelTamanoLetra = root.Q<Label>("Label_TamanoLetra");

        // Botones navegación
        botonRanking = root.Q<Button>("Boton_Ranking");
        botonAtras = root.Q<Button>("Boton_Atras");
        botonCerrarSesion = root.Q<Button>("Boton_CerrarSesion");
        botonCreditos = root.Q<Button>("Boton_Creditos");
        botonTutorial = root.Q<Button>("Boton_Tutorial"); 
        botonManual = root.Q<Button>("Boton_Manual");

        // Paneles y Ranking
        panelRankingOverlay = root.Q<VisualElement>("PanelRankingOverlay");
        listaRanking = root.Q<ScrollView>("ListaRanking");
        botonCerrarRanking = root.Q<Button>("Boton_CerrarRanking");
        mensajeVacio = root.Q<Label>("MensajeVacio");
        panelCreditosOverlay = root.Q<VisualElement>("PanelCreditosOverlay");
        botonCerrarCreditos = root.Q<Button>("Boton_CerrarCreditos");
    }

    private void RegistrarEventos()
    {
        if (sliderMusica != null) sliderMusica.RegisterValueChangedCallback(OnMusicaChanged);
        if (sliderSFX != null) sliderSFX.RegisterValueChangedCallback(OnSFXChanged);
        if (sliderTamanoLetra != null) sliderTamanoLetra.RegisterValueChangedCallback(OnTamanoLetraChanged);

        if (botonRanking != null) botonRanking.clicked += OnRankingClick;
        if (botonCreditos != null) botonCreditos.clicked += OnCreditosClick;
        if (botonCerrarRanking != null) botonCerrarRanking.clicked += CerrarRanking;
        if (botonCerrarCreditos != null) botonCerrarCreditos.clicked += CerrarCreditos;
        if (botonTutorial != null) botonTutorial.clicked += OnTutorialClick; 
        if (botonManual != null) botonManual.clicked += OnManualClick;
        if (botonCerrarSesion != null) botonCerrarSesion.clicked += OnCerrarSesionClick;

        if (botonAtras != null) 
            botonAtras.clicked += () => {
                Debug.Log("[Ajustes] Volviendo al menú principal");
                SceneManager.LoadScene("MenuPrincipalT");
            };
    }

    private void OnTutorialClick() 
    { 
        Debug.Log("[Ajustes] Abriendo tutorial web");
        string ruta = System.IO.Path.Combine(Application.streamingAssetsPath, "Tutorial/index.html");
        Application.OpenURL("file://" + ruta); 
    }

    private void OnManualClick() 
    { 
        Debug.Log("[Ajustes] Abriendo manual PDF");
        string ruta = System.IO.Path.Combine(Application.streamingAssetsPath, "Manual/manual.pdf"); 
        Application.OpenURL("file://" + ruta); 
    }

    private void OnMusicaChanged(ChangeEvent<float> evt) 
    {
        float vol = evt.newValue;
        if (labelMusica != null) labelMusica.text = vol.ToString("F0") + "%";
        PlayerPrefs.SetFloat("VolumenMusica", vol);
        if (AudioManager.Instance != null) AudioManager.Instance.SetVolumenMusica(vol / 100f);
    }

    private void OnSFXChanged(ChangeEvent<float> evt) 
    {
        float vol = evt.newValue;
        if (labelSFX != null) labelSFX.text = vol.ToString("F0") + "%";
        PlayerPrefs.SetFloat("VolumenSFX", vol);
        if (AudioManager.Instance != null) AudioManager.Instance.SetVolumenSFX(vol / 100f);
    }

    private void OnTamanoLetraChanged(ChangeEvent<float> evt) 
    {
        float tam = evt.newValue;
        if (labelTamanoLetra != null) labelTamanoLetra.text = tam.ToString("F0") + "%";
        PlayerPrefs.SetFloat("TamanoLetra", tam);
        AplicarTamanoLetraUI(tam);
    }

    private void AplicarTamanoLetraUI(float porcentaje) 
    {
        float escala = porcentaje / 100f;
        root.Query<Label>().ForEach(label => {
            if (label.name != "Label_Musica" && label.name != "Label_SFX" && label.name != "Label_TamanoLetra")
            {
                float baseSize = DeterminarTamanoBase(label.text);
                label.style.fontSize = baseSize * escala;
            }
        });
    }

    private float DeterminarTamanoBase(string texto)
    {
        if (string.IsNullOrEmpty(texto)) return 22f;
        if (texto.Contains("OPCIONES") || texto.Contains("HOGWARTS")) return 50f;
        if (texto.Contains("AUDIO") || texto.Contains("ACCESIBILIDAD") || texto.Contains("RANKING")) return 32f;
        return 22f;
    }

    private void OnRankingClick() 
    {
        Debug.Log("[Ajustes] Solicitando datos de ranking...");
        if (panelRankingOverlay != null) panelRankingOverlay.style.display = DisplayStyle.Flex;
        
        if (ApiManager.Instance != null)
        {
            ApiManager.Instance.ObtenerRanking((exito, lista) => {
                if (exito && lista != null) MostrarRankingEnUI(lista);
                else MostrarMensajeVacio();
            });
        }
    }

    private void CerrarRanking() 
    {
        if (panelRankingOverlay != null) panelRankingOverlay.style.display = DisplayStyle.None;
    }

    private void MostrarRankingEnUI(ApiManager.RankingEntry[] ranking) 
    {
        if (listaRanking == null) return;
        listaRanking.Clear();
        if (mensajeVacio != null) mensajeVacio.style.display = DisplayStyle.None;

        for (int i = 0; i < ranking.Length; i++) 
        {
            var entrada = ranking[i];
            var fila = new VisualElement();
            fila.style.flexDirection = FlexDirection.Row;
            fila.style.justifyContent = Justify.SpaceBetween;
            fila.style.paddingTop = 12;
            fila.style.marginBottom = 8;

            string medalla = i == 0 ? "🥇" : i == 1 ? "🥈" : i == 2 ? "🥉" : (i + 1) + ".";
            fila.Add(new Label(medalla) { style = { minWidth = 50, fontSize = 24, color = Color.white } });
            fila.Add(new Label(entrada.username) { style = { flexGrow = 1, fontSize = 20, color = Color.grey } });
            fila.Add(new Label(entrada.puntuacion + " pts") { style = { fontSize = 22, color = Color.yellow } });
            
            listaRanking.Add(fila);
        }
    }

    private void MostrarMensajeVacio() 
    {
        if (mensajeVacio != null) mensajeVacio.style.display = DisplayStyle.Flex;
    }

    private void OnCreditosClick() 
    {
        if (panelCreditosOverlay != null) panelCreditosOverlay.style.display = DisplayStyle.Flex;
    }

    private void CerrarCreditos() 
    {
        if (panelCreditosOverlay != null) panelCreditosOverlay.style.display = DisplayStyle.None;
    }

    private void CargarConfiguracion() 
    {
        float vMus = PlayerPrefs.GetFloat("VolumenMusica", 75f);
        if (sliderMusica != null) sliderMusica.value = vMus;
        
        float vSFX = PlayerPrefs.GetFloat("VolumenSFX", 75f);
        if (sliderSFX != null) sliderSFX.value = vSFX;

        float tLet = PlayerPrefs.GetFloat("TamanoLetra", 100f);
        if (sliderTamanoLetra != null) sliderTamanoLetra.value = tLet;
        
        Invoke("AplicarTamanoInicial", 0.1f);
    }

    private void AplicarTamanoInicial() 
    {
        AplicarTamanoLetraUI(PlayerPrefs.GetFloat("TamanoLetra", 100f));
    }

    private void OnCerrarSesionClick() 
    {
        Debug.Log("[Ajustes] Cerrando sesión y borrando credenciales persistentes");
        PlayerPrefs.DeleteKey("player_id");
        PlayerPrefs.DeleteKey("username");
        PlayerPrefs.DeleteKey("nombre");
        PlayerPrefs.Save();
        SceneManager.LoadScene("LoginScene");
    }
}