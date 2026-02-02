using UnityEngine;

public class ResetProgreso : MonoBehaviour
{
    void Update()
    {
        // Presiona R para resetear todo
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetearTodo();
        }
    }
    
   public void ResetearTodo()
{
    Debug.LogWarning("ESETEANDO TODO EL PROGRESO...");
    
    // Borrar ABSOLUTAMENTE TODO
    PlayerPrefs.DeleteAll();
    PlayerPrefs.Save();
    
    // Esperar un frame
    System.Threading.Thread.Sleep(100);
    
    // Inicializar SOLO Mundo 1
    PlayerPrefs.SetInt("Mundo_1_Desbloqueado", 1);
    
    // Asegurarse de que los demás estén bloqueados
    PlayerPrefs.SetInt("Mundo_2_Desbloqueado", 0);
    PlayerPrefs.SetInt("Mundo_3_Desbloqueado", 0);
    PlayerPrefs.SetInt("Mundo_4_Desbloqueado", 0);
    PlayerPrefs.SetInt("Mundo_5_Desbloqueado", 0);
    
    PlayerPrefs.Save();
    
    Debug.Log("Progreso borrado completamente");
    Debug.Log("Solo Mundo 1 desbloqueado");
    Debug.Log("Presiona F5 para actualizar la UI");
    
    // Forzar actualización de UI
    var controller = FindObjectOfType<NivelesUIController>();
    if (controller != null)
    {
        controller.SendMessage("ActualizarUI", SendMessageOptions.DontRequireReceiver);
    }
}
}

