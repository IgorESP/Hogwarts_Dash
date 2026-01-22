using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using System.Linq;

public class RegistroUIController : MonoBehaviour
{
    private UIDocument uiDocument;
    private VisualElement root;
    
    // Campos de texto
    private TextField campoNombre;
    private TextField campoApellido;
    private TextField campoContrasena;
    private TextField campoConfirmar;
    
    // Botones
    private Button botonCrear;
    private Button botonVolver;
    
    // Mensaje de error
    private Label mensajeError;
    
    void Start()
    {
        Debug.Log("🎬 RegistroUIController - Start() ejecutándose...");
        
        // Obtener referencias
        uiDocument = GetComponent<UIDocument>();
        root = uiDocument.rootVisualElement;
        
        // Obtener campos
        campoNombre = root.Q<TextField>("Campo_Nombre");
        campoApellido = root.Q<TextField>("Campo_Apellido");
        campoContrasena = root.Q<TextField>("Campo_Contrasena");
        campoConfirmar = root.Q<TextField>("Campo_Confirmar");
        
        // Obtener botones
        botonCrear = root.Q<Button>("Boton_Crear");
        botonVolver = root.Q<Button>("Boton_Volver");
        
        // Obtener mensaje de error
        mensajeError = root.Q<Label>("Mensaje_Error");
        
        // Registrar eventos
        botonCrear.clicked += OnCrearClick;
        botonVolver.clicked += OnVolverClick;
        
        // Ocultar error al escribir
        campoNombre.RegisterValueChangedCallback(evt => OcultarError());
        campoApellido.RegisterValueChangedCallback(evt => OcultarError());
        campoContrasena.RegisterValueChangedCallback(evt => OcultarError());
        campoConfirmar.RegisterValueChangedCallback(evt => OcultarError());
        
        Debug.Log("✅ Pantalla de Registro lista");
    }
    
    private void OnCrearClick()
    {
        Debug.Log("🔥🔥🔥 BOTÓN CREAR CUENTA CLICKEADO 🔥🔥🔥");
        
        string nombre = campoNombre.value;
        string apellido = campoApellido.value;
        string contrasena = campoContrasena.value;
        string confirmar = campoConfirmar.value;
        
        // VALIDACIÓN 1: Campos vacíos
        if (string.IsNullOrEmpty(nombre))
        {
            MostrarError("⚠️ Por favor ingresa tu nombre");
            return;
        }
        
        if (string.IsNullOrEmpty(apellido))
        {
            MostrarError("⚠️ Por favor ingresa tu apellido");
            return;
        }
        
        if (string.IsNullOrEmpty(contrasena))
        {
            MostrarError("⚠️ Por favor ingresa una contraseña");
            return;
        }
        
        if (string.IsNullOrEmpty(confirmar))
        {
            MostrarError("⚠️ Por favor confirma tu contraseña");
            return;
        }
        
        // VALIDACIÓN 2: Longitud mínima
        if (nombre.Length < 2)
        {
            MostrarError("⚠️ El nombre debe tener al menos 2 caracteres");
            return;
        }
        
        if (apellido.Length < 2)
        {
            MostrarError("⚠️ El apellido debe tener al menos 2 caracteres");
            return;
        }
        
        if (contrasena.Length < 4)
        {
            MostrarError("⚠️ La contraseña debe tener al menos 4 caracteres");
            return;
        }
        
        // VALIDACIÓN 3: Contraseñas coinciden
        if (contrasena != confirmar)
        {
            MostrarError("❌ Las contraseñas no coinciden");
            return;
        }
        
        // VALIDACIÓN 4: Seguridad de contraseña
        if (!contrasena.Any(char.IsUpper))
        {
            MostrarError("⚠️ La contraseña debe contener al menos una MAYÚSCULA");
            return;
        }
        
        if (!contrasena.Any(char.IsLower))
        {
            MostrarError("⚠️ La contraseña debe contener al menos una minúscula");
            return;
        }
        
        if (!contrasena.Any(char.IsDigit))
        {
            MostrarError("⚠️ La contraseña debe contener al menos un NÚMERO");
            return;
        }
        
        if (!contrasena.Any(c => !char.IsLetterOrDigit(c)))
        {
            MostrarError("⚠️ La contraseña debe contener al menos un SÍMBOLO (@, #, $, etc.)");
            return;
        }
        
        // Crear el nombre de usuario combinando nombre y apellido
        string usuario = $"{nombre}.{apellido}".ToLower();
        
        // VALIDACIÓN 5: Usuario ya existe
        if (PlayerPrefs.HasKey($"User_{usuario}_Pass"))
        {
            MostrarError($"❌ El usuario '{usuario}' ya existe");
            return;
        }
        
        // REGISTRAR USUARIO
        Debug.Log($"✅ Registrando usuario: {usuario}");
        PlayerPrefs.SetString($"User_{usuario}_Pass", contrasena);
        PlayerPrefs.SetString($"User_{usuario}_Nombre", nombre);
        PlayerPrefs.SetString($"User_{usuario}_Apellido", apellido);
        PlayerPrefs.SetInt($"User_{usuario}_Creado", 1);
        PlayerPrefs.Save();
        
        // Iniciar sesión automáticamente
        PlayerPrefs.SetString("UsuarioActivo", usuario);
        PlayerPrefs.SetInt("SesionIniciada", 1);
        PlayerPrefs.Save();
        
        Debug.Log($"🎉 ¡Registro exitoso! Usuario: {usuario}");
        Debug.Log("🎮 Cargando MenuPrincipalT...");
        
        // Ir al menú principal
        SceneManager.LoadScene("MenuPrincipalT");
    }
    
    private void OnVolverClick()
    {
        Debug.Log("🔥🔥🔥 BOTÓN VOLVER CLICKEADO 🔥🔥🔥");
        Debug.Log("⬅️ Volviendo al Login...");
        SceneManager.LoadScene("LoginScene");
    }
    
    private void MostrarError(string mensaje)
    {
        mensajeError.text = mensaje;
        mensajeError.style.display = DisplayStyle.Flex;
        Debug.LogWarning(mensaje);
    }
    
    private void OcultarError()
    {
        mensajeError.style.display = DisplayStyle.None;
    }
}