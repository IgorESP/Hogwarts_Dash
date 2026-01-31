using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using System.Linq;

public class LoginUIController : MonoBehaviour
{
    private UIDocument uiDocument;
    private VisualElement root;
    private TextField campoUsuario;
    private TextField campoContrasena;
    private Button botonIniciarSesion;
    private Button botonRegistrarse;
    private Label mensajeError;
    
    void Start()
    {
        Debug.Log("LoginUIController iniciado");
        uiDocument = GetComponent<UIDocument>();
        if (uiDocument == null) { Debug.LogError("UIDocument faltante"); return; }
        
        root = uiDocument.rootVisualElement;
        campoUsuario = root.Q<TextField>("Campo_Usuario");
        campoContrasena = root.Q<TextField>("Campo_Contrasena");
        botonIniciarSesion = root.Q<Button>("Boton_IniciarSesion");
        botonRegistrarse = root.Q<Button>("Boton_Registrarse");
        mensajeError = root.Q<Label>("Mensaje_Error");
        
        if (campoUsuario == null || campoContrasena == null || botonIniciarSesion == null) 
            return;
            
        botonIniciarSesion.clicked += OnIniciarSesionClick;
        botonRegistrarse.clicked += OnRegistrarseClick;
        campoUsuario.RegisterValueChangedCallback(evt => OcultarError());
        campoContrasena.RegisterValueChangedCallback(evt => OcultarError());
    }
    
    private void OnIniciarSesionClick()
    {
        string usuario = campoUsuario.value;
        string contrasena = campoContrasena.value;
        
        if (string.IsNullOrEmpty(usuario) || string.IsNullOrEmpty(contrasena)) 
        {
            MostrarError("Usuario y contraseña requeridos");
            return;
        }
        
        if (ValidarLogin(usuario, contrasena))
        {
            PlayerPrefs.SetString("UsuarioActivo", usuario);
            PlayerPrefs.SetInt("SesionIniciada", 1);
            PlayerPrefs.Save();
            SceneManager.LoadScene("MenuPrincipalT");
        }
        else
        {
            MostrarError("Usuario o contraseña incorrectos");
        }
    }
    
    private void OnRegistrarseClick()
    {
        SceneManager.LoadScene("RegistroScene");
    }
    
    private bool ValidarLogin(string usuario, string contrasena)
    {
        string contrasenaGuardada = PlayerPrefs.GetString($"User_{usuario}_Pass", "");
        return contrasena == contrasenaGuardada && !string.IsNullOrEmpty(contrasenaGuardada);
    }
    
    private void MostrarError(string mensaje)
    {
        if (mensajeError != null)
        {
            mensajeError.text = $"✗ {mensaje}";
            mensajeError.style.display = DisplayStyle.Flex;
        }
    }
    
    private void OcultarError()
    {
        if (mensajeError != null)
            mensajeError.style.display = DisplayStyle.None;
    }
}
