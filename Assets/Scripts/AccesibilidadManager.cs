using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Gestor universal de accesibilidad
/// Aplica modo daltónico y tamaño de letra en CUALQUIER escena automáticamente
/// Se debe agregar al GameObject que tiene el UIDocument en cada escena
/// </summary>
public class AccesibilidadManager : MonoBehaviour
{
    // ====================================
    // INICIALIZACIÓN
    // ====================================
    void Start()
    {
        // Esperar un frame para que la UI se cargue completamente
        Invoke("AplicarConfiguracion", 0.1f);
    }

    // ====================================
    // APLICAR CONFIGURACIÓN
    // ====================================
    
    /// <summary>
    /// Aplica la configuración de accesibilidad guardada
    /// </summary>
    private void AplicarConfiguracion()
    {
        var uiDoc = GetComponent<UIDocument>();
        if (uiDoc == null)
        {
            Debug.LogWarning("⚠️ No se encontró UIDocument en " + gameObject.name);
            return;
        }

        var root = uiDoc.rootVisualElement;
        if (root == null)
        {
            Debug.LogWarning("⚠️ rootVisualElement es null en " + gameObject.name);
            return;
        }

        // Aplicar modo daltónico
        AplicarModoDaltonico(root);

        // Aplicar tamaño de letra
        AplicarTamanoLetra(root);
    }

    // ====================================
    // MODO DALTÓNICO
    // ====================================
    
    /// <summary>
    /// Aplica el modo daltónico si está activado
    /// </summary>
    private void AplicarModoDaltonico(VisualElement root)
    {
        bool modoDaltonico = PlayerPrefs.GetInt("ModoDaltonico", 0) == 1;
        
        if (modoDaltonico)
        {
            root.AddToClassList("modo-daltonico-activado");
            Debug.Log("🎨 Modo daltónico ACTIVADO en: " + gameObject.scene.name);
        }
        else
        {
            root.RemoveFromClassList("modo-daltonico-activado");
            Debug.Log("🎨 Modo daltónico DESACTIVADO en: " + gameObject.scene.name);
        }
    }

    // ====================================
    // TAMAÑO DE LETRA
    // ====================================
    
    /// <summary>
    /// Aplica el tamaño de letra configurado
    /// </summary>
    private void AplicarTamanoLetra(VisualElement root)
    {
        float tamano = PlayerPrefs.GetFloat("TamanoLetra", 100f);
        float escala = tamano / 100f;

        root.Query<Label>().ForEach(label => {
            // No cambiar labels de porcentajes en ajustes
            if (label.name != "Label_Musica" && 
                label.name != "Label_SFX" && 
                label.name != "Label_TamanoLetra")
            {
                float baseSize = DeterminarTamanoBase(label.text);
                label.style.fontSize = baseSize * escala;
            }
        });

        Debug.Log("📏 Tamaño de letra aplicado: " + tamano + "% en " + gameObject.scene.name);
    }

    /// <summary>
    /// Determina el tamaño base según el contenido del texto
    /// </summary>
    private float DeterminarTamanoBase(string texto)
    {
        if (string.IsNullOrEmpty(texto))
            return 22f;

        // Títulos principales muy grandes
        if (texto.Contains("HOGWARTS") || texto.Contains("OPCIONES"))
            return 50f;

        // Subtítulos de secciones
        if (texto.Contains("AUDIO") || 
            texto.Contains("ACCESIBILIDAD") ||
            texto.Contains("CRÉDITOS") || 
            texto.Contains("RANKING") ||
            texto.Contains("SELECCIONA") || 
            texto.Contains("SELECCIÓN") ||
            texto.Contains("TU PERSONAJE") ||
            texto.Contains("TU CASA") ||
            texto.Contains("MUNDOS"))
            return 32f;

        // Texto de botones grandes
        if (texto.Contains("ENTRAR") || 
            texto.Contains("CREAR CUENTA") ||
            texto.Contains("VER PERFIL") || 
            texto.Contains("CERRAR SESIÓN") ||
            texto.Contains("REGISTRARSE") ||
            texto.Contains("ENTRAR AL JUEGO"))
            return 26f;

        // Etiquetas y texto normal
        return 22f;
    }
}