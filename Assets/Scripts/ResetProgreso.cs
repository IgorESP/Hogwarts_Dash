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
    
    /// <summary> 
    /// Borra todos los PlayerPrefs y restablece el estado inicial del juego. 
    /// </summary>
    public void ResetearTodo()
    {
        Debug.LogWarning("[ResetProgreso] !PELIGRO! Borrando todos los PlayerPrefs");
        
        // Elimina absolutamente todas las claves guardadas
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        
        // Vuelve a bloquear todos los mundos excepto el primero
        PlayerPrefs.SetInt("Mundo_1_Desbloqueado", 1);

        // Mundos 2 a 5 quedan bloqueados
        for (int i = 2; i <= 5; i++) 
            PlayerPrefs.SetInt($"Mundo_{i}_Desbloqueado", 0);
        
        PlayerPrefs.Save();
        
        Debug.Log("[ResetProgreso] Proceso completado. Mundo 1 habilitado solamente.");
        
        // Intentar refrescar la interfaz si existe
        var controller = FindFirstObjectByType<NivelesUIController>();
        if (controller != null)
        {
            controller.SendMessage("ActualizarUI", SendMessageOptions.DontRequireReceiver);
        }
    }
}
