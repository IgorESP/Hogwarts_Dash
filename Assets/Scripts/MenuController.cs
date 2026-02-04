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
        uiDocument = GetComponent<UIDocument>();
        if (uiDocument == null) return;
        
        var root = uiDocument.rootVisualElement;
        
        // Sincronizar configuraciones visuales de accesibilidad
        if (NivelesManager.Instance != null) NivelesManager.Instance.SincronizarAccesibilidad(root);
        
        // Obtener botones por nombre en el UXML
        botonPlay = root.Q<Button>("Boton_play");
        botonPerfil = root.Q<Button>("Boton_perfil");
        botonAjustes = root.Q<Button>("Boton_Ajustes");
        
        if (botonPlay != null) botonPlay.clicked += OnPlayClicked;
        if (botonPerfil != null) botonPerfil.clicked += OnPerfilClicked;
        if (botonAjustes != null) botonAjustes.clicked += OnAjustesClicked;
    }

    void OnDisable()
    {
        // Limpiar suscripciones para evitar fugas de memoria
        if (botonPlay != null) botonPlay.clicked -= OnPlayClicked;
        if (botonPerfil != null) botonPerfil.clicked -= OnPerfilClicked;
        if (botonAjustes != null) botonAjustes.clicked -= OnAjustesClicked;
    }

    void OnPlayClicked() { Debug.Log("[Menu] Navegando a Selección de Niveles"); SceneManager.LoadScene("PlayScene"); }
    void OnPerfilClicked() { Debug.Log("[Menu] Navegando a Perfil de Jugador"); SceneManager.LoadScene("PerfilScene"); }
    void OnAjustesClicked() { Debug.Log("[Menu] Navegando a Ajustes"); SceneManager.LoadScene("AjustesScene"); }
}