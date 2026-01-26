using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Controla el movimiento principal, los cambios de modo (Cubo, Bola, UFO, Cohete) 
/// y el sistema de muerte/reinicio para un juego estilo Geometry Dash.
/// </summary>
public class MovimientoGD : MonoBehaviour
{
    [Header("Movimiento")]
    public float velocidad = 10.4f;
    public float fuerzaSalto = 20f;
    public float gravedadEscala = 9f;

    [Header("Efecto Spawn")]
    [Tooltip("Distancia desde el punto de inicio para el efecto de entrada.")]
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
    private Vector3 puntoDeAparicion;

    /// <summary>
    /// Inicializa los componentes, configura la física y prepara el punto de spawn.
    /// </summary>
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
        
        Debug.Log("<color=green>Jugador Inicializado:</color> Esperando inicio de nivel.");
    }

    /// <summary>
    /// Gestiona la entrada del jugador (teclado/ratón) en cada frame.
    /// </summary>
    void Update()
    {
        if (haMuerto) return;

        // Detectar pulsación única (GetKeyDown)
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            if (modoBola)
            {
                if (EstaTocandoSuperficieBola())
                {
                    rb.gravityScale *= -1;
                    Debug.Log("Bola: Cambio de gravedad a " + rb.gravityScale);
                }
            }
            
            if (modoUFO) 
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0); 
                rb.AddForce(Vector2.up * (fuerzaSalto * 0.6f), ForceMode2D.Impulse);
                Debug.Log("UFO: Impulso de salto aplicado.");
            }
        }

        // Detectar si se mantiene pulsado
        pedirSalto = Input.GetKey(KeyCode.Space) || Input.GetMouseButton(0);
    }

    /// <summary>
    /// Comprueba si el objeto Bola está tocando el techo o el suelo para permitir el cambio de gravedad.
    /// </summary>
    /// <returns>True si hay contacto arriba o abajo.</returns>
    bool EstaTocandoSuperficieBola()
    {
        float margen = 0.1f;
        bool tocandoAbajo = Physics2D.OverlapBox(col.bounds.center + Vector3.down * (col.bounds.extents.y + margen), new Vector2(col.bounds.size.x * 0.8f, 0.05f), 0f, capaSuelo);
        bool tocandoArriba = Physics2D.OverlapBox(col.bounds.center + Vector3.up * (col.bounds.extents.y + margen), new Vector2(col.bounds.size.x * 0.8f, 0.05f), 0f, capaSuelo);

        return tocandoAbajo || tocandoArriba;
    }

    /// <summary>
    /// Maneja el movimiento basado en física y las rotaciones visuales.
    /// </summary>
    void FixedUpdate()
    {
        if (haMuerto) return;

        // Velocidad constante hacia adelante
        rb.linearVelocity = new Vector2(velocidad, rb.linearVelocity.y);

        // LÓGICA MODO CUBO (Normal)
        if (!modoCohete && !modoBola && !modoUFO) 
        {
            bool tocandoSuelo = Physics2D.OverlapBox(col.bounds.center + Vector3.down * (col.bounds.extents.y + 0.05f), new Vector2(col.bounds.size.x * 0.9f, 0.1f), 0f, capaSuelo);

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
        // LÓGICA MODO COHETE (Escoba)
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
        // LÓGICA MODO BOLA
        else if (modoBola) 
        {
            float direccionRot = rb.gravityScale > 0 ? -1f : 1f;
            rb.MoveRotation(rb.rotation + (direccionRot * 600f * Time.fixedDeltaTime));
        }
    }

    /// <summary>
    /// Detecta disparadores para cambios de modo o colisiones con objetos especiales.
    /// </summary>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Obstaculo")) Morir();
        if (collision.CompareTag("PortalCohete")) ActivarModoCohete();
        if (collision.CompareTag("PortalBola")) ActivarModoBola(); 
        if (collision.CompareTag("PortalUfo")) ActivarModoUfo(); 
        if (collision.CompareTag("PortalCubo")) ActivarModoCubo();
        if (collision.CompareTag("Moneda")) 
        {
            Debug.Log("<color=yellow>¡Snitch dorada recogida!</color>");
        }
    }

    void ActivarModoCohete()
    {
        Debug.Log("Activando Modo Escoba (Cohete).");
        DesactivarModos(); 
        modoCohete = true;
        if (objetoEscoba != null) objetoEscoba.SetActive(true);
        rb.gravityScale = 2.5f; 
        rb.linearVelocity = new Vector2(velocidad, 0); 
    }

    void ActivarModoBola()
    {
        Debug.Log("Activando Modo Bola.");
        DesactivarModos(); 
        modoBola = true;
        rend.enabled = false; 
        if (objetoBola != null) objetoBola.SetActive(true); 
        rb.gravityScale = 4.5f; 
        rb.linearVelocity = new Vector2(velocidad, 0);
    }

    void ActivarModoUfo()
    {
        Debug.Log("Activando Modo UFO.");
        DesactivarModos(); 
        modoUFO = true; 
        if (objetoUFO != null) objetoUFO.SetActive(true);
        rb.gravityScale = 4.0f; 
        rb.linearVelocity = new Vector2(velocidad, 0); 
    }

    void ActivarModoCubo()
    {
        Debug.Log("Regresando a Modo Cubo.");
        DesactivarModos(); 
        rb.linearVelocity = new Vector2(velocidad, 0);
    }

    /// <summary>
    /// Limpia todos los estados especiales y resetea la visualización del jugador.
    /// </summary>
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
    }

    /// <summary>
    /// Ejecuta la secuencia de muerte, detiene la física y reinicia el nivel.
    /// </summary>
    void Morir()
    {
        if (haMuerto) return;
        haMuerto = true;
        Debug.Log("<color=red>Jugador derrotado.</color> Reiniciando...");

        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Static; // Detiene toda la física

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

    /// <summary>
    /// Detecta colisiones frontales con paredes para activar la muerte.
    /// </summary>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        foreach (ContactPoint2D punto in collision.contacts)
        {
            // Si el choque es de frente (normal en X negativa)
            if (punto.normal.x < -0.9f) 
            { 
                Debug.Log("Colisión frontal detectada.");
                Morir(); 
                break; 
            }
        }
    }

    /// <summary>
    /// Dibuja el área de detección del suelo en el editor de Unity.
    /// </summary>
    void OnDrawGizmos()
    {
        if (!col) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(col.bounds.center + Vector3.down * (col.bounds.extents.y + 0.05f), new Vector2(col.bounds.size.x * 0.9f, 0.1f));
    }
}