using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using System.Linq;

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
        Debug.Log("🎬 LoginUIController - Start() ejecutándose...");
        
        // Obtener referencias
        uiDocument = GetComponent<UIDocument>();
        
        if (uiDocument == null)
        {
            Debug.LogError("❌ UIDocument no encontrado!");
            return;
        }
        
        root = uiDocument.rootVisualElement;
        
        if (root == null)
        {
            Debug.LogError("❌ Root VisualElement no encontrado!");
            return;
        }
        
        // Obtener campos CON VALIDACIÓN
        campoUsuario = root.Q<TextField>("Campo_Usuario");
        campoContrasena = root.Q<TextField>("Campo_Contrasena");
        
        if (campoUsuario == null)
        {
            Debug.LogError("❌ Campo_Usuario no encontrado!");
            return;
        }
        
        if (campoContrasena == null)
        {
            Debug.LogError("❌ Campo_Contrasena no encontrado!");
            return;
        }
        
        Debug.Log("✅ Campos encontrados correctamente");
        
        // Obtener botones CON VALIDACIÓN
        botonIniciarSesion = root.Q<Button>("Boton_IniciarSesion");
        botonRegistrarse = root.Q<Button>("Boton_Registrarse");
        
        if (botonIniciarSesion == null)
        {
            Debug.LogError("❌ Boton_IniciarSesion no encontrado!");
            return;
        }
        
        if (botonRegistrarse == null)
        {
            Debug.LogError("❌ Boton_Registrarse no encontrado!");
            return;
        }
        
        Debug.Log("✅ Botones encontrados correctamente");
        
        // Obtener mensaje de error CON VALIDACIÓN
        mensajeError = root.Q<Label>("Mensaje_Error");
        
        if (mensajeError == null)
        {
            Debug.LogWarning("⚠️ Mensaje_Error no encontrado (crear manualmente si es necesario)");
        }
        else
        {
            Debug.Log("✅ Mensaje_Error encontrado");
        }
        
        // Registrar eventos
        botonIniciarSesion.clicked += OnIniciarSesionClick;
        botonRegistrarse.clicked += OnRegistrarseClick;
        
        Debug.Log("✅ Eventos registrados");
        
        // Permitir Enter para iniciar sesión
        campoUsuario.RegisterCallback<KeyDownEvent>(OnKeyDown);
        campoContrasena.RegisterCallback<KeyDownEvent>(OnKeyDown);
        
        // Ocultar error al escribir
        campoUsuario.RegisterValueChangedCallback(evt => OcultarError());
        campoContrasena.RegisterValueChangedCallback(evt => OcultarError());
        
        Debug.Log("✅ Pantalla de Login lista completamente");
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
        Debug.Log("🔥🔥🔥 BOTÓN INICIAR SESIÓN CLICKEADO 🔥🔥🔥");
        
        // VALIDACIÓN EXTRA: Verificar que los campos existan
        if (campoUsuario == null || campoContrasena == null)
        {
            Debug.LogError("❌ Los campos no están inicializados!");
            return;
        }
        
        string usuario = campoUsuario.value;
        string contrasena = campoContrasena.value;
        
        Debug.Log($"📝 Usuario ingresado: {usuario}");
        Debug.Log($"🔒 Contraseña ingresada: {'*'.ToString().PadLeft(contrasena.Length, '*')}");
        
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
            
            Debug.Log("💾 Sesión guardada");
            
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
        Debug.Log("🔥🔥🔥 BOTÓN REGISTRARSE CLICKEADO 🔥🔥🔥");
        Debug.Log("📝 Cargando pantalla de registro...");
        
        try
        {
            SceneManager.LoadScene("RegistroScene");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ Error al cargar RegistroScene: {e.Message}");
            Debug.LogError("⚠️ Verifica que 'RegistroScene' esté en Build Settings");
        }
    }
    
    private bool ValidarLogin(string usuario, string contrasena)
    {
        // Sistema simple con PlayerPrefs
        string contrasenaGuardada = PlayerPrefs.GetString($"User_{usuario}_Pass", "");
        
        if (string.IsNullOrEmpty(contrasenaGuardada))
        {
            Debug.LogWarning($"⚠️ Usuario '{usuario}' no encontrado en el sistema");
            return false;
        }
        
        bool esValido = contrasena == contrasenaGuardada;
        
        if (esValido)
        {
            Debug.Log($"✅ Contraseña correcta para '{usuario}'");
        }
        else
        {
            Debug.LogWarning($"❌ Contraseña incorrecta para '{usuario}'");
        }
        
        return esValido;
    }
    
    private void MostrarError(string mensaje)
    {
        if (mensajeError != null)
        {
            mensajeError.text = mensaje;
            mensajeError.style.display = DisplayStyle.Flex;
        }
        else
        {
            Debug.LogWarning("⚠️ No se puede mostrar el error en UI (mensajeError es null)");
        }
        
        Debug.LogWarning(mensaje);
    }
    
    private void OcultarError()
    {
        if (mensajeError != null)
        {
            mensajeError.style.display = DisplayStyle.None;
        }
    }
}