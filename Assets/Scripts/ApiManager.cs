using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;
using System.Text;

/// <summary>
/// Gestor de API para comunicación con el backend (Hogwarts Dash)
/// </summary>
public class ApiManager : MonoBehaviour
{
    public static ApiManager Instance { get; private set; }

    [Header("Configuración de API")]
    [Tooltip("URL base para las peticiones al backend")]
    public string apiBaseUrl = "https://hogwartsdashapi.onrender.com/api/v1";
    
    [Header("Seguridad")]
    [Tooltip("API Key para autorizar peticiones")]
    public string apiKey = "hogwarts_dash_2026_admin";
    
    [Header("Timeouts")]
    public int timeoutSeconds = 90;

    void Awake() 
    {
        if (Instance == null) 
        { 
            Instance = this; 
            DontDestroyOnLoad(gameObject);
            Debug.Log($"[API] Gestor inicializado orientado hacia: {apiBaseUrl}");
        }
        else { Destroy(gameObject); }
    }

    /// <summary>
    /// Intenta iniciar sesión en el servidor
    /// </summary>
    public void Login(string user, string pass, Action<bool, string, UserData> callback) 
    {
        var requestData = new LoginRequest { username = user, password = pass };
        Debug.Log($"[API] Solicitando login para: {user}");
        
        StartCoroutine(PostRequest<LoginResponse>("/jugadores/login", requestData, (exito, res) => {
            if (exito && res != null && res.usuario != null) 
            {
                PlayerPrefs.SetString("player_id", res.usuario.player_id);
                PlayerPrefs.SetString("username", res.usuario.username);
                PlayerPrefs.Save();
                callback(true, "Acceso concedido", res.usuario);
            }
            else callback(false, res?.mensaje ?? "Error de conexión", null);
        }));
    }

    /// <summary>
    /// Registra un nuevo jugador
    /// </summary>
    public void Registrar(string user, string nom, string ape, string pass, Action<bool, string> callback) 
    {
        var data = new RegistroRequest { username = user, nombre = nom, apellido = ape, password = pass };
        Debug.Log($"[API] Registrando nuevo usuario: {user}");

        StartCoroutine(PostRequest<ApiResponse>("/jugadores/register", data, (exito, res) => {
            if (exito) callback(true, "Registro completado");
            else callback(false, res?.mensaje ?? "Error al registrar");
        }));
    }

    /// <summary>
    /// Obtiene el top de puntuaciones del servidor
    /// </summary>
    public void ObtenerRanking(Action<bool, RankingEntry[]> callback) 
    {
        StartCoroutine(GetRequest<RankingEntry[]>("/jugadores/top", callback));
    }

    // ====================================
    // INFRAESTRUCTURA DE PETICIONES (HTTP)
    // ====================================

    private IEnumerator PostRequest<T>(string endpoint, object data, Action<bool, T> callback) 
    {
        string json = JsonUtility.ToJson(data);
        string url = apiBaseUrl.TrimEnd('/') + endpoint;
        
        using (UnityWebRequest req = new UnityWebRequest(url, "POST")) 
        {
            byte[] body = Encoding.UTF8.GetBytes(json);
            req.uploadHandler = new UploadHandlerRaw(body);
            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader("Content-Type", "application/json");
            if (!string.IsNullOrEmpty(apiKey)) req.SetRequestHeader("X-API-Key", apiKey);
            req.timeout = timeoutSeconds;
            
            yield return req.SendWebRequest();
            
            if (req.result == UnityWebRequest.Result.Success) 
            {
                Debug.Log($"[API] POST Exitoso a {endpoint}");
                callback(true, JsonUtility.FromJson<T>(req.downloadHandler.text));
            } 
            else 
            {
                Debug.LogWarning($"[API] Error POST {endpoint}: {req.error}");
                callback(false, JsonUtility.FromJson<T>(req.downloadHandler.text));
            }
        }
    }

    private IEnumerator GetRequest<T>(string endpoint, Action<bool, T> callback) 
    {
        string url = apiBaseUrl.TrimEnd('/') + endpoint;
        using (UnityWebRequest req = UnityWebRequest.Get(url)) 
        {
            if (!string.IsNullOrEmpty(apiKey)) req.SetRequestHeader("X-API-Key", apiKey);
            req.timeout = timeoutSeconds;
            yield return req.SendWebRequest();
            
            if (req.result == UnityWebRequest.Result.Success) 
            {
                Debug.Log($"[API] GET Exitoso de {endpoint}");
                // Wrapper para listas JSON en Unity
                string wrappedJson = "{ \"ranking\": " + req.downloadHandler.text + " }";
                RankingList wrapper = JsonUtility.FromJson<RankingList>(wrappedJson);
                callback(true, (T)(object)wrapper.ranking);
            } 
            else callback(false, default);
        }
    }

    // DTOs (Data Transfer Objects)
    [Serializable] public class LoginRequest { public string username, password; }
    [Serializable] public class RegistroRequest { public string nombre, apellido, username, password; }
    [Serializable] public class ApiResponse { public bool exito; public string mensaje; public string detail; }
    [Serializable] public class LoginResponse { public bool exito; public string mensaje; public UserData usuario; }
    [Serializable] public class UserData { public string player_id, username; public int total_puntuacion; }
    [Serializable] public class RankingEntry { public string username; public int puntuacion; }
    [Serializable] public class RankingList { public RankingEntry[] ranking; }
}