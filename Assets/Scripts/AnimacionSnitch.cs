using UnityEngine;
using System.Collections;

public class AnimacionSnitch : MonoBehaviour
{
    [Header("Flotado (Idle)")]
    public float amplitud = 0.4f;
    public float frecuencia = 1.5f;
    private Vector3 posInicial;

    [Header("Animación de Recolección")]
    public float velocidadSubida = 6f;
    public float duracionEfecto = 2.0f; // Aumentado para que te dé tiempo a verla
    
    private bool recogida = false;
    private SpriteRenderer sprite;

    void Start()
    {
        posInicial = transform.position;
        sprite = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (!recogida)
        {
            float nuevoY = posInicial.y + Mathf.Sin(Time.time * Mathf.PI * frecuencia) * amplitud;
            transform.position = new Vector3(transform.position.x, nuevoY, transform.position.z);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Verifica que sea el jugador y que no se haya recogido ya
        if (other.CompareTag("Player") && !recogida)
        {
            Debug.Log("Iniciando Animación de Recolección...");
            StartCoroutine(SecuenciaRecoleccion());
        }
    }

    IEnumerator SecuenciaRecoleccion()
    {
        recogida = true;

        // 1. IMPORTANTE: Desactivar el Collider para que no detecte más toques
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        float tiempo = 0;
        Vector3 escalaInicial = transform.localScale;
        Color colorInicial = sprite.color;

        // 2. Bucle de animación
        while (tiempo < duracionEfecto)
        {
            tiempo += Time.deltaTime;
            float t = tiempo / duracionEfecto;

            // Subir
            transform.position += Vector3.up * velocidadSubida * Time.deltaTime;

            // Encogerse gradualmente
            transform.localScale = Vector3.Lerp(escalaInicial, Vector3.zero, t);

            // Desvanecerse (Alpha)
            if (sprite != null)
            {
                sprite.color = new Color(colorInicial.r, colorInicial.g, colorInicial.b, 1 - t);
            }

            yield return null; // Espera al siguiente frame
        }

        // 3. Solo se destruye cuando el bucle termina
        Debug.Log("Animación terminada, destruyendo objeto.");
        Destroy(gameObject);
    }
}