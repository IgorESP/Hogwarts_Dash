using UnityEngine;

/// <summary>
/// Utilidad de prueba para invocar el panel de resultados manualmente (Tecla T)
/// </summary>
public class TestResultados : MonoBehaviour
{
    void Update()
    {
        // Si el jugador pulsa la tecla T, se ejecuta la simulación
        if (Input.GetKeyDown(KeyCode.T))
        {
            InvocarSimulacion();
        }
    }
    
    /// <summary> 
    /// Busca el ResultadosController en la escena y le envía datos simulados. 
    /// </summary>
    void InvocarSimulacion()
    {
        Debug.Log("[Debug] Simulando fin de nivel...");

        // Buscar el panel de resultados activo en la escena
        ResultadosController res = FindFirstObjectByType<ResultadosController>();
        
        if (res != null)
        {
            // Mostrar resultados falsos para probar la UI 
            // Parámetros: porcentaje, intentos, snitches, victoria
            res.MostrarResultados(85, 3, 5, true);
        }
        else 
        {
            Debug.LogError("[Debug] ResultadosController no encontrado en esta escena.");
        }
    }
}