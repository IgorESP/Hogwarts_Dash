using UnityEngine;

/// <summary>
/// Obstáculo letal (pinchos, sierras, etc) que causa muerte del jugador
/// </summary>
public class Obstacle : MonoBehaviour
{
    /// <summary>
    /// Detecta colisión con trigger
    /// </summary>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log($"[Obstacle] Jugador impactó con obstáculo (Trigger): {gameObject.name}");
            GameManager.Instance.PlayerDied();
            collision.gameObject.SetActive(false); // Desactivar jugador
        }
    }

    /// <summary>
    /// Detecta colisión física
    /// </summary>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log($"[Obstacle] Jugador impactó con obstáculo (Físico): {gameObject.name}");
            GameManager.Instance.PlayerDied();
            collision.gameObject.SetActive(false);
        }
    }
}
