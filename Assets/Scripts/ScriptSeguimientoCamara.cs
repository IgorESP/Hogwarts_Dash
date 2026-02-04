using UnityEngine;

/// <summary>
/// Seguimiento de cámara estilo Geometry Dash - Movimiento estricto en X, suave en Y
/// </summary>
public class CameraFollowGD : MonoBehaviour
{
    [Tooltip("Transform del jugador a seguir")]
    public Transform jugador;
    
    [Tooltip("Offset horizontal (distancia delante del jugador)")]
    public float offsetX = 3f;
    
    [Tooltip("Offset vertical")]
    public float offsetY = 0f;
    
    [Tooltip("Suavizado del movimiento vertical")]
    public float suavizado = 5f;

    /// <summary>
    /// Actualiza posición de la cámara - Se ejecuta después de todos los Update()
    /// </summary>
    void LateUpdate()
    {
        // EJE X: Seguimiento estricto (sin Lerp) para evitar oscilaciones
        float xObjetivo = jugador.position.x + offsetX;

        // EJE Y: Seguimiento suave para saltos y caídas
        float yObjetivo = Mathf.Lerp(transform.position.y, jugador.position.y + offsetY, suavizado * Time.deltaTime);

        transform.position = new Vector3(xObjetivo, yObjetivo, transform.position.z);

        // 🔒 IMPORTANTE: La cámara jamás rota
        transform.rotation = Quaternion.identity;
    }
}
