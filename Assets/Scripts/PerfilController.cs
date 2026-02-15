using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Gestiona la selección de casa y personalización visual del personaje
/// </summary>
public class PerfilUIController : MonoBehaviour
{
    private UIDocument uiDocument;
    private VisualElement root;

    // Botones de selección de casa
    private Button botonGryffindor, botonHufflepuff, botonRavenclaw, botonSlytherin, botonConfirmar, botonVolver;

    // Botones de navegación
    private VisualElement iconoCubo, iconoBola, iconoCohete, iconoUFO;

    // Iconos de previsualización del personaje
    private string casaSeleccionada = "Gryffindor";
    
    /// <summary> 
    /// Clase auxiliar para agrupar rutas de assets según la casa. 
    /// </summary>
    private class AssetsCasa {
        public string cubo, bola, cohete, ufo;
        public AssetsCasa(string c, string b, string co, string u) { cubo = c; bola = b; cohete = co; ufo = u; }
    }
    
    void Start()
    {
        uiDocument = GetComponent<UIDocument>();
        if (uiDocument != null) root = uiDocument.rootVisualElement;
        
        // Aplicar accesibilidad global (tamaño de letra, modo daltónico)
        if (NivelesManager.Instance != null) NivelesManager.Instance.SincronizarAccesibilidad(root);
        
        // Buscar elementos del UXML
        ObtenerReferencias();

        // Registrar eventos de botones
        RegistrarEventos();
        
        // Cargar casa por defecto
        SeleccionarCasa("Gryffindor");

        AñadirSonidoABotones();
    }

    /// <summary> 
    /// Obtiene referencias a todos los elementos del UXML. 
    /// </summary>
    private void ObtenerReferencias()
    {
        if (root == null) return;

        botonGryffindor = root.Q<Button>("Boton_Gryffindor");
        botonHufflepuff = root.Q<Button>("Boton_Hufflepuff");
        botonRavenclaw = root.Q<Button>("Boton_Ravenclaw");
        botonSlytherin = root.Q<Button>("Boton_Slytherin");

        botonConfirmar = root.Q<Button>("Boton_Confirmar");
        botonVolver = root.Q<Button>("Boton_Volver");

        iconoCubo = root.Q<VisualElement>("Icono_Cubo");
        iconoBola = root.Q<VisualElement>("Icono_Bola");
        iconoCohete = root.Q<VisualElement>("Icono_Cohete");
        iconoUFO = root.Q<VisualElement>("Icono_UFO");
    }

    /// <summary> 
    /// Registra los eventos de los botones. 
    /// </summary>
    private void RegistrarEventos()
    {
        botonGryffindor.clicked += () => SeleccionarCasa("Gryffindor");
        botonHufflepuff.clicked += () => SeleccionarCasa("Hufflepuff");
        botonRavenclaw.clicked += () => SeleccionarCasa("Ravenclaw");
        botonSlytherin.clicked += () => SeleccionarCasa("Slytherin");

        botonConfirmar.clicked += OnConfirmarClick;
        botonVolver.clicked += OnVolverClick;
    }
    
    /// <summary>
    /// Cambia visualmente la casa seleccionada y actualiza los iconos de previsualización
    /// </summary>
    private void SeleccionarCasa(string nombreCasa)
    {
        Debug.Log($"[Perfil] Casa seleccionada: {nombreCasa}");
        
        // Limpiar estilos previos
        botonGryffindor.RemoveFromClassList("casa-seleccionada");
        botonHufflepuff.RemoveFromClassList("casa-seleccionada");
        botonRavenclaw.RemoveFromClassList("casa-seleccionada");
        botonSlytherin.RemoveFromClassList("casa-seleccionada");
        
        casaSeleccionada = nombreCasa;
        
        // Aplicar nuevos estilos e iconos según la casa
        switch (nombreCasa)
        {
            case "Gryffindor":
                botonGryffindor.AddToClassList("casa-seleccionada");
                ActualizarIconos(new AssetsCasa("Assets/ASSETS_CUBOS/CUBO1.png", "Assets/ASSETS_CUBOS/Bola_gryffindor.png", "Assets/ASSETS_CUBOS/Cohete_gryffindor.png", "Assets/ASSETS_CUBOS/UFO_gryffindor.png")); 
                break;
            case "Hufflepuff":
                botonHufflepuff.AddToClassList("casa-seleccionada");
                ActualizarIconos(new AssetsCasa("Assets/ASSETS_CUBOS/CUBO2.png", "Assets/ASSETS_CUBOS/Bola_hufflepuff.png", "Assets/ASSETS_CUBOS/Cohete_hufflepuff.png", "Assets/ASSETS_CUBOS/UFO_hufflepuff.png")); 
                break;
            case "Ravenclaw":
                botonRavenclaw.AddToClassList("casa-seleccionada");
                ActualizarIconos(new AssetsCasa("Assets/ASSETS_CUBOS/CUBO3.png", "Assets/ASSETS_CUBOS/Bola_ravenclaw.png", "Assets/ASSETS_CUBOS/Cohete_ravenclaw.png", "Assets/ASSETS_CUBOS/UFO_ravenclaw.png")); 
                break;
            case "Slytherin":
                botonSlytherin.AddToClassList("casa-seleccionada");
                ActualizarIconos(new AssetsCasa("Assets/ASSETS_CUBOS/CUBO4.png", "Assets/ASSETS_CUBOS/Bola_slytherin.png", "Assets/ASSETS_CUBOS/Cohete_slytherin.png", "Assets/ASSETS_CUBOS/UFO_slytherin.png")); 
                break;
        }
    }
    
    /// <summary>
    /// Carga las texturas de previsualización de los modos de juego (Solo editor para previsualización directa)
    /// </summary>
    private void ActualizarIconos(AssetsCasa assets) {
        #if UNITY_EDITOR
        iconoCubo.style.backgroundImage = new StyleBackground(AssetDatabase.LoadAssetAtPath<Texture2D>(assets.cubo));
        iconoBola.style.backgroundImage = new StyleBackground(AssetDatabase.LoadAssetAtPath<Texture2D>(assets.bola));
        iconoCohete.style.backgroundImage = new StyleBackground(AssetDatabase.LoadAssetAtPath<Texture2D>(assets.cohete));
        iconoUFO.style.backgroundImage = new StyleBackground(AssetDatabase.LoadAssetAtPath<Texture2D>(assets.ufo));
        #endif
    }
    
    /// <summary> 
    /// Guarda la casa seleccionada y navega a la escena de juego. 
    //// </summary>
    private void OnConfirmarClick() { 
        Debug.Log($"[Perfil] Guardando casa {casaSeleccionada} y entrando al nivel");

        PlayerPrefs.SetString("CasaSeleccionada", casaSeleccionada); 
        PlayerPrefs.Save(); 

        SceneManager.LoadScene("PlayScene"); 
    }

    /// <summary> 
    /// Vuelve al menú principal. 
    /// </summary>
    private void OnVolverClick() => SceneManager.LoadScene("MenuPrincipalT");

    /// <summary> 
    /// Añade sonido de clic a todos los botones del menú cambiar skins. 
    /// </summary>
    private void AñadirSonidoABotones()
    {
        if (root == null) return;

        var botones = root.Query<Button>().ToList();

        foreach (var b in botones)
        {
            b.clicked += () => AudioManager.Instance.PlayBoton();
        }
    }
}