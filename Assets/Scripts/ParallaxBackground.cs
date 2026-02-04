using UnityEngine;

/// <summary>
/// Efecto parallax para fondos - Crea sensación de profundidad
/// </summary>
public class ParallaxBackground : MonoBehaviour
{
    [Header("Configuración")]
    [Tooltip("Referencia a la cámara a seguir")]
    public Transform cameraTransform;
    
    [Range(0f, 1f)]
    [Tooltip("0 = se mueve con la cámara, 1 = estático en el mundo")]
    public float parallaxEffect = 0.5f;

    private float lastCameraX;
    private float textureUnitSizeX;

    private void Start()
    {
        if (cameraTransform == null)
            cameraTransform = Camera.main.transform;

        lastCameraX = cameraTransform.position.x;

        // Obtener tamaño del sprite para tiling infinito
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null && spriteRenderer.sprite != null)
        {
            textureUnitSizeX = spriteRenderer.sprite.bounds.size.x;
        }
    }

    /// <summary>
    /// Actualiza posición del fondo con efecto parallax
    /// </summary>
    private void LateUpdate()
    {
        // Calcular movimiento parallax
        float deltaX = cameraTransform.position.x - lastCameraX;
        float parallaxMovement = deltaX * parallaxEffect;
        
        transform.Translate(new Vector3(parallaxMovement, 0, 0));

        lastCameraX = cameraTransform.position.x;

        // Lógica de repetición infinita (si el sprite es tileable)
        if (textureUnitSizeX > 0)
        {
            if (Mathf.Abs(cameraTransform.position.x - transform.position.x) >= textureUnitSizeX)
            {
                float offsetPosition = (cameraTransform.position.x - transform.position.x) % textureUnitSizeX;
                transform.position = new Vector3(cameraTransform.position.x + offsetPosition, transform.position.y);
            }
        }
    }
}
