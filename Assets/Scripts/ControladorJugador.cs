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

    [Header("Efectos y Muerte")]
    public Animator anim;
    public ParticleSystem sistemaParticulas;
    public float tiempoEsperaMuerte = 1.0f;

    [Header("Modos Especiales")]
    public GameObject objetoEscoba;   
    public GameObject objetoBola;     
    public GameObject objetoUFO;      // <-- AÑADIDO (Faltaba declarar)
    public bool modoCohete = false;
    public bool modoBola = false;     
    public bool modoUFO = false;      // <-- AÑADIDO (Faltaba declarar)

    Rigidbody2D rb;
    BoxCollider2D col;
    SpriteRenderer rend;

    bool pedirSalto;
    float rotacionRestante;
    bool haMuerto = false;
    private Vector3 puntoDeAparicion;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<BoxCollider2D>();
        rend = GetComponent<SpriteRenderer>(); 
        
        rb.gravityScale = gravedadEscala;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;

        DesactivarModos(); 

        puntoDeAparicion = transform.position;
        transform.position = new Vector3(puntoDeAparicion.x - distanciaAtras, puntoDeAparicion.y, puntoDeAparicion.z);
    }

    void Update()
    {
        if (haMuerto) return;

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            if (modoBola) rb.gravityScale *= -1; 
            
            // <-- AÑADIDO: Lógica de salto UFO tipo Flappy Bird
            if (modoUFO) 
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0); 
                rb.AddForce(Vector2.up * (fuerzaSalto * 0.6f), ForceMode2D.Impulse);
            }
        }

        pedirSalto = Input.GetKey(KeyCode.Space) || Input.GetMouseButton(0);
    }

    void FixedUpdate()
    {
        if (haMuerto) return;

        rb.linearVelocity = new Vector2(velocidad, rb.linearVelocity.y);

        if (!modoCohete && !modoBola && !modoUFO) 
        {
            bool tocandoSuelo = Physics2D.OverlapBox(
                col.bounds.center + Vector3.down * (col.bounds.extents.y + 0.05f),
                new Vector2(col.bounds.size.x * 0.9f, 0.1f),
                0f,
                capaSuelo
            );

            if (pedirSalto && tocandoSuelo)
            {

                //Debug saltando si esta
                // sout tocandosuelo
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, fuerzaSalto);
                rotacionRestante = 180f;
            }

            if (rotacionRestante > 0f)
            {
                float paso = 720f * Time.fixedDeltaTime;
                float rotar = Mathf.Min(paso, rotacionRestante);
                rb.MoveRotation(rb.rotation - rotar);
                rotacionRestante -= rotar;
            }
        }
        else if (modoCohete) 
        {
            if (pedirSalto) 
            {
                rb.AddForce(Vector2.up * fuerzaSalto * 3.5f, ForceMode2D.Force);
                if (rb.linearVelocity.y > velocidad) rb.linearVelocity = new Vector2(rb.linearVelocity.x, velocidad);
            }
            float anguloZ = Mathf.Clamp(rb.linearVelocity.y * 3f, -25f, 25f);
            rb.MoveRotation(anguloZ);
        }
        else if (modoBola) 
        {
            float direccionRot = rb.gravityScale > 0 ? -1f : 1f;
            rb.MoveRotation(rb.rotation + (direccionRot * 600f * Time.fixedDeltaTime));
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Obstaculo")) Morir();
        if (collision.CompareTag("PortalCohete")) ActivarModoCohete();
        if (collision.CompareTag("PortalBola")) ActivarModoBola(); 
        if (collision.CompareTag("PortalUfo")) ActivarModoUfo(); 
        if (collision.CompareTag("PortalCubo")) ActivarModoCubo();
    }

    void ActivarModoCohete()
    {
        DesactivarModos(); 
        modoCohete = true;
        if (objetoEscoba != null) objetoEscoba.SetActive(true);
        rb.gravityScale = 2.5f; 
        rb.linearVelocity = new Vector2(velocidad, 0); 
    }

    void ActivarModoBola()
    {
        DesactivarModos(); 
        modoBola = true;
        rend.enabled = false; 
        if (objetoBola != null) objetoBola.SetActive(true); 
        rb.gravityScale = 4.5f; 
        rb.linearVelocity = new Vector2(velocidad, 0);
    }

void ActivarModoUfo()
{
    DesactivarModos(); 
    modoUFO = true; 

    // ❌ NO apagamos el renderer del cubo
    // rend.enabled = false;  ← ELIMINADA

    if (objetoUFO != null)
    {
        objetoUFO.SetActive(true);
    }

    rb.gravityScale = 4.0f; 
    rb.linearVelocity = new Vector2(velocidad, 0); 
}

void ActivarModoCubo()
    {
        DesactivarModos(); // Esto ya pone la gravedad normal, enciende el cubo y apaga todo lo demás
        rb.linearVelocity = new Vector2(velocidad, 0); // Resetea la velocidad para que no salga disparado
    }

    void DesactivarModos() 
    {
        modoCohete = false;
        modoBola = false;
        modoUFO = false; // <-- AÑADIDO
        rend.enabled = true; 
        if (objetoEscoba != null) objetoEscoba.SetActive(false);
        if (objetoBola != null) objetoBola.SetActive(false);
        if (objetoUFO != null) objetoUFO.SetActive(false); // <-- AÑADIDO
        rb.gravityScale = gravedadEscala; 
        transform.rotation = Quaternion.identity;
    }

    void Morir()
    {
        if (haMuerto) return;
        haMuerto = true;
        rb.linearVelocity = Vector2.zero;
        rb.isKinematic = true;

        if(objetoEscoba != null) objetoEscoba.SetActive(false);
        if(objetoBola != null) objetoBola.SetActive(false);
        if(objetoUFO != null) objetoUFO.SetActive(false); // <-- AÑADIDO
        rend.enabled = false; 

        if (sistemaParticulas != null) sistemaParticulas.Play();
        if (anim != null) anim.SetTrigger("Murio");
        StartCoroutine(EsperarYReiniciar());
    }

    System.Collections.IEnumerator EsperarYReiniciar()
    {
        yield return new WaitForSeconds(tiempoEsperaMuerte);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        foreach (ContactPoint2D punto in collision.contacts)
        {
            if (punto.normal.x < -0.9f) { Morir(); break; }
        }
    }

    void OnDrawGizmos()
    {
        if (!col) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(col.bounds.center + Vector3.down * (col.bounds.extents.y + 0.05f), new Vector2(col.bounds.size.x * 0.9f, 0.1f));
    }
} 