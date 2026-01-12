using UnityEngine;

public class Obstacle : MonoBehaviour
{
    // Este script se adjunta a pinchos o sierras
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GameManager.Instance.PlayerDied();
            // Desactivar jugador visualmente
            collision.gameObject.SetActive(false);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            GameManager.Instance.PlayerDied();
            collision.gameObject.SetActive(false);
        }
    }
}
