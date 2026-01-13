using UnityEngine;

public class CameraFollowGD : MonoBehaviour
{
    public Transform jugador;
    public float offsetX = 3f;
    public float offsetY = 0f;
    public float suavizado = 5f;

    void LateUpdate()
    {
        Vector3 objetivo = new Vector3(
            jugador.position.x + offsetX,
            jugador.position.y + offsetY,
            transform.position.z
        );

        transform.position = Vector3.Lerp(
            transform.position,
            objetivo,
            suavizado * Time.deltaTime
        );

        // 🔒 MUY IMPORTANTE: la cámara jamás rota
        transform.rotation = Quaternion.identity;
    }
}
