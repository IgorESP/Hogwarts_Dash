using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishLevel : MonoBehaviour
{
    [Header("Configuración")]
    public float tiempoEspera = 0.5f;
    private bool nivelCompletado = false;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !nivelCompletado)
        {
            nivelCompletado = true;
            Invoke("FinalizarNivel", tiempoEspera);
        }
    }
    
    private void FinalizarNivel()
    {
        int mundoActual = PlayerPrefs.GetInt("MundoSeleccionado", 1);
        float porcentaje = 100f; 
        int monedasRecogidas = ContarMonedasRecogidas();
        int intentos = ObtenerIntentos();
        
        PlayerPrefs.SetFloat("UltimoPorcentaje", porcentaje);
        PlayerPrefs.SetInt("UltimosIntentos", intentos);
        PlayerPrefs.SetInt("UltimasSnitches", monedasRecogidas);
        PlayerPrefs.Save();
        
        if (GameManager.Instance != null)
        {
            GameManager.Instance.FinalizarNivel();
        }
        else
        {
            SceneManager.LoadScene("ResultadosScene");
        }
    }
    
    private int ContarMonedasRecogidas()
    {
        // CAMBIO: Buscamos todos los scripts de Snitch, incluyendo los desactivados
        AnimacionSnitch[] todasLasSnitches = Resources.FindObjectsOfTypeAll<AnimacionSnitch>();
        int recogidas = 0;
        
        foreach (AnimacionSnitch snitch in todasLasSnitches)
        {
            // Solo contamos los que pertenecen a la escena y tienen 'recogida' en true
            if (snitch.gameObject.scene.name != null && snitch.recogida)
            {
                recogidas++;
            }
        }
        return recogidas;
    }
    
    private int ObtenerIntentos()
    {
        string escenaActual = SceneManager.GetActiveScene().name;
        return PlayerPrefs.GetInt("Intentos_" + escenaActual, 0);
    }
}