using UnityEngine;

public class CameraFollowGD : MonoBehaviour
{
    public Transform jugador;
    public float offsetX = 3f;
    public float offsetY = 0f;
    public float suavizado = 5f;

    void LateUpdate()
    {
        // EJE X: Seguimiento estricto (sin Lerp) para evitar oscilaciones con la velocidad constante
        float xObjetivo = jugador.position.x + offsetX;

        // EJE Y: Seguimiento suave para saltos y caídas
        float yObjetivo = Mathf.Lerp(transform.position.y, jugador.position.y + offsetY, suavizado * Time.deltaTime);

        transform.position = new Vector3(xObjetivo, yObjetivo, transform.position.z);

        // 🔒 MUY IMPORTANTE: la cámara jamás rota
        transform.rotation = Quaternion.identity;
    }
}
