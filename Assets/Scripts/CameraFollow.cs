using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float xOffset = 3f; // Offset ajustado para acercar la cámara
    [Header("Zoom Pixel Art")]
    public float orthographicSize = 5f; // Tamaño estándar para ver detalles en assets 32x32 (si PPU=32)

    void Start()
    {
        Camera cam = GetComponent<Camera>();
        if (cam != null)
        {
            cam.orthographic = true;
            cam.orthographicSize = orthographicSize;
        }
    }
    
    void LateUpdate()
    {
        if (target != null)
        {
            Vector3 temp = transform.position;
            // Solo seguimos en X, Y fijo (o ajustar según necesidad)
            temp.x = target.position.x + xOffset;
            transform.position = temp;
        }
    }
}
