using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

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

    private void AplicarAccesibilidad()
    {
        if (root == null) return;
        if (PlayerPrefs.GetInt("ModoDaltonico", 0) == 1) root.AddToClassList("modo-daltonico-activado");

        float escala = PlayerPrefs.GetFloat("TamanoLetra", 100f) / 100f;
        root.Query<Label>().ForEach(label => label.style.fontSize = 24f * escala);
    }

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

    private void RegistrarEventos()
    {
        if (botonLogin != null)      botonLogin.clicked      += OnLoginClick;
        if (botonRegistrarse != null) botonRegistrarse.clicked += OnRegistrarseClick;
    }

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

    // ✅ CORREGIDO: ApiManager.UserData en vez de dynamic
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

    private void OnRegistrarseClick() => SceneManager.LoadScene("RegistroScene");

    private void MostrarError(string msg)
    {
        if (mensajeError != null) { mensajeError.text = msg; mensajeError.style.display = DisplayStyle.Flex; }
    }

    private void OcultarError()
    {
        if (mensajeError != null) mensajeError.style.display = DisplayStyle.None;
    }
}