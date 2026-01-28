using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;


public class MovimientoGD : MonoBehaviour
{
    [Header("Movimiento")]
    public float velocidad = 10.4f;
    public float fuerzaSalto = 20f;
    public float gravedadEscala = 9f;


    [Header("Detección (Raycast y Layers)")]
    public LayerMask capaSuelo;
    public LayerMask capaMuerte;
    public float distanciaRayo = 0.6f;


    [Header("Efectos y Muerte")]
    public Animator anim;
    public ParticleSystem sistemaParticulas;
    public float tiempoEsperaMuerte = 1.0f;


    [Header("Modos Especiales")]
    public GameObject objetoEscoba;  
    public GameObject objetoBola;    
    public GameObject objetoUFO;      
    public bool modoCohete = false;
    public bool modoBola = false;    
    public bool modoUFO = false;      


    Rigidbody2D rb;
    BoxCollider2D col;
    SpriteRenderer rend;


    bool pedirSalto;
    [HideInInspector] public float rotacionRestante;
    bool haMuerto = false;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<BoxCollider2D>();
        rend = GetComponent<SpriteRenderer>();
       
        rb.gravityScale = gravedadEscala;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;


        DesactivarModos();
    }


    void Update()
    {
        if (haMuerto) return;


        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            if (modoBola && EstaTocandoSuperficieBola())
            {
                rb.gravityScale *= -1;
            }
           
            if (modoUFO)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
                rb.AddForce(Vector2.up * (fuerzaSalto * 0.6f), ForceMode2D.Impulse);
            }
        }


        pedirSalto = Input.GetKey(KeyCode.Space) || Input.GetMouseButton(0);
    }


    bool EstaTocandoSuperficieBola()
    {
        bool abajo = Physics2D.Raycast(transform.position, Vector2.down, distanciaRayo, capaSuelo);
        bool arriba = Physics2D.Raycast(transform.position, Vector2.up, distanciaRayo, capaSuelo);
        return abajo || arriba;
    }


    void FixedUpdate()
    {
        if (haMuerto) return;


        rb.linearVelocity = new Vector2(velocidad, rb.linearVelocity.y);


        if (!modoCohete && !modoBola && !modoUFO)
        {
            // Raycasts en las esquinas inferiores para evitar bugs en uniones de bloques
            Vector2 rayoIzquierdo = new Vector2(col.bounds.min.x, col.bounds.center.y);
            Vector2 rayoDerecho = new Vector2(col.bounds.max.x, col.bounds.center.y);
           
            bool tocandoSuelo = Physics2D.Raycast(rayoIzquierdo, Vector2.down, distanciaRayo, capaSuelo) ||
                                Physics2D.Raycast(rayoDerecho, Vector2.down, distanciaRayo, capaSuelo);


            if (pedirSalto && tocandoSuelo)
            {
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
            }
            float anguloZ = Mathf.Clamp(rb.linearVelocity.y * 3f, -25f, 25f);
            rb.MoveRotation(anguloZ);
        }
        else if (modoBola)
        {
            float direccionRot = rb.gravityScale > 0 ? -1f : 1f;
            rb.MoveRotation(rb.rotation + (direccionRot * 600f * Time.fixedDeltaTime));
        }
        else if (modoUFO)
        {
            // Forzamos a que el UFO siempre esté "derecho"
            rb.MoveRotation(0f);
            rotacionRestante = 0f; // Cancelamos cualquier rotación de salto previa
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        // MUERTE POR LAYER
        if (((1 << collision.gameObject.layer) & capaMuerte) != 0) Morir();


        // Portales
        if (collision.CompareTag("PortalCohete")) ActivarModoCohete();
        if (collision.CompareTag("PortalBola")) ActivarModoBola();
        if (collision.CompareTag("PortalUfo")) ActivarModoUfo();
        if (collision.CompareTag("PortalCubo")) ActivarModoCubo();
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 1. Muerte si chocamos contra algo en la capa de muerte
        if (((1 << collision.gameObject.layer) & capaMuerte) != 0)
        {
            Morir();
            return;
        }


        // 2. Muerte por choque frontal contra paredes (capaSuelo)
        foreach (ContactPoint2D punto in collision.contacts)
        {
            if (punto.normal.x < -0.9f)
            {
                Debug.Log("Colisión frontal detectada.");
                Morir();
                break;
            }
        }
    }


    // --- MÉTODOS DE MODOS ---
    void ActivarModoCohete() { DesactivarModos(); modoCohete = true; if (objetoEscoba != null) objetoEscoba.SetActive(true); rb.gravityScale = 2.5f; rb.linearVelocity = new Vector2(velocidad, 0); }
    void ActivarModoBola() { DesactivarModos(); modoBola = true; rend.enabled = false; if (objetoBola != null) objetoBola.SetActive(true); rb.gravityScale = 4.5f; rb.linearVelocity = new Vector2(velocidad, 0); }
    void ActivarModoUfo() { DesactivarModos(); modoUFO = true; if (objetoUFO != null) objetoUFO.SetActive(true); rb.gravityScale = 4.0f; rb.linearVelocity = new Vector2(velocidad, 0); }
    void ActivarModoCubo() { DesactivarModos(); rb.linearVelocity = new Vector2(velocidad, 0); }


    void DesactivarModos()
    {
        modoCohete = false; modoBola = false; modoUFO = false;
        rend.enabled = true;
        if (objetoEscoba != null) objetoEscoba.SetActive(false);
        if (objetoBola != null) objetoBola.SetActive(false);
        if (objetoUFO != null) objetoUFO.SetActive(false);
        rb.gravityScale = gravedadEscala;
        transform.rotation = Quaternion.identity;
    }


    void Morir()
    {
        if (haMuerto) return;
        haMuerto = true;
        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Static;


        if(objetoEscoba != null) objetoEscoba.SetActive(false);
        if(objetoBola != null) objetoBola.SetActive(false);
        if(objetoUFO != null) objetoUFO.SetActive(false);
        rend.enabled = false;


        if (sistemaParticulas != null) sistemaParticulas.Play();
        if (anim != null) anim.SetTrigger("Murio");
       
        StartCoroutine(EsperarYReiniciar());
    }


    IEnumerator EsperarYReiniciar()
    {
        yield return new WaitForSeconds(tiempoEsperaMuerte);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }


    void OnDrawGizmos()
    {
        if (!col) return;
        Gizmos.color = Color.cyan;
        // Dibujamos los rayos para que los ajustes en el Inspector
        Vector2 rIzquierda = new Vector2(col.bounds.min.x, col.bounds.center.y);
        Vector2 rDerecha = new Vector2(col.bounds.max.x, col.bounds.center.y);
        Gizmos.DrawLine(rIzquierda, rIzquierda + Vector2.down * distanciaRayo);
        Gizmos.DrawLine(rDerecha, rDerecha + Vector2.down * distanciaRayo);
    }
}
