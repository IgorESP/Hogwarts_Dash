using UnityEngine;
using System.Collections;

/// <summary>
/// Gestiona la animación de flotado y recolección de las Snitches (monedas)
/// </summary>
public class AnimacionSnitch : MonoBehaviour
{
    [Header("Flotado (Idle)")]
    [Tooltip("Amplitud del movimiento de flotado")]
    public float amplitud = 0.4f;
    
    [Tooltip("Frecuencia del movimiento de flotado")]
    public float frecuencia = 1.5f;
    
    private Vector3 posInicial;

    [Header("Animación de Recolección")]
    [Tooltip("Velocidad de ascenso al ser recogida")]
    public float velocidadSubida = 6f;
    
    [Tooltip("Duración total del efecto de recolección")]
    public float duracionEfecto = 2.0f; 
    
    [HideInInspector] public bool recogida = false; // Flag para tracking de estadísticas
    private SpriteRenderer sprite;

    void Start()
    {
        posInicial = transform.position;
        sprite = GetComponent<SpriteRenderer>();
    }

    /// <summary>
    /// Animación de flotado (movimiento sinusoidal vertical)
    /// </summary>
    void Update()
    {
        if (!recogida)
        {
            float nuevoY = posInicial.y + Mathf.Sin(Time.time * Mathf.PI * frecuencia) * amplitud;
            transform.position = new Vector3(transform.position.x, nuevoY, transform.position.z);
        }
    }

    /// <summary>
    /// Detecta cuando el jugador recoge la Snitch
    /// </summary>
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !recogida)
        {
            Debug.Log($"[AnimacionSnitch] Snitch '{gameObject.name}' recogida");
            StartCoroutine(SecuenciaRecoleccion());
        }
    }

    /// <summary>
    /// Animación de recolección: subir, encoger y desvanecer
    /// </summary>
    IEnumerator SecuenciaRecoleccion()
    {
        recogida = true;

        // Desactivar colisionador para evitar múltiples triggers
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        float tiempo = 0;
        Vector3 escalaInicial = transform.localScale;
        Color colorInicial = sprite.color;

        // Animación progresiva
        while (tiempo < duracionEfecto)
        {
            tiempo += Time.deltaTime;
            float t = tiempo / duracionEfecto;

            // Subir, encoger y desvanecer simultáneamente
            transform.position += Vector3.up * velocidadSubida * Time.deltaTime;
            transform.localScale = Vector3.Lerp(escalaInicial, Vector3.zero, t);

            if (sprite != null)
            {
                sprite.color = new Color(colorInicial.r, colorInicial.g, colorInicial.b, 1 - t);
            }

            yield return null; 
        }

        // Desactivar en lugar de destruir para permitir conteo de estadísticas
        gameObject.SetActive(false);
        Debug.Log($"[AnimacionSnitch] Snitch '{gameObject.name}' desactivada");
    }
}