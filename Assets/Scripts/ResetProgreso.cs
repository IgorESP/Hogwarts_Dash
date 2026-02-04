using UnityEngine;

/// <summary>
/// Herramienta de Debug para resetear todo el progreso del jugador (Tecla R)
/// </summary>
public class ResetProgreso : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetearTodo();
        }
    }
    
    public void ResetearTodo()
    {
        Debug.LogWarning("[ResetProgreso] !PELIGRO! Borrando todos los PlayerPrefs");
        
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        
        // Bloqueo manual vs Desbloqueo de nivel inicial
        PlayerPrefs.SetInt("Mundo_1_Desbloqueado", 1);
        for (int i = 2; i <= 5; i++) PlayerPrefs.SetInt($"Mundo_{i}_Desbloqueado", 0);
        
        PlayerPrefs.Save();
        
        Debug.Log("[ResetProgreso] Proceso completado. Mundo 1 habilitado solamente.");
        
        // Intentar refrescar la interfaz si existe
        var controller = FindFirstObjectByType<NivelesUIController>();
        if (controller != null) controller.SendMessage("ActualizarUI", SendMessageOptions.DontRequireReceiver);
    }
}
