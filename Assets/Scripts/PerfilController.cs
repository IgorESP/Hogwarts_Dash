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
    
    // Botones de casas
    private Button botonGryffindor;
    private Button botonHufflepuff;
    private Button botonRavenclaw;
    private Button botonSlytherin;
    
    // Otros botones
    private Button botonConfirmar;
    private Button botonVolver;
    
    // Iconos (cubo, bola, cohete, UFO)
    private VisualElement iconoCubo;
    private VisualElement iconoBola;
    private VisualElement iconoCohete;
    private VisualElement iconoUFO;
    
    // Casa seleccionada (Gryffindor por defecto)
    private string casaSeleccionada = "Gryffindor";
    
    // Clase para organizar las rutas de assets de cada casa
    private class AssetsCasa
    {
        public string cubo;
        public string bola;
        public string cohete;
        public string ufo;
        
        public AssetsCasa(string cubo, string bola, string cohete, string ufo)
        {
            this.cubo = cubo;
            this.bola = bola;
            this.cohete = cohete;
            this.ufo = ufo;
        }
    }
    
    void Start()
    {
        // Obtener referencias del UI Document
        uiDocument = GetComponent<UIDocument>();
        root = uiDocument.rootVisualElement;
        
        // Obtener botones de casas
        botonGryffindor = root.Q<Button>("Boton_Gryffindor");
        botonHufflepuff = root.Q<Button>("Boton_Hufflepuff");
        botonRavenclaw = root.Q<Button>("Boton_Ravenclaw");
        botonSlytherin = root.Q<Button>("Boton_Slytherin");
        
        // Obtener otros botones
        botonConfirmar = root.Q<Button>("Boton_Confirmar");
        botonVolver = root.Q<Button>("Boton_Volver");
        
        // Obtener iconos (EN EL ORDEN: cubo, bola, cohete, UFO)
        iconoCubo = root.Q<VisualElement>("Icono_Cubo");
        iconoBola = root.Q<VisualElement>("Icono_Bola");
        iconoCohete = root.Q<VisualElement>("Icono_Cohete");
        iconoUFO = root.Q<VisualElement>("Icono_UFO");
        
        // Registrar eventos de clic para selección de casas
        botonGryffindor.clicked += () => SeleccionarCasa("Gryffindor");
        botonHufflepuff.clicked += () => SeleccionarCasa("Hufflepuff");
        botonRavenclaw.clicked += () => SeleccionarCasa("Ravenclaw");
        botonSlytherin.clicked += () => SeleccionarCasa("Slytherin");
        
        // Registrar eventos de otros botones
        botonConfirmar.clicked += OnConfirmarClick;
        botonVolver.clicked += OnVolverClick;
        
        // Aplicar clases CSS a los botones de casas
        botonGryffindor.AddToClassList("boton-casa");
        botonHufflepuff.AddToClassList("boton-casa");
        botonRavenclaw.AddToClassList("boton-casa");
        botonSlytherin.AddToClassList("boton-casa");
        
        // SELECCIONAR GRYFFINDOR POR DEFECTO AL INICIAR
        SeleccionarCasa("Gryffindor");
    }
    
    private void SeleccionarCasa(string nombreCasa)
    {
        // Limpiar selección anterior de todos los botones
        botonGryffindor.RemoveFromClassList("casa-seleccionada");
        botonHufflepuff.RemoveFromClassList("casa-seleccionada");
        botonRavenclaw.RemoveFromClassList("casa-seleccionada");
        botonSlytherin.RemoveFromClassList("casa-seleccionada");
        
        // Limpiar colores anteriores del root
        root.RemoveFromClassList("color-gryffindor");
        root.RemoveFromClassList("color-hufflepuff");
        root.RemoveFromClassList("color-ravenclaw");
        root.RemoveFromClassList("color-slytherin");
        
        // Guardar casa seleccionada
        casaSeleccionada = nombreCasa;
        
        // Aplicar nueva selección según la casa - NOMBRES CORREGIDOS
        switch (nombreCasa)
        {
            case "Gryffindor":
                botonGryffindor.AddToClassList("casa-seleccionada");
                root.AddToClassList("color-gryffindor");
                ActualizarIconos(new AssetsCasa(
                    "Assets/ASSETS_CUBOS/CUBO1.png",
                    "Assets/ASSETS_BOLA/Bola_4.png",
                    "Assets/ASSETS_COHETO/Cohete4.png",
                    "Assets/ASSETS_UFO/UFO4.png"
                ));
                break;
                
            case "Hufflepuff":
                botonHufflepuff.AddToClassList("casa-seleccionada");
                root.AddToClassList("color-hufflepuff");
                ActualizarIconos(new AssetsCasa(
                    "Assets/ASSETS_CUBOS/CUBO2.png",
                    "Assets/ASSETS_BOLA/Bola_3.png",
                    "Assets/ASSETS_COHETO/Cohete3.png",
                    "Assets/ASSETS_UFO/UFO2.png"
                ));
                break;
                
            case "Ravenclaw":
                botonRavenclaw.AddToClassList("casa-seleccionada");
                root.AddToClassList("color-ravenclaw");
                ActualizarIconos(new AssetsCasa(
                    "Assets/ASSETS_CUBOS/CUBO3.png",
                    "Assets/ASSETS_BOLA/Bola_2.png",
                    "Assets/ASSETS_COHETO/Cohete2.png",
                    "Assets/ASSETS_UFO/UFO3.png"
                ));
                break;
                
            case "Slytherin":
                botonSlytherin.AddToClassList("casa-seleccionada");
                root.AddToClassList("color-slytherin");
                ActualizarIconos(new AssetsCasa(
                    "Assets/ASSETS_CUBOS/CUBO4.png",
                    "Assets/ASSETS_BOLA/Bola_1.png",
                    "Assets/ASSETS_COHETO/Cohete1.png",
                    "Assets/ASSETS_UFO/UFO1.png"
                ));
                break;
        }
        
        Debug.Log($"✅ Casa seleccionada: {nombreCasa}");
    }
    
    private void ActualizarIconos(AssetsCasa assets)
    {
        #if UNITY_EDITOR
        // Cargar los assets usando AssetDatabase (solo funciona en el Editor)
        Texture2D cuboTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(assets.cubo);
        Texture2D bolaTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(assets.bola);
        Texture2D coheteTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(assets.cohete);
        Texture2D ufoTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(assets.ufo);
        
        // Aplicar texturas a los iconos
        if (cuboTexture != null)
        {
            iconoCubo.style.backgroundImage = new StyleBackground(cuboTexture);
            Debug.Log($"✅ Cubo cargado: {assets.cubo}");
        }
        else
            Debug.LogWarning($"⚠️ No se pudo cargar el cubo: {assets.cubo}");
            
        if (bolaTexture != null)
        {
            iconoBola.style.backgroundImage = new StyleBackground(bolaTexture);
            Debug.Log($"✅ Bola cargada: {assets.bola}");
        }
        else
            Debug.LogWarning($"⚠️ No se pudo cargar la bola: {assets.bola}");
            
        if (coheteTexture != null)
        {
            iconoCohete.style.backgroundImage = new StyleBackground(coheteTexture);
            Debug.Log($"✅ Cohete cargado: {assets.cohete}");
        }
        else
            Debug.LogWarning($"⚠️ No se pudo cargar el cohete: {assets.cohete}");
            
        if (ufoTexture != null)
        {
            iconoUFO.style.backgroundImage = new StyleBackground(ufoTexture);
            Debug.Log($"✅ UFO cargado: {assets.ufo}");
        }
        else
            Debug.LogWarning($"⚠️ No se pudo cargar el UFO: {assets.ufo}");
            
        #else
        // En build, necesitarías usar Resources o Addressables
        Debug.LogWarning("⚠️ La carga de assets solo funciona en el Editor de Unity.");
        Debug.LogWarning("Para builds, mueve los assets a una carpeta 'Resources' o usa Addressables.");
        #endif
    }
    
    private void OnConfirmarClick()
    {
        Debug.Log($"🎮 ¡CONFIRMADO! Casa seleccionada: {casaSeleccionada}");
        
        // Guardar la casa en PlayerPrefs para usarla en el juego
        PlayerPrefs.SetString("CasaSeleccionada", casaSeleccionada);
        PlayerPrefs.Save();
        
        Debug.Log("💾 Casa guardada en PlayerPrefs. Cargando PlayScene...");
        
        // Cargar la escena del juego
        SceneManager.LoadScene("PlayScene");
    }
    
    private void OnVolverClick()
    {
        Debug.Log("⬅️ Volviendo al menú principal...");
        SceneManager.LoadScene("MenuPrincipalT");
    }
}