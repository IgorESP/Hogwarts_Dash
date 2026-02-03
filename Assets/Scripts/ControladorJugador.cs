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
    public AudioSource audioSource;
    public AudioClip sonidoMuerte;
    public AudioClip sonidoMoneda;

    [Header("Modos Especiales")]
    public GameObject objetoEscoba;  
    public GameObject objetoBola;    
    public GameObject objetoUFO;      
    public bool modoCohete = false;
    public bool modoBola = false;    
    public bool modoUFO = false;      

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

        if (audioSource == null) audioSource = GetComponent<AudioSource>();

        circleCol = GetComponent<CircleCollider2D>();
        if (circleCol == null)
        {
            circleCol = gameObject.AddComponent<CircleCollider2D>();
            if(col != null) 
            {
                circleCol.radius = col.size.y / 2f;
                circleCol.offset = col.offset; 
            }
            circleCol.enabled = false; 
        }
       
        rb.gravityScale = gravedadEscala;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;

        DesactivarModos();
    }

    void Update()
    {
        if (haMuerto) return;

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            if (modoBola) jumpBufferCounter = jumpBufferTime;
           
            if (modoUFO)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
                rb.AddForce(Vector2.up * (fuerzaSalto * 0.6f), ForceMode2D.Impulse);
            }
        }

        pedirSalto = Input.GetKey(KeyCode.Space) || Input.GetMouseButton(0);
        if (jumpBufferCounter > 0) jumpBufferCounter -= Time.deltaTime;
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
        if (collision.CompareTag("Moneda"))
        {
           if (audioSource != null && sonidoMoneda != null) 
    {
        audioSource.PlayOneShot(sonidoMoneda);
    }

    AnimacionSnitch animSnitch = collision.GetComponent<AnimacionSnitch>();
    if (animSnitch != null)
    {
        // CAMBIO: Aseguramos que se ejecute la lógica de la Snitch
        // La propia Snitch se desactivará al final de su animación.
        Debug.Log("Snitch detectada");
    }
    else
    {
        collision.gameObject.SetActive(false);
    }
    
    return; 
}

        if (collision.CompareTag("PortalCohete")) { ActivarModoCohete(); return; }
        if (collision.CompareTag("PortalBola")) { ActivarModoBola(); return; }
        if (collision.CompareTag("PortalUfo")) { ActivarModoUfo(); return; }
        if (collision.CompareTag("PortalCubo")) { ActivarModoCubo(); return; }

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

        if (((1 << collision.gameObject.layer) & capaMuerte) != 0)
        {
            Morir();
            return;
        }

        foreach (ContactPoint2D punto in collision.contacts)
        {
            if (punto.normal.x < -0.9f)
            {
                Morir();
                break;
            }
        }
    }

    void ActivarModoCohete() { DesactivarModos(); modoCohete = true; if (objetoEscoba != null) objetoEscoba.SetActive(true); rb.gravityScale = 2.5f; rb.linearVelocity = new Vector2(velocidad, 0); }
    void ActivarModoBola() { DesactivarModos(); modoBola = true; rend.enabled = false; if (objetoBola != null) objetoBola.SetActive(true); rb.gravityScale = 4.5f; rb.linearVelocity = new Vector2(velocidad, 0); if(col) col.enabled = false; if(circleCol) circleCol.enabled = true; }
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
        if(col) col.enabled = true;
        if(circleCol) circleCol.enabled = false;
    }

    void Morir()
    {
        if (haMuerto) return;

        haMuerto = true;
        
        // ✅ REGISTRAR INTENTO (muerte)
        string escenaActual = SceneManager.GetActiveScene().name;
        int intentosActuales = PlayerPrefs.GetInt("Intentos_" + escenaActual, 0);
        intentosActuales++;
        PlayerPrefs.SetInt("Intentos_" + escenaActual, intentosActuales);
        PlayerPrefs.SetInt("IntentosActuales", intentosActuales);
        PlayerPrefs.Save();
        
        Debug.Log($"💀 MUERTE #{intentosActuales}");
        
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
