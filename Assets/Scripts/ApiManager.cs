using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;
using System.Text;

public class ApiManager : MonoBehaviour
{
    public static ApiManager Instance { get; private set; }

    [Header("⚙️ Configuración")]
    public string apiBaseUrl = "https://hogwartsdashapi.onrender.com/api/v1/jugadores"; 
    public int timeoutSeconds = 90; 

    void Awake() {
        if (Instance == null) { 
            Instance = this; 
            DontDestroyOnLoad(gameObject);
            apiBaseUrl = "https://hogwartsdashapi.onrender.com/api/v1/jugadores";
        }
        else { Destroy(gameObject); }
    }

    public void Login(string user, string pass, Action<bool, string, UserData> callback) {
        var requestData = new LoginRequest { username = user, password = pass };
        StartCoroutine(PostRequest<LoginResponse>("/login", requestData, (exito, res) => {
            if (exito && res != null) callback(true, "¡Bienvenido!", res.usuario);
            else callback(false, res?.mensaje ?? res?.detail ?? "Error de credenciales", null);
        }));
    }

    public void Registrar(string user, string nom, string ape, string pass, Action<bool, string> callback) {
        var data = new RegistroRequest { username = user, nombre = nom, apellido = ape, password = pass };
        StartCoroutine(PostRequest<ApiResponse>("/register", data, (exito, res) => {
            if (exito) callback(true, "Registro exitoso");
            else callback(false, res?.mensaje ?? res?.detail ?? "Error al registrar");
        }));
    }

    // --- CORRECCIÓN DEL RANKING (Ruta /top) ---
    public void ObtenerRanking(Action<bool, RankingEntry[]> callback) {
        // Cambiado /ranking por /top según tu Swagger
        StartCoroutine(GetRequest<RankingEntry[]>("/top", (exito, res) => {
            callback(exito, res);
        }));
    }

    private IEnumerator PostRequest<T>(string endpoint, object data, Action<bool, T> callback) {
        string json = JsonUtility.ToJson(data);
        string url = apiBaseUrl.TrimEnd('/') + endpoint;
        using (UnityWebRequest req = new UnityWebRequest(url, "POST")) {
            byte[] body = Encoding.UTF8.GetBytes(json);
            req.uploadHandler = new UploadHandlerRaw(body);
            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader("Content-Type", "application/json");
            req.timeout = timeoutSeconds; 
            yield return req.SendWebRequest();
            if (req.result == UnityWebRequest.Result.Success) {
                callback(true, JsonUtility.FromJson<T>(req.downloadHandler.text));
            } else {
                T errorRes = default;
                try { errorRes = JsonUtility.FromJson<T>(req.downloadHandler.text); } catch { }
                callback(false, errorRes);
            }
        }
    }

    private IEnumerator GetRequest<T>(string endpoint, Action<bool, T> callback) {
        using (UnityWebRequest req = UnityWebRequest.Get(apiBaseUrl + endpoint)) {
            req.timeout = timeoutSeconds;
            yield return req.SendWebRequest();
            if (req.result == UnityWebRequest.Result.Success) {
                // Truco para leer listas [] en Unity
                string json = "{ \"ranking\": " + req.downloadHandler.text + " }";
                RankingList wrapper = JsonUtility.FromJson<RankingList>(json);
                callback(true, (T)(object)wrapper.ranking);
            } else {
                Debug.LogError("Error en Ranking: " + req.error);
                callback(false, default);
            }
        }
    }

    [Serializable] public class LoginRequest { public string username; public string password; }
    [Serializable] public class RegistroRequest { public string nombre; public string apellido; public string username; public string password; }
    [Serializable] public class ApiResponse { public bool exito; public string mensaje; public string detail; }
    [Serializable] public class LoginResponse { public bool exito; public string mensaje; public string detail; public string token; public UserData usuario; }
    [Serializable] public class UserData { public string id; public string username; public string nombre; public string apellido; }
    [Serializable] public class RankingEntry { public string username; public int puntos; }
    [Serializable] public class RankingList { public RankingEntry[] ranking; }
}