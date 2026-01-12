using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    [Header("Configuración")]
    public Transform cameraTransform;
    [Range(0f, 1f)]
    public float parallaxEffect = 0.5f; // 0 = se mueve con la cámara (estático visualmente), 1 = quieto en el mundo

    private float lastCameraX;
    private float textureUnitSizeX;

    private void Start()
    {
        if (cameraTransform == null)
            cameraTransform = Camera.main.transform;

        lastCameraX = cameraTransform.position.x;

        // Intentar obtener el tamaño del sprite para hacer tiling infinito (opcional)
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null && spriteRenderer.sprite != null)
        {
            textureUnitSizeX = spriteRenderer.sprite.bounds.size.x;
        }
    }

    private void LateUpdate()
    {
        float deltaX = cameraTransform.position.x - lastCameraX;
        float parallaxMovement = deltaX * parallaxEffect;
        
        transform.Translate(new Vector3(parallaxMovement, 0, 0));

        lastCameraX = cameraTransform.position.x;

        // Lógica de repetición infinita (si la imagen es tileable)
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
