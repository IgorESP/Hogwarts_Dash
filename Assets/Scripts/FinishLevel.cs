using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Script para detectar cuando el jugador completa un nivel.
/// Cuenta intentos automáticamente cada vez que se reinicia la escena.
/// </summary>
public class FinishLevel : MonoBehaviour
{
    [Header("Configuración")]
    [Tooltip("Tiempo de espera antes de mostrar resultados")]
    public float tiempoEspera = 0.5f;
    
    private bool nivelCompletado = false;
    private static int intentosAcumulados = 0;
    
    void Start()
    {
        // Cada vez que se carga la escena, incrementar intentos
        // EXCEPTO la primera vez
        if (!PlayerPrefs.HasKey("PrimeraVez_" + SceneManager.GetActiveScene().name))
        {
            // Primera vez en este nivel, no contar como intento
            PlayerPrefs.SetInt("PrimeraVez_" + SceneManager.GetActiveScene().name, 1);
            intentosAcumulados = 0;
        }
        else
        {
            // Ya jugó antes, esto es un reintento
            intentosAcumulados++;
        }
        
        Debug.Log($"[FinishLevel] Intentos actuales: {intentosAcumulados}");
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !nivelCompletado)
        {
            Debug.Log("[FinishLevel] JUGADOR DETECTADO - Finalizando nivel!");
            nivelCompletado = true;
            Invoke("FinalizarNivel", tiempoEspera);
        }
    }
    
    private void FinalizarNivel()
    {
        Debug.Log("[FinishLevel] Nivel completado!");
        
        // Obtener mundo actual
        int mundoActual = PlayerPrefs.GetInt("MundoSeleccionado", 1);
        
        // Contar snitches recogidas (inactivas)
        int snitchesRecogidas = ContarSnitchesRecogidas();
        
        // Calcular porcentaje (siempre 100% si llegó aquí)
        float porcentaje = 100f;
        
        Debug.Log($"[FinishLevel] Estadísticas:");
        Debug.Log($"  - Porcentaje: {porcentaje}%");
        Debug.Log($"  - Intentos: {intentosAcumulados}");
        Debug.Log($"  - Snitches: {snitchesRecogidas}");
        
        // Guardar en NivelesManager
        if (NivelesManager.Instance != null)
        {
            NivelesManager.Instance.CompletarNivel(mundoActual, 1, porcentaje);
        }
        
        // Guardar estadísticas para el panel
        PlayerPrefs.SetFloat("UltimoPorcentaje", porcentaje);
        PlayerPrefs.SetInt("UltimosIntentos", intentosAcumulados);
        PlayerPrefs.SetInt("UltimasSnitches", snitchesRecogidas);
        PlayerPrefs.SetInt("UltimoMundoCompletado", mundoActual);
        PlayerPrefs.Save();
        
        // Resetear intentos para el próximo nivel
        intentosAcumulados = 0;
        PlayerPrefs.DeleteKey("PrimeraVez_" + SceneManager.GetActiveScene().name);
        
        // Llamar a GameManager si existe
        if (GameManager.Instance != null)
        {
            GameManager.Instance.FinalizarNivel();
        }
        else
        {
            // Si no hay GameManager, cargar ResultadosScene directamente
            SceneManager.LoadScene("ResultadosScene");
        }
    }
    
    private int ContarSnitchesRecogidas()
    {
        try
        {
            GameObject[] snitches = GameObject.FindGameObjectsWithTag("Snitch");
            int recogidas = 0;
            
            foreach (GameObject snitch in snitches)
            {
                if (!snitch.activeInHierarchy)
                    recogidas++;
            }
            
            Debug.Log($"[FinishLevel] Snitches recogidas: {recogidas}/{snitches.Length}");
            return recogidas;
        }
        catch
        {
            Debug.LogWarning("[FinishLevel] Tag 'Snitch' no existe");
            return 0;
        }
    }
    
    private void OnDrawGizmos()
    {
        Collider2D col = GetComponent<Collider2D>();
        if (col == null) return;
        
        Gizmos.color = nivelCompletado ? Color.red : Color.green;
        
        if (col is BoxCollider2D box)
        {
            Gizmos.DrawWireCube(transform.position + (Vector3)box.offset, box.size);
        }
        else if (col is CircleCollider2D circle)
        {
            Gizmos.DrawWireSphere(transform.position + (Vector3)circle.offset, circle.radius);
        }
    }
}