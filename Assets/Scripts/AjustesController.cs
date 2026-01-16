using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class AjustesController : MonoBehaviour
{
    private UIDocument uiDocument;
    private VisualElement root;
    
    // Botones
    private Button botonVolver;
    private Button botonCreditos;
    private Button botonRanking;
    private Button botonPerfil;
    private Button botonCerrarSesion;
    
    // Sliders de audio
    private Slider sliderMusica;
    private Slider sliderSFX;
    private Label labelMusica;
    private Label labelSFX;
    
    // Toggles de accesibilidad
    private Toggle toggleAltoContraste;
    private Toggle toggleReducirMovimiento;
    
    void Start()
    {
        // Obtener referencias
        uiDocument = GetComponent<UIDocument>();
        
        if (uiDocument == null)
        {
            Debug.LogError("❌ UIDocument no encontrado!");
            return;
        }
        
        root = uiDocument.rootVisualElement;
        
        if (root == null)
        {
            Debug.LogError("❌ Root VisualElement no encontrado!");
            return;
        }
        
        // Obtener botones con validación
        botonVolver = root.Q<Button>("Boton_Atras");
        botonCreditos = root.Q<Button>("Boton_Creditos");
        botonRanking = root.Q<Button>("Boton_Ranking");
        botonPerfil = root.Q<Button>("Boton_Perfil");
        botonCerrarSesion = root.Q<Button>("Boton_CerrarSesion");
        
        // Validar botones
        if (botonVolver == null) Debug.LogWarning("⚠️ Boton_Volver no encontrado");
        if (botonCreditos == null) Debug.LogWarning("⚠️ Boton_Creditos no encontrado");
        if (botonRanking == null) Debug.LogWarning("⚠️ Boton_Ranking no encontrado");
        if (botonPerfil == null) Debug.LogWarning("⚠️ Boton_Perfil no encontrado");
        if (botonCerrarSesion == null) Debug.LogWarning("⚠️ Boton_CerrarSesion no encontrado");
        
        // Obtener sliders con validación
        sliderMusica = root.Q<Slider>("Slider_Musica");
        sliderSFX = root.Q<Slider>("Slider_SFX");
        labelMusica = root.Q<Label>("Label_Musica");
        labelSFX = root.Q<Label>("Label_SFX");
        
        // Validar sliders
        if (sliderMusica == null) Debug.LogWarning("⚠️ Slider_Musica no encontrado");
        if (sliderSFX == null) Debug.LogWarning("⚠️ Slider_SFX no encontrado");
        if (labelMusica == null) Debug.LogWarning("⚠️ Label_Musica no encontrado");
        if (labelSFX == null) Debug.LogWarning("⚠️ Label_SFX no encontrado");
        
        // Obtener toggles con validación
        toggleAltoContraste = root.Q<Toggle>("Toggle_AltoContraste");
        toggleReducirMovimiento = root.Q<Toggle>("Toggle_ReducirMovimiento");
        
        // Validar toggles
        if (toggleAltoContraste == null) Debug.LogWarning("⚠️ Toggle_AltoContraste no encontrado");
        if (toggleReducirMovimiento == null) Debug.LogWarning("⚠️ Toggle_ReducirMovimiento no encontrado");
        
        // Registrar eventos solo si los elementos existen
        if (botonVolver != null) botonVolver.clicked += OnVolverClick;
        if (botonCreditos != null) botonCreditos.clicked += OnCreditosClick;
        if (botonRanking != null) botonRanking.clicked += OnRankingClick;
        if (botonPerfil != null) botonPerfil.clicked += OnPerfilClick;
        if (botonCerrarSesion != null) botonCerrarSesion.clicked += OnCerrarSesionClick;
        
        // Registrar eventos de sliders
        if (sliderMusica != null) sliderMusica.RegisterValueChangedCallback(OnMusicaChanged);
        if (sliderSFX != null) sliderSFX.RegisterValueChangedCallback(OnSFXChanged);
        
        // Registrar eventos de toggles
        if (toggleAltoContraste != null) toggleAltoContraste.RegisterValueChangedCallback(OnAltoContrasteChanged);
        if (toggleReducirMovimiento != null) toggleReducirMovimiento.RegisterValueChangedCallback(OnReducirMovimientoChanged);
        
        // Cargar configuración guardada
        CargarConfiguracion();
        
        Debug.Log("✅ Pantalla de Ajustes cargada correctamente");
    }
    
    private void CargarConfiguracion()
    {
        // Cargar volumen de música
        if (sliderMusica != null && labelMusica != null)
        {
            float volumenMusica = PlayerPrefs.GetFloat("VolumenMusica", 75f);
            sliderMusica.value = volumenMusica;
            labelMusica.text = $"{volumenMusica:F0}%";
        }
        
        // Cargar volumen de SFX
        if (sliderSFX != null && labelSFX != null)
        {
            float volumenSFX = PlayerPrefs.GetFloat("VolumenSFX", 75f);
            sliderSFX.value = volumenSFX;
            labelSFX.text = $"{volumenSFX:F0}%";
        }
        
        // Cargar opciones de accesibilidad
        if (toggleAltoContraste != null)
        {
            bool altoContraste = PlayerPrefs.GetInt("AltoContraste", 0) == 1;
            toggleAltoContraste.value = altoContraste;
        }
        
        if (toggleReducirMovimiento != null)
        {
            bool reducirMovimiento = PlayerPrefs.GetInt("ReducirMovimiento", 0) == 1;
            toggleReducirMovimiento.value = reducirMovimiento;
        }
    }
    
    private void OnMusicaChanged(ChangeEvent<float> evt)
    {
        if (labelMusica != null)
        {
            float volumen = evt.newValue;
            labelMusica.text = $"{volumen:F0}%";
            PlayerPrefs.SetFloat("VolumenMusica", volumen);
            PlayerPrefs.Save();
            Debug.Log($"🎵 Volumen música: {volumen}%");
        }
    }
    
    private void OnSFXChanged(ChangeEvent<float> evt)
    {
        if (labelSFX != null)
        {
            float volumen = evt.newValue;
            labelSFX.text = $"{volumen:F0}%";
            PlayerPrefs.SetFloat("VolumenSFX", volumen);
            PlayerPrefs.Save();
            Debug.Log($"🔊 Volumen SFX: {volumen}%");
        }
    }
    
    private void OnAltoContrasteChanged(ChangeEvent<bool> evt)
    {
        bool activado = evt.newValue;
        PlayerPrefs.SetInt("AltoContraste", activado ? 1 : 0);
        PlayerPrefs.Save();
        Debug.Log($"{(activado ? "✅" : "❌")} Alto contraste: {(activado ? "Activado" : "Desactivado")}");
    }
    
    private void OnReducirMovimientoChanged(ChangeEvent<bool> evt)
    {
        bool activado = evt.newValue;
        PlayerPrefs.SetInt("ReducirMovimiento", activado ? 1 : 0);
        PlayerPrefs.Save();
        Debug.Log($"{(activado ? "✅" : "❌")} Reducir movimiento: {(activado ? "Activado" : "Desactivado")}");
    }
    
    private void OnVolverClick()
    {
        Debug.Log("⬅️ Volviendo al menú principal...");
        SceneManager.LoadScene("MenuPrincipalT");
    }
    
    private void OnCreditosClick()
    {
        Debug.Log("📜 Mostrando créditos...");
        // Implementa tu lógica de créditos aquí
    }
    
    private void OnRankingClick()
    {
        Debug.Log("🏆 Mostrando ranking...");
        // Implementa tu lógica de ranking aquí
    }
    
    private void OnPerfilClick()
    {
        Debug.Log("👤 Mostrando perfil...");
        SceneManager.LoadScene("PerfilScene");
    }
    
    private void OnCerrarSesionClick()
    {
        Debug.Log("🚪 Cerrando sesión...");
        PlayerPrefs.DeleteKey("UsuarioActivo");
        PlayerPrefs.DeleteKey("SesionIniciada");
        PlayerPrefs.Save();
        SceneManager.LoadScene("LoginScene");
    }
}