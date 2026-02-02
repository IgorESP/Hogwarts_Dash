using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;
using System.Text;

/// <summary>
/// Gestor de comunicación con la API de Hogwarts Dash
/// Maneja login, registro, ranking y otras peticiones
/// </summary>
public class ApiManager : MonoBehaviour
{
    // ====================================
    // SINGLETON
    // ====================================
    public static ApiManager Instance { get; private set; }

    // ====================================
    // CONFIGURACIÓN
    // ====================================
    [Header("Configuración de API")]
    public string apiBaseUrl = "https://hogwartsdashapi.onrender.com/api/v1";
    
    [Header("Seguridad")]
    [Tooltip("API Key proporcionada por el backend")]
    public string apiKey = "hogwarts_dash_2026_admin";
    
    [Header("Timeouts")]
    public int timeoutSeconds = 90;

    // ====================================
    // INICIALIZACIÓN
    // ====================================
    void Awake() 
    {
        if (Instance == null) 
        { 
            Instance = this; 
            DontDestroyOnLoad(gameObject);
            
            // Forzar URL correcta
            apiBaseUrl = "https://hogwartsdashapi.onrender.com/api/v1";
            
            // Cargar API Key persistente o usar default
            apiKey = PlayerPrefs.GetString("ApiKey", "hogwarts_dash_2026_admin");
            
            Debug.Log("✅ ApiManager inicializado");
            Debug.Log("🌐 URL Base: " + apiBaseUrl);
            Debug.Log("🔑 API Key: " + (string.IsNullOrEmpty(apiKey) ? "No configurada" : "Configurada"));
        }
        else 
        { 
            Destroy(gameObject); 
        }
    }

    // ====================================
    // MÉTODOS PÚBLICOS
    // ====================================

    /// <summary>
    /// Cambiar la API Key en tiempo de ejecución (para pruebas)
    /// </summary>
    public void CambiarApiKey(string nuevaKey)
    {
        apiKey = nuevaKey;
        PlayerPrefs.SetString("ApiKey", nuevaKey);
        PlayerPrefs.Save();
        Debug.Log("🔑 API Key actualizada a: " + nuevaKey);
    }

    /// <summary>
    /// Inicia sesión con usuario y contraseña
    /// </summary>
    public void Login(string user, string pass, Action<bool, string, UserData> callback) 
    {
        var requestData = new LoginRequest { username = user, password = pass };
        
        StartCoroutine(PostRequest<LoginResponse>("/jugadores/login", requestData, (exito, res) => {
            if (exito && res != null) 
            {
                // Guardar datos del usuario
                if (res.usuario != null)
                {
                    PlayerPrefs.SetString("player_id", res.usuario.player_id);
                    PlayerPrefs.SetString("username", res.usuario.username);
                    PlayerPrefs.SetString("nombre", res.usuario.nombre);
                    PlayerPrefs.Save();
                }
                
                callback(true, "¡Bienvenido!", res.usuario);
            }
            else 
            {
                callback(false, res?.mensaje ?? res?.detail ?? "Usuario o contraseña incorrectos", null);
            }
        }));
    }

    /// <summary>
    /// Registra un nuevo usuario
    /// </summary>
    public void Registrar(string user, string nom, string ape, string pass, Action<bool, string> callback) 
    {
        var data = new RegistroRequest 
        { 
            username = user, 
            nombre = nom, 
            apellido = ape, 
            password = pass 
        };
        
        StartCoroutine(PostRequest<ApiResponse>("/jugadores/register", data, (exito, res) => {
            if (exito) 
            {
                callback(true, "Cuenta creada exitosamente");
            }
            else 
            {
                string error = res?.mensaje ?? res?.detail ?? "Error al crear cuenta";
                callback(false, error);
            }
        }));
    }

    /// <summary>
    /// Obtiene el ranking de mejores jugadores
    /// </summary>
    public void ObtenerRanking(Action<bool, RankingEntry[]> callback) 
    {
        StartCoroutine(GetRequest<RankingEntry[]>("/jugadores/top", (exito, res) => {
            callback(exito, res);
        }));
    }

    // ====================================
    // PETICIONES HTTP PRIVADAS
    // ====================================

    /// <summary>
    /// Realiza una petición POST a la API
    /// </summary>
    private IEnumerator PostRequest<T>(string endpoint, object data, Action<bool, T> callback) 
    {
        string json = JsonUtility.ToJson(data);
        string url = apiBaseUrl.TrimEnd('/') + endpoint;
        
        Debug.Log("📤 POST: " + url);
        Debug.Log("📦 Data: " + json);
        
        using (UnityWebRequest req = new UnityWebRequest(url, "POST")) 
        {
            byte[] body = Encoding.UTF8.GetBytes(json);
            req.uploadHandler = new UploadHandlerRaw(body);
            req.downloadHandler = new DownloadHandlerBuffer();
            
            // Headers
            req.SetRequestHeader("Content-Type", "application/json");
            
            // Agregar API Key - INTENTAR DIFERENTES FORMATOS
            if (!string.IsNullOrEmpty(apiKey))
            {
                // Probar con X-API-Key (estándar)
                req.SetRequestHeader("X-API-Key", apiKey);
                
                // También probar con api-key (por si acaso)
                req.SetRequestHeader("api-key", apiKey);
                
                Debug.Log("🔑 Enviando API Key: " + apiKey);
            }
            
            req.timeout = timeoutSeconds;
            
            yield return req.SendWebRequest();
            
            if (req.result == UnityWebRequest.Result.Success) 
            {
                Debug.Log("✅ Success: " + req.downloadHandler.text);
                
                T response = JsonUtility.FromJson<T>(req.downloadHandler.text);
                callback(true, response);
            } 
            else 
            {
                Debug.LogError("❌ Error: " + req.error);
                Debug.LogError("❌ URL: " + url);
                Debug.LogError("❌ Response: " + req.downloadHandler.text);
                
                T errorRes = default;
                try 
                { 
                    errorRes = JsonUtility.FromJson<T>(req.downloadHandler.text); 
                } 
                catch (Exception e)
                {
                    Debug.LogError("❌ Parse Error: " + e.Message);
                }
                
                callback(false, errorRes);
            }
        }
    }

    /// <summary>
    /// Realiza una petición GET a la API
    /// </summary>
    private IEnumerator GetRequest<T>(string endpoint, Action<bool, T> callback) 
    {
        string url = apiBaseUrl.TrimEnd('/') + endpoint;
        
        Debug.Log("📥 GET: " + url);
        
        using (UnityWebRequest req = UnityWebRequest.Get(url)) 
        {
            // Agregar API Key - INTENTAR DIFERENTES FORMATOS
            if (!string.IsNullOrEmpty(apiKey))
            {
                req.SetRequestHeader("X-API-Key", apiKey);
                req.SetRequestHeader("api-key", apiKey);
                Debug.Log("🔑 Enviando API Key: " + apiKey);
            }
            
            req.timeout = timeoutSeconds;
            
            yield return req.SendWebRequest();
            
            if (req.result == UnityWebRequest.Result.Success) 
            {
                Debug.Log("✅ Success: " + req.downloadHandler.text);
                
                // Truco para leer listas [] en Unity
                string json = "{ \"ranking\": " + req.downloadHandler.text + " }";
                RankingList wrapper = JsonUtility.FromJson<RankingList>(json);
                
                callback(true, (T)(object)wrapper.ranking);
            } 
            else 
            {
                Debug.LogError("❌ Error: " + req.error);
                Debug.LogError("❌ Response: " + req.downloadHandler.text);
                
                callback(false, default);
            }
        }
    }

    // ====================================
    // CLASES DE DATOS (DTOs)
    // ====================================
    
    [Serializable] 
    public class LoginRequest 
    { 
        public string username; 
        public string password; 
    }
    
    [Serializable] 
    public class RegistroRequest 
    { 
        public string nombre; 
        public string apellido; 
        public string username; 
        public string password; 
    }
    
    [Serializable] 
    public class ApiResponse 
    { 
        public bool exito; 
        public string mensaje; 
        public string detail; 
        public UserData jugador;  // La API devuelve el jugador creado
    }
    
    [Serializable] 
    public class LoginResponse 
    { 
        public bool exito; 
        public string mensaje; 
        public string detail; 
        public string token; 
        public UserData usuario; 
    }
    
    [Serializable] 
    public class UserData 
    { 
        public string player_id;        // ID del jugador
        public string username;         // Nombre de usuario
        public string nombre;           // Nombre real
        public string apellido;         // Apellido
        public int total_partidas;      // Total de partidas jugadas
        public int total_puntuacion;    // Puntuación total acumulada
    }
    
    [Serializable] 
    public class RankingEntry 
    { 
        public int rank;                // Posición en el ranking
        public string player_id;        // ID del jugador
        public string username;         // Nombre de usuario
        public string nombre;           // Nombre real
        public int puntuacion;          // ⬅️ CAMBIO: era "puntos", ahora "puntuacion"
        public int partidas;            // Número de partidas jugadas
    }
    
    [Serializable] 
    public class RankingList 
    { 
        public RankingEntry[] ranking; 
    }
}