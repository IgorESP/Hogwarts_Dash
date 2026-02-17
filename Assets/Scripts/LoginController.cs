using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

/// <summary>
/// Controlador de la interfaz de inicio de sesión
/// </summary>
public class LoginUIController : MonoBehaviour
{
    private VisualElement root;

    // Campos de entrada
    private TextField campoUsuario, campoContrasena;

    // Botones
    private Button botonLogin, botonRegistrarse;

    // Mensaje de error/éxito
    private Label mensajeError;

    void Start()
    {
        Debug.Log("[Login] Inicializando pantalla de login");
        var uiDoc = GetComponent<UIDocument>();
        if (uiDoc != null) root = uiDoc.rootVisualElement;

        AplicarAccesibilidad();
        ObtenerReferencias();
        RegistrarEventos();

        // Si viene de un registro exitoso, mostrar mensaje en verde
        string mensajeExito = PlayerPrefs.GetString("MensajeExito", "");
        if (!string.IsNullOrEmpty(mensajeExito))
        {
            mensajeError.style.color = new StyleColor(Color.green);
            MostrarError(mensajeExito);
            PlayerPrefs.DeleteKey("MensajeExito");
        }
    }

    /// <summary>
    /// Ajusta el color y grosor del texto dentro de un TextField para mejorar visibilidad
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
    /// Aplica ajustes de accesibilidad guardados en PlayerPrefs
    /// </summary>
    private void AplicarAccesibilidad()
    {
        if (root == null) return;
        if (PlayerPrefs.GetInt("ModoDaltonico", 0) == 1) root.AddToClassList("modo-daltonico-activado");

        float escala = PlayerPrefs.GetFloat("TamanoLetra", 100f) / 100f;
        root.Query<Label>().ForEach(label => label.style.fontSize = 24f * escala);
    }

    /// <summary>
    /// Obtiene referencias a todos los elementos del UXML
    /// </summary>
    private void ObtenerReferencias()
    {
        if (root == null) return;
        campoUsuario    = root.Q<TextField>("Campo_Usuario");
        campoContrasena = root.Q<TextField>("Campo_Contrasena");
        botonLogin      = root.Q<Button>("Boton_IniciarSesion");
        botonRegistrarse= root.Q<Button>("Boton_Registrarse");
        mensajeError    = root.Q<Label>("Mensaje_Error");

        ForzarColorTexto(campoUsuario);
        ForzarColorTexto(campoContrasena);
    }

    /// <summary>
    /// Registra los eventos de los botones
    /// </summary>
    private void RegistrarEventos()
    {
        if (botonLogin != null)       botonLogin.clicked       += OnLoginClick;
        if (botonRegistrarse != null) botonRegistrarse.clicked += OnRegistrarseClick;
    }

    /// <summary>
    /// Lógica al pulsar "Entrar": valida campos y llama a la API
    /// </summary>
    private void OnLoginClick()
    {
        OcultarError();

        if (string.IsNullOrEmpty(campoUsuario.value) || string.IsNullOrEmpty(campoContrasena.value))
        {
            MostrarError("Por favor, rellena todos los campos");
            return;
        }

        botonLogin.SetEnabled(false);
        botonLogin.text = "Cargando...";

        Debug.Log($"[Login] Intentando acceder con: {campoUsuario.value}");
        ApiManager.Instance.Login(campoUsuario.value.ToLower().Trim(), campoContrasena.value, OnLoginRespuesta);
    }

    /// <summary>
    /// Callback con la respuesta de la API tras intentar el login
    /// </summary>
    private void OnLoginRespuesta(bool exito, string mensaje, ApiManager.UserData datos)
    {
        botonLogin?.SetEnabled(true);
        if (botonLogin != null) botonLogin.text = "ENTRAR";

        if (exito)
        {
            Debug.Log($"[Login] Sesión iniciada: {datos?.username}");
            SceneManager.LoadScene("MenuPrincipalT");
        }
        else
        {
            Debug.LogWarning($"[Login] Error: {mensaje}");
            MostrarError(mensaje);
        }
    }

    /// <summary>
    /// Navega a la pantalla de registro
    /// </summary>
    private void OnRegistrarseClick() => SceneManager.LoadScene("RegistroScene");

    /// <summary>
    /// Muestra un mensaje en el label de error/éxito
    /// </summary>
    private void MostrarError(string msg)
    {
        if (mensajeError != null) 
        { 
            mensajeError.text = msg; 
            mensajeError.style.display = DisplayStyle.Flex; 
        }
    }

    /// <summary>
    /// Oculta el mensaje de error/éxito
    /// </summary>
    private void OcultarError()
    {
        if (mensajeError != null) 
            mensajeError.style.display = DisplayStyle.None;
    }
}