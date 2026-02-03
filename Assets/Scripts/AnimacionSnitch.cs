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
    public float duracionEfecto = 2.0f; 
    
    // CAMBIO: Ahora es public para que el contador pueda leerla
    [HideInInspector] public bool recogida = false;
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
        if (other.CompareTag("Player") && !recogida)
        {
            StartCoroutine(SecuenciaRecoleccion());
        }
    }

    IEnumerator SecuenciaRecoleccion()
    {
        recogida = true;

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        float tiempo = 0;
        Vector3 escalaInicial = transform.localScale;
        Color colorInicial = sprite.color;

        while (tiempo < duracionEfecto)
        {
            tiempo += Time.deltaTime;
            float t = tiempo / duracionEfecto;

            transform.position += Vector3.up * velocidadSubida * Time.deltaTime;
            transform.localScale = Vector3.Lerp(escalaInicial, Vector3.zero, t);

            if (sprite != null)
            {
                sprite.color = new Color(colorInicial.r, colorInicial.g, colorInicial.b, 1 - t);
            }

            yield return null; 
        }

        // CAMBIO: Desactivar en lugar de destruir para que el contador lo encuentre
        gameObject.SetActive(false);
    }
}