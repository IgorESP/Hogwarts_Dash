using UnityEngine;

/// <summary>
/// Plataforma de salto estilo Geometry Dash - Impulsa al jugador hacia arriba
/// </summary>
public class JumpPadGD : MonoBehaviour
{
    [Tooltip("Fuerza del impulso aplicado al jugador")]
    public float fuerzaSalto = 35f;
    
    [Tooltip("Animator para animación de activación")]
    public Animator anim;

    /// <summary>
    /// Detecta cuando el jugador toca la plataforma de salto
    /// </summary>
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            MovimientoGD playerScript = other.GetComponent<MovimientoGD>();
            Rigidbody2D rb = other.GetComponent<Rigidbody2D>();

            if (rb != null && playerScript != null)
            {
                // Resetear velocidad vertical y aplicar impulso
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
                rb.AddForce(Vector2.up * fuerzaSalto, ForceMode2D.Impulse);

                // Activar rotación del cubo
                playerScript.rotacionRestante = 180f;

                // Reproducir animación
                if (anim != null) anim.SetTrigger("Activar");
                
                Debug.Log($"[JumpPad] Impulso aplicado - Fuerza: {fuerzaSalto}");
            }
        }
    }
}