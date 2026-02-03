using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Script para detectar cuando el jugador completa un nivel.
/// Cuenta intentos automáticamente cada vez que se reinicia la escena.
/// </summary>
public class FinishLevel : MonoBehaviour
{
    [Header("Configuración")]
    [Tooltip("Tiempo de espera antes de mostrar resultados")]
    public float tiempoEspera = 0.5f;
    
    private bool nivelCompletado = false;
    private static int intentosAcumulados = 0;
    
    void Start()
    {
        // Cada vez que se carga la escena, incrementar intentos
        // EXCEPTO la primera vez
        string escenaKey = SceneManager.GetActiveScene().name;
        string primeraVezKey = "PrimeraVez_" + escenaKey;
        
        if (!PlayerPrefs.HasKey(primeraVezKey))
        {
            // Primera vez en este nivel, no contar como intento
            PlayerPrefs.SetInt(primeraVezKey, 1);
            intentosAcumulados = 0;
        }
        else
        {
            // Ya jugó antes, esto es un reintento
            intentosAcumulados++;
        }
        
        Debug.Log($"[FinishLevel] Intentos actuales: {intentosAcumulados}");
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !nivelCompletado)
        {
            Debug.Log("[FinishLevel] JUGADOR DETECTADO - Finalizando nivel!");
            nivelCompletado = true;
            Invoke("FinalizarNivel", tiempoEspera);
        }
    }
    
    private void FinalizarNivel()
    {
        Debug.Log("[FinishLevel] Nivel completado!");
        
        // Obtener mundo actual
        int mundoActual = PlayerPrefs.GetInt("MundoSeleccionado", 1);
        
        // CONTADOR CORREGIDO DE SNITCHES
        int snitchesRecogidas = ContarSnitchesRecogidas();
        
        // Calcular porcentaje (siempre 100% si llegó aquí)
        float porcentaje = 100f;
        
        Debug.Log($"[FinishLevel] Estadísticas:");
        Debug.Log($"  - Porcentaje: {porcentaje}%");
        Debug.Log($"  - Intentos: {intentosAcumulados}");
        Debug.Log($"  - Snitches: {snitchesRecogidas}");
        
        // Guardar en NivelesManager
        if (NivelesManager.Instance != null)
        {
            NivelesManager.Instance.CompletarNivel(mundoActual, 1, porcentaje);
        }
        
        // Guardar estadísticas para el panel
        PlayerPrefs.SetFloat("UltimoPorcentaje", porcentaje);
        PlayerPrefs.SetInt("UltimosIntentos", intentosAcumulados);
        PlayerPrefs.SetInt("UltimasSnitches", snitchesRecogidas);
        PlayerPrefs.SetInt("UltimoMundoCompletado", mundoActual);
        PlayerPrefs.Save();
        
        // Resetear intentos para el próximo nivel
        intentosAcumulados = 0;
        PlayerPrefs.DeleteKey("PrimeraVez_" + SceneManager.GetActiveScene().name);
        
        // Llamar a GameManager si existe
        if (GameManager.Instance != null)
        {
            GameManager.Instance.FinalizarNivel();
        }
        else
        {
            // Si no hay GameManager, cargar ResultadosScene directamente
            SceneManager.LoadScene("ResultadosScene");
        }
    }
    
    /// <summary>
    /// CUENTA SNITCHES RECOGIDAS CORRECTAMENTE:
    /// 1. Guarda total al inicio del nivel
    /// 2. Cuenta las que quedan ACTIVAS al final
    /// 3. Recogidas = total - activas
    /// </summary>
    private int ContarSnitchesRecogidas()
    {
        try
        {
            string escenaKey = SceneManager.GetActiveScene().name;
            string totalSnitchesKey = "TotalSnitches_" + escenaKey;
            
            // Buscar snitches ACTIVAS (las que quedan por recoger)
            GameObject[] snitchesActivas = GameObject.FindGameObjectsWithTag("Snitch");
            
            // Primera vez en el nivel: guardar el total
            int totalSnitches = snitchesActivas.Length;
            if (!PlayerPrefs.HasKey(totalSnitchesKey))
            {
                PlayerPrefs.SetInt(totalSnitchesKey, totalSnitches);
                Debug.Log($"[FinishLevel] Total snitches registrado: {totalSnitches}");
            }
            
            // Obtener total guardado
            int totalEsperado = PlayerPrefs.GetInt(totalSnitchesKey, totalSnitches);
            
            // Recogidas = total - las que quedan activas
            int recogidas = totalEsperado - snitchesActivas.Length;
            
            Debug.Log($"[FinishLevel] Snitches: Total={totalEsperado}, Activas={snitchesActivas.Length}, Recogidas={recogidas}");
            return recogidas;
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"[FinishLevel] Error contando snitches: {e.Message}");
            return 0;
        }
    }
    
    private void OnDrawGizmos()
    {
        Collider2D col = GetComponent<Collider2D>();
        if (col == null) return;
        
        Gizmos.color = nivelCompletado ? Color.red : Color.green;
        
        if (col is BoxCollider2D box)
        {
            Gizmos.DrawWireCube(transform.position + (Vector3)box.offset, box.size);
        }
        else if (col is CircleCollider2D circle)
        {
            Gizmos.DrawWireSphere(transform.position + (Vector3)circle.offset, circle.radius);
        }
    }
}
