using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

/// <summary>
/// Controlador del menú principal del juego
/// </summary>
public class MenuController : MonoBehaviour
{
    private UIDocument uiDocument;
    private Button botonPlay, botonPerfil, botonAjustes;

    void OnEnable()
    {
        Debug.Log("[Menu] Cargando menú principal");

        // Obtener el UIDocument del objeto
        uiDocument = GetComponent<UIDocument>();
        if (uiDocument == null) return;
        
        // Obtener el root del UXML
        var root = uiDocument.rootVisualElement;
        
        // Aplicar accesibilidad global (modo daltónico, tamaño de letra, etc.) 
        // Esto lo gestiona NivelesManager para mantener coherencia entre escenas
        if (NivelesManager.Instance != null) 
            NivelesManager.Instance.SincronizarAccesibilidad(root);
        
        // Buscar botones por nombre definido en el UXML
        botonPlay = root.Q<Button>("Boton_play");
        botonPerfil = root.Q<Button>("Boton_perfil");
        botonAjustes = root.Q<Button>("Boton_Ajustes");
        
        // Registrar eventos de clic
        if (botonPlay != null) botonPlay.clicked += OnPlayClicked;
        if (botonPerfil != null) botonPerfil.clicked += OnPerfilClicked;
        if (botonAjustes != null) botonAjustes.clicked += OnAjustesClicked;
    }

    void OnDisable()
    {
        /// Importante: limpiar eventos para evitar fugas de memoria
        if (botonPlay != null) botonPlay.clicked -= OnPlayClicked;
        if (botonPerfil != null) botonPerfil.clicked -= OnPerfilClicked;
        if (botonAjustes != null) botonAjustes.clicked -= OnAjustesClicked;
    }

    /// <summary> 
    /// Navega a la escena de selección de niveles. 
    /// </summary>
    void OnPlayClicked() 
    { 
        Debug.Log("[Menu] Navegando a Selección de Niveles"); 
        SceneManager.LoadScene("PlayScene"); 
    }

    /// <summary> 
    /// Navega a la escena del perfil del jugador. 
    /// </summary>
    void OnPerfilClicked() 
    { 
        Debug.Log("[Menu] Navegando a Perfil de Jugador"); 
        SceneManager.LoadScene("PerfilScene"); 
    }

    /// <summary> 
    /// Navega a la escena de ajustes. 
    /// </summary>
    void OnAjustesClicked() 
    { 
        Debug.Log("[Menu] Navegando a Ajustes"); 
        SceneManager.LoadScene("AjustesScene"); 
    }
}