using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

/// <summary>
/// Controlador de la pantalla de inicio de sesión
/// Gestiona la autenticación del usuario a través de la API
/// </summary>
public class LoginUIController : MonoBehaviour
{
    // ====================================
    // REFERENCIAS UI
    // ====================================
    private VisualElement root;
    private TextField campoUsuario, campoContrasena;
    private Button botonLogin, botonRegistrarse;
    private Label mensajeError;

    // ====================================
    // INICIALIZACIÓN
    // ====================================
    void Start() 
    {
        // Obtener el documento UI
        var uiDoc = GetComponent<UIDocument>();
        root = uiDoc.rootVisualElement;

        // Aplicar configuración de accesibilidad guardada
        AplicarAccesibilidad();

        // Obtener referencias a los elementos de la UI
        ObtenerReferencias();

        // Registrar eventos de botones
        RegistrarEventos();
    }

    // ====================================
    // ACCESIBILIDAD
    // ====================================
    
    /// <summary>
    /// Aplica las configuraciones de accesibilidad guardadas
    /// (Modo daltónico y tamaño de texto)
    /// </summary>
    private void AplicarAccesibilidad()
    {
        // Activar modo daltónico si está habilitado
        if (PlayerPrefs.GetInt("ModoDaltonico", 0) == 1) 
        {
            root.AddToClassList("modo-daltonico-activado");
        }

        // Ajustar tamaño de texto según preferencia del usuario
        float porcentaje = PlayerPrefs.GetFloat("TamanoLetra", 100f);
        float escala = porcentaje / 100f;
        
        root.Query<Label>().ForEach(label => {
            label.style.fontSize = 24f * escala; // 24px es el tamaño base
        });
    }

    // ====================================
    // REFERENCIAS UI
    // ====================================
    
    /// <summary>
    /// Obtiene las referencias a todos los elementos de la UI
    /// </summary>
    private void ObtenerReferencias()
    {
        campoUsuario = root.Q<TextField>("Campo_Usuario");
        campoContrasena = root.Q<TextField>("Campo_Contrasena");
        botonLogin = root.Q<Button>("Boton_IniciarSesion");
        botonRegistrarse = root.Q<Button>("Boton_Registrarse");
        mensajeError = root.Q<Label>("Mensaje_Error");
    }

    // ====================================
    // EVENTOS
    // ====================================
    
    /// <summary>
    /// Registra los eventos de los botones
    /// </summary>
    private void RegistrarEventos()
    {
        botonLogin.clicked += OnLoginClick;
        botonRegistrarse.clicked += OnRegistrarseClick;
    }

    // ====================================
    // MANEJO DE LOGIN
    // ====================================
    
    /// <summary>
    /// Maneja el evento de click en el botón de login
    /// Valida los campos y llama a la API para autenticar
    /// </summary>
    private void OnLoginClick() 
    {
        // Ocultar mensaje de error previo
        OcultarError();

        // Validar que los campos no estén vacíos
        if (string.IsNullOrEmpty(campoUsuario.value)) 
        {
            MostrarError("Por favor,\ningresa tu usuario");
            return;
        }

        if (string.IsNullOrEmpty(campoContrasena.value)) 
        {
            MostrarError("Por favor,\ningresa tu contraseña");
            return;
        }

        // Deshabilitar botón mientras se procesa
        botonLogin.SetEnabled(false);
        botonLogin.text = "Iniciando...";

        // Llamar a la API para autenticar
        ApiManager.Instance.Login(
            campoUsuario.value, 
            campoContrasena.value, 
            OnLoginRespuesta
        );
    }

    /// <summary>
    /// Callback que se ejecuta cuando la API responde
    /// </summary>
    private void OnLoginRespuesta(bool exito, string mensaje, dynamic datos)
    {
        // Rehabilitar botón
        botonLogin.SetEnabled(true);
        botonLogin.text = "ENTRAR";

        if (exito) 
        {
            // Login exitoso - ir al menú principal
            Debug.Log("✅ Login exitoso - Redirigiendo");
            SceneManager.LoadScene("MenuPrincipalT");
        } 
        else 
        {
            // Login fallido - mostrar error
            MostrarError(mensaje);
        }
    }

    // ====================================
    // NAVEGACIÓN
    // ====================================
    
    /// <summary>
    /// Navega a la pantalla de registro
    /// </summary>
    private void OnRegistrarseClick()
    {
        SceneManager.LoadScene("RegistroScene");
    }

    // ====================================
    // MENSAJES DE ERROR
    // ====================================
    
    /// <summary>
    /// Muestra un mensaje de error en la pantalla
    /// </summary>
    private void MostrarError(string mensaje) 
    {
        if (mensajeError != null) 
        {
            mensajeError.text = mensaje;
            mensajeError.style.display = DisplayStyle.Flex;
            mensajeError.style.whiteSpace = WhiteSpace.Normal;
            
            Debug.LogWarning("Login Error: " + mensaje);
        }
    }

    /// <summary>
    /// Oculta el mensaje de error
    /// </summary>
    private void OcultarError()
    {
        if (mensajeError != null)
        {
            mensajeError.style.display = DisplayStyle.None;
        }
    }
}