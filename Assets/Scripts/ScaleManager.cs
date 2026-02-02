using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using System.Collections;

public class ScaleManager : MonoBehaviour
{
    private static ScaleManager instance;
    
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("ScaleManager inicializado");
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    
    void Start()
    {
        AplicarConfiguracion();
    }
    
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    
    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"🔄 Escena cargada: {scene.name}");
        StartCoroutine(AplicarConfiguracionConRetraso());
    }
    
    private IEnumerator AplicarConfiguracionConRetraso()
    {
        yield return new WaitForEndOfFrame();
        AplicarConfiguracion();
    }
    
    public void AplicarConfiguracion()
    {
        AplicarEscalaTexto();
        AplicarColoresDaltonicos();
    }
    
    private void AplicarEscalaTexto()
    {
        float escala = PlayerPrefs.GetFloat("EscalaTexto", 1f);
        
        if (escala == 1f)
        {
            Debug.Log("Tamaño de texto: 100%");
            return;
        }
        
        Debug.Log($"Aplicando tamaño: {escala * 100}%");
        
        UIDocument[] todosLosUIDocuments = GameObject.FindObjectsByType<UIDocument>(FindObjectsSortMode.None);
        
        foreach (UIDocument uiDoc in todosLosUIDocuments)
        {
            if (uiDoc.rootVisualElement != null)
            {
                var labels = uiDoc.rootVisualElement.Query<Label>().ToList();
                
                foreach (Label label in labels)
                {
                    if (label.resolvedStyle.fontSize != 0)
                    {
                        float tamañoActual = label.resolvedStyle.fontSize;
                        float nuevoTamaño = tamañoActual * escala;
                        label.style.fontSize = nuevoTamaño;
                    }
                }
                
                Debug.Log($"  {labels.Count} textos escalados");
            }
        }
    }
    
    private void AplicarColoresDaltonicos()
    {
        bool modoDaltonico = PlayerPrefs.GetInt("ModoDaltonico", 0) == 1;
        
        if (!modoDaltonico)
        {
            Debug.Log(" Colores: Normales");
            return;
        }
        
        Debug.Log(" Modo DALTÓNICO activo");
        
        UIDocument[] todosLosUIDocuments = GameObject.FindObjectsByType<UIDocument>(FindObjectsSortMode.None);
        
        foreach (UIDocument uiDoc in todosLosUIDocuments)
        {
            if (uiDoc.rootVisualElement != null)
            {
                AplicarColoresUniversales(uiDoc.rootVisualElement);
            }
        }
    }
    
    private void AplicarColoresUniversales(VisualElement root)
    {
        Button mundo1 = root.Q<Button>("Mundo_1");
        if (mundo1 != null)
        {
            Color colorGryffindor = new Color(0.84f, 0.37f, 0f);
            mundo1.style.borderTopColor = colorGryffindor;
            mundo1.style.borderRightColor = colorGryffindor;
            mundo1.style.borderBottomColor = colorGryffindor;
            mundo1.style.borderLeftColor = colorGryffindor;
            Debug.Log("   Mundo 1 → Naranja");
        }
        
        Button mundo2 = root.Q<Button>("Mundo_2");
        if (mundo2 != null)
        {
            Color colorSlytherin = new Color(0.34f, 0.71f, 0.91f);
            mundo2.style.borderTopColor = colorSlytherin;
            mundo2.style.borderRightColor = colorSlytherin;
            mundo2.style.borderBottomColor = colorSlytherin;
            mundo2.style.borderLeftColor = colorSlytherin;
            Debug.Log("   Mundo 2 → Azul celeste");
        }
        
        Button mundo3 = root.Q<Button>("Mundo_3");
        if (mundo3 != null)
        {
            Color colorRavenclaw = new Color(0f, 0.45f, 0.7f);
            mundo3.style.borderTopColor = colorRavenclaw;
            mundo3.style.borderRightColor = colorRavenclaw;
            mundo3.style.borderBottomColor = colorRavenclaw;
            mundo3.style.borderLeftColor = colorRavenclaw;
            Debug.Log("   Mundo 3 → Azul fuerte");
        }
        
        Button mundo4 = root.Q<Button>("Mundo_4");
        if (mundo4 != null)
        {
            Color colorHufflepuff = new Color(0.94f, 0.89f, 0.26f);
            mundo4.style.borderTopColor = colorHufflepuff;
            mundo4.style.borderRightColor = colorHufflepuff;
            mundo4.style.borderBottomColor = colorHufflepuff;
            mundo4.style.borderLeftColor = colorHufflepuff;
            Debug.Log("   Mundo 4 → Amarillo");
        }
    }
    
    public static Color ObtenerColorCasa(string casa)
    {
        bool modoDaltonico = PlayerPrefs.GetInt("ModoDaltonico", 0) == 1;
        
        if (!modoDaltonico)
        {
            switch (casa.ToLower())
            {
                case "gryffindor":
                    return new Color(0.8f, 0f, 0f);
                case "slytherin":
                    return new Color(0.1f, 0.28f, 0.16f);
                case "ravenclaw":
                    return new Color(0.05f, 0.1f, 0.25f);
                case "hufflepuff":
                    return new Color(0.93f, 0.73f, 0.22f);
                default:
                    return Color.white;
            }
        }
        else
        {
            switch (casa.ToLower())
            {
                case "gryffindor":
                    return new Color(0.84f, 0.37f, 0f);
                case "slytherin":
                    return new Color(0.34f, 0.71f, 0.91f);
                case "ravenclaw":
                    return new Color(0f, 0.45f, 0.7f);
                case "hufflepuff":
                    return new Color(0.94f, 0.89f, 0.26f);
                default:
                    return Color.white;
            }
        }
    }
    
    public static void Recalcular()
    {
        if (instance != null)
        {
            instance.AplicarConfiguracion();
        }
    }
}