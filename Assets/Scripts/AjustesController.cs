using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

/// <summary>
/// Controlador de la pantalla de ajustes
/// Gestiona audio, accesibilidad, ranking y créditos
/// </summary>
public class AjustesController : MonoBehaviour
{
    // ====================================
    // REFERENCIAS UI
    // ====================================
    private VisualElement root;
    
    // Audio
    private Slider sliderMusica, sliderSFX;
    private Label labelMusica, labelSFX;
    
    // Accesibilidad
    private Slider sliderTamanoLetra;
    private Label labelTamanoLetra;
    
    // Botones
    private Button botonRanking, botonAtras, botonCerrarSesion, botonCreditos, botonTutorial, botonManual;

    // Ranking
    private VisualElement panelRankingOverlay;
    private ScrollView listaRanking;
    private Button botonCerrarRanking;
    private Label mensajeVacio;

    // Créditos
    private VisualElement panelCreditosOverlay;
    private Button botonCerrarCreditos;

    // ====================================
    // INICIALIZACIÓN
    // ====================================
    void Start() 
    {
        var uiDoc = GetComponent<UIDocument>();
        root = uiDoc.rootVisualElement;

        ObtenerReferencias();
        RegistrarEventos();
        CargarConfiguracion();
    }

    // ====================================
    // OBTENER REFERENCIAS
    // ====================================
    private void ObtenerReferencias()
    {
        // Audio
        sliderMusica = root.Q<Slider>("Slider_Musica");
        sliderSFX = root.Q<Slider>("Slider_SFX");
        labelMusica = root.Q<Label>("Label_Musica");
        labelSFX = root.Q<Label>("Label_SFX");

        // Accesibilidad
        sliderTamanoLetra = root.Q<Slider>("Slider_TamanoLetra");
        labelTamanoLetra = root.Q<Label>("Label_TamanoLetra");

        // Botones
        botonRanking = root.Q<Button>("Boton_Ranking");
        botonAtras = root.Q<Button>("Boton_Atras");
        botonCerrarSesion = root.Q<Button>("Boton_CerrarSesion");
        botonCreditos = root.Q<Button>("Boton_Creditos");

        // Ranking
        panelRankingOverlay = root.Q<VisualElement>("PanelRankingOverlay");
        listaRanking = root.Q<ScrollView>("ListaRanking");
        botonCerrarRanking = root.Q<Button>("Boton_CerrarRanking");
        mensajeVacio = root.Q<Label>("MensajeVacio");

        // Créditos
        panelCreditosOverlay = root.Q<VisualElement>("PanelCreditosOverlay");
        botonCerrarCreditos = root.Q<Button>("Boton_CerrarCreditos");

        // Tutorial
        botonTutorial = root.Q<Button>("Boton_Tutorial"); 

        // Manual
        botonManual = root.Q<Button>("Boton_Manual");
    }

    // ====================================
    // REGISTRAR EVENTOS
    // ====================================
    private void RegistrarEventos()
    {
        // Audio
        if (sliderMusica != null) 
            sliderMusica.RegisterValueChangedCallback(OnMusicaChanged);
        if (sliderSFX != null) 
            sliderSFX.RegisterValueChangedCallback(OnSFXChanged);

        // Accesibilidad
        if (sliderTamanoLetra != null) 
            sliderTamanoLetra.RegisterValueChangedCallback(OnTamanoLetraChanged);

        // Botones
        if (botonRanking != null) 
            botonRanking.clicked += OnRankingClick;
        if (botonCreditos != null) 
            botonCreditos.clicked += OnCreditosClick;
        if (botonAtras != null) 
            botonAtras.clicked += () => SceneManager.LoadScene("MenuPrincipalT");
        if (botonCerrarSesion != null) 
            botonCerrarSesion.clicked += OnCerrarSesionClick;
        if (botonCerrarRanking != null) 
            botonCerrarRanking.clicked += CerrarRanking;
        if (botonCerrarCreditos != null) 
            botonCerrarCreditos.clicked += CerrarCreditos;
        
        // Tutorial
        if (botonTutorial != null) 
            botonTutorial.clicked += OnTutorialClick; 

        // Manual
        if (botonManual != null) 
            botonManual.clicked += OnManualClick;
    }


    private void OnTutorialClick() 
    { 
        string ruta = System.IO.Path.Combine(Application.streamingAssetsPath, "Tutorial/index.html");
        Application.OpenURL("file://" + ruta); 
    }

    private void OnManualClick() 
    { 
        string ruta = System.IO.Path.Combine(Application.streamingAssetsPath, "Manual/manual.pdf"); 
        Application.OpenURL("file://" + ruta); 
    }

    // ====================================
    // AUDIO
    // ====================================
    
    private void OnMusicaChanged(ChangeEvent<float> evt) 
    {
        float vol = evt.newValue;
        
        if (labelMusica != null) 
            labelMusica.text = vol.ToString("F0") + "%";
        
        PlayerPrefs.SetFloat("VolumenMusica", vol);
        PlayerPrefs.Save();
        
        if (AudioManager.Instance != null) 
            AudioManager.Instance.SetVolumenMusica(vol / 100f);
    }

    private void OnSFXChanged(ChangeEvent<float> evt) 
    {
        float vol = evt.newValue;
        
        if (labelSFX != null) 
            labelSFX.text = vol.ToString("F0") + "%";
        
        PlayerPrefs.SetFloat("VolumenSFX", vol);
        PlayerPrefs.Save();
        
        if (AudioManager.Instance != null) 
            AudioManager.Instance.SetVolumenSFX(vol / 100f);
    }

    // ====================================
    // ACCESIBILIDAD
    // ====================================
    
    private void OnTamanoLetraChanged(ChangeEvent<float> evt) 
    {
        float tam = evt.newValue;
        
        if (labelTamanoLetra != null) 
            labelTamanoLetra.text = tam.ToString("F0") + "%";
        
        PlayerPrefs.SetFloat("TamanoLetra", tam);
        PlayerPrefs.Save();
        
        AplicarTamanoLetraUI(tam);
    }

    private void AplicarTamanoLetraUI(float porcentaje) 
    {
        float escala = porcentaje / 100f;
        
        root.Query<Label>().ForEach(label => {
            if (label.name != "Label_Musica" && 
                label.name != "Label_SFX" && 
                label.name != "Label_TamanoLetra")
            {
                float baseSize = DeterminarTamanoBase(label.text);
                label.style.fontSize = baseSize * escala;
            }
        });
        
        Debug.Log("📏 Tamaño de letra ajustado a " + porcentaje + "%");
    }

    private float DeterminarTamanoBase(string texto)
    {
        if (string.IsNullOrEmpty(texto))
            return 22f;

        if (texto.Contains("OPCIONES") || texto.Contains("HOGWARTS"))
            return 50f;

        if (texto.Contains("AUDIO") || texto.Contains("ACCESIBILIDAD") ||
            texto.Contains("CRÉDITOS") || texto.Contains("RANKING") ||
            texto.Contains("SELECCIONA") || texto.Contains("SELECCIÓN"))
            return 32f;

        if (texto.Contains("ENTRAR") ||
            texto.Contains("CERRAR SESIÓN") || texto.Contains("CREAR CUENTA") ||
            texto.Contains("REGISTRARSE"))
            return 26f;

        return 22f;
    }

    // ====================================
    // RANKING
    // ====================================
    
    private void OnRankingClick() 
    {
        if (ApiManager.Instance == null) 
        {
            Debug.LogError("⚠️ ApiManager no encontrado");
            return;
        }

        Debug.Log("📊 Abriendo ranking...");
        MostrarPanelRanking();
        
        ApiManager.Instance.ObtenerRanking((exito, lista) => {
            if (exito && lista != null && lista.Length > 0) 
            {
                Debug.Log("✅ Ranking recibido: " + lista.Length + " jugadores");
                MostrarRankingEnUI(lista);
            } 
            else 
            {
                Debug.LogError("❌ Error al obtener ranking");
                MostrarMensajeVacio();
            }
        });
    }

    private void MostrarPanelRanking() 
    {
        if (panelRankingOverlay != null) 
        {
            panelRankingOverlay.style.display = DisplayStyle.Flex;
        }
    }

    private void CerrarRanking() 
    {
        if (panelRankingOverlay != null) 
        {
            panelRankingOverlay.style.display = DisplayStyle.None;
        }
        
        if (listaRanking != null) 
        {
            listaRanking.Clear();
        }
    }

    private void MostrarRankingEnUI(ApiManager.RankingEntry[] ranking) 
    {
        if (listaRanking == null) return;
        
        listaRanking.Clear();
        
        if (mensajeVacio != null) 
            mensajeVacio.style.display = DisplayStyle.None;

        for (int i = 0; i < ranking.Length; i++) 
        {
            var entrada = ranking[i];
            
            var fila = new VisualElement();
            fila.style.flexDirection = FlexDirection.Row;
            fila.style.justifyContent = Justify.SpaceBetween;
            fila.style.alignItems = Align.Center;
            fila.style.backgroundColor = i % 2 == 0 ? 
                new Color(0.1f, 0.1f, 0.1f, 0.5f) : 
                new Color(0.15f, 0.15f, 0.15f, 0.5f);
            fila.style.paddingTop = 12;
            fila.style.paddingBottom = 12;
            fila.style.paddingLeft = 15;
            fila.style.paddingRight = 15;
            fila.style.borderBottomWidth = 1;
            fila.style.borderBottomColor = new Color(1f, 0.84f, 0f, 0.2f);
            fila.style.borderTopLeftRadius = 8;
            fila.style.borderTopRightRadius = 8;
            fila.style.borderBottomLeftRadius = 8;
            fila.style.borderBottomRightRadius = 8;
            fila.style.marginBottom = 8;

            string medalla = i == 0 ? "🥇" : i == 1 ? "🥈" : i == 2 ? "🥉" : (i + 1) + ".";
            
            var labelPosicion = new Label(medalla);
            labelPosicion.style.fontSize = 24;
            labelPosicion.style.color = Color.white;
            labelPosicion.style.unityFontStyleAndWeight = FontStyle.Bold;
            labelPosicion.style.minWidth = 50;

            var labelNombre = new Label(entrada.username);
            labelNombre.style.fontSize = 20;
            labelNombre.style.color = new Color(0.9f, 0.9f, 0.9f);
            labelNombre.style.flexGrow = 1;
            labelNombre.style.unityTextAlign = TextAnchor.MiddleLeft;

            // ✅ CORREGIDO: Cambiado de .puntos a .puntuacion
            var labelPuntos = new Label(entrada.puntuacion + " pts");
            labelPuntos.style.fontSize = 22;
            labelPuntos.style.color = new Color(1f, 0.84f, 0f);
            labelPuntos.style.unityFontStyleAndWeight = FontStyle.Bold;

            fila.Add(labelPosicion);
            fila.Add(labelNombre);
            fila.Add(labelPuntos);
            listaRanking.Add(fila);
        }
    }

    private void MostrarMensajeVacio() 
    {
        if (listaRanking != null) 
            listaRanking.Clear();
        
        if (mensajeVacio != null) 
            mensajeVacio.style.display = DisplayStyle.Flex;
    }

    // ====================================
    // CRÉDITOS
    // ====================================
    
    private void OnCreditosClick() 
    {
        Debug.Log("🎬 Abriendo créditos...");
        
        if (panelCreditosOverlay != null) 
        {
            panelCreditosOverlay.style.display = DisplayStyle.Flex;
        }
    }

    private void CerrarCreditos() 
    {
        Debug.Log("❌ Cerrando créditos...");
        
        if (panelCreditosOverlay != null) 
        {
            panelCreditosOverlay.style.display = DisplayStyle.None;
        }
    }

    // ====================================
    // CARGAR CONFIGURACIÓN
    // ====================================
    
    private void CargarConfiguracion() 
    {
        // Audio
        float vMus = PlayerPrefs.GetFloat("VolumenMusica", 75f);
        if (sliderMusica != null) 
        {
            sliderMusica.value = vMus;
            if (labelMusica != null) 
                labelMusica.text = vMus.ToString("F0") + "%";
        }

        float vSFX = PlayerPrefs.GetFloat("VolumenSFX", 75f);
        if (sliderSFX != null) 
        {
            sliderSFX.value = vSFX;
            if (labelSFX != null) 
                labelSFX.text = vSFX.ToString("F0") + "%";
        }

        // Tamaño de letra - VALOR POR DEFECTO: 100%
        float tLet = PlayerPrefs.GetFloat("TamanoLetra", 100f);
        if (sliderTamanoLetra != null) 
        {
            sliderTamanoLetra.value = tLet;
            if (labelTamanoLetra != null) 
                labelTamanoLetra.text = tLet.ToString("F0") + "%";
        }
        
        Invoke("AplicarTamanoInicial", 0.1f);
    }

    private void AplicarTamanoInicial() 
    {
        float tam = PlayerPrefs.GetFloat("TamanoLetra", 100f);
        AplicarTamanoLetraUI(tam);
    }

    // ====================================
    // CERRAR SESIÓN
    // ====================================
    
    private void OnCerrarSesionClick() 
    {
        Debug.Log("🚪 Cerrando sesión...");
        
        PlayerPrefs.DeleteKey("player_id");
        PlayerPrefs.DeleteKey("username");
        PlayerPrefs.DeleteKey("nombre");
        PlayerPrefs.Save();
        
        SceneManager.LoadScene("LoginScene");
    }
}