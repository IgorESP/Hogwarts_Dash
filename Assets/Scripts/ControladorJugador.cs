using UnityEngine;

public class MovimientoGD : MonoBehaviour
{
    [Header("Movimiento")]
    public float velocidad = 10.4f;
    public float fuerzaSalto = 20f;
    public float gravedadEscala = 9f;

    [Header("Suelo")]
    public LayerMask capaSuelo;

    Rigidbody2D rb;
    BoxCollider2D col;

    bool pedirSalto;
    bool enSalto;
    float rotacionObjetivo;
    float rotacionRestante;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<BoxCollider2D>();
        rb.gravityScale = gravedadEscala;
    }

    void Update()
    {
        // INPUT SIEMPRE AQUÍ
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            pedirSalto = true;
        }
    }

    void FixedUpdate()
    {
        // Movimiento horizontal constante
        rb.linearVelocity = new Vector2(velocidad, rb.linearVelocity.y);

        bool tocandoSuelo = Physics2D.OverlapBox(
            col.bounds.center + Vector3.down * (col.bounds.extents.y + 0.05f),
            new Vector2(col.bounds.size.x * 0.9f, 0.1f),
            0f,
            capaSuelo
        );

        // Salto
      if (pedirSalto && tocandoSuelo)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, fuerzaSalto);

            rotacionRestante = 180f; // SIEMPRE 180
        }


        pedirSalto = false;

        // Rotación 180° exactos
       if (rotacionRestante > 0f)
        {
            float paso = 720f * Time.fixedDeltaTime;
            float rotar = Mathf.Min(paso, rotacionRestante);

            rb.MoveRotation(rb.rotation - rotar); // ← SIEMPRE hacia delante
            rotacionRestante -= rotar;
        }

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
