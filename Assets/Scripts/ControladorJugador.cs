using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// Controlador principal del jugador para juego estilo Geometry Dash
/// Gestiona múltiples modos de movimiento: Cubo, Cohete (Escoba), Bola y UFO
/// Incluye sistema de muerte y reinicio, detección de colisiones y efectos
/// </summary>
public class MovimientoGD : MonoBehaviour
{
    // ========================================
    // CONFIGURACIÓN DE MOVIMIENTO
    // ========================================
    
    [Header("Movimiento")]
    [Tooltip("Velocidad horizontal constante del jugador")]
    public float velocidad = 10.4f;
    
    [Tooltip("Fuerza vertical aplicada al saltar (modo Cubo) o impulso (modos especiales)")]
    public float fuerzaSalto = 20f;
    
    [Tooltip("Escala de gravedad por defecto (modo Cubo)")]
    public float gravedadEscala = 9f;

    // ========================================
    // SISTEMA DE DETECCIÓN (RAYCASTS)
    // ========================================
    
    [Header("Detección (Raycast y Layers)")]
    [Tooltip("Layer que representa el suelo sobre el que se puede saltar")]
    public LayerMask capaSuelo;
    
    [Tooltip("Layer que causa muerte al contacto (pinchos, obstáculos, etc.)")]
    public LayerMask capaMuerte;
    
    [Tooltip("Distancia del raycast para detectar suelo/techo")]
    public float distanciaRayo = 0.6f;

    // ========================================
    // EFECTOS VISUALES Y AUDIO
    // ========================================
    
    [Header("Efectos y Muerte")]
    [Tooltip("Animator para animaciones del jugador")]
    public Animator anim;
    
    [Tooltip("Sistema de partículas que se reproduce al morir")]
    public ParticleSystem sistemaParticulas;
    
    [Tooltip("Tiempo de espera antes de reiniciar la escena tras morir")]
    public float tiempoEsperaMuerte = 1.0f;

    [Header("Sonidos (NUEVO)")]
    [Tooltip("Componente AudioSource para reproducir sonidos")]
    public AudioSource audioSource;
    
    [Tooltip("Sonido que se reproduce al morir")]
    public AudioClip sonidoMuerte;
    
    [Tooltip("Sonido que se reproduce al recoger una moneda")]
    public AudioClip sonidoMoneda;

    // ========================================
    // MODOS DE JUEGO ESPECIALES
    // ========================================
    
    [Header("Modos Especiales")]
    [Tooltip("GameObject visual del modo Cohete (Escoba)")]
    public GameObject objetoEscoba;  
    
    [Tooltip("GameObject visual del modo Bola")]
    public GameObject objetoBola;    
    
    [Tooltip("GameObject visual del modo UFO")]
    public GameObject objetoUFO;      
    
    [Tooltip("Indica si el modo Cohete (Escoba) está activo")]
    public bool modoCohete = false;
    
    [Tooltip("Indica si el modo Bola está activo")]
    public bool modoBola = false;    
    
    [Tooltip("Indica si el modo UFO está activo")]
    public bool modoUFO = false;      

    // ========================================
    // COMPONENTES PRIVADOS (Referencias)
    // ========================================
    
    Rigidbody2D rb;            // Física del jugador
    BoxCollider2D col;         // Colisionador para modo Cubo/Cohete/UFO
    CircleCollider2D circleCol; // Colisionador para modo Bola
    SpriteRenderer rend;       // Renderizador del sprite del jugador

    // ========================================
    // VARIABLES DE CONTROL DE SALTO
    // ========================================
    
    bool pedirSalto;           // True si el jugador mantiene presionada la tecla de salto
    float jumpBufferCounter;   // Temporizador para "jump buffer" (permite saltos más permisivos)
    
    [Tooltip("Tiempo que se recuerda el input de salto para modo Bola")]
    public float jumpBufferTime = 0.15f; 

    // ========================================
    // VARIABLES DE ESTADO
    // ========================================
    
    [HideInInspector] public float rotacionRestante; // Control de rotación suave en modo Cubo
    bool haMuerto = false;     // Flag para evitar procesar lógica tras morir

    // ========================================
    // INICIALIZACIÓN
    // ========================================
    
    /// <summary>
    /// Se ejecuta al inicio del juego
    /// Inicializa componentes, configura física y desactiva modos especiales
    /// </summary>
    void Start()
    {
        Debug.Log("[MovimientoGD] Inicializando jugador...");
        
        // Obtener referencias a componentes
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<BoxCollider2D>();
        rend = GetComponent<SpriteRenderer>();

        // Configurar AudioSource si no está asignado en el Inspector
        if (audioSource == null) audioSource = GetComponent<AudioSource>();

        // Configurar CircleCollider para el modo Bola
        circleCol = GetComponent<CircleCollider2D>();
        if (circleCol == null)
        {
            Debug.Log("[MovimientoGD] CircleCollider2D no encontrado, creando uno nuevo para modo Bola");
            circleCol = gameObject.AddComponent<CircleCollider2D>();
            if(col != null) 
            {
                // Ajustar el radio del círculo basado en el BoxCollider
                circleCol.radius = col.size.y / 2f;
                circleCol.offset = col.offset; 
            }
            circleCol.enabled = false; // Desactivado por defecto (solo para modo Bola)
        }
       
        // Configurar física por defecto
        rb.gravityScale = gravedadEscala;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate; // Suaviza movimiento

        // Iniciar en modo Cubo (modo por defecto)
        DesactivarModos();
        
        Debug.Log($"[MovimientoGD] Inicialización completa - Velocidad: {velocidad}, Gravedad: {gravedadEscala}");
    }

    // ========================================
    // UPDATE - PROCESAMIENTO DE INPUT
    // ========================================
    
    /// <summary>
    /// Se ejecuta cada frame
    /// Procesa input del jugador (salto/vuelo) según el modo activo
    /// </summary>
    void Update()
    {
        // Si el jugador ha muerto, no procesar más inputs
        if (haMuerto) return;

        // DETECCIÓN DE PULSACIÓN INICIAL (GetKeyDown/GetMouseButtonDown)
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            // MODO BOLA: Activar jump buffer para invertir gravedad
            if (modoBola) 
            {
                jumpBufferCounter = jumpBufferTime;
                Debug.Log("[MovimientoGD] Jump buffer activado para modo Bola");
            }
           
            // MODO UFO: Aplicar impulso vertical inmediato
            if (modoUFO)
            {
                // Resetear velocidad vertical para impulso consistente
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
                rb.AddForce(Vector2.up * (fuerzaSalto * 0.6f), ForceMode2D.Impulse);
                Debug.Log("[MovimientoGD] Impulso UFO aplicado");
            }
        }

        // DETECCIÓN DE TECLA MANTENIDA (GetKey/GetMouseButton)
        // Usado para modo Cubo (salto sostenido) y modo Cohete (vuelo continuo)
        pedirSalto = Input.GetKey(KeyCode.Space) || Input.GetMouseButton(0);
        
        // Decrementar el contador del jump buffer
        if (jumpBufferCounter > 0) jumpBufferCounter -= Time.deltaTime;
    }

    // ========================================
    // DETECCIÓN DE SUPERFICIE (MODO BOLA)
    // ========================================
    
    /// <summary>
    /// Detecta si la bola está tocando suelo o techo
    /// En modo Bola, el jugador puede invertir gravedad en ambas superficies
    /// </summary>
    /// <returns>True si hay superficie arriba o abajo</returns>
    bool EstaTocandoSuperficieBola()
    {
        // Raycast hacia abajo
        bool abajo = Physics2D.Raycast(transform.position, Vector2.down, distanciaRayo, capaSuelo);
        
        // Raycast hacia arriba
        bool arriba = Physics2D.Raycast(transform.position, Vector2.up, distanciaRayo, capaSuelo);
        
        return abajo || arriba;
    }

    // ========================================
    // FIXED UPDATE - FÍSICA Y MOVIMIENTO
    // ========================================
    
    /// <summary>
    /// Se ejecuta en intervalos fijos de tiempo (física)
    /// Maneja el movimiento, saltos y rotación según el modo activo
    /// </summary>
    void FixedUpdate()
    {
        // Si el jugador ha muerto, no procesar física
        if (haMuerto) return;

        // MOVIMIENTO HORIZONTAL CONSTANTE (todos los modos)
        // La velocidad X siempre es constante, solo varía la Y
        rb.linearVelocity = new Vector2(velocidad, rb.linearVelocity.y);

        // ========================================
        // MODO CUBO (Modo por defecto)
        // ========================================
        if (!modoCohete && !modoBola && !modoUFO)
        {
            // DETECCIÓN DE SUELO con 2 raycasts (esquinas del BoxCollider)
            Vector2 rayoIzquierdo = new Vector2(col.bounds.min.x, col.bounds.center.y);
            Vector2 rayoDerecho = new Vector2(col.bounds.max.x, col.bounds.center.y);
            
            bool tocandoSuelo = Physics2D.Raycast(rayoIzquierdo, Vector2.down, distanciaRayo, capaSuelo) ||
                                Physics2D.Raycast(rayoDerecho, Vector2.down, distanciaRayo, capaSuelo);

            // SALTO: Solo si está en el suelo y se pide salto
            if (pedirSalto && tocandoSuelo)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, fuerzaSalto);
                rotacionRestante = 180f; // Iniciar rotación de 180 grados (media vuelta)
                Debug.Log("[MovimientoGD] Salto ejecutado en modo Cubo");
            }

            // ROTACIÓN SUAVE (estilo Geometry Dash)
            // El cubo rota 180° por cada salto de forma gradual
            if (rotacionRestante > 0f)
            {
                float paso = 720f * Time.fixedDeltaTime; // Velocidad de rotación (grados/seg)
                float rotar = Mathf.Min(paso, rotacionRestante);
                rb.MoveRotation(rb.rotation - rotar);
                rotacionRestante -= rotar;
            }
        }
        // ========================================
        // MODO COHETE (Escoba)
        // ========================================
        else if (modoCohete)
        {
            // Aplicar fuerza hacia arriba mientras se mantiene presionado el botón
            if (pedirSalto) 
            {
                rb.AddForce(Vector2.up * fuerzaSalto * 3.5f, ForceMode2D.Force);
            }
            
            // ROTACIÓN DINÁMICA: La escoba se inclina según la velocidad vertical
            // velocityY positiva = apunta hacia arriba, negativa = apunta hacia abajo
            float anguloZ = Mathf.Clamp(rb.linearVelocity.y * 3f, -25f, 25f);
            rb.MoveRotation(anguloZ);
        }
        // ========================================
        // MODO BOLA
        // ========================================
        else if (modoBola)
        {
            // INVERSIÓN DE GRAVEDAD al tocar superficie
            if (jumpBufferCounter > 0 && EstaTocandoSuperficieBola())
            {
                // Invertir dirección de la gravedad (positiva <-> negativa)
                rb.gravityScale *= -1;
                jumpBufferCounter = 0; // Consumir el jump buffer
                Debug.Log($"[MovimientoGD] Gravedad invertida en modo Bola - Nueva gravedad: {rb.gravityScale}");
            }
            
            // ROTACIÓN CONTINUA: La bola rueda en dirección de la gravedad
            // Si gravedad es positiva (hacia abajo) -> rota horario (-1)
            // Si gravedad es negativa (hacia arriba) -> rota antihorario (+1)
            float direccionRot = rb.gravityScale > 0 ? -1f : 1f;
            transform.Rotate(Vector3.forward * direccionRot * 800f * Time.fixedDeltaTime);
        }
        // ========================================
        // MODO UFO
        // ========================================
        else if (modoUFO)
        {
            // ROTACIÓN FIJA: El UFO siempre mira hacia adelante (sin rotación)
            rb.MoveRotation(0f);
            rotacionRestante = 0f;
        }
    }

    // ========================================
    // DETECCIÓN DE COLISIONES (TRIGGERS)
    // ========================================
    
    /// <summary>
    /// Se ejecuta cuando un objeto con Trigger entra en contacto con el jugador
    /// Maneja: Monedas, Portales de cambio de modo, Obstáculos letales
    /// </summary>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // MONEDAS / COLECCIONABLES
        if (collision.CompareTag("Moneda"))
        {
            if (audioSource != null && sonidoMoneda != null) 
            {
                audioSource.PlayOneShot(sonidoMoneda);
                Debug.Log("[MovimientoGD] Moneda recogida");
            }

            // NOTA: AnimacionSnitch.cs gestiona la lógica de recolección
            // Este script solo reproduce el sonido
            return; 
        }
        
        // PORTALES DE CAMBIO DE MODO
        if (collision.CompareTag("PortalCohete")) { ActivarModoCohete(); return; }
        if (collision.CompareTag("PortalBola")) { ActivarModoBola(); return; }
        if (collision.CompareTag("PortalUfo")) { ActivarModoUfo(); return; }
        if (collision.CompareTag("PortalCubo")) { ActivarModoCubo(); return; }

        // OBSTÁCULOS LETALES (por Layer)
        // Verifica si el objeto pertenece a la capa de muerte usando máscaras de bits
        if (((1 << collision.gameObject.layer) & capaMuerte) != 0)
        {
             Debug.Log($"[MovimientoGD] Colisión con objeto letal (Trigger): {collision.gameObject.name}");
             Morir();
        }
    }

    // ========================================
    // DETECCIÓN DE COLISIONES (FÍSICAS)
    // ========================================
    
    /// <summary>
    /// Se ejecuta cuando un objeto sólido (sin Trigger) colisiona con el jugador
    /// Maneja: Portales sólidos, Obstáculos letales, Colisiones frontales
    /// </summary>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // PORTALES DE CAMBIO DE MODO (versión física)
        if (collision.gameObject.CompareTag("PortalCohete")) { ActivarModoCohete(); return; }
        if (collision.gameObject.CompareTag("PortalBola")) { ActivarModoBola(); return; }
        if (collision.gameObject.CompareTag("PortalUfo")) { ActivarModoUfo(); return; }
        if (collision.gameObject.CompareTag("PortalCubo")) { ActivarModoCubo(); return; }

        // OBSTÁCULOS LETALES (por Layer)
        if (((1 << collision.gameObject.layer) & capaMuerte) != 0)
        {
            Debug.Log($"[MovimientoGD] Colisión con objeto letal (Física): {collision.gameObject.name}");
            Morir();
            return;
        }

        // MUERTE POR COLISIÓN FRONTAL
        // Si el jugador choca de frente con un objeto (normal apuntando hacia la izquierda)
        foreach (ContactPoint2D punto in collision.contacts)
        {
            // normal.x < -0.9f significa que el jugador chocó frontalmente (de derecha a izquierda)
            if (punto.normal.x < -0.9f)
            {
                Debug.Log($"[MovimientoGD] Muerte por colisión frontal con: {collision.gameObject.name}");
                Morir();
                break;
            }
        }
    }

    // ========================================
    // ACTIVACIÓN DE MODOS ESPECIALES
    // ========================================
    
    /// <summary>
    /// Activa el modo Cohete (Escoba)
    /// Permite volar manteniendo presionado el botón de salto
    /// </summary>
    void ActivarModoCohete() 
    { 
        DesactivarModos(); 
        modoCohete = true; 
        if (objetoEscoba != null) objetoEscoba.SetActive(true); 
        rb.gravityScale = 2.5f; 
        rb.linearVelocity = new Vector2(velocidad, 0); 
        Debug.Log("[MovimientoGD] ✈️ Modo Cohete activado");
    }
    
    /// <summary>
    /// Activa el modo Bola
    /// Permite invertir gravedad al tocar suelo/techo
    /// Cambia de BoxCollider a CircleCollider
    /// </summary>
    void ActivarModoBola() 
    { 
        DesactivarModos(); 
        modoBola = true; 
        rend.enabled = false; // Ocultar sprite del cubo
        if (objetoBola != null) objetoBola.SetActive(true); 
        rb.gravityScale = 4.5f; 
        rb.linearVelocity = new Vector2(velocidad, 0); 
        
        // Cambiar colisionador de Box a Circle
        if(col) col.enabled = false; 
        if(circleCol) circleCol.enabled = true; 
        
        Debug.Log("[MovimientoGD] ⚽ Modo Bola activado");
    }
    
    /// <summary>
    /// Activa el modo UFO
    /// Permite impulsos verticales controlados
    /// </summary>
    void ActivarModoUfo() 
    { 
        DesactivarModos(); 
        modoUFO = true; 
        if (objetoUFO != null) objetoUFO.SetActive(true); 
        rb.gravityScale = 4.0f; 
        rb.linearVelocity = new Vector2(velocidad, 0); 
        Debug.Log("[MovimientoGD] 🛸 Modo UFO activado");
    }
    
    /// <summary>
    /// Activa el modo Cubo (modo por defecto)
    /// Salto clásico estilo plataformas
    /// </summary>
    void ActivarModoCubo() 
    { 
        DesactivarModos(); 
        rb.linearVelocity = new Vector2(velocidad, 0); 
        Debug.Log("[MovimientoGD] 🟦 Modo Cubo activado");
    }

    // ========================================
    // DESACTIVACIÓN DE TODOS LOS MODOS
    // ========================================
    
    /// <summary>
    /// Desactiva todos los modos especiales y resetea al estado por defecto (Cubo)
    /// Restablece física, colisionadores, rotación y objetos visuales
    /// </summary>
    void DesactivarModos()
    {
        // Desactivar flags de todos los modos
        modoCohete = false; 
        modoBola = false; 
        modoUFO = false;
        
        // Mostrar sprite del cubo por defecto
        rend.enabled = true;
        
        // Ocultar todos los objetos visuales de modos especiales
        if (objetoEscoba != null) objetoEscoba.SetActive(false);
        if (objetoBola != null) objetoBola.SetActive(false);
        if (objetoUFO != null) objetoUFO.SetActive(false);
        
        // Restablecer física por defecto
        rb.gravityScale = gravedadEscala;
        
        // Resetear rotación a 0 grados
        transform.rotation = Quaternion.identity;
        
        // Restablecer colisionadores (Box activo, Circle inactivo)
        if(col) col.enabled = true;
        if(circleCol) circleCol.enabled = false;
    }

    // ========================================
    // SISTEMA DE MUERTE
    // ========================================
    
    /// <summary>
    /// Ejecuta la secuencia de muerte del jugador
    /// Registra estadísticas, reproduce efectos y reinicia el nivel
    /// </summary>
    void Morir()
    {
        // Prevenir múltiples llamadas a Morir() en el mismo frame
        if (haMuerto) return;

        haMuerto = true;
        Debug.Log("[MovimientoGD] ☠️ Jugador ha muerto");
        
        // ========================================
        // REGISTRO DE ESTADÍSTICAS (PlayerPrefs)
        // ========================================
        
        string escenaActual = SceneManager.GetActiveScene().name;
        int intentosActuales = PlayerPrefs.GetInt("Intentos_" + escenaActual, 0);
        intentosActuales++;
        
        // Guardar intentos por escena
        PlayerPrefs.SetInt("Intentos_" + escenaActual, intentosActuales);
        PlayerPrefs.SetInt("IntentosActuales", intentosActuales);
        PlayerPrefs.Save();
        
        Debug.Log($"💀 MUERTE #{intentosActuales} en nivel: {escenaActual}");
        
        // ========================================
        // EFECTOS DE MUERTE
        // ========================================
        
        // Reproducir sonido de muerte
        if (audioSource && sonidoMuerte) 
        {
            audioSource.PlayOneShot(sonidoMuerte);
            Debug.Log("[MovimientoGD] Sonido de muerte reproducido");
        }

        // Detener movimiento completamente
        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Static; // Hacer el Rigidbody estático (sin física)

        // Ocultar todos los objetos visuales
        if(objetoEscoba != null) objetoEscoba.SetActive(false);
        if(objetoBola != null) objetoBola.SetActive(false);
        if(objetoUFO != null) objetoUFO.SetActive(false);
        rend.enabled = false; // Ocultar sprite del jugador

        // Reproducir efectos visuales
        if (sistemaParticulas != null) 
        {
            sistemaParticulas.Play();
            Debug.Log("[MovimientoGD] Partículas de muerte activadas");
        }
        
        if (anim != null) 
        {
            anim.SetTrigger("Murio");
            Debug.Log("[MovimientoGD] Animación de muerte iniciada");
        }
       
        // Iniciar corrutina para reiniciar el nivel
        StartCoroutine(EsperarYReiniciar());
    }

    /// <summary>
    /// Corrutina que espera un tiempo y luego reinicia la escena actual
    /// </summary>
    IEnumerator EsperarYReiniciar()
    {
        Debug.Log($"[MovimientoGD] Esperando {tiempoEsperaMuerte} segundos antes de reiniciar...");
        
        // Esperar el tiempo configurado
        yield return new WaitForSeconds(tiempoEsperaMuerte);
        
        // Recargar la escena actual
        string escenaActual = SceneManager.GetActiveScene().name;
        Debug.Log($"[MovimientoGD] 🔄 Reiniciando nivel: {escenaActual}");
        SceneManager.LoadScene(escenaActual);
    }

    // ========================================
    // VISUALIZACIÓN DE DEBUG (EDITOR)
    // ========================================
    
    /// <summary>
    /// Dibuja Gizmos en el Editor de Unity para visualizar los raycasts
    /// CYAN: Raycasts de detección de suelo en modo Cubo
    /// YELLOW: Raycasts de detección de superficie en modo Bola
    /// </summary>
    void OnDrawGizmos()
    {
        // Si no hay BoxCollider, no dibujar nada
        if (!col) return;
        
        // RAYCASTS MODO CUBO (Detección de suelo)
        Gizmos.color = Color.cyan;
        Vector2 rIzquierda = new Vector2(col.bounds.min.x, col.bounds.center.y);
        Vector2 rDerecha = new Vector2(col.bounds.max.x, col.bounds.center.y);
        Gizmos.DrawLine(rIzquierda, rIzquierda + Vector2.down * distanciaRayo);
        Gizmos.DrawLine(rDerecha, rDerecha + Vector2.down * distanciaRayo);
        
        // RAYCASTS MODO BOLA (Detección de superficie arriba y abajo)
        if (modoBola)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, (Vector2)transform.position + Vector2.up * distanciaRayo);
            Gizmos.DrawLine(transform.position, (Vector2)transform.position + Vector2.down * distanciaRayo);
        }
    }
}
