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
    
    // Controles de accesibilidad (MODIFICADOS)
    private Slider sliderTamanoLetra;
    private Label labelTamanoLetra;
    private Toggle toggleDaltonicos;
    
    void Start()
    {
        // Obtener referencias
        uiDocument = GetComponent<UIDocument>();
        
        if (uiDocument == null)
        {
            Debug.LogError("UIDocument no encontrado");
            return;
        }
        
        root = uiDocument.rootVisualElement;
        
        if (root == null)
        {
            Debug.LogError("Root VisualElement no encontrado");
            return;
        }
        
        // Obtener botones con validación
        botonVolver = root.Q<Button>("Boton_Atras");
        botonCreditos = root.Q<Button>("Boton_Creditos");
        botonRanking = root.Q<Button>("Boton_Ranking");
        botonPerfil = root.Q<Button>("Boton_Perfil");
        botonCerrarSesion = root.Q<Button>("Boton_CerrarSesion");
        
        // Validar botones
        if (botonVolver == null) Debug.LogWarning("Boton_Atras no encontrado");
        if (botonCreditos == null) Debug.LogWarning("Boton_Creditos no encontrado");
        if (botonRanking == null) Debug.LogWarning("Boton_Ranking no encontrado");
        if (botonPerfil == null) Debug.LogWarning(" Boton_Perfil no encontrado");
        if (botonCerrarSesion == null) Debug.LogWarning("Boton_CerrarSesion no encontrado");
        
        // Obtener sliders con validación
        sliderMusica = root.Q<Slider>("Slider_Musica");
        sliderSFX = root.Q<Slider>("Slider_SFX");
        labelMusica = root.Q<Label>("Label_Musica");
        labelSFX = root.Q<Label>("Label_SFX");
        
        // Validar sliders
        if (sliderMusica == null) Debug.LogWarning("Slider_Musica no encontrado");
        if (sliderSFX == null) Debug.LogWarning("Slider_SFX no encontrado");
        if (labelMusica == null) Debug.LogWarning("Label_Musica no encontrado");
        if (labelSFX == null) Debug.LogWarning("Label_SFX no encontrado");
        
        // Obtener controles de accesibilidad (MODIFICADOS)
        sliderTamanoLetra = root.Q<Slider>("Slider_TamanoLetra");
        labelTamanoLetra = root.Q<Label>("Label_TamanoLetra");
        toggleDaltonicos = root.Q<Toggle>("Toggle_Daltonicos");
        
        // Validar controles de accesibilidad
        if (sliderTamanoLetra == null) Debug.LogWarning("Slider_TamanoLetra no encontrado");
        if (labelTamanoLetra == null) Debug.LogWarning("Label_TamanoLetra no encontrado");
        if (toggleDaltonicos == null) Debug.LogWarning("Toggle_Daltonicos no encontrado");
        
        // Registrar eventos solo si los elementos existen
        if (botonVolver != null) botonVolver.clicked += OnVolverClick;
        if (botonCreditos != null) botonCreditos.clicked += OnCreditosClick;
        if (botonRanking != null) botonRanking.clicked += OnRankingClick;
        if (botonPerfil != null) botonPerfil.clicked += OnPerfilClick;
        if (botonCerrarSesion != null) botonCerrarSesion.clicked += OnCerrarSesionClick;
        
        // Registrar eventos de sliders
        if (sliderMusica != null) sliderMusica.RegisterValueChangedCallback(OnMusicaChanged);
        if (sliderSFX != null) sliderSFX.RegisterValueChangedCallback(OnSFXChanged);
        
        // Registrar eventos de accesibilidad (MODIFICADOS)
        if (sliderTamanoLetra != null) sliderTamanoLetra.RegisterValueChangedCallback(OnTamanoLetraChanged);
        if (toggleDaltonicos != null) toggleDaltonicos.RegisterValueChangedCallback(OnDaltonicosChanged);
        
        // Cargar configuración guardada
        CargarConfiguracion();
        
        Debug.Log("Pantalla de Ajustes cargada correctamente");
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
        
        // Cargar tamaño de letra (NUEVO)
        // Cargar tamaño de letra
        if (sliderTamanoLetra != null && labelTamanoLetra != null)
        {
            float tamanoLetra = PlayerPrefs.GetFloat("TamanoLetra", 100f);
            sliderTamanoLetra.value = tamanoLetra;
            labelTamanoLetra.text = $"{tamanoLetra:F0}%";
            
            //  APLICAR el tamaño guardado
            AplicarTamanoLetraUI(tamanoLetra);
        }   
                
        // Cargar modo daltónico (NUEVO)
        if (toggleDaltonicos != null)
        {
            bool modoDaltonico = PlayerPrefs.GetInt("ModoDaltonico", 0) == 1;
            toggleDaltonicos.value = modoDaltonico;
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
        
        // Aplicar en tiempo real
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetVolumenMusica(volumen / 100f);
        }
        
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
        
        // Aplicar en tiempo real
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetVolumenSFX(volumen / 100f);
        }
        
        Debug.Log($"🔊 Volumen SFX: {volumen}%");
    }
}
    
    // NUEVOS MÉTODOS PARA ACCESIBILIDAD
 private void OnTamanoLetraChanged(ChangeEvent<float> evt)
{
    if (labelTamanoLetra != null)
    {
        float tamano = evt.newValue;
        labelTamanoLetra.text = $"{tamano:F0}%";
        
        // Guardar configuración
        PlayerPrefs.SetFloat("TamanoLetra", tamano);
        PlayerPrefs.SetFloat("EscalaTexto", tamano / 100f);
        PlayerPrefs.Save();
        
        // Aplicar INMEDIATAMENTE
        AplicarTamanoLetraUI(tamano);
        
        Debug.Log($"Tamaño letra: {tamano}%");
    }
}
    private void OnDaltonicosChanged(ChangeEvent<bool> evt)
    {
        bool activado = evt.newValue;
        PlayerPrefs.SetInt("ModoDaltonico", activado ? 1 : 0);
        PlayerPrefs.Save();
        Debug.Log($"{(activado ? "" : "")} Modo daltónico: {(activado ? "Activado" : "Desactivado")}");
    }
    
    private void OnVolverClick()
    {
        Debug.Log("olviendo al menú principal...");
        SceneManager.LoadScene("MenuPrincipalT");
    }
    
    private void OnCreditosClick()
    {
        Debug.Log("Mostrando créditos...");
        // Implementa tu lógica de créditos aquí
    }
    
    private void OnRankingClick()
    {
        Debug.Log("Mostrando ranking...");
        // Implementa tu lógica de ranking aquí
    }
    
    private void OnPerfilClick()
    {
        Debug.Log("Mostrando perfil...");
        SceneManager.LoadScene("PerfilScene");
    }
    
   private void OnCerrarSesionClick()
    {
        Debug.Log("Cerrando sesión...");
        PlayerPrefs.DeleteKey("UsuarioActivo");
        PlayerPrefs.DeleteKey("SesionIniciada");
        PlayerPrefs.Save();
        SceneManager.LoadScene("LoginScene");
    }
    
    // ====================================
    // APLICAR TAMAÑO DE LETRA
    // ====================================
    
    private void AplicarTamanoLetraUI(float porcentaje)
    {
        float escala = porcentaje / 100f;
        Debug.Log($" Aplicando escala de texto: {escala}x");
        
        var todosLosLabels = root.Query<Label>().ToList();
        Debug.Log($"Se encontraron {todosLosLabels.Count} labels para escalar");
        
        foreach (var label in todosLosLabels)
        {
            string nombreLabel = label.name;
            float tamañoBase;
            
            if (PlayerPrefs.HasKey($"TamañoBase_{nombreLabel}"))
            {
                tamañoBase = PlayerPrefs.GetFloat($"TamañoBase_{nombreLabel}");
            }
            else
            {
                if (label.resolvedStyle.fontSize != 0)
                {
                    tamañoBase = label.resolvedStyle.fontSize;
                    PlayerPrefs.SetFloat($"TamañoBase_{nombreLabel}", tamañoBase);
                }
                else
                {
                    continue;
                }
            }
            
            float nuevoTamaño = tamañoBase * escala;
            label.style.fontSize = nuevoTamaño;
        }
        
        PlayerPrefs.Save();
    }
}
    
    
