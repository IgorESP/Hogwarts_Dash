using UnityEngine;

/// <summary>
/// Gestiona la recolección de monedas (Snitches) por el jugador
/// </summary>
public class RecoleccionMoneda : MonoBehaviour
{
    [Tooltip("ID único de la moneda (1, 2, 3, etc.)")]
    public int coinID;

    /// <summary>
    /// Detecta cuando el jugador recoge la moneda
    /// </summary>
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log($"[RecoleccionMoneda] ¡Snitch {coinID} recogida!");

            // Si tiene animación de recolección, NO destruir aquí
            // AnimacionSnitch se encargará de destruir el objeto tras la animación
            if (GetComponent<AnimacionSnitch>() != null) 
            {
                Debug.Log($"[RecoleccionMoneda] Snitch {coinID} tiene animación, esperando...");
                return; 
            }
            
            // Si no tiene animación, destruir inmediatamente
            Destroy(gameObject);
        }
    }
}
