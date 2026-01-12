using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public enum PlayerMode { Cube, Ship, Ball }

    [Header("Configuración General")]
    public float moveSpeed = 8f;
    public PlayerMode currentMode = PlayerMode.Cube;
    public LayerMask groundLayer;
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;

    [Header("Modo Cubo")]
    public float jumpForce = 14f;
    public float gravityScaleCube = 5f;

    [Header("Modo Nave")]
    public float flyForce = 25f; // Fuerza continua hacia arriba
    public float gravityScaleShip = 3f;

    [Header("Modo Bola")]
    public float gravityScaleBall = 5f;

    private Rigidbody2D rb;
    private bool isGrounded;
    private bool isUpsideDown = false; 

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        SetMode(currentMode);
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

    void HandleCube(bool clicked)
    {
        rb.gravityScale = gravityScaleCube * (isUpsideDown ? -1 : 1);

        if (clicked && isGrounded)
        {
            Jump(jumpForce);
        }
    }

    void HandleShip(bool holding)
    {
        rb.gravityScale = gravityScaleShip * (isUpsideDown ? -1 : 1);

        if (holding)
        {
             // Añadir fuerza para volar hacia arriba (contrarrestar gravedad)
             // Multiplicamos por la escala de gravedad/dirección para saber qué es "arriba"
             float dir = isUpsideDown ? -1 : 1;
             rb.AddForce(Vector2.up * flyForce * dir);
        }

        // Rotación suave nave
        float verticalVel = Mathf.Clamp(rb.linearVelocity.y, -10f, 10f); // Clamp visual
        float rot = verticalVel * 3f; 
        transform.rotation = Quaternion.Euler(0, 0, rot);
    }

    void HandleBall(bool clicked)
    {
        rb.gravityScale = gravityScaleBall * (isUpsideDown ? -1 : 1);

        // En modo bola, al hacer click en el suelo, se invierte la gravedad
        if (clicked && isGrounded)
        {
            isUpsideDown = !isUpsideDown;
            rb.gravityScale = gravityScaleBall * (isUpsideDown ? -1 : 1);
        }
    }

    void Jump(float force)
    {
        float dir = isUpsideDown ? -1 : 1;
        // Reseteamos velocidad Y para salto consistente
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, force * dir);
    }

    public void SetMode(PlayerMode newMode)
    {
        currentMode = newMode;
        isUpsideDown = false; // Resetear gravedad al cambiar (opcional)
        transform.rotation = Quaternion.identity;

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

    // Colisiones con triggers de transformación, victoria o muerte
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("ModeShip")) SetMode(PlayerMode.Ship);
        else if (collision.CompareTag("ModeCube")) SetMode(PlayerMode.Cube);
        else if (collision.CompareTag("ModeBall")) SetMode(PlayerMode.Ball);
        else if (collision.CompareTag("Win")) GameManager.Instance.LevelComplete();
    }
    
    // Colisión física (si los obstáculos no son triggers)
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            GameManager.Instance.PlayerDied();
            gameObject.SetActive(false); // Ocultar jugador
        }
    }
}
