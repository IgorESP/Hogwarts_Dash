using UnityEngine;

/// <summary>
/// Script que controla el seguimiento de la cámara al jugador
/// La cámara sigue al objetivo (target) en el eje X con un offset configurable
/// Diseñado para juegos de plataformas 2D con estilo Pixel Art
/// </summary>
public class CameraFollow : MonoBehaviour
{
    // ========================================
    // CAMPOS PÚBLICOS - Configurables desde el Inspector
    // ========================================
    
    [Tooltip("Transform del objeto que la cámara debe seguir (normalmente el jugador)")]
    public Transform target;
    
    [Tooltip("Distancia horizontal delante del jugador donde se posiciona la cámara")]
    public float xOffset = 3f; // Offset ajustado para acercar la cámara y dar visión adelante
    
    [Header("Zoom Pixel Art")]
    [Tooltip("Tamaño ortográfico de la cámara. Valores menores = más zoom")]
    public float orthographicSize = 5f; // Tamaño estándar para ver detalles en assets 32x32 (si PPU=32)

    // ========================================
    // MÉTODO DE INICIALIZACIÓN
    // ========================================
    
    /// <summary>
    /// Se ejecuta al inicio del juego
    /// Configura la cámara en modo ortográfico para pixel art
    /// </summary>
    void Start()
    {
        // Obtener el componente Camera adjunto a este GameObject
        Camera cam = GetComponent<Camera>();
        
        // Verificar que la cámara existe
        if (cam != null)
        {
            // Configurar cámara ortográfica (ideal para juegos 2D/Pixel Art)
            cam.orthographic = true;
            cam.orthographicSize = orthographicSize;
            
            Debug.Log($"[CameraFollow] Cámara inicializada - Modo: Ortográfico, Tamaño: {orthographicSize}");
        }
        else
        {
            Debug.LogError("[CameraFollow] No se encontró componente Camera en este GameObject!");
        }
        
        // Verificar si se asignó un objetivo
        if (target == null)
        {
            Debug.LogWarning("[CameraFollow] No se ha asignado un 'target' en el Inspector. La cámara no seguirá ningún objeto.");
        }
        else
        {
            Debug.Log($"[CameraFollow] Target asignado: {target.name}");
        }
    }
    
    // ========================================
    // MÉTODO DE ACTUALIZACIÓN DE CÁMARA
    // ========================================
    
    /// <summary>
    /// Se ejecuta después de todos los Update()
    /// LateUpdate es ideal para cámaras porque garantiza que el jugador ya se movió
    /// </summary>
    void LateUpdate()
    {
        // Verificar que existe un objetivo a seguir
        if (target != null)
        {
            // Guardar la posición actual de la cámara en una variable temporal
            Vector3 temp = transform.position;
            
            // SEGUIMIENTO HORIZONTAL (EJE X)
            // Actualizamos solo la posición X para seguir al jugador horizontalmente
            // Sumamos el xOffset para dar visión adelante (útil en juegos de plataformas)
            temp.x = target.position.x + xOffset;
            
            // NOTA: Y y Z se mantienen fijos
            // - Y (vertical) se mantiene constante para evitar que la cámara suba/baje
            // - Z (profundidad) debe ser negativo en 2D para que la cámara pueda ver los sprites
            
            // Aplicar la nueva posición a la cámara
            transform.position = temp;
            
            // Log opcional (comentado para rendimiento, descomentar si necesitas debug)
            // Debug.Log($"[CameraFollow] Posición cámara: X={temp.x:F2}, Y={temp.y:F2}, Z={temp.z:F2}");
        }
        else
        {
            // Si el objetivo se destruye durante el juego, registrar advertencia
            // (Esto podría pasar si el jugador muere y se destruye el GameObject)
            Debug.LogWarning("[CameraFollow] El target es null. La cámara no se moverá.");
        }
    }
}
