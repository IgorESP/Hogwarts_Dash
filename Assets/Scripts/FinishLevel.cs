using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishLevel : MonoBehaviour
{
    [Header("Configuración")]
    public string escenaFinal = "MainScene";
    public float tiempoEspera = 0.5f;
    
    private bool nivelCompletado = false;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"Trigger 2D → {other.name} (Tag: '{other.tag}')");
        
        if (other.CompareTag("Player") && !nivelCompletado)
        {
            Debug.Log("PLAYER DETECTADO - Finalizando nivel!");
            nivelCompletado = true;
            FinalizarNivel();
        }
    }
    
    private void FinalizarNivel()
    {
        Debug.Log("¡Nivel completado!");
        Invoke("CargarMenuFinal", tiempoEspera);
    }
    
    private void CargarMenuFinal()
    {
        Debug.Log($"Cargando escena: {escenaFinal}");
        SceneManager.LoadScene(escenaFinal);
    }
}
