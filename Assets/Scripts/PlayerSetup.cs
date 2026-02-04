using UnityEngine;

/// <summary>
/// Herramienta de configuración automática del jugador - Se ejecuta una sola vez
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class PlayerSetup : MonoBehaviour
{
    /// <summary>
    /// Configura automáticamente el jugador con todos los componentes necesarios
    /// </summary>
    public void Setup()
    {
        Debug.Log("[PlayerSetup] Iniciando configuración automática del jugador...");
        
        // 1. Asignar Tag
        if (!gameObject.CompareTag("Player"))
        {
            gameObject.tag = "Player";
            Debug.Log("[PlayerSetup] ✔ Tag 'Player' asignado");
        }

        // 2. Añadir Rigidbody2D
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous; // Vital para no atravesar paredes
            rb.constraints = RigidbodyConstraints2D.FreezeRotation; // Evitar rotación no deseada
            Debug.Log("[PlayerSetup] ✔ Rigidbody2D añadido y configurado");
        }

        // 3. Añadir Collider
        BoxCollider2D col = GetComponent<BoxCollider2D>();
        if (col == null)
        {
            col = gameObject.AddComponent<BoxCollider2D>();
            Debug.Log("[PlayerSetup] ✔ BoxCollider2D añadido");
        }

        // 4. Añadir PlayerController
        PlayerController controller = GetComponent<PlayerController>();
        if (controller == null)
        {
            controller = gameObject.AddComponent<PlayerController>();
            
            // Crear GroundCheck
            Transform check = transform.Find("GroundCheck");
            if (check == null)
            {
                check = new GameObject("GroundCheck").transform;
                check.SetParent(transform);
                check.localPosition = new Vector3(0, -0.5f, 0); // Posicionar en los pies
            }
            controller.groundCheck = check;
            
            // Asignar layer de suelo
            controller.groundLayer = LayerMask.GetMask("Ground");
            
            Debug.Log("[PlayerSetup] ✔ PlayerController añadido");
        }
        
        Debug.Log("[PlayerSetup] ✨ ¡Configuración completada! El script se autodestruirá");
        DestroyImmediate(this); // Autodestruirse tras configuración
    }

    /// <summary>
    /// Se ejecuta automáticamente al añadir el script en el editor
    /// </summary>
    private void Reset()
    {
        Setup();
    }
}
