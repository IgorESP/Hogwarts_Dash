using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// Gestiona el escalado global de textos y colores de accesibilidad entre escenas
/// </summary>
public class ScaleManager : MonoBehaviour
{
    private static ScaleManager instance;
    
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("[ScaleManager] Persistencia activada");
        }
        else 
        { 
            Destroy(gameObject); 
            return; 
        }
    }
    
    /// <summary> 
    /// Se suscribe al evento de carga de escena para aplicar ajustes automáticamente. 
    /// </summary>
    void OnEnable() 
    { 
        SceneManager.sceneLoaded += (s, m) => StartCoroutine(RefrescarAjustes()); 
    }
    
    /// <summary> 
    /// Espera un frame para asegurar que UI Toolkit ha cargado completamente. 
    /// </summary>
    private IEnumerator RefrescarAjustes()
    {
        yield return new WaitForEndOfFrame();
        AplicarTodo();
    }
    
    /// <summary> 
    /// Aplica todos los ajustes de accesibilidad disponibles. 
    /// </summary>
    public void AplicarTodo()
    {
        AplicarEscalaTexto();
        AplicarModoDaltonico();
    }
    
    /// <summary> 
    /// Escala el tamaño de todas las fuentes de la UI según la preferencia del usuario. 
    /// </summary>
    private void AplicarEscalaTexto()
    {
        float escala = PlayerPrefs.GetFloat("EscalaTexto", 1f);

        // Si la escala es 1, no hay nada que modificar
        if (escala == 1f) return;
        
        Debug.Log($"[ScaleManager] Escalando fuentes al {escala * 100}%");
        
        // Buscar todos los UIDocument activos en la escena
        foreach (UIDocument ui in FindObjectsByType<UIDocument>(FindObjectsSortMode.None))
        {
            if (ui.rootVisualElement != null)
            {
                ui.rootVisualElement.Query<Label>().ForEach(l => {
                    if (l.resolvedStyle.fontSize != 0) 
                        l.style.fontSize = l.resolvedStyle.fontSize * escala;
                });
            }
        }
    }
    
    /// <summary> 
    /// Aplica el modo daltónico si está activado en PlayerPrefs. 
    /// </summary>
    private void AplicarModoDaltonico()
    {
        if (PlayerPrefs.GetInt("ModoDaltonico", 0) != 1) return;
        
        Debug.Log("[ScaleManager] Inyectando paleta daltónica");

        // Aplicar ajustes de color a todos los UIDocument
        foreach (UIDocument ui in FindObjectsByType<UIDocument>(FindObjectsSortMode.None))
        {
            if (ui.rootVisualElement != null) AjustarColores(ui.rootVisualElement);
        }
    }
    
    /// <summary> 
    /// Ajusta colores específicos de botones para mejorar accesibilidad. 
    /// </summary>
    private void AjustarColores(VisualElement root)
    {
        // Colores optimizados para accesibilidad
        Color g = new Color(0.84f, 0.37f, 0f);
        Color s = new Color(0.34f, 0.71f, 0.91f);
        
        // Aplicar colores a botones concretos (ejemplo: mundos)
        root.Q<Button>("Mundo_1")?.style.SetBorderColor(g);
        root.Q<Button>("Mundo_2")?.style.SetBorderColor(s);
    }
}

/// <summary> 
/// Extensión para facilitar el cambio de color de borde en UI Toolkit. 
/// </summary>
public static class UIUtils {
    public static void SetBorderColor(this IStyle style, Color color) 
    {
        style.borderTopColor = 
        style.borderRightColor = 
        style.borderBottomColor = 
        style.borderLeftColor = color;
    }
}