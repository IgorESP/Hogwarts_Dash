using UnityEngine;

public class OrbeAmarilloGD : MonoBehaviour
{
    public float fuerzaImpulso = 25f;

    private bool jugadorEnRango = false;
    private MovimientoGD scriptJugador; // Referencia al script principal

    void Update()
    {
        if (jugadorEnRango && (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)))
        {
            Saltar();
        }
    }

    void Saltar()
    {
        if (scriptJugador != null)
        {
            Rigidbody2D rb = scriptJugador.GetComponent<Rigidbody2D>();
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
            rb.AddForce(Vector2.up * fuerzaImpulso, ForceMode2D.Impulse);

            // ACTIVAR ROTACIÓN: Le decimos al jugador que gire 180 grados
            scriptJugador.rotacionRestante = 180f;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorEnRango = true;
            scriptJugador = other.GetComponent<MovimientoGD>();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorEnRango = false;
            scriptJugador = null;
        }
    }
}