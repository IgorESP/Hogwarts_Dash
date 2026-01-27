using UnityEngine;

public class RecoleccionMoneda : MonoBehaviour
{
    public int coinID; // Así identificas si es la 1, 2 o 3

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Aquí llamarías a tu GameManager para guardar el progreso
            Debug.Log("¡Snitch " + coinID + " recogida!");
            
            // Efecto visual o sonido antes de destruir
            Destroy(gameObject);
        }
    }
}
