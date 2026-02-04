using UnityEngine;

/// <summary>
/// Controlador de jugador con múltiples modos: Cubo, Nave y Bola
/// Versión simplificada estilo Geometry Dash
/// </summary>
public class PlayerController : MonoBehaviour
{
    public enum PlayerMode { Cube, Ship, Ball }

    [Header("Configuración General")]
    [Tooltip("Velocidad horizontal constante")]
    public float moveSpeed = 8f;
    
    [Tooltip("Modo actual del jugador")]
    public PlayerMode currentMode = PlayerMode.Cube;
    
    [Tooltip("Layer del suelo para detección")]
    public LayerMask groundLayer;
    
    [Tooltip("Transform para detectar si está en el suelo")]
    public Transform groundCheck;
    
    [Tooltip("Radio de detección de suelo")]
    public float groundCheckRadius = 0.2f;

    [Header("Modo Cubo")]
    [Tooltip("Fuerza del salto en modo Cubo")]
    public float jumpForce = 14f;
    
    [Tooltip("Escala de gravedad en modo Cubo")]
    public float gravityScaleCube = 5f;

    [Header("Modo Nave")]
    [Tooltip("Fuerza continua hacia arriba al volar")]
    public float flyForce = 25f;
    
    [Tooltip("Escala de gravedad en modo Nave")]
    public float gravityScaleShip = 3f;

    [Header("Modo Bola")]
    [Tooltip("Escala de gravedad en modo Bola")]
    public float gravityScaleBall = 5f;

    private Rigidbody2D rb;             // Componente de física
    private bool isGrounded;             // True si está tocando el suelo
    private bool isUpsideDown = false;  // True si la gravedad está invertida

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        SetMode(currentMode);
        Debug.Log($"[PlayerController] Inicializado en modo: {currentMode}");
    }

    private void Update()
    {
        // 1. Detección de Suelo
        if (groundCheck != null)
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // 2. Input
        // Soporta teclado ("Jump" default es Espacio) y toque pantalla
        bool inputHeld = Input.GetButton("Jump") || Input.touchCount > 0;
        bool inputDown = Input.GetButtonDown("Jump") || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began);

        // 3. Lógica por Estado
        switch (currentMode)
        {
            case PlayerMode.Cube:
                HandleCube(inputDown);
                break;
            case PlayerMode.Ship:
                HandleShip(inputHeld);
                break;
            case PlayerMode.Ball:
                HandleBall(inputDown);
                break;
        }

        // 4. Transformaciones visuales (Rotación)
        if (currentMode == PlayerMode.Cube)
        {
            if (!isGrounded)
            {
                // Rotar constantemente en el aire
                transform.Rotate(Vector3.back * 400f * Time.deltaTime);
            }
            else
            {
                // Alinear al suelo
                Quaternion target = Quaternion.Euler(0, 0, Mathf.Round(transform.rotation.eulerAngles.z / 90) * 90);
                transform.rotation = Quaternion.Lerp(transform.rotation, target, Time.deltaTime * 15f);
            }
        }
    }

    private void FixedUpdate()
    {
        // Movimiento constante en X (FixedUpdate para físicas estables)
        // Mantenemos la velocidad vertical calculada por físicas/input
        rb.linearVelocity = new Vector2(moveSpeed, rb.linearVelocity.y);
    }

    /// <summary>
    /// Lógica del modo Cubo - Salto clásico de plataformas
    /// </summary>
    void HandleCube(bool clicked)
    {
        rb.gravityScale = gravityScaleCube * (isUpsideDown ? -1 : 1);

        if (clicked && isGrounded)
        {
            Jump(jumpForce);
            Debug.Log("[PlayerController] Salto (Cubo)");
        }
    }

    /// <summary>
    /// Lógica del modo Nave - Vuelo continuo al mantener presionado
    /// </summary>
    void HandleShip(bool holding)
    {
        rb.gravityScale = gravityScaleShip * (isUpsideDown ? -1 : 1);

        if (holding)
        {
             // Añadir fuerza para volar hacia arriba (contrarrestar gravedad)
             float dir = isUpsideDown ? -1 : 1;
             rb.AddForce(Vector2.up * flyForce * dir);
        }

        // Rotación dinámica según velocidad vertical
        float verticalVel = Mathf.Clamp(rb.linearVelocity.y, -10f, 10f);
        float rot = verticalVel * 3f; 
        transform.rotation = Quaternion.Euler(0, 0, rot);
    }

    /// <summary>
    /// Lógica del modo Bola - Inversión de gravedad al hacer clic
    /// </summary>
    void HandleBall(bool clicked)
    {
        rb.gravityScale = gravityScaleBall * (isUpsideDown ? -1 : 1);

        // En modo bola, al hacer click en el suelo, se invierte la gravedad
        if (clicked && isGrounded)
        {
            isUpsideDown = !isUpsideDown;
            rb.gravityScale = gravityScaleBall * (isUpsideDown ? -1 : 1);
            Debug.Log($"[PlayerController] Gravedad invertida (Bola) - UpsideDown: {isUpsideDown}");
        }
    }

    /// <summary>
    /// Ejecuta un salto con la fuerza especificada
    /// </summary>
    void Jump(float force)
    {
        float dir = isUpsideDown ? -1 : 1;
        // Resetear velocidad Y para salto consistente
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, force * dir);
    }

    /// <summary>
    /// Cambia el modo del jugador (Cubo, Nave o Bola)
    /// </summary>
    public void SetMode(PlayerMode newMode)
    {
        currentMode = newMode;
        isUpsideDown = false; // Resetear gravedad al cambiar
        transform.rotation = Quaternion.identity;
        
        Debug.Log($"[PlayerController] Modo cambiado a: {newMode}");

        switch (newMode)
        {
            case PlayerMode.Cube:
                rb.gravityScale = gravityScaleCube;
                break;
            case PlayerMode.Ship:
                rb.gravityScale = gravityScaleShip;
                break;
            case PlayerMode.Ball:
                rb.gravityScale = gravityScaleBall;
                break;
        }
    }

    /// <summary>
    /// Detecta colisiones con triggers: portales de modo, meta, etc.
    /// </summary>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("ModeShip")) SetMode(PlayerMode.Ship);
        else if (collision.CompareTag("ModeCube")) SetMode(PlayerMode.Cube);
        else if (collision.CompareTag("ModeBall")) SetMode(PlayerMode.Ball);
        else if (collision.CompareTag("Win")) 
        {
            Debug.Log("[PlayerController] ¡Nivel completado!");
            GameManager.Instance.LevelComplete();
        }
    }
    
    /// <summary>
    /// Detecta colisiones físicas con obstáculos
    /// </summary>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            Debug.Log($"[PlayerController] Muerte por obstáculo: {collision.gameObject.name}");
            GameManager.Instance.PlayerDied();
            gameObject.SetActive(false); // Ocultar jugador
        }
    }
}
