using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

/// <summary>
/// Gestor centralizado de comunicación con la API REST de Hogwarts Dash.
/// Singleton que persiste entre escenas.
/// </summary>
public class ApiManager : MonoBehaviour
{
    public static ApiManager Instance { get; private set; }

    [Header("Configuración de API")]
    public string apiBaseUrl = "https://hogwartsdashapi.onrender.com/api/v1";
    
    [Header("Seguridad")]
    public string apiKey = "hogwarts_dash_2026_admin";
    
    [Header("Timeouts")]
    public int timeoutSeconds = 90;

    /// <summary>
    /// Inicializa el singleton y evita duplicados entre escenas
    /// </summary>
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
        else 
        { 
            Destroy(gameObject); 
        }
    }

    // ---------------------------------------------------------
    // ----------------------- LOGIN ---------------------------
    // ---------------------------------------------------------

    /// <summary>
    /// Autentica un jugador contra la API.
    /// Guarda player_id y username en PlayerPrefs si el login es exitoso.
    /// </summary>
    /// <param name="user">Nombre de usuario</param>
    /// <param name="pass">Contraseña</param>
    /// <param name="callback">Callback con (éxito, mensaje, datos del jugador)</param>
    public void Login(string user, string pass, Action<bool, string, UserData> callback) 
    {
        var requestData = new LoginRequest { username = user, password = pass };
        Debug.Log($"[API] Solicitando login para: {user}");
        
        StartCoroutine(PostRequest<LoginResponse>("/jugadores/login", requestData, (exito, res) => {
            if (exito && res != null && !string.IsNullOrEmpty(res.player_id)) 
            {
                PlayerPrefs.SetString("player_id", res.player_id);
                PlayerPrefs.SetString("username", res.username);
                PlayerPrefs.Save();

                var userData = new UserData { player_id = res.player_id, username = res.username };
                callback(true, "Acceso concedido", userData);
            }
            else callback(false, res?.message ?? "Error de conexión", null);
        }));
    }

    // ---------------------------------------------------------
    // ----------------------- REGISTRO ------------------------
    // ---------------------------------------------------------

    /// <summary>
    /// Registra un nuevo jugador en el sistema.
    /// </summary>
    /// <param name="user">Nombre de usuario único</param>
    /// <param name="nom">Nombre real</param>
    /// <param name="ape">Apellido</param>
    /// <param name="pass">Contraseña</param>
    /// <param name="callback">Callback con (éxito, mensaje)</param>
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

    // ---------------------------------------------------------
    // ----------------------- RANKING -------------------------
    // ---------------------------------------------------------

    /// <summary>
    /// Obtiene el top 10 de jugadores ordenado por snitches
    /// </summary>
    /// <param name="callback">Callback con (éxito, array de entradas del ranking)</param>
    public void ObtenerRanking(Action<bool, RankingEntry[]> callback) 
    {
        StartCoroutine(GetRequest<RankingEntry[]>("/jugadores/top", callback));
    }

    // ---------------------------------------------------------
    // ----------------------- VER PERFIL -------------------------
    // ---------------------------------------------------------

    public void ObtenerJugador(string playerId, Action<bool, PlayerData> callback)
    {
        StartCoroutine(GetRequestSingle<PlayerData>($"/jugadores/{playerId}", callback));
    }

    private IEnumerator GetRequestSingle<T>(string endpoint, Action<bool, T> callback)
    {
        string url = apiBaseUrl.TrimEnd('/') + endpoint;
        Debug.Log($"[API] GET → {url}");

        using (UnityWebRequest req = UnityWebRequest.Get(url))
        {
            if (!string.IsNullOrEmpty(apiKey)) req.SetRequestHeader("api-key", apiKey);
            req.timeout = timeoutSeconds;
            yield return req.SendWebRequest();

            if (req.result == UnityWebRequest.Result.Success)
                callback(true, JsonUtility.FromJson<T>(req.downloadHandler.text));
            else
                callback(false, default(T));
        }
    }

    // ---------------------------------------------------------
    // ----------------------- PROGRESO -------------------------
    // ---------------------------------------------------------

    /// <summary>
    /// Actualiza progreso de nivel específico (muerte o victoria)
    /// </summary>
    public void ActualizarProgresoNivel(string playerId, string nivel, int porcentaje, int snitch, int intentos, float tiempo, Action<bool, string> callback)
    {
        var data = new ProgresoNivelUpdate 
        { 
            porcentaje = porcentaje, 
            snitch = snitch, 
            intentos = intentos, 
            tiempo = tiempo 
        };
        
        string endpoint = $"/{playerId}/progreso/{nivel}";
        Debug.Log($"[API] Actualizando progreso: {endpoint} → {porcentaje}% +{snitch}s");
        
        StartCoroutine(PostRequest<ApiResponse>(endpoint, data, (exito, res) => {
            if (exito) 
            {
                callback(true, "Progreso sincronizado");

                if (Instance != null)
                {
                    ActualizarStatsGlobales(playerId, (statsOk, stats) => {
                        if (Instance != null && stats != null)
                            Debug.Log($"[AUTO] Stats sync: {stats.snitch_totales} snitch totales");
                    });
                }
            }
            else callback(false, res?.mensaje ?? "Error al guardar progreso");
        }));
    }

    /// <summary>
    /// Recalcula y sincroniza estadísticas globales del jugador
    /// </summary>
    public void ActualizarStatsGlobales(string playerId, Action<bool, StatsResponse> callback)
    {
        string endpoint = $"/jugadores/{playerId}/stats";
        Debug.Log($"[API] Actualizando stats globales: {endpoint}");
        
        StartCoroutine(GetRequestSingle<StatsResponse>(endpoint, (exito, res) => {
            if (exito && res != null) 
                Debug.Log($"[API] Stats actualizadas: {res.snitch_totales} snitch totales");
            callback(exito, res);
        }));
    }

    // ---------------------------------------------------------
    // ------------------- PETICIONES HTTP ---------------------
    // ---------------------------------------------------------

    /// <summary>
    /// Realiza una petición POST con body JSON y deserializa la respuesta.
    /// </summary>
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

    /// <summary>
    /// Realiza una petición GET y deserializa la respuesta.
    /// El JSON array se envuelve en un objeto para compatibilidad con JsonUtility.
    /// </summary>
    private IEnumerator GetRequest<T>(string endpoint, Action<bool, T> callback) 
    {
        string url = apiBaseUrl.TrimEnd('/') + endpoint;
        Debug.Log($"[API] GET → {url}");

        using (UnityWebRequest req = UnityWebRequest.Get(url)) 
        {
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

    // ---------------------------------------------------------
    // ------------------- MODELOS DE DATOS --------------------
    // ---------------------------------------------------------

    [Serializable] public class LoginRequest   { public string username, password; }
    [Serializable] public class RegistroRequest { public string nombre, apellido, username, password; }
    [Serializable] public class ApiResponse    { public bool exito; public string mensaje; public string detail; }
    [Serializable] public class LoginResponse { public string player_id; public string username; public string nombre; public string apellido; public int total_intentos; public int niveles_completados; public int snitch_totales; public string message;}
    [Serializable] public class UserData       { public string player_id, username; public int total_puntuacion; }
    [Serializable] public class RankingEntry   { public string username; public int puntuacion; }
    [Serializable] public class RankingList    { public RankingEntry[] ranking; }
    [Serializable] public class PlayerData { public string username; public int total_intentos; public int niveles_completados; public int snitch_totales; public string created_at;}
    [Serializable] public class ProgresoNivelUpdate { public float porcentaje; public int snitch; public int intentos;  public float tiempo;}
    [Serializable] public class ProgresoResponse { public string message; public string nivel; public float porcentaje_max; public int intentos_total; public int snitch_total; public float tiempo_total;}
    [Serializable] public class StatsResponse { public string player_id; public int total_intentos; public int snitch_totales; public int niveles_completados; public string message;}

}