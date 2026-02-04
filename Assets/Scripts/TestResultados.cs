using UnityEngine;

/// <summary>
/// Utilidad de prueba para invocar el panel de resultados manualmente (Tecla T)
/// </summary>
public class TestResultados : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            InvocarSimulacion();
        }
    }
    
    void InvocarSimulacion()
    {
        Debug.Log("[Debug] Simulando fin de nivel...");
        ResultadosController res = FindFirstObjectByType<ResultadosController>();
        
        if (res != null)
        {
            res.MostrarResultados(85, 3, 5, true);
        }
        else Debug.LogError("[Debug] ResultadosController no encontrado en esta escena.");
    }
}