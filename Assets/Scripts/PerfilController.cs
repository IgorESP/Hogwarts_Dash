using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class PerfilUIController : MonoBehaviour
{
    private UIDocument uiDocument;
    private VisualElement root;
    private Button botonGryffindor, botonHufflepuff, botonRavenclaw, botonSlytherin, botonConfirmar, botonVolver;
    private VisualElement iconoCubo, iconoBola, iconoCohete, iconoUFO;
    private string casaSeleccionada = "Gryffindor";
    
    private class AssetsCasa {
        public string cubo, bola, cohete, ufo;
        public AssetsCasa(string c, string b, string co, string u) { cubo = c; bola = b; cohete = co; ufo = u; }
    }
    
    void Start()
    {
        uiDocument = GetComponent<UIDocument>();
        root = uiDocument.rootVisualElement;
        
        // --- APLICAR ACCESIBILIDAD ---
        if (NivelesManager.Instance != null) NivelesManager.Instance.SincronizarAccesibilidad(root);
        
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
        
        botonGryffindor.clicked += () => SeleccionarCasa("Gryffindor");
        botonHufflepuff.clicked += () => SeleccionarCasa("Hufflepuff");
        botonRavenclaw.clicked += () => SeleccionarCasa("Ravenclaw");
        botonSlytherin.clicked += () => SeleccionarCasa("Slytherin");
        botonConfirmar.clicked += OnConfirmarClick;
        botonVolver.clicked += OnVolverClick;
        
        SeleccionarCasa("Gryffindor");
    }
    
    private void SeleccionarCasa(string nombreCasa)
    {
        botonGryffindor.RemoveFromClassList("casa-seleccionada");
        botonHufflepuff.RemoveFromClassList("casa-seleccionada");
        botonRavenclaw.RemoveFromClassList("casa-seleccionada");
        botonSlytherin.RemoveFromClassList("casa-seleccionada");
        root.RemoveFromClassList("color-gryffindor");
        root.RemoveFromClassList("color-hufflepuff");
        root.RemoveFromClassList("color-ravenclaw");
        root.RemoveFromClassList("color-slytherin");
        
        casaSeleccionada = nombreCasa;
        
        switch (nombreCasa) {
            case "Gryffindor":
                botonGryffindor.AddToClassList("casa-seleccionada"); root.AddToClassList("color-gryffindor");
                ActualizarIconos(new AssetsCasa("Assets/ASSETS_CUBOS/CUBO1.png", "Assets/ASSETS_CUBOS/Bola_gryffindor.png", "Assets/ASSETS_CUBOS/Cohete_gryffindor.png", "Assets/ASSETS_CUBOS/UFO_gryffindor.png")); break;
            case "Hufflepuff":
                botonHufflepuff.AddToClassList("casa-seleccionada"); root.AddToClassList("color-hufflepuff");
                ActualizarIconos(new AssetsCasa("Assets/ASSETS_CUBOS/CUBO2.png", "Assets/ASSETS_CUBOS/Bola_hufflepuff.png", "Assets/ASSETS_CUBOS/Cohete_hufflepuff.png", "Assets/ASSETS_CUBOS/UFO_hufflepuff.png")); break;
            case "Ravenclaw":
                botonRavenclaw.AddToClassList("casa-seleccionada"); root.AddToClassList("color-ravenclaw");
                ActualizarIconos(new AssetsCasa("Assets/ASSETS_CUBOS/CUBO3.png", "Assets/ASSETS_CUBOS/Bola_ravenclaw.png", "Assets/ASSETS_CUBOS/Cohete_ravenclaw.png", "Assets/ASSETS_CUBOS/UFO_ravenclaw.png")); break;
            case "Slytherin":
                botonSlytherin.AddToClassList("casa-seleccionada"); root.AddToClassList("color-slytherin");
                ActualizarIconos(new AssetsCasa("Assets/ASSETS_CUBOS/CUBO4.png", "Assets/ASSETS_CUBOS/Bola_slytherin.png", "Assets/ASSETS_CUBOS/Cohete_slytherin.png", "Assets/ASSETS_CUBOS/UFO_slytherin.png")); break;
        }
    }
    
    private void ActualizarIconos(AssetsCasa assets) {
        #if UNITY_EDITOR
        iconoCubo.style.backgroundImage = new StyleBackground(AssetDatabase.LoadAssetAtPath<Texture2D>(assets.cubo));
        iconoBola.style.backgroundImage = new StyleBackground(AssetDatabase.LoadAssetAtPath<Texture2D>(assets.bola));
        iconoCohete.style.backgroundImage = new StyleBackground(AssetDatabase.LoadAssetAtPath<Texture2D>(assets.cohete));
        iconoUFO.style.backgroundImage = new StyleBackground(AssetDatabase.LoadAssetAtPath<Texture2D>(assets.ufo));
        #endif
    }
    
    private void OnConfirmarClick() { PlayerPrefs.SetString("CasaSeleccionada", casaSeleccionada); PlayerPrefs.Save(); SceneManager.LoadScene("PlayScene"); }
    private void OnVolverClick() => SceneManager.LoadScene("MenuPrincipalT");
}