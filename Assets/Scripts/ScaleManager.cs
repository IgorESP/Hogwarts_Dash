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
        else { Destroy(gameObject); return; }
    }
    
    void OnEnable() { SceneManager.sceneLoaded += (s, m) => StartCoroutine(RefrescarAjustes()); }
    
    private IEnumerator RefrescarAjustes()
    {
        yield return new WaitForEndOfFrame();
        AplicarTodo();
    }
    
    public void AplicarTodo()
    {
        AplicarEscalaTexto();
        AplicarModoDaltonico();
    }
    
    private void AplicarEscalaTexto()
    {
        float escala = PlayerPrefs.GetFloat("EscalaTexto", 1f);
        if (escala == 1f) return;
        
        Debug.Log($"[ScaleManager] Escalando fuentes al {escala * 100}%");
        
        foreach (UIDocument ui in FindObjectsByType<UIDocument>(FindObjectsSortMode.None))
        {
            if (ui.rootVisualElement != null)
            {
                ui.rootVisualElement.Query<Label>().ForEach(l => {
                    if (l.resolvedStyle.fontSize != 0) l.style.fontSize = l.resolvedStyle.fontSize * escala;
                });
            }
        }
    }
    
    private void AplicarModoDaltonico()
    {
        if (PlayerPrefs.GetInt("ModoDaltonico", 0) != 1) return;
        
        Debug.Log("[ScaleManager] Inyectando paleta daltónica");
        foreach (UIDocument ui in FindObjectsByType<UIDocument>(FindObjectsSortMode.None))
        {
            if (ui.rootVisualElement != null) AjustarColores(ui.rootVisualElement);
        }
    }
    
    private void AjustarColores(VisualElement root)
    {
        // Colores optimizados para accesibilidad
        Color g = new Color(0.84f, 0.37f, 0f); // Naranja
        Color s = new Color(0.34f, 0.71f, 0.91f); // Azul cielo
        
        root.Q<Button>("Mundo_1")?.style.SetBorderColor(g);
        root.Q<Button>("Mundo_2")?.style.SetBorderColor(s);
    }
}

public static class UIUtils {
    public static void SetBorderColor(this IStyle style, Color color) {
        style.borderTopColor = style.borderRightColor = style.borderBottomColor = style.borderLeftColor = color;
    }
}