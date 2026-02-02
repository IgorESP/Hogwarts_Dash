using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class AjustesController : MonoBehaviour
{
    private VisualElement root;
    
    // Referencias UI
    private Slider sliderMusica, sliderSFX, sliderTamanoLetra;
    private Label labelMusica, labelSFX, labelTamanoLetra;
    private Toggle toggleDaltonicos;
    private Button botonRanking, botonAtras, botonCerrarSesion, botonCreditos, botonPerfil;

    // --- PANEL DE RANKING ---
    private VisualElement panelRankingOverlay;
    private ScrollView listaRanking;
    private Button botonCerrarRanking;
    private Label mensajeVacio;

    // --- PANEL DE CRÉDITOS ---
    private VisualElement panelCreditosOverlay;
    private Button botonCerrarCreditos;

    void Start() {
        var uiDoc = GetComponent<UIDocument>();
        root = uiDoc.rootVisualElement;

        // --- 1. CAPTURAR ELEMENTOS ---
        sliderMusica = root.Q<Slider>("Slider_Musica");
        sliderSFX = root.Q<Slider>("Slider_SFX");
        labelMusica = root.Q<Label>("Label_Musica");
        labelSFX = root.Q<Label>("Label_SFX");

        sliderTamanoLetra = root.Q<Slider>("Slider_TamanoLetra");
        labelTamanoLetra = root.Q<Label>("Label_TamanoLetra");
        toggleDaltonicos = root.Q<Toggle>("Toggle_Daltonicos");

        botonRanking = root.Q<Button>("Boton_Ranking");
        botonAtras = root.Q<Button>("Boton_Atras");
        botonCerrarSesion = root.Q<Button>("Boton_CerrarSesion");
        botonCreditos = root.Q<Button>("Boton_Creditos");
        botonPerfil = root.Q<Button>("Boton_Perfil");

        panelRankingOverlay = root.Q<VisualElement>("PanelRankingOverlay");
        listaRanking = root.Q<ScrollView>("ListaRanking");
        botonCerrarRanking = root.Q<Button>("Boton_CerrarRanking");
        mensajeVacio = root.Q<Label>("MensajeVacio");

        panelCreditosOverlay = root.Q<VisualElement>("PanelCreditosOverlay");
        botonCerrarCreditos = root.Q<Button>("Boton_CerrarCreditos");

        // --- 2. REGISTRAR EVENTOS (TODO SE APLICA AUTOMÁTICAMENTE) ---
        if (sliderMusica != null) sliderMusica.RegisterValueChangedCallback(OnMusicaChanged);
        if (sliderSFX != null) sliderSFX.RegisterValueChangedCallback(OnSFXChanged);
        if (sliderTamanoLetra != null) sliderTamanoLetra.RegisterValueChangedCallback(OnTamanoLetraChanged);
        if (toggleDaltonicos != null) toggleDaltonicos.RegisterValueChangedCallback(OnDaltonicosChanged);

        if (botonRanking != null) botonRanking.clicked += OnRankingClick;
        if (botonCreditos != null) botonCreditos.clicked += OnCreditosClick;
        if (botonAtras != null) botonAtras.clicked += () => SceneManager.LoadScene("MenuPrincipalT");
        if (botonPerfil != null) botonPerfil.clicked += () => SceneManager.LoadScene("PerfilScene");
        if (botonCerrarSesion != null) botonCerrarSesion.clicked += OnCerrarSesionClick;
        if (botonCerrarRanking != null) botonCerrarRanking.clicked += CerrarRanking;
        if (botonCerrarCreditos != null) botonCerrarCreditos.clicked += CerrarCreditos;

        // --- 3. CARGAR CONFIGURACIÓN ---
        CargarConfiguracion();
    }

    // ====================================
    // LÓGICA DE AUDIO
    // ====================================
    private void OnMusicaChanged(ChangeEvent<float> evt) {
        float vol = evt.newValue;
        if (labelMusica != null) labelMusica.text = $"{vol:F0}%";
        PlayerPrefs.SetFloat("VolumenMusica", vol);
        if (AudioManager.Instance != null) AudioManager.Instance.SetVolumenMusica(vol / 100f);
    }

    private void OnSFXChanged(ChangeEvent<float> evt) {
        float vol = evt.newValue;
        if (labelSFX != null) labelSFX.text = $"{vol:F0}%";
        PlayerPrefs.SetFloat("VolumenSFX", vol);
        if (AudioManager.Instance != null) AudioManager.Instance.SetVolumenSFX(vol / 100f);
    }

    // ====================================
    // LÓGICA DE ACCESIBILIDAD (SE APLICA AUTOMÁTICAMENTE)
    // ====================================
    private void OnTamanoLetraChanged(ChangeEvent<float> evt) {
        float tam = evt.newValue;
        if (labelTamanoLetra != null) labelTamanoLetra.text = $"{tam:F0}%";
        
        // Guardar y aplicar inmediatamente
        PlayerPrefs.SetFloat("TamanoLetra", tam);
        PlayerPrefs.Save();
        AplicarTamanoLetraUI(tam);
    }

    private void AplicarTamanoLetraUI(float porcentaje) {
        float escala = porcentaje / 100f;
        var todosLosLabels = root.Query<Label>().ToList();
        foreach (var label in todosLosLabels) {
            // Guardamos el tamaño original la primera vez
            if (!PlayerPrefs.HasKey($"BaseSize_{label.name}_{label.text.Length}")) {
                float baseSize = label.resolvedStyle.fontSize;
                if (baseSize > 0) PlayerPrefs.SetFloat($"BaseSize_{label.name}_{label.text.Length}", baseSize);
                else continue;
            }
            float originalSize = PlayerPrefs.GetFloat($"BaseSize_{label.name}_{label.text.Length}");
            label.style.fontSize = originalSize * escala;
        }
    }

    private void OnDaltonicosChanged(ChangeEvent<bool> evt) {
        bool activado = evt.newValue;
        
        // Guardar y aplicar inmediatamente
        PlayerPrefs.SetInt("ModoDaltonico", activado ? 1 : 0);
        PlayerPrefs.Save();
        AplicarModoDaltonico(activado);
    }

    private void AplicarModoDaltonico(bool activado) {
        if (activado) root.AddToClassList("modo-daltonico-activado");
        else root.RemoveFromClassList("modo-daltonico-activado");
        
        Debug.Log("🎨 Modo daltónico: " + (activado ? "Activado" : "Desactivado"));
    }

    // ====================================
    // LÓGICA DE RANKING (API)
    // ====================================
    private void OnRankingClick() {
        if (ApiManager.Instance == null) {
            Debug.LogError("⚠️ No existe ApiManager. Empieza desde el Login.");
            return;
        }

        Debug.Log("📊 Consultando Ranking...");
        MostrarPanelRanking();
        
        ApiManager.Instance.ObtenerRanking((exito, lista) => {
            if (exito && lista != null && lista.Length > 0) {
                Debug.Log($"✅ Ranking recibido. Total: {lista.Length}");
                MostrarRankingEnUI(lista);
            } else {
                Debug.LogError("❌ Error al obtener ranking o ranking vacío");
                MostrarMensajeVacio();
            }
        });
    }

    private void MostrarPanelRanking() {
        if (panelRankingOverlay != null) {
            panelRankingOverlay.style.display = DisplayStyle.Flex;
        }
    }

    private void CerrarRanking() {
        if (panelRankingOverlay != null) {
            panelRankingOverlay.style.display = DisplayStyle.None;
        }
        if (listaRanking != null) {
            listaRanking.Clear();
        }
    }

    private void MostrarRankingEnUI(ApiManager.RankingEntry[] ranking) {
        if (listaRanking == null) return;
        
        listaRanking.Clear();
        if (mensajeVacio != null) mensajeVacio.style.display = DisplayStyle.None;

        for (int i = 0; i < ranking.Length; i++) {
            var entrada = ranking[i];
            
            var fila = new VisualElement();
            fila.style.flexDirection = FlexDirection.Row;
            fila.style.justifyContent = Justify.SpaceBetween;
            fila.style.alignItems = Align.Center;
            fila.style.backgroundColor = i % 2 == 0 ? new Color(0.1f, 0.1f, 0.1f, 0.5f) : new Color(0.15f, 0.15f, 0.15f, 0.5f);
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

            string medalla = i == 0 ? "🥇" : i == 1 ? "🥈" : i == 2 ? "🥉" : $"{i + 1}.";
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

            var labelPuntos = new Label($"{entrada.puntos} pts");
            labelPuntos.style.fontSize = 22;
            labelPuntos.style.color = new Color(1f, 0.84f, 0f);
            labelPuntos.style.unityFontStyleAndWeight = FontStyle.Bold;

            fila.Add(labelPosicion);
            fila.Add(labelNombre);
            fila.Add(labelPuntos);
            listaRanking.Add(fila);
        }
    }

    private void MostrarMensajeVacio() {
        if (listaRanking != null) listaRanking.Clear();
        if (mensajeVacio != null) mensajeVacio.style.display = DisplayStyle.Flex;
    }

    // ====================================
    // LÓGICA DE CRÉDITOS
    // ====================================
    private void OnCreditosClick() {
        Debug.Log("🎬 Abriendo Créditos...");
        if (panelCreditosOverlay != null) {
            panelCreditosOverlay.style.display = DisplayStyle.Flex;
        }
    }

    private void CerrarCreditos() {
        Debug.Log("❌ Cerrando Créditos...");
        if (panelCreditosOverlay != null) {
            panelCreditosOverlay.style.display = DisplayStyle.None;
        }
    }

    // ====================================
    // CARGAR CONFIGURACIÓN
    // ====================================
    private void CargarConfiguracion() {
        // Audio
        float vMus = PlayerPrefs.GetFloat("VolumenMusica", 75f);
        if (sliderMusica != null) sliderMusica.value = vMus;

        float vSFX = PlayerPrefs.GetFloat("VolumenSFX", 75f);
        if (sliderSFX != null) sliderSFX.value = vSFX;

        // Tamaño Letra - VALOR POR DEFECTO: 100%
        float tLet = PlayerPrefs.GetFloat("TamanoLetra", 100f);
        if (sliderTamanoLetra != null) sliderTamanoLetra.value = tLet;
        Invoke("DelayedFontScale", 0.1f);

        // Daltonismo - VALOR POR DEFECTO: Desactivado (0)
        bool mDal = PlayerPrefs.GetInt("ModoDaltonico", 0) == 1;
        if (toggleDaltonicos != null) toggleDaltonicos.value = mDal;
        AplicarModoDaltonico(mDal);
    }

    private void DelayedFontScale() {
        AplicarTamanoLetraUI(PlayerPrefs.GetFloat("TamanoLetra", 100f));
    }

    private void OnCerrarSesionClick() {
        PlayerPrefs.DeleteKey("UsuarioActivo");
        PlayerPrefs.DeleteKey("UserId");
        PlayerPrefs.DeleteKey("Username");
        PlayerPrefs.Save();
        SceneManager.LoadScene("LoginScene");
    }
}