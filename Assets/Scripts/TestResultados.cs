using UnityEngine;

public class TestResultados : MonoBehaviour
{
    void Start()
    {
        Debug.Log("🧪 TestResultados iniciado - Presiona T para ver el menú");
    }
    
    void Update()
    {
        // Presiona T para mostrar el menú
        if (Input.GetKeyDown(KeyCode.T))
        {
            Debug.Log("⌨️ Tecla T presionada");
            MostrarMenuPrueba();
        }
    }
    
    void MostrarMenuPrueba()
    {
        Debug.Log("🔍 Buscando ResultadosController...");
        
        ResultadosController resultados = FindObjectOfType<ResultadosController>();
        
        if (resultados != null)
        {
            Debug.Log("✅ ResultadosController encontrado, mostrando menú...");
            
            resultados.MostrarResultados(
                porcentaje: 85f,
                intentos: 3,
                snitches: 5,
                estrellas: 2
            );
        }
        else
        {
            Debug.LogError("❌ ResultadosController NO encontrado en la escena");
            Debug.LogError("Verifica que el GameObject tenga el componente ResultadosController");
        }
    }
}