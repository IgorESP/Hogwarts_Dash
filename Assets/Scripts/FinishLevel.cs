using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Script para detectar cuando el jugador completa un nivel.
/// Colocar este script en un GameObject con Collider2D (Is Trigger = true) en la meta del nivel.
/// </summary>
public class FinishLevel : MonoBehaviour
{
    [Header("Configuración")]
    [Tooltip("Escena a cargar si no hay GameManager (fallback)")]
    public string escenaFallback = "MenuPrincipalT";
    
    [Tooltip("Tiempo de espera antes de cargar la escena (segundos)")]
    public float tiempoEspera = 0.5f;
    
    // Variable para evitar múltiples activaciones
    private bool nivelCompletado = false;
    
    /// <summary>
    /// Se ejecuta cuando algo entra en el trigger.
    /// Solo detecta objetos 2D con Collider2D.
    /// </summary>
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Log para debug - ver qué está tocando el trigger
        Debug.Log($"[FinishLevel] Trigger detectado: {other.name} (Tag: '{other.tag}')");
        
        // Verificar que sea el jugador y que no hayamos completado ya
        if (other.CompareTag("Player") && !nivelCompletado)
        {
            Debug.Log("[FinishLevel] JUGADOR DETECTADO - Finalizando nivel!");
            nivelCompletado = true;
            FinalizarNivel();
        }
    }
    
    /// <summary>
    /// Llama al GameManager para procesar el final del nivel.
    /// Si no existe GameManager, carga la escena de fallback.
    /// </summary>
    private void FinalizarNivel()
    {
        Debug.Log("[FinishLevel] Nivel completado!");
        
        // Intentar llamar al GameManager
        if (GameManager.Instance != null)
        {
            // El GameManager se encarga de:
            // 1. Calcular porcentaje de monedas
            // 2. Registrar estrellas
            // 3. Desbloquear siguiente nivel
            // 4. Mostrar pantalla de resultados
            GameManager.Instance.FinalizarNivel();
        }
        else
        {
            // Si no hay GameManager, cargar escena de fallback
            Debug.LogWarning("[FinishLevel] GameManager no encontrado - Cargando escena de fallback");
            Invoke("CargarEscenaFallback", tiempoEspera);
        }
    }
    
    /// <summary>
    /// Carga la escena de fallback si no hay GameManager.
    /// </summary>
    private void CargarEscenaFallback()
    {
        Debug.Log($"[FinishLevel] Cargando escena de fallback: {escenaFallback}");
        SceneManager.LoadScene(escenaFallback);
    }
    
    // ============================================
    // MÉTODOS DE DEBUG
    // ============================================
    
    /// <summary>
    /// Dibuja el área del trigger en el editor (Gizmos).
    /// Solo visible en Scene View.
    /// </summary>
    private void OnDrawGizmos()
    {
        // Obtener el collider
        Collider2D col = GetComponent<Collider2D>();
        if (col == null) return;
        
        // Dibujar en color verde si está activo
        Gizmos.color = nivelCompletado ? Color.red : Color.green;
        
        // Dibujar el área del trigger
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