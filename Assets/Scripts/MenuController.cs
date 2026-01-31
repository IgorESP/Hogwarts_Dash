using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    private UIDocument uiDocument;
    private Button botonPlay, botonPerfil, botonAjustes;

    void OnEnable()
    {
        uiDocument = GetComponent<UIDocument>();
        var root = uiDocument.rootVisualElement;
        
        // --- APLICAR ACCESIBILIDAD ---
        if (NivelesManager.Instance != null) NivelesManager.Instance.SincronizarAccesibilidad(root);
        
        botonPlay = root.Q<Button>("Boton_play");
        botonPerfil = root.Q<Button>("Boton_perfil");
        botonAjustes = root.Q<Button>("Boton_Ajustes");
        
        if (botonPlay != null) botonPlay.clicked += OnPlayClicked;
        if (botonPerfil != null) botonPerfil.clicked += OnPerfilClicked;
        if (botonAjustes != null) botonAjustes.clicked += OnAjustesClicked;
    }

    void OnDisable()
    {
        if (botonPlay != null) botonPlay.clicked -= OnPlayClicked;
        if (botonPerfil != null) botonPerfil.clicked -= OnPerfilClicked;
        if (botonAjustes != null) botonAjustes.clicked -= OnAjustesClicked;
    }

    void OnPlayClicked() => SceneManager.LoadScene("PlayScene");
    void OnPerfilClicked() => SceneManager.LoadScene("PerfilScene");
    void OnAjustesClicked() => SceneManager.LoadScene("AjustesScene");
}