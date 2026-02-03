using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Script para detectar cuando el jugador completa un nivel.
/// ✅ FIX: Intentos NO se duplican + Monedas tag "moneda"
/// </summary>
public class FinishLevel : MonoBehaviour
{
    [Header("Configuración")]
    [Tooltip("Tiempo de espera antes de mostrar resultados")]
    public float tiempoEspera = 0.5f;
    
    private bool nivelCompletado = false;
    
    void Start()
    {
        // ✅ FIX INTENTOS: Usa PlayerPrefs, NO static que se duplica
        string escenaKey = SceneManager.GetActiveScene().name;
        string primeraVezKey = "PrimeraVez_" + escenaKey;
        string intentosKey = "Intentos_" + escenaKey;
        
        if (!PlayerPrefs.HasKey(primeraVezKey))
        {
            // ⭐ PRIMERA VEZ = 0 intentos
            PlayerPrefs.SetInt(primeraVezKey, 1);
            PlayerPrefs.SetInt(intentosKey, 0);
            Debug.Log("[FinishLevel] ⭐ PRIMERA VEZ → 0 intentos");
        }
        else
        {
            // 🔄 REINICIO = +1 intento (leer anterior + sumar)
            int intentosAnteriores = PlayerPrefs.GetInt(intentosKey, 0);
            int nuevosIntentos = intentosAnteriores + 1;
            PlayerPrefs.SetInt(intentosKey, nuevosIntentos);
            PlayerPrefs.Save();
            Debug.Log($"[FinishLevel] 🔄 REINICIO #{nuevosIntentos} (era {intentosAnteriores})");
        }
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
        Debug.Log("[FinishLevel] 🎉 Nivel completado!");
        
        string escenaKey = SceneManager.GetActiveScene().name;
        int intentos = PlayerPrefs.GetInt("Intentos_" + escenaKey, 0);
        int mundoActual = PlayerPrefs.GetInt("MundoSeleccionado", 1);
        
        // ✅ MONEDAS (fallback múltiples tags)
        int monedasRecogidas = ContarMonedasRecogidas();
        float porcentaje = 100f;
        
        Debug.Log($"[FinishLevel] 📊 ESTADÍSTICAS:");
        Debug.Log($"  Porcentaje: {porcentaje}%");
        Debug.Log($"  Intentos: {intentos}");
        Debug.Log($"  Monedas: {monedasRecogidas}");
        
        // Guardar NivelesManager
        if (NivelesManager.Instance != null)
            NivelesManager.Instance.CompletarNivel(mundoActual, 1, porcentaje);
        
        // ✅ GUARDAR para GameManager (CORREGIDO: monedas)
        PlayerPrefs.SetFloat("UltimoPorcentaje", porcentaje);
        PlayerPrefs.SetInt("UltimosIntentos", intentos);
        PlayerPrefs.SetInt("UltimasMonedas", monedasRecogidas);  // ← MONEDAS
        PlayerPrefs.SetInt("UltimoMundoCompletado", mundoActual);
        PlayerPrefs.Save();
        
        // Llamar GameManager
        if (GameManager.Instance != null)
            GameManager.Instance.FinalizarNivel();
        else
            SceneManager.LoadScene("ResultadosScene");
    }
    
    /// <summary>
    /// ✅ MONEDAS con fallback de tags comunes
    /// </summary>
    private int ContarMonedasRecogidas()
    {
        try
        {
            string escenaKey = SceneManager.GetActiveScene().name;
            string totalKey = "TotalMonedas_" + escenaKey;
            
            // Probar múltiples tags comunes
            GameObject[] monedas = GameObject.FindGameObjectsWithTag("moneda");
            if (monedas.Length == 0) monedas = GameObject.FindGameObjectsWithTag("Coin");
            if (monedas.Length == 0) monedas = GameObject.FindGameObjectsWithTag("Collectible");
            if (monedas.Length == 0) monedas = GameObject.FindGameObjectsWithTag("Moneda");
            
            int totalMonedas = monedas.Length;
            
            // Registrar total si es primera vez
            if (!PlayerPrefs.HasKey(totalKey))
            {
                PlayerPrefs.SetInt(totalKey, totalMonedas);
                Debug.Log($"[FinishLevel] 💰 Total monedas: {totalMonedas} (tag usado)");
            }
            
            int totalEsperado = PlayerPrefs.GetInt(totalKey, totalMonedas);
            int recogidas = totalEsperado - monedas.Length;
            
            Debug.Log($"[FinishLevel] 💰 Total={totalEsperado} | Activas={monedas.Length} | Recogidas={recogidas}");
            return recogidas;
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"[FinishLevel] ❌ Error monedas: {e.Message}");
            return 0;
        }
    }
    
    private void OnDrawGizmos()
    {
        Collider2D col = GetComponent<Collider2D>();
        if (col == null) return;
        
        Gizmos.color = nivelCompletado ? Color.red : Color.green;
        
        if (col is BoxCollider2D box)
            Gizmos.DrawWireCube(transform.position + (Vector3)box.offset, box.size);
        else if (col is CircleCollider2D circle)
            Gizmos.DrawWireSphere(transform.position + (Vector3)circle.offset, circle.radius);
    }
}
