using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Script para detectar cuando el jugador completa un nivel.
/// Porcentaje: SIEMPRE 100% al llegar al final
/// Cuenta: Intentos (muertes) y Monedas recogidas
/// </summary>
public class FinishLevel : MonoBehaviour
{
    [Header("Configuración")]
    [Tooltip("Tiempo de espera antes de mostrar resultados")]
    public float tiempoEspera = 0.5f;
    
    private bool nivelCompletado = false;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !nivelCompletado)
        {
            Debug.Log("[FinishLevel] ¡JUGADOR DETECTADO! Finalizando nivel...");
            nivelCompletado = true;
            Invoke("FinalizarNivel", tiempoEspera);
        }
    }
    
    private void FinalizarNivel()
    {
        Debug.Log("[FinishLevel] 🎉 Nivel completado!");
        
        int mundoActual = PlayerPrefs.GetInt("MundoSeleccionado", 1);
        
        // ESTADÍSTICAS
        float porcentaje = 100f; // ✅ SIEMPRE 100%
        int monedasRecogidas = ContarMonedasRecogidas();
        int intentos = ObtenerIntentos();
        
        Debug.Log($"[FinishLevel] 📊 ESTADÍSTICAS:");
        Debug.Log($"   ✅ Porcentaje: 100% (nivel completado)");
        Debug.Log($"   💀 Intentos: {intentos}");
        Debug.Log($"   💰 Monedas: {monedasRecogidas}");
        
        // Guardar en NivelesManager
        if (NivelesManager.Instance != null)
        {
            NivelesManager.Instance.CompletarNivel(mundoActual, 1, porcentaje);
        }
        
        // Guardar para panel de resultados
        PlayerPrefs.SetFloat("UltimoPorcentaje", porcentaje);
        PlayerPrefs.SetInt("UltimosIntentos", intentos);
        PlayerPrefs.SetInt("UltimasSnitches", monedasRecogidas);
        PlayerPrefs.SetInt("UltimoMundoCompletado", mundoActual);
        PlayerPrefs.Save();
        
        // Resetear intentos para el próximo intento
        string escenaActual = SceneManager.GetActiveScene().name;
        PlayerPrefs.DeleteKey("Intentos_" + escenaActual);
        PlayerPrefs.DeleteKey("IntentosActuales");
        
        // Llamar GameManager
        if (GameManager.Instance != null)
        {
            GameManager.Instance.FinalizarNivel();
        }
        else
        {
            SceneManager.LoadScene("ResultadosScene");
        }
    }
    
    /// <summary>
    /// Cuenta monedas recogidas (inactivas con tag "Moneda")
    /// </summary>
    private int ContarMonedasRecogidas()
    {
        try
        {
            GameObject[] monedas = GameObject.FindGameObjectsWithTag("Moneda");
            int recogidas = 0;
            
            foreach (GameObject moneda in monedas)
            {
                if (!moneda.activeInHierarchy)
                    recogidas++;
            }
            
            Debug.Log($"[FinishLevel] 💰 Monedas recogidas: {recogidas}/{monedas.Length}");
            return recogidas;
        }
        catch
        {
            Debug.LogWarning("[FinishLevel] ⚠️ No hay monedas con tag 'Moneda'");
            return 0;
        }
    }
    
    /// <summary>
    /// Obtiene intentos (muertes) desde PlayerPrefs
    /// </summary>
    private int ObtenerIntentos()
    {
        string escenaActual = SceneManager.GetActiveScene().name;
        int intentos = PlayerPrefs.GetInt("Intentos_" + escenaActual, 0);
        
        Debug.Log($"[FinishLevel] 💀 Intentos totales: {intentos}");
        return intentos;
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