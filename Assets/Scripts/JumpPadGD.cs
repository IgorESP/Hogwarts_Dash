using UnityEngine;

public class JumpPadGD : MonoBehaviour
{
    public float fuerzaSalto = 35f;
    public Animator anim;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            MovimientoGD playerScript = other.GetComponent<MovimientoGD>();
            Rigidbody2D rb = other.GetComponent<Rigidbody2D>();

            if (rb != null && playerScript != null)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
                rb.AddForce(Vector2.up * fuerzaSalto, ForceMode2D.Impulse);

                // ACTIVAR ROTACIÓN: Igual que en el salto normal
                playerScript.rotacionRestante = 180f;

                if (anim != null) anim.SetTrigger("Activar");
            }
        }
    }
}