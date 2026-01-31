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

    void Start() {
        var uiDoc = GetComponent<UIDocument>();
        root = uiDoc.rootVisualElement;

        // --- 1. CAPTURAR ELEMENTOS ---
        // Audio
        sliderMusica = root.Q<Slider>("Slider_Musica");
        sliderSFX = root.Q<Slider>("Slider_SFX");
        labelMusica = root.Q<Label>("Label_Musica");
        labelSFX = root.Q<Label>("Label_SFX");

        // Accesibilidad
        sliderTamanoLetra = root.Q<Slider>("Slider_TamanoLetra");
        labelTamanoLetra = root.Q<Label>("Label_TamanoLetra");
        toggleDaltonicos = root.Q<Toggle>("Toggle_Daltonicos");

        // Botones
        botonRanking = root.Q<Button>("Boton_Ranking");
        botonAtras = root.Q<Button>("Boton_Atras");
        botonCerrarSesion = root.Q<Button>("Boton_CerrarSesion");
        botonCreditos = root.Q<Button>("Boton_Creditos");
        botonPerfil = root.Q<Button>("Boton_Perfil");

        // --- 2. REGISTRAR EVENTOS ---
        if (sliderMusica != null) sliderMusica.RegisterValueChangedCallback(OnMusicaChanged);
        if (sliderSFX != null) sliderSFX.RegisterValueChangedCallback(OnSFXChanged);
        if (sliderTamanoLetra != null) sliderTamanoLetra.RegisterValueChangedCallback(OnTamanoLetraChanged);
        if (toggleDaltonicos != null) toggleDaltonicos.RegisterValueChangedCallback(OnDaltonicosChanged);

        if (botonRanking != null) botonRanking.clicked += OnRankingClick;
        if (botonAtras != null) botonAtras.clicked += () => SceneManager.LoadScene("MenuPrincipalT");
        if (botonPerfil != null) botonPerfil.clicked += () => SceneManager.LoadScene("PerfilScene");
        if (botonCerrarSesion != null) botonCerrarSesion.clicked += OnCerrarSesionClick;

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
    // LÓGICA DE ACCESIBILIDAD
    // ====================================
    private void OnTamanoLetraChanged(ChangeEvent<float> evt) {
        float tam = evt.newValue;
        if (labelTamanoLetra != null) labelTamanoLetra.text = $"{tam:F0}%";
        PlayerPrefs.SetFloat("TamanoLetra", tam);
        AplicarTamanoLetraUI(tam);
    }

    private void AplicarTamanoLetraUI(float porcentaje) {
        float escala = porcentaje / 100f;
        var todosLosLabels = root.Query<Label>().ToList();
        foreach (var label in todosLosLabels) {
            // Guardamos el tamaño original la primera vez para no perder la proporción
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
        AplicarModoDaltonico(evt.newValue);
    }

    private void AplicarModoDaltonico(bool activado) {
        if (activado) root.AddToClassList("modo-daltonico-activado");
        else root.RemoveFromClassList("modo-daltonico-activado");
        
        PlayerPrefs.SetInt("ModoDaltonico", activado ? 1 : 0);
        Debug.Log(" Modo daltónico: " + (activado ? "Activado" : "Desactivado"));
    }

    // ====================================
    // LÓGICA DE RANKING (API)
    // ====================================
    private void OnRankingClick() {
        if (ApiManager.Instance == null) {
            Debug.LogError("⚠️ No existe ApiManager. Empieza desde el Login.");
            return;
        }

        Debug.Log("Consultando Ranking...");
        ApiManager.Instance.ObtenerRanking((exito, lista) => {
            if (exito) {
                Debug.Log("Ranking recibido. Total: " + lista.Length);
                // Aquí podrías mostrarlo en una lista UI
            } else {
                Debug.LogError("Error al obtener ranking de la base de datos.");
            }
        });
    }

    // ====================================
    // CARGAR Y CERRAR SESIÓN
    // ====================================
    private void CargarConfiguracion() {
        // Audio
        float vMus = PlayerPrefs.GetFloat("VolumenMusica", 75f);
        if (sliderMusica != null) sliderMusica.value = vMus;

        float vSFX = PlayerPrefs.GetFloat("VolumenSFX", 75f);
        if (sliderSFX != null) sliderSFX.value = vSFX;

        // Tamaño Letra
        float tLet = PlayerPrefs.GetFloat("TamanoLetra", 100f);
        if (sliderTamanoLetra != null) sliderTamanoLetra.value = tLet;
        // Esperamos un frame para aplicar el tamaño porque resolvedStyle tarda en cargar
        Invoke("DelayedFontScale", 0.1f);

        // Daltonismo
        bool mDal = PlayerPrefs.GetInt("ModoDaltonico", 0) == 1;
        if (toggleDaltonicos != null) toggleDaltonicos.value = mDal;
        AplicarModoDaltonico(mDal);
    }

    private void DelayedFontScale() {
        AplicarTamanoLetraUI(PlayerPrefs.GetFloat("TamanoLetra", 100f));
    }

    private void OnCerrarSesionClick() {
        PlayerPrefs.DeleteKey("UsuarioActivo"); // O lo que uses para la sesión
        PlayerPrefs.Save();
        SceneManager.LoadScene("LoginScene");
    }
}