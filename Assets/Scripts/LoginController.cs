using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using System.Linq; // ← IMPORTANTE: Necesario para .Any()

public class LoginUIController : MonoBehaviour
{
    private UIDocument uiDocument;
    private VisualElement root;
    
    // Campos de texto
    private TextField campoUsuario;
    private TextField campoContrasena;
    
    // Botones
    private Button botonIniciarSesion;
    private Button botonRegistrarse;
    
    // Mensaje de error
    private Label mensajeError;
    
    void Start()
    {
        // Obtener referencias
        uiDocument = GetComponent<UIDocument>();
        root = uiDocument.rootVisualElement;
        
        // Obtener campos
        campoUsuario = root.Q<TextField>("Campo_Usuario");
        campoContrasena = root.Q<TextField>("Campo_Contrasena");
        
        // Obtener botones
        botonIniciarSesion = root.Q<Button>("Boton_IniciarSesion");
        botonRegistrarse = root.Q<Button>("Boton_Registrarse");
        
        // Obtener mensaje de error
        mensajeError = root.Q<Label>("Mensaje_Error");
        
        // Registrar eventos - AMBOS VAN A MenuPrincipalT
        botonIniciarSesion.clicked += OnIniciarSesionClick;
        botonRegistrarse.clicked += OnRegistrarseClick;
        
        // Permitir Enter para iniciar sesión
        campoUsuario.RegisterCallback<KeyDownEvent>(OnKeyDown);
        campoContrasena.RegisterCallback<KeyDownEvent>(OnKeyDown);
        
        // Ocultar error al escribir
        campoUsuario.RegisterValueChangedCallback(evt => OcultarError());
        campoContrasena.RegisterValueChangedCallback(evt => OcultarError());
        
        Debug.Log("🔐 Pantalla de Login lista");
    }
    
    private void OnKeyDown(KeyDownEvent evt)
    {
        if (evt.keyCode == KeyCode.Return || evt.keyCode == KeyCode.KeypadEnter)
        {
            OnIniciarSesionClick();
        }
    }
    
    private void OnIniciarSesionClick()
    {
        string usuario = campoUsuario.value;
        string contrasena = campoContrasena.value;
        
        // Validar campos vacíos
        if (string.IsNullOrEmpty(usuario))
        {
            MostrarError("⚠️ Por favor ingresa tu usuario");
            return;
        }
        
        if (string.IsNullOrEmpty(contrasena))
        {
            MostrarError("⚠️ Por favor ingresa tu contraseña");
            return;
        }
        
        // Validar credenciales
        if (ValidarLogin(usuario, contrasena))
        {
            Debug.Log($"✅ Login exitoso: {usuario}");
            
            // Guardar sesión
            PlayerPrefs.SetString("UsuarioActivo", usuario);
            PlayerPrefs.SetInt("SesionIniciada", 1);
            PlayerPrefs.Save();
            
            // IR A MENU PRINCIPAL
            Debug.Log("🎮 Cargando MenuPrincipalT...");
            SceneManager.LoadScene("MenuPrincipalT");
        }
        else
        {
            MostrarError("❌ Usuario o contraseña incorrectos");
        }
    }
    
    private void OnRegistrarseClick()
    {
        string usuario = campoUsuario.value;
        string contrasena = campoContrasena.value;
        
        // Validar campos vacíos
        if (string.IsNullOrEmpty(usuario))
        {
            MostrarError("⚠️ Por favor ingresa un usuario");
            return;
        }
        
        if (string.IsNullOrEmpty(contrasena))
        {
            MostrarError("⚠️ Por favor ingresa una contraseña");
            return;
        }
        
        // Validar longitud
        if (usuario.Length < 3)
        {
            MostrarError("⚠️ El usuario debe tener al menos 3 caracteres");
            return;
        }
        
        // Validar longitud mínima
        if (contrasena.Length < 4)
        {
            MostrarError("⚠️La contraseña debe tener al menos 4 caracteres");
            return;
        }

        // Validar que tenga al menos una mayúscula
        if (!contrasena.Any(char.IsUpper))
        {
            MostrarError("⚠️La contraseña debe contener al menos una letra mayúscula");
            return;
        }

        // Validar que tenga al menos una minúscula
        if (!contrasena.Any(char.IsLower))
        {
            MostrarError("⚠️La contraseña debe contener al menos una letra minúscula");
            return;
        }

        // Validar que tenga al menos un número
        if (!contrasena.Any(char.IsDigit))
        {
            MostrarError("⚠️La contraseña debe contener al menos un número");
            return;
        }

        // Validar que tenga al menos un símbolo
        if (!contrasena.Any(c => !char.IsLetterOrDigit(c)))
        {
            MostrarError("⚠️La contraseña debe contener al menos un símbolo");
            return;
        }
        
        // Registrar usuario
        if (RegistrarNuevoUsuario(usuario, contrasena))
        {
            Debug.Log($"✅ Usuario registrado: {usuario}");
            
            // Guardar sesión del nuevo usuario
            PlayerPrefs.SetString("UsuarioActivo", usuario);
            PlayerPrefs.SetInt("SesionIniciada", 1);
            PlayerPrefs.Save();
            
            // IR A MENU PRINCIPAL DIRECTAMENTE
            Debug.Log("🎮 Registro exitoso. Cargando MenuPrincipalT...");
            SceneManager.LoadScene("MenuPrincipalT");
        }
        else
        {
            MostrarError("❌ El usuario ya existe. Intenta otro nombre.");
        }
    }
    
    private bool ValidarLogin(string usuario, string contrasena)
    {
        // Sistema simple con PlayerPrefs
        string contrasenaGuardada = PlayerPrefs.GetString($"User_{usuario}_Pass", "");
        
        if (string.IsNullOrEmpty(contrasenaGuardada))
        {
            Debug.LogWarning("⚠️ Usuario no encontrado");
            return false;
        }
        
        return contrasena == contrasenaGuardada;
    }
    
    private bool RegistrarNuevoUsuario(string usuario, string contrasena)
    {
        // Verificar si ya existe
        if (PlayerPrefs.HasKey($"User_{usuario}_Pass"))
        {
            Debug.LogWarning("⚠️ Usuario ya existe");
            return false;
        }
        
        // Guardar usuario
        PlayerPrefs.SetString($"User_{usuario}_Pass", contrasena);
        PlayerPrefs.SetInt($"User_{usuario}_Creado", 1);
        PlayerPrefs.Save();
        
        return true;
    }
    
    private void MostrarError(string mensaje)
    {
        mensajeError.text = mensaje;
        mensajeError.style.display = DisplayStyle.Flex;
    }
    
    private void OcultarError()
    {
        mensajeError.style.display = DisplayStyle.None;
    }
}