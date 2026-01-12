using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class PlayerSetup : MonoBehaviour
{
    // Este script es una herramienta de un solo uso para configurar tu personaje rápidamente.
    // Solo añádelo a tu objeto con sprite y se configurará solo.

    public void Setup()
    {
        // 1. Asignar Tag
        if (!gameObject.CompareTag("Player"))
        {
            gameObject.tag = "Player";
            Debug.Log("✔ Tag 'Player' asignado.");
        }

        // 2. Añadir Rigidbody2D
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous; // Vital para no atravesar paredes
            rb.constraints = RigidbodyConstraints2D.FreezeRotation; // Que no ruede como pelota
            Debug.Log("✔ Rigidbody2D añadido y configurado.");
        }

        // 3. Añadir Collider
        BoxCollider2D col = GetComponent<BoxCollider2D>();
        if (col == null)
        {
            col = gameObject.AddComponent<BoxCollider2D>();
            Debug.Log("✔ BoxCollider2D añadido.");
        }

        // 4. Añadir PlayerController
        PlayerController controller = GetComponent<PlayerController>();
        if (controller == null)
        {
            controller = gameObject.AddComponent<PlayerController>();
            
            // Intentar buscar los hijos para el GroundCheck
            Transform check = transform.Find("GroundCheck");
            if (check == null)
            {
                check = new GameObject("GroundCheck").transform;
                check.SetParent(transform);
                check.localPosition = new Vector3(0, -0.5f, 0); // Ponerlo en los pies
            }
            controller.groundCheck = check;
            
            // Intentar asignar layer de suelo (esto asume que el Layer 3 o 6 es ground, depende del proyecto, se avisa)
             controller.groundLayer = LayerMask.GetMask("Ground");
            
            Debug.Log("✔ PlayerController añadido.");
        }
        
        Debug.Log("✨ ¡LISTO! Tu cubo Slytherin ahora es el Jugador. Borra este script 'PlayerSetup' si quieres.");
        DestroyImmediate(this); // Autodestruirse para limpiar
    }

    // Se ejecuta al añadir el script en el editor
    private void Reset()
    {
        Setup();
    }
}
