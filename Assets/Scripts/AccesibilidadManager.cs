using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Gestor universal de accesibilidad que aplica configuraciones visuales automáticamente
/// </summary>
public class AccesibilidadManager : MonoBehaviour
{
    void Start()
    {
        // Esperar brevemente para asegurar que el UIDocument esté listo
        Invoke("AplicarConfiguracion", 0.1f);
    }

    /// <summary>
    /// Recupera y aplica la configuración de accesibilidad guardada en PlayerPrefs
    /// </summary>
    private void AplicarConfiguracion()
    {
        var uiDoc = GetComponent<UIDocument>();
        if (uiDoc == null)
        {
            Debug.LogWarning($"[Accesibilidad] No hay UIDocument en {gameObject.name}. Ignorando.");
            return;
        }

        var root = uiDoc.rootVisualElement;
        if (root == null) return;

        // Aplicar capas de estilo según ajustes
        //AplicarModoDaltonico(root);
        AplicarTamanoLetra(root);
    }

    /// <summary>
    /// Escala el tamaño de las fuentes basándose en el porcentaje guardado
    /// </summary>
    private void AplicarTamanoLetra(VisualElement root)
    {
        float tamano = PlayerPrefs.GetFloat("TamanoLetra", 100f);
        float escala = tamano / 100f;

        root.Query<Label>().ForEach(label => {
            // Excluir labels específicos que no deben escalar (como los propios porcentajes)
            if (label.name != "Label_Musica" && 
                label.name != "Label_SFX" && 
                label.name != "Label_TamanoLetra")
            {
                float baseSize = DeterminarTamanoBase(label.text);
                label.style.fontSize = baseSize * escala;
            }
        });

        Debug.Log($"[Accesibilidad] Escala de fuente a {tamano}% en {gameObject.scene.name}");
    }

    /// <summary>
    /// Mapeo de tamaños base para diferentes tipos de texto
    /// </summary>
    private float DeterminarTamanoBase(string texto)
    {
        if (string.IsNullOrEmpty(texto)) return 22f;

        // Títulos grandes
        if (texto.Contains("HOGWARTS") || texto.Contains("OPCIONES")) return 50f;

        // Títulos de sección
        if (texto.Contains("AUDIO") || texto.Contains("ACCESIBILIDAD") ||
            texto.Contains("CRÉDITOS") || texto.Contains("RANKING") ||
            texto.Contains("TU CASA") || texto.Contains("MUNDOS")) return 32f;

        // Botones e interacciones
        if (texto.Contains("ENTRAR") || texto.Contains("CREAR CUENTA") ||
            texto.Contains("REGISTRARSE") || texto.Contains("CERRAR SESIÓN")) return 26f;

        return 22f; // Tamaño estándar
    }
}