using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class LoginUIController : MonoBehaviour
{
    private VisualElement root;
    private TextField campoUsuario, campoContrasena;
    private Button botonLogin, botonRegistrarse;
    private Label mensajeError;

    void Start() {
        var uiDoc = GetComponent<UIDocument>();
        root = uiDoc.rootVisualElement;

        // --- APLICAR AJUSTES DE ACCESIBILIDAD ---
        // 1. Solo aplica el color si está activado (ModoDaltonico == 1)
        if (PlayerPrefs.GetInt("ModoDaltonico", 0) == 1) {
            root.AddToClassList("modo-daltonico-activado");
        }

        // 2. Solo cambia el tamaño si el usuario movió el slider
        float porcentaje = PlayerPrefs.GetFloat("TamanoLetra", 100f);
        float escala = porcentaje / 100f;
        root.Query<Label>().ForEach(label => {
            label.style.fontSize = 24f * escala; // 24px es el tamaño base
        });

        campoUsuario = root.Q<TextField>("Campo_Usuario");
        campoContrasena = root.Q<TextField>("Campo_Contrasena");
        botonLogin = root.Q<Button>("Boton_IniciarSesion");
        botonRegistrarse = root.Q<Button>("Boton_Registrarse");
        mensajeError = root.Q<Label>("Mensaje_Error");

        botonLogin.clicked += OnLoginClick;
        botonRegistrarse.clicked += () => SceneManager.LoadScene("RegistroScene");
    }

    private void OnLoginClick() {
        if (string.IsNullOrEmpty(campoUsuario.value)) return;
        botonLogin.SetEnabled(false);
        ApiManager.Instance.Login(campoUsuario.value, campoContrasena.value, (exito, msg, datos) => {
            botonLogin.SetEnabled(true);
            if (exito) SceneManager.LoadScene("MenuPrincipalT");
            else MostrarError(msg);
        });
    }

    private void MostrarError(string msg) {
        if (mensajeError != null) { mensajeError.text = "✗ " + msg; mensajeError.style.display = DisplayStyle.Flex; }
    }
}