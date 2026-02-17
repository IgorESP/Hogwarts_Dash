using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;

/// <summary>
/// Controlador de la interfaz de registro de nuevos usuarios
/// </summary>
public class RegistroUIController : MonoBehaviour
{
    private VisualElement root;

    // Campos de entrada
    private TextField campoNom, campoApe, campoUsuario, campoPass, campoConf;

    // Botones
    private Button botonCrear, botonVolver;

    // Mensaje de error
    private Label mensajeError;

    // Constantes de validación
    private const int LONGITUD_MIN_NOMBRE = 2;
    private const int LONGITUD_MIN_PASS = 6;

    // Reglas de contraseña: mayúscula + minúscula + número
    private const string REGEX_PASS = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$";

    void Start() 
    {
        Debug.Log("[Registro] Inicializando pantalla de registro");

        var uiDoc = GetComponent<UIDocument>();
        if (uiDoc != null) 
            root = uiDoc.rootVisualElement;

        AplicarAccesibilidad();
        ObtenerReferencias();
        RegistrarEventos();
    }

    /// <summary> 
    /// Aplica ajustes de accesibilidad guardados en PlayerPrefs. 
    /// </summary>
    private void AplicarAccesibilidad()
    {
        if (root == null) return;

        if (PlayerPrefs.GetInt("ModoDaltonico", 0) == 1) 
            root.AddToClassList("modo-daltonico-activado");

        float escala = PlayerPrefs.GetFloat("TamanoLetra", 100f) / 100f;
        root.Query<Label>().ForEach(l => l.style.fontSize = 25f * escala);
    }

    /// <summary> 
    /// Ajusta el estilo interno del TextField para mejorar visibilidad. 
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
    /// Obtiene referencias a todos los elementos del UXML. 
    /// </summary>
    private void ObtenerReferencias()
    {
        if (root == null) return;

        campoNom = root.Q<TextField>("Campo_Nombre");
        campoApe = root.Q<TextField>("Campo_Apellido");
        campoUsuario = root.Q<TextField>("Campo_Usuario");
        campoPass = root.Q<TextField>("Campo_Contrasena");
        campoConf = root.Q<TextField>("Campo_Confirmar");

        botonCrear = root.Q<Button>("Boton_Crear");
        botonVolver = root.Q<Button>("Boton_Volver");

        mensajeError = root.Q<Label>("Mensaje_Error");

        // Aplicar estilo a todos los campos
        ForzarColorTexto(campoNom); 
        ForzarColorTexto(campoApe); 
        ForzarColorTexto(campoUsuario);
        ForzarColorTexto(campoPass); 
        ForzarColorTexto(campoConf);
    }

    /// <summary> 
    /// Registra los eventos de los botones. 
    /// </summary>
    private void RegistrarEventos()
    {
        if (botonCrear != null) 
            botonCrear.clicked += OnCrearClick;

        if (botonVolver != null) 
            botonVolver.clicked += () => SceneManager.LoadScene("LoginScene");
    }

    /// <summary> 
    /// Lógica principal al pulsar "Crear cuenta". 
    /// Incluye todas las validaciones de campos. 
    /// </summary>
    private void OnCrearClick() 
    {
        OcultarError();

        // Validaciones de nombre y apellido
        if (string.IsNullOrEmpty(campoNom.value) || campoNom.value.Length < LONGITUD_MIN_NOMBRE) 
        { 
            MostrarError("Nombre inválido"); 
            return; 
        }
        if (string.IsNullOrEmpty(campoApe.value) || campoApe.value.Length < LONGITUD_MIN_NOMBRE) 
        { 
            MostrarError("Apellido inválido"); 
            return; 
        }

        // Validación de usuario
        if (string.IsNullOrEmpty(campoUsuario.value) || campoUsuario.value.Length < 3) 
        { 
            MostrarError("Usuario muy corto"); 
            return; 
        }

        // Validación de contraseña
        if (string.IsNullOrEmpty(campoPass.value) || campoPass.value.Length < LONGITUD_MIN_PASS) 
        { 
            MostrarError("Password muy corto"); 
            return; 
        }
        if (!Regex.IsMatch(campoPass.value, REGEX_PASS)) 
        { 
            MostrarError("Password debe incluir Mayus, minus y número"); 
            return; 
        }

        // Confirmación de contraseña
        if (campoPass.value != campoConf.value) 
        { 
            MostrarError("Las contraseñas no coinciden"); 
            return; 
        }

        // Si todo está correcto, ejecutar registro
        EjecutarRegistro();
    }

    /// <summary> 
    /// Envía los datos al servidor mediante ApiManager. 
    /// </summary>
    private void EjecutarRegistro()
    {
        botonCrear.SetEnabled(false);
        botonCrear.text = "Enviando...";

        Debug.Log($"[Registro] Solicitando registro para usuario: {campoUsuario.value}");

        ApiManager.Instance.Registrar(
            campoUsuario.value.ToLower().Trim(), 
            campoNom.value, 
            campoApe.value, 
            campoPass.value, 
            (exito, msg) => 
            {
                botonCrear.SetEnabled(true);
                botonCrear.text = "CREAR CUENTA";

                if (exito) 
                { 
                    Debug.Log("[Registro] Usuario creado con éxito"); 
                    PlayerPrefs.SetString("MensajeExito", "¡Cuenta creada!");
                    SceneManager.LoadScene("LoginScene"); 
                }
                else 
                { 
                    Debug.LogWarning($"[Registro] Fallo en registro: {msg}"); 
                    MostrarError(msg); 
                }
            }
        );
    }

    /// <summary> 
    /// Muestra un mensaje de error en pantalla. 
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
    /// Oculta el mensaje de error. 
    /// </summary>
    private void OcultarError() 
    { 
        if (mensajeError != null) 
            mensajeError.style.display = DisplayStyle.None; 
    }
}