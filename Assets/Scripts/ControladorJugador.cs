using UnityEngine;
using UnityEngine.SceneManagement;

public class MovimientoGD : MonoBehaviour
{
    [Header("Movimiento")]
    public float velocidad = 10.4f;
    public float fuerzaSalto = 20f;
    public float gravedadEscala = 9f;

    [Header("Efecto Spawn")]
    public float distanciaAtras = 6f; 

    [Header("Suelo")]
    public LayerMask capaSuelo;

    [Header("Efectos y Muerte")] // ← NUEVA SECCIÓN
    public Animator anim;
    public ParticleSystem sistemaParticulas; // ← AÑADIR ESTO
    public float tiempoEsperaMuerte = 1.0f;
    
    Rigidbody2D rb;
    BoxCollider2D col;
    SpriteRenderer rend; // ← Para ocultar el cubo

    bool pedirSalto;
    float rotacionRestante;
    bool haMuerto = false;
    private Vector3 puntoDeAparicion;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<BoxCollider2D>();
        rend = GetComponent<SpriteRenderer>(); // ← Obtener el renderizador
        
        rb.gravityScale = gravedadEscala;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;

        // IMPORTANTE: Asegurar que el cubo sea visible al reiniciar
        rend.enabled = true; 

        puntoDeAparicion = transform.position;
        transform.position = new Vector3(puntoDeAparicion.x - distanciaAtras, puntoDeAparicion.y, puntoDeAparicion.z);
    }

    void Update()
    {
        if (haMuerto) return; // Bloquea input si ha muerto

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            pedirSalto = true;
        }
    }

    void FixedUpdate()
    {
        if (haMuerto) return; // Bloquea movimiento si ha muerto

        rb.linearVelocity = new Vector2(velocidad, rb.linearVelocity.y);

        bool tocandoSuelo = Physics2D.OverlapBox(
            col.bounds.center + Vector3.down * (col.bounds.extents.y + 0.05f),
            new Vector2(col.bounds.size.x * 0.9f, 0.1f),
            0f,
            capaSuelo
        );

        if (pedirSalto && tocandoSuelo)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, fuerzaSalto);
            rotacionRestante = 180f;
        }

        pedirSalto = false;

        if (rotacionRestante > 0f)
        {
            float paso = 720f * Time.fixedDeltaTime;
            float rotar = Mathf.Min(paso, rotacionRestante);
            rb.MoveRotation(rb.rotation - rotar);
            rotacionRestante -= rotar;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Obstaculo")) Morir();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        foreach (ContactPoint2D punto in collision.contacts)
        {
            if (punto.normal.x < -0.9f) 
            {
                Morir();
                break;
            }
        }
    }

    void Morir()
    {
        if (haMuerto) return;
        haMuerto = true;

        // 1. Detener y ocultar
        rb.linearVelocity = Vector2.zero;
        rb.isKinematic = true;
        rend.enabled = false; // El cubo desaparece para que se vea la explosión

        // 2. Partículas
        if (sistemaParticulas != null)
        {
            sistemaParticulas.Play();
        }

        // 3. Animator
        if (anim != null)
        {
            anim.SetTrigger("Murio");
        }

        StartCoroutine(EsperarYReiniciar());
    }

    System.Collections.IEnumerator EsperarYReiniciar()
    {
        yield return new WaitForSeconds(tiempoEsperaMuerte);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void OnDrawGizmos()
    {
        if (!col) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(
            col.bounds.center + Vector3.down * (col.bounds.extents.y + 0.05f),
            new Vector2(col.bounds.size.x * 0.9f, 0.1f)
        );
    }
}