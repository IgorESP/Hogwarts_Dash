using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    private UIDocument uiDocument;
    private Button botonPlay;
    private Button botonPerfil;
    private Button botonAjustes;

    void OnEnable()
    {
        // Obtener el UIDocument
        uiDocument = GetComponent<UIDocument>();
        
        // Obtener la raíz del documento
        var root = uiDocument.rootVisualElement;
        
        // Buscar los botones por su nombre
        botonPlay = root.Q<Button>("Boton_play");
        botonPerfil = root.Q<Button>("Boton_perfil");
        botonAjustes = root.Q<Button>("Boton_Ajustes");
        
        // Registrar los eventos de click
        if (botonPlay != null)
            botonPlay.clicked += OnPlayClicked;
            
        if (botonPerfil != null)
            botonPerfil.clicked += OnPerfilClicked;
            
        if (botonAjustes != null)
            botonAjustes.clicked += OnAjustesClicked;
    }

    void OnDisable()
    {
        // Desregistrar los eventos cuando se desactive
        if (botonPlay != null)
            botonPlay.clicked -= OnPlayClicked;
            
        if (botonPerfil != null)
            botonPerfil.clicked -= OnPerfilClicked;
            
        if (botonAjustes != null)
            botonAjustes.clicked -= OnAjustesClicked;
    }

    // Función cuando se hace click en Play
    void OnPlayClicked()
    {
        Debug.Log("Botón Play presionado");
        // Aquí cargarías tu escena del juego
        // SceneManager.LoadScene("JuegoScene");
        SceneManager.LoadScene("PlayScene");
    }

    // Función cuando se hace click en Perfil
    void OnPerfilClicked()
    {
        Debug.Log("Botón Perfil presionado");
        SceneManager.LoadScene("PerfilScene");
    }

    // Función cuando se hace click en Ajustes
    void OnAjustesClicked()
    {
        Debug.Log("Botón Ajustes presionado");
        SceneManager.LoadScene("AjustesScene");
    }
}
