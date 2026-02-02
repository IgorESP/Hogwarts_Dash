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

    [Header("Sonidos (NUEVO)")]
    public AudioSource audioSource;    // Arrastra aquí el AudioSource del Cubo
    public AudioClip sonidoMuerte;     // Arrastra aquí el wav de explosión
    public AudioClip sonidoMoneda;     // Arrastra aquí el wav de moneda

    [Header("Modos Especiales")]
    public GameObject objetoEscoba;  
    public GameObject objetoBola;    
    public GameObject objetoUFO;      
    public bool modoCohete = false;
    public bool modoBola = false;    
    public bool modoUFO = false;

    // ============================================
    // ⬇️ AGREGADO: Sistema de Progreso
    // ============================================
    private float distanciaInicial;
    // ============================================

    Rigidbody2D rb;
    BoxCollider2D col;
    CircleCollider2D circleCol; 
    SpriteRenderer rend;

    bool pedirSalto;
    float jumpBufferCounter; 
    public float jumpBufferTime = 0.15f; 

    [HideInInspector] public float rotacionRestante;
    bool haMuerto = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<BoxCollider2D>();
        rend = GetComponent<SpriteRenderer>();

        // Si no asignaste el AudioSource en el Inspector, lo busca automáticamente
        if (audioSource == null) audioSource = GetComponent<AudioSource>();

        // Crear/Buscar Collider Circular
        circleCol = GetComponent<CircleCollider2D>();
        if (circleCol == null)
        {
            circleCol = gameObject.AddComponent<CircleCollider2D>();
            float radioCalculado = col.size.y / 2f;
            circleCol.radius = radioCalculado;
        }
        circleCol.enabled = false;
        circleCol.isTrigger = true;

        // ============================================
        // ⬇️ AGREGADO: Inicializar sistema de progreso
        // ============================================
        distanciaInicial = transform.position.x;
        if (GameManager.Instance != null) 
        {
            GameManager.Instance.InicializarDistanciaMeta(distanciaInicial);
        }
        // ============================================
    }

    void Update()
    {
        if (haMuerto) return;

        if (Input.GetMouseButtonDown(0))
        {
            pedirSalto = true;
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
            if (jumpBufferCounter < 0) pedirSalto = false;
        }

        // ============================================
        // ⬇️ AGREGADO: Actualizar progreso de distancia
        // ============================================
        if (GameManager.Instance != null) 
        {
            float distanciaRecorrida = transform.position.x - distanciaInicial;
            GameManager.Instance.ActualizarDistanciaRecorrida(distanciaRecorrida);
        }
        // ============================================
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
            if (pedirSalto) rb.AddForce(Vector2.up * fuerzaSalto * 3.5f, ForceMode2D.Force);
            float anguloZ = Mathf.Clamp(rb.linearVelocity.y * 3f, -25f, 25f);
            rb.MoveRotation(anguloZ);
        }
        else if (modoBola)
        {
            if (jumpBufferCounter > 0 && EstaTocandoSuperficieBola())
            {
                rb.gravityScale *= -1;
                jumpBufferCounter = 0;
            }
            float direccionRot = rb.gravityScale > 0 ? -1f : 1f;
            transform.Rotate(Vector3.forward * direccionRot * 800f * Time.fixedDeltaTime);
        }
        else if (modoUFO)
        {
            rb.MoveRotation(0f);
            rotacionRestante = 0f;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 1. PORTALES
        if (collision.CompareTag("PortalCohete")) { ActivarModoCohete(); return; }
        if (collision.CompareTag("PortalBola")) { ActivarModoBola(); return; }
        if (collision.CompareTag("PortalUfo")) { ActivarModoUfo(); return; }
        if (collision.CompareTag("PortalCubo")) { ActivarModoCubo(); return; }

        // ============================================
        // ⬇️ DETECCIÓN DE MONEDA (con sonido)
        // ============================================
        if (collision.CompareTag("Moneda"))
        {
            // Reproducir sonido
            if (audioSource != null && sonidoMoneda != null) 
            {
                audioSource.PlayOneShot(sonidoMoneda);
            }

            // Registrar en GameManager
            if (GameManager.Instance != null) 
            {
                GameManager.Instance.IncrementarProgreso(1);
            }

            // Comprobar si tiene animación de Snitch
            AnimacionSnitch animSnitch = collision.GetComponent<AnimacionSnitch>();
            if (animSnitch != null)
            {
                Debug.Log("Snitch detectada");
            }
            else
            {
                collision.gameObject.SetActive(false);
            }
            
            return; 
        }

        // ============================================
        // ⬇️ DETECCIÓN DE COIN (alternativa sin sonido)
        // ============================================
        if (collision.CompareTag("Coin"))
        {
            if (GameManager.Instance != null) 
            {
                GameManager.Instance.IncrementarProgreso(1);
            }
            Destroy(collision.gameObject);
            return;
        }
        
        // ============================================
        // ⬇️ DETECCIÓN DE SNITCH DORADA (bonus)
        // ============================================
        if (collision.CompareTag("Snitch"))
        {
            if (GameManager.Instance != null) 
            {
                GameManager.Instance.RegistrarSnitchRecogida();
            }
            Destroy(collision.gameObject);
            return;
        }

        // ============================================
        // ⬇️ META DEL NIVEL (finalizar)
        // ============================================
        if (collision.CompareTag("Finish"))
        {
            if (GameManager.Instance != null) 
            {
                GameManager.Instance.FinalizarNivel();
            }
            return;
        }
        // ============================================

        // 2. Muerte si colisiona con capa de muerte
        if (((1 << collision.gameObject.layer) & capaMuerte) != 0)
        {
            Morir();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("PortalCohete")) { ActivarModoCohete(); return; }
        if (collision.gameObject.CompareTag("PortalBola")) { ActivarModoBola(); return; }
        if (collision.gameObject.CompareTag("PortalUfo")) { ActivarModoUfo(); return; }
        if (collision.gameObject.CompareTag("PortalCubo")) { ActivarModoCubo(); return; }

        // Muerte si chocamos contra algo en la capa de muerte
        if (((1 << collision.gameObject.layer) & capaMuerte) != 0)
        {
            Debug.Log($"Muerte por Colisión con: {collision.gameObject.name}");
            Morir();
            return;
        }

        // Muerte por choque frontal contra paredes (capaSuelo)
        foreach (ContactPoint2D punto in collision.contacts)
        {
            if (punto.normal.x < -0.9f)
            {
                Debug.Log($"Colisión frontal detectada con {collision.gameObject.name}");
                Morir();
                break;
            }
        }
    }

    // --- MÉTODOS DE MODOS ---
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
        
        if(col) col.enabled = false;
        if(circleCol) circleCol.enabled = true;
    }

    void ActivarModoUfo() 
    { 
        DesactivarModos(); 
        modoUFO = true; 
        if (objetoUFO != null) objetoUFO.SetActive(true); 
        rb.gravityScale = 4.0f; 
        rb.linearVelocity = new Vector2(velocidad, 0); 
    }

    void ActivarModoCubo() 
    { 
        DesactivarModos(); 
        rb.linearVelocity = new Vector2(velocidad, 0); 
    }

    void DesactivarModos()
    {
        modoCohete = false; 
        modoBola = false; 
        modoUFO = false;
        rend.enabled = true;
        
        if (objetoEscoba != null) objetoEscoba.SetActive(false);
        if (objetoBola != null) objetoBola.SetActive(false);
        if (objetoUFO != null) objetoUFO.SetActive(false);
        
        rb.gravityScale = gravedadEscala;
        transform.rotation = Quaternion.identity;
        
        if(col) col.enabled = true;
        if(circleCol) circleCol.enabled = false;
    }

    void Morir()
    {
        if (haMuerto) return;

        haMuerto = true;

        // ============================================
        // ⬇️ AGREGADO: Registrar intento
        // ============================================
        if (GameManager.Instance != null) GameManager.Instance.RegistrarIntento();
        // ============================================
        
        // --- NUEVO: SONIDO DE MUERTE ---
        if (audioSource && sonidoMuerte) audioSource.PlayOneShot(sonidoMuerte);

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
        Vector2 rIzquierda = new Vector2(col.bounds.min.x, col.bounds.center.y);
        Vector2 rDerecha = new Vector2(col.bounds.max.x, col.bounds.center.y);
        Gizmos.DrawLine(rIzquierda, rIzquierda + Vector2.down * distanciaRayo);
        Gizmos.DrawLine(rDerecha, rDerecha + Vector2.down * distanciaRayo);
        if (modoBola)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, (Vector2)transform.position + Vector2.up * distanciaRayo);
            Gizmos.DrawLine(transform.position, (Vector2)transform.position + Vector2.down * distanciaRayo);
        }
    }
}