using UnityEngine;

/// <summary>
/// Orbe amarillo estilo Geometry Dash - Proporciona impulso vertical al jugador al presionar salto
/// </summary>
public class OrbeAmarilloGD : MonoBehaviour
{
    [Tooltip("Fuerza del impulso vertical aplicado al jugador")]
    public float fuerzaImpulso = 25f;

    [Header("Efectos Visuales")]
    [Tooltip("Sistema de partículas que se emite al usar el orbe")]
    public ParticleSystem particulasBrillo;
    
    // NOTA: Configura las partículas DIRECTAMENTE en el Inspector de Unity.
    // Valores recomendados para efecto Geometry Dash:
    // - Start Lifetime: 0.3, Start Speed: 20, Start Size: 0.2
    
    [Tooltip("Cantidad de partículas emitidas al activar el orbe")]
    public int cantidadParticulasExplosion = 300; 

    private bool jugadorEnRango = false;      // True si el jugador está dentro del trigger
    private MovimientoGD scriptJugador;       // Referencia al controlador del jugador

    void Update()
    {
        // Si el jugador está en rango y presiona salto, ejecutar impulso
        if (jugadorEnRango && (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)))
        {
            Saltar();
        }
    }

    /// <summary>
    /// Aplica impulso vertical al jugador y reproduce efectos visuales
    /// </summary>
    void Saltar()
    {
        Debug.Log($"[OrbeAmarillo] Orbe activado - Impulso: {fuerzaImpulso}");

        // Emitir partículas de explosión
        if (particulasBrillo != null)
        {
            particulasBrillo.Emit(cantidadParticulasExplosion);
        }

        // Aplicar física del salto
        if (scriptJugador != null)
        {
            Rigidbody2D rb = scriptJugador.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                // Resetear velocidad vertical para impulso consistente
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
                rb.AddForce(Vector2.up * fuerzaImpulso, ForceMode2D.Impulse);
                
                // Iniciar rotación del cubo (estilo Geometry Dash)
                scriptJugador.rotacionRestante = 180f;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Detectar cuando el jugador entra en el área del orbe
        if (other.CompareTag("Player"))
        {
            jugadorEnRango = true;
            scriptJugador = other.GetComponent<MovimientoGD>();
            Debug.Log("[OrbeAmarillo] Jugador en rango");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // Limpiar referencias cuando el jugador sale del área
        if (other.CompareTag("Player"))
        {
            jugadorEnRango = false;
            scriptJugador = null;
            Debug.Log("[OrbeAmarillo] Jugador fuera de rango");
        }
    }
}
