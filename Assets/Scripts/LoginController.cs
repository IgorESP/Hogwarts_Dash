using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

/// <summary>
/// Controlador de la interfaz de inicio de sesión
/// </summary>
public class LoginUIController : MonoBehaviour
{
    private VisualElement root;
    private TextField campoUsuario, campoContrasena;
    private Button botonLogin, botonRegistrarse;
    private Label mensajeError;

    void Start()
    {
        Debug.Log("[Login] Inicializando pantalla de login");
        var uiDoc = GetComponent<UIDocument>();
        if (uiDoc != null) root = uiDoc.rootVisualElement;

        AplicarAccesibilidad();
        ObtenerReferencias();
        RegistrarEventos();
    }

    /// <summary>
    /// Forzado de estilo para inputs de texto (compatibilidad visual)
    /// </summary>
    private void ForzarColorTexto(TextField tf)
    {
        if (tf == null) return;
        var input = tf.Q<VisualElement>("unity-text-input"); 
        if (input != null) 
        {
            input.style.color = Color.black; 
            input.style.unityFontStyleAndWeight = FontStyle.Bold; 
        } 
    }

    /// <summary> 
    /// Aplica ajustes de accesibilidad guardados en PlayerPrefs. 
    /// </summary>
    private void AplicarAccesibilidad()
    {
        if (root == null) return;

        // Activar modo daltónico si está habilitado
        if (PlayerPrefs.GetInt("ModoDaltonico", 0) == 1) 
            root.AddToClassList("modo-daltonico-activado");

        // Ajustar tamaño de letra global
        float porcentaje = PlayerPrefs.GetFloat("TamanoLetra", 100f);
        float escala = porcentaje / 100f;

        root.Query<Label>().ForEach(label => {
            label.style.fontSize = 24f * escala; 
        });
    }

    /// <summary> 
    /// Obtiene referencias a los elementos del UXML. 
    /// </summary>
    private void ObtenerReferencias()
    {
        if (root == null) return;
        campoUsuario = root.Q<TextField>("Campo_Usuario");
        campoContrasena = root.Q<TextField>("Campo_Contrasena");
        botonLogin = root.Q<Button>("Boton_IniciarSesion");
        botonRegistrarse = root.Q<Button>("Boton_Registrarse");
        mensajeError = root.Q<Label>("Mensaje_Error");

        // Ajustar estilo de los inputs
        ForzarColorTexto(campoUsuario); 
        ForzarColorTexto(campoContrasena);
    }

    /// <summary> 
    /// Registra los eventos de los botones. 
    /// </summary>
    private void RegistrarEventos()
    {
        if (botonLogin != null) botonLogin.clicked += OnLoginClick;
        if (botonRegistrarse != null) botonRegistrarse.clicked += OnRegistrarseClick;
    }

    /// <summary> 
    /// Lógica al pulsar el botón de iniciar sesión. 
    /// </summary>
    private void OnLoginClick()
    {
        OcultarError();

        // Validación básica
        if (string.IsNullOrEmpty(campoUsuario.value) || string.IsNullOrEmpty(campoContrasena.value))
        {
            MostrarError("Por favor, rellena todos los campos");
            return;
        }

        // Desactivar botón mientras se procesa
        botonLogin.SetEnabled(false);
        botonLogin.text = "Cargando...";

        Debug.Log($"[Login] Intentando acceder con el usuario: {campoUsuario.value}");

        // Llamada al backend
        ApiManager.Instance.Login(campoUsuario.value, campoContrasena.value, OnLoginRespuesta);
    }

    /// <summary> 
    /// Callback que recibe la respuesta del servidor tras intentar iniciar sesión. 
    /// </summary>
    private void OnLoginRespuesta(bool exito, string mensaje, dynamic datos)
    {
        // Restaurar botón
        if (botonLogin != null)
        {
            botonLogin.SetEnabled(true);
            botonLogin.text = "ENTRAR";
        }

        if (exito)
        {
            Debug.Log("[Login] Sesión iniciada con éxito");
            SceneManager.LoadScene("MenuPrincipalT");
        }
        else
        {
            Debug.LogWarning($"[Login] Error de autenticación: {mensaje}");
            MostrarError(mensaje);
        }
    }

    /// <summary> 
    /// Navega a la escena de registro. 
     /// </summary>
    private void OnRegistrarseClick() => SceneManager.LoadScene("RegistroScene");

    /// <summary> 
    /// Muestra un mensaje de error en pantalla. 
    /// </summary>
    private void MostrarError(string mensaje)
    {
        if (mensajeError != null)
        {
            mensajeError.text = mensaje;
            mensajeError.style.display = DisplayStyle.Flex;
        }
    }

    /// <summary> 
    /// Oculta el mensaje de error. 
    /// </summary>
    private void OcultarError()
    {
        if (mensajeError != null) 
            mensajeError.style.display = DisplayStyle.None;
    }
}