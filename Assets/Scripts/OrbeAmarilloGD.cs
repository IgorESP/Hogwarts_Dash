using UnityEngine;

public class OrbeAmarilloGD : MonoBehaviour
{
    public float fuerzaImpulso = 25f;

    [Header("Efectos Visuales")]
    public ParticleSystem particulasBrillo; // Arrastra aquí el sistema de partículas hijo
    
    // NOTA: Configura las partículas DIRECTAMENTE en el Inspector de Unity.
    // Valores recomendados para efecto Geometry Dash:
    // - Start Lifetime: 0.3
    // - Start Speed: 20
    // - Start Size: 0.2
    
    public int cantidadParticulasExplosion = 300; 

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
        // 1. Emitir partículas
        if (particulasBrillo != null)
        {
            particulasBrillo.Emit(cantidadParticulasExplosion);
        }

        // 2. Lógica física del salto
        if (scriptJugador != null)
        {
            Rigidbody2D rb = scriptJugador.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
                rb.AddForce(Vector2.up * fuerzaImpulso, ForceMode2D.Impulse);
                
                scriptJugador.rotacionRestante = 180f;
            }
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
