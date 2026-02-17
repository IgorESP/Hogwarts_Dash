using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;
using System.Text;

public class ApiManager : MonoBehaviour
{
    public static ApiManager Instance { get; private set; }

    [Header("Configuración de API")]
    public string apiBaseUrl = "https://hogwartsdashapi.onrender.com/api/v1";
    
    [Header("Seguridad")]
    public string apiKey = "hogwarts_dash_2026_admin";
    
    [Header("Timeouts")]
    public int timeoutSeconds = 90;

    void Awake() 
    {
        if (Instance == null) 
        { 
            Instance = this; 
            DontDestroyOnLoad(gameObject);
            
            apiBaseUrl = "https://hogwartsdashapi.onrender.com/api/v1";
            apiKey = "hogwarts_dash_2026_admin";
            timeoutSeconds = 90;
            
            Debug.Log($"[API] Gestor inicializado orientado hacia: {apiBaseUrl}");
        }
        else { Destroy(gameObject); }
    }

    public void Login(string user, string pass, Action<bool, string, UserData> callback) 
    {
        var requestData = new LoginRequest { username = user, password = pass };
        Debug.Log($"[API] Solicitando login para: {user}");
        
        StartCoroutine(PostRequest<LoginResponse>("/jugadores/login", requestData, (exito, res) => {
            Debug.Log($"[API] Respuesta login - exito:{exito} mensaje:{res?.mensaje} detail:{res?.detail}");

            if (exito && res != null && res.usuario != null) 
            {
                PlayerPrefs.SetString("player_id", res.usuario.player_id);
                PlayerPrefs.SetString("username", res.usuario.username);
                PlayerPrefs.Save();
                callback(true, "Acceso concedido", res.usuario);
            }
            else callback(false, res?.mensaje ?? res?.detail ?? "Error de conexión", null);
        }));
    }

    public void Registrar(string user, string nom, string ape, string pass, Action<bool, string> callback) 
    {
        var data = new RegistroRequest { username = user, nombre = nom, apellido = ape, password = pass };
        Debug.Log($"[API] Registrando nuevo usuario: {user}");

        StartCoroutine(PostRequest<ApiResponse>("/jugadores/register", data, (exito, res) => {
            Debug.Log($"[API] Respuesta registro - exito:{exito} mensaje:{res?.mensaje} detail:{res?.detail}");

            if (exito) callback(true, "Registro completado");
            else callback(false, res?.mensaje ?? res?.detail ?? "Error al registrar");
        }));
    }

    public void ObtenerRanking(Action<bool, RankingEntry[]> callback) 
    {
        StartCoroutine(GetRequest<RankingEntry[]>("/jugadores/top", callback));
    }

    private IEnumerator PostRequest<T>(string endpoint, object data, Action<bool, T> callback) 
    {
        string json = JsonUtility.ToJson(data);
        string url = apiBaseUrl.TrimEnd('/') + endpoint;

        Debug.Log($"[API] POST → {url}");
        Debug.Log($"[API] Body: {json}");
        
        using (UnityWebRequest req = new UnityWebRequest(url, "POST")) 
        {
            byte[] body = Encoding.UTF8.GetBytes(json);
            req.uploadHandler = new UploadHandlerRaw(body);
            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader("Content-Type", "application/json");
            // ✅ CORREGIDO: "api-key" en minúsculas, no "X-API-Key"
            if (!string.IsNullOrEmpty(apiKey)) req.SetRequestHeader("api-key", apiKey);
            req.timeout = timeoutSeconds;
            
            yield return req.SendWebRequest();

            Debug.Log($"[API] Código respuesta: {req.responseCode}");
            Debug.Log($"[API] Respuesta completa: {req.downloadHandler.text}");
            
            if (req.result == UnityWebRequest.Result.Success) 
            {
                Debug.Log($"[API] POST Exitoso a {endpoint}");
                callback(true, JsonUtility.FromJson<T>(req.downloadHandler.text));
            } 
            else 
            {
                Debug.LogWarning($"[API] Error POST {endpoint}: {req.error}");
                Debug.LogWarning($"[API] Contenido error: {req.downloadHandler.text}");
                
                try 
                {
                    callback(false, JsonUtility.FromJson<T>(req.downloadHandler.text));
                }
                catch 
                {
                    callback(false, default(T));
                }
            }
        }
    }

    private IEnumerator GetRequest<T>(string endpoint, Action<bool, T> callback) 
    {
        string url = apiBaseUrl.TrimEnd('/') + endpoint;
        Debug.Log($"[API] GET → {url}");

        using (UnityWebRequest req = UnityWebRequest.Get(url)) 
        {
            // ✅ CORREGIDO: "api-key" en minúsculas
            if (!string.IsNullOrEmpty(apiKey)) req.SetRequestHeader("api-key", apiKey);
            req.timeout = timeoutSeconds;
            yield return req.SendWebRequest();

            Debug.Log($"[API] Código respuesta: {req.responseCode}");
            Debug.Log($"[API] Respuesta completa: {req.downloadHandler.text}");
            
            if (req.result == UnityWebRequest.Result.Success) 
            {
                Debug.Log($"[API] GET Exitoso de {endpoint}");
                string wrappedJson = "{ \"ranking\": " + req.downloadHandler.text + " }";
                RankingList wrapper = JsonUtility.FromJson<RankingList>(wrappedJson);
                callback(true, (T)(object)wrapper.ranking);
            } 
            else 
            {
                Debug.LogWarning($"[API] Error GET {endpoint}: {req.error}");
                callback(false, default(T));
            }
        }
    }

    [Serializable] public class LoginRequest   { public string username, password; }
    [Serializable] public class RegistroRequest { public string nombre, apellido, username, password; }
    [Serializable] public class ApiResponse    { public bool exito; public string mensaje; public string detail; }
    [Serializable] public class LoginResponse  { public bool exito; public string mensaje; public string detail; public UserData usuario; }
    [Serializable] public class UserData       { public string player_id, username; public int total_puntuacion; }
    [Serializable] public class RankingEntry   { public string username; public int puntuacion; }
    [Serializable] public class RankingList    { public RankingEntry[] ranking; }
}