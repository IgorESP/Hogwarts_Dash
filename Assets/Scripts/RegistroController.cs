using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;

/// <summary>
/// Controlador de la pantalla de registro
/// Gestiona la creación de nuevas cuentas de usuario
/// </summary>
public class RegistroUIController : MonoBehaviour
{
    // ====================================
    // REFERENCIAS UI
    // ====================================
    private VisualElement root;
    private TextField campoNom, campoApe, campoPass, campoConf;
    private Button botonCrear, botonVolver;
    private Label mensajeError;

    // ====================================
    // CONFIGURACIÓN DE VALIDACIÓN
    // ====================================
    private const int LONGITUD_MINIMA_NOMBRE = 2;
    private const int LONGITUD_MINIMA_PASSWORD = 6;
    
    // Expresión regular para validar contraseña
    // Debe tener: al menos 1 mayúscula, 1 minúscula y 1 número
    private const string REGEX_PASSWORD = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$";

    // ====================================
    // INICIALIZACIÓN
    // ====================================
    void Start() 
    {
        // Obtener el documento UI
        var uiDoc = GetComponent<UIDocument>();
        root = uiDoc.rootVisualElement;

        // Aplicar configuración de accesibilidad
        AplicarAccesibilidad();

        // Obtener referencias a los elementos
        ObtenerReferencias();

        // Registrar eventos
        RegistrarEventos();
    }

    // ====================================
    // ACCESIBILIDAD
    // ====================================
    
    /// <summary>
    /// Aplica las configuraciones de accesibilidad guardadas
    /// </summary>
    private void AplicarAccesibilidad()
    {
        // Activar modo daltónico si está habilitado
        if (PlayerPrefs.GetInt("ModoDaltonico", 0) == 1) 
        {
            root.AddToClassList("modo-daltonico-activado");
        }

        // Ajustar tamaño de texto
        float escala = PlayerPrefs.GetFloat("TamanoLetra", 100f) / 100f;
        root.Query<Label>().ForEach(l => l.style.fontSize = 25f * escala);
    }

    // ====================================
    // REFERENCIAS UI
    // ====================================
    
    /// <summary>
    /// Obtiene las referencias a todos los elementos de la UI
    /// </summary>
    private void ObtenerReferencias()
    {
        campoNom = root.Q<TextField>("Campo_Nombre");
        campoApe = root.Q<TextField>("Campo_Apellido");
        campoPass = root.Q<TextField>("Campo_Contrasena");
        campoConf = root.Q<TextField>("Campo_Confirmar");
        botonCrear = root.Q<Button>("Boton_Crear");
        botonVolver = root.Q<Button>("Boton_Volver");
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
        botonCrear.clicked += OnCrearClick;
        botonVolver.clicked += OnVolverClick;
    }

    // ====================================
    // VALIDACIÓN Y REGISTRO
    // ====================================
    
    /// <summary>
    /// Maneja el evento de crear cuenta
    /// Valida todos los campos antes de enviar a la API
    /// </summary>
    private void OnCrearClick() 
    {
        // Ocultar errores previos
        OcultarError();

        // Validar nombre
        if (string.IsNullOrEmpty(campoNom.value))
        {
            MostrarError("Por favor,\ningresa tu nombre");
            return;
        }

        if (campoNom.value.Length < LONGITUD_MINIMA_NOMBRE) 
        { 
            MostrarError("Nombre: mínimo\n2 caracteres");
            return; 
        }

        // Validar apellido
        if (string.IsNullOrEmpty(campoApe.value))
        {
            MostrarError("Por favor,\ningresa tu apellido");
            return;
        }

        if (campoApe.value.Length < LONGITUD_MINIMA_NOMBRE) 
        { 
            MostrarError("Apellido: mínimo\n2 caracteres");
            return; 
        }

        // Validar contraseña
        if (string.IsNullOrEmpty(campoPass.value))
        {
            MostrarError("Por favor,\ningresa una contraseña");
            return;
        }

        if (campoPass.value.Length < LONGITUD_MINIMA_PASSWORD) 
        { 
            MostrarError("Contraseña: mínimo\n6 caracteres");
            return; 
        }

        // Validar complejidad de contraseña
        if (!Regex.IsMatch(campoPass.value, REGEX_PASSWORD)) 
        {
            MostrarError("Debe tener mayúscula,\nminúscula y número");
            return;
        }

        // Validar confirmación
        if (string.IsNullOrEmpty(campoConf.value))
        {
            MostrarError("Por favor,\nconfirma tu contraseña");
            return;
        }

        if (campoPass.value != campoConf.value) 
        { 
            MostrarError("Las contraseñas\nno coinciden");
            return; 
        }

        // Si todas las validaciones pasaron, crear cuenta
        CrearCuenta();
    }

    /// <summary>
    /// Crea la cuenta de usuario llamando a la API
    /// </summary>
    private void CrearCuenta()
    {
        // Deshabilitar botón mientras se procesa
        botonCrear.SetEnabled(false);
        botonCrear.text = "Creando...";

        // Generar nombre de usuario a partir del nombre
        string usuario = campoNom.value.ToLower().Replace(" ", "");

        // Llamar a la API para registrar
        ApiManager.Instance.Registrar(
            usuario, 
            campoNom.value, 
            campoApe.value, 
            campoPass.value, 
            OnRegistroRespuesta
        );
    }

    /// <summary>
    /// Callback que se ejecuta cuando la API responde
    /// </summary>
    private void OnRegistroRespuesta(bool exito, string mensaje)
    {
        // Rehabilitar botón
        botonCrear.SetEnabled(true);
        botonCrear.text = "CREAR CUENTA";

        if (exito) 
        {
            // Registro exitoso - volver al login
            Debug.Log("✅ Cuenta creada - Redirigiendo al login");
            SceneManager.LoadScene("LoginScene");
        } 
        else 
        {
            // Registro fallido - mostrar error
            MostrarError(mensaje);
        }
    }

    // ====================================
    // NAVEGACIÓN
    // ====================================
    
    /// <summary>
    /// Regresa a la pantalla de login
    /// </summary>
    private void OnVolverClick()
    {
        SceneManager.LoadScene("LoginScene");
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
            mensajeError.style.whiteSpace = WhiteSpace.Normal; // Permitir saltos de línea
            
            Debug.LogWarning("Registro Error: " + mensaje);
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