using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;

/// <summary>
/// Gestor centralizado para comunicación con API
/// Maneja todas las peticiones HTTP (POST, GET, etc.)
/// </summary>
public class ApiManager : MonoBehaviour
{
    // ====================================
    // SINGLETON
    // ====================================
    
    public static ApiManager Instance { get; private set; }
    
    // ====================================
    // CONFIGURACIÓN DE API
    // ====================================
    
    [Header("⚙️ Configuración de API")]
    [Tooltip("https://hogwartsdashapi.onrender.com/docs")]
    public string apiBaseUrl = "http://localhost:3000/api";
    
    [Tooltip("Tiempo máximo de espera para peticiones (segundos)")]
    public int timeoutSeconds = 10;
    
    // ====================================
    // ENDPOINTS
    // ====================================
    
    private string loginEndpoint => $"{apiBaseUrl}/login";
    private string registerEndpoint => $"{apiBaseUrl}/register";
    private string getUserEndpoint => $"{apiBaseUrl}/user";
    
    // ====================================
    // INICIALIZACIÓN
    // ====================================
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("🌐 ApiManager inicializado");
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    // ====================================
    // REGISTRO DE USUARIO
    // ====================================
    
    public void Registrar(string usuario, string nombre, string apellido, string contrasena, Action<bool, string> callback)
    {
        StartCoroutine(RegistrarCoroutine(usuario, nombre, apellido, contrasena, callback));
    }
    
    private IEnumerator RegistrarCoroutine(string usuario, string nombre, string apellido, string contrasena, Action<bool, string> callback)
    {
        Debug.Log($"📝 Intentando registrar usuario: {usuario}");
        
        // Crear JSON con los datos
        var registroData = new RegistroRequest
        {
            usuario = usuario,
            nombre = nombre,
            apellido = apellido,
            contrasena = contrasena
        };
        
        string jsonData = JsonUtility.ToJson(registroData);
        
        // Crear petición POST
        using (UnityWebRequest request = new UnityWebRequest(registerEndpoint, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.timeout = timeoutSeconds;
            
            // Enviar petición
            yield return request.SendWebRequest();
            
            // Procesar respuesta
            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log($"✅ Registro exitoso: {request.downloadHandler.text}");
                
                // Parsear respuesta
                try
                {
                    var response = JsonUtility.FromJson<ApiResponse>(request.downloadHandler.text);
                    callback?.Invoke(true, response.mensaje);
                }
                catch
                {
                    callback?.Invoke(true, "Usuario registrado correctamente");
                }
            }
            else
            {
                Debug.LogError($"❌ Error en registro: {request.error}");
                string errorMsg = ObtenerMensajeError(request);
                callback?.Invoke(false, errorMsg);
            }
        }
    }
    
    // ====================================
    // LOGIN DE USUARIO
    // ====================================
    
    public void Login(string usuario, string contrasena, Action<bool, string, UserData> callback)
    {
        StartCoroutine(LoginCoroutine(usuario, contrasena, callback));
    }
    
    private IEnumerator LoginCoroutine(string usuario, string contrasena, Action<bool, string, UserData> callback)
    {
        Debug.Log($"🔐 Intentando login: {usuario}");
        
        // Crear JSON con credenciales
        var loginData = new LoginRequest
        {
            usuario = usuario,
            contrasena = contrasena
        };
        
        string jsonData = JsonUtility.ToJson(loginData);
        
        // Crear petición POST
        using (UnityWebRequest request = new UnityWebRequest(loginEndpoint, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.timeout = timeoutSeconds;
            
            // Enviar petición
            yield return request.SendWebRequest();
            
            // Procesar respuesta
            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log($"✅ Login exitoso: {request.downloadHandler.text}");
                
                try
                {
                    var response = JsonUtility.FromJson<LoginResponse>(request.downloadHandler.text);
                    
                    // Guardar token si existe
                    if (!string.IsNullOrEmpty(response.token))
                    {
                        PlayerPrefs.SetString("AuthToken", response.token);
                        PlayerPrefs.Save();
                    }
                    
                    callback?.Invoke(true, response.mensaje, response.usuario);
                }
                catch (Exception e)
                {
                    Debug.LogError($"Error al parsear respuesta: {e.Message}");
                    callback?.Invoke(false, "Error al procesar respuesta del servidor", null);
                }
            }
            else
            {
                Debug.LogError($"❌ Error en login: {request.error}");
                string errorMsg = ObtenerMensajeError(request);
                callback?.Invoke(false, errorMsg, null);
            }
        }
    }
    
    // ====================================
    // OBTENER DATOS DE USUARIO
    // ====================================
    
    public void ObtenerUsuario(string token, Action<bool, UserData> callback)
    {
        StartCoroutine(ObtenerUsuarioCoroutine(token, callback));
    }
    
    private IEnumerator ObtenerUsuarioCoroutine(string token, Action<bool, UserData> callback)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(getUserEndpoint))
        {
            request.SetRequestHeader("Authorization", $"Bearer {token}");
            request.timeout = timeoutSeconds;
            
            yield return request.SendWebRequest();
            
            if (request.result == UnityWebRequest.Result.Success)
            {
                try
                {
                    var userData = JsonUtility.FromJson<UserData>(request.downloadHandler.text);
                    callback?.Invoke(true, userData);
                }
                catch
                {
                    callback?.Invoke(false, null);
                }
            }
            else
            {
                callback?.Invoke(false, null);
            }
        }
    }
    
    // ====================================
    // UTILIDADES
    // ====================================
    
    private string ObtenerMensajeError(UnityWebRequest request)
    {
        if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            return "No se pudo conectar con el servidor. Verifica tu conexión a internet.";
        }
        else if (request.result == UnityWebRequest.Result.ProtocolError)
        {
            // Intentar obtener mensaje del servidor
            try
            {
                var errorResponse = JsonUtility.FromJson<ApiResponse>(request.downloadHandler.text);
                return errorResponse.mensaje;
            }
            catch
            {
                return $"Error del servidor: {request.responseCode}";
            }
        }
        else
        {
            return "Error desconocido. Intenta de nuevo.";
        }
    }
    
    // ====================================
    // CLASES DE DATOS
    // ====================================
    
    [Serializable]
    public class RegistroRequest
    {
        public string usuario;
        public string nombre;
        public string apellido;
        public string contrasena;
    }
    
    [Serializable]
    public class LoginRequest
    {
        public string usuario;
        public string contrasena;
    }
    
    [Serializable]
    public class ApiResponse
    {
        public bool exito;
        public string mensaje;
    }
    
    [Serializable]
    public class LoginResponse
    {
        public bool exito;
        public string mensaje;
        public string token;
        public UserData usuario;
    }
    
    [Serializable]
    public class UserData
    {
        public string id;
        public string usuario;
        public string nombre;
        public string apellido;
        public string email;
    }
}