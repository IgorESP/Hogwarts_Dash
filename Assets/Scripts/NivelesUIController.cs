using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class NivelesUIController : MonoBehaviour
{
    private UIDocument uiDocument;
    private VisualElement root;
    
    // Botones
    private Button botonAtras;
    private Button mundo1;
    private Button mundo2;
    private Button mundo3;
    private Button mundo4;
    private Button mundo5;
    
    // Label de progreso
    private Label labelProgreso;
    
    // Estado de niveles (guarda qué mundos están desbloqueados)
    private bool[] mundosDesbloqueados = new bool[5];
    
    void Start()
    {
        Debug.Log("🎬 NivelesUIController - Start() ejecutándose...");
        
        // Obtener referencias
        uiDocument = GetComponent<UIDocument>();
        
        if (uiDocument == null)
        {
            Debug.LogError("❌ UIDocument no encontrado!");
            return;
        }
        
        root = uiDocument.rootVisualElement;
        
        if (root == null)
        {
            Debug.LogError("❌ Root VisualElement no encontrado!");
            return;
        }
        
        // Obtener botones
        botonAtras = root.Q<Button>("Boton_Atras");
        mundo1 = root.Q<Button>("Mundo_1");
        mundo2 = root.Q<Button>("Mundo_2");
        mundo3 = root.Q<Button>("Mundo_3");
        mundo4 = root.Q<Button>("Mundo_4");
        mundo5 = root.Q<Button>("Mundo_5");
        
        // Validar botones
        if (botonAtras == null) Debug.LogWarning("⚠️ Boton_Atras no encontrado");
        else Debug.Log("✅ Boton_Atras encontrado");
        
        if (mundo1 == null) Debug.LogWarning("⚠️ Mundo_1 no encontrado");
        if (mundo2 == null) Debug.LogWarning("⚠️ Mundo_2 no encontrado");
        if (mundo3 == null) Debug.LogWarning("⚠️ Mundo_3 no encontrado");
        if (mundo4 == null) Debug.LogWarning("⚠️ Mundo_4 no encontrado");
        if (mundo5 == null) Debug.LogWarning("⚠️ Mundo_5 no encontrado");
        
        // Obtener label de progreso
        labelProgreso = root.Q<Label>("Progreso");
        
        if (labelProgreso == null) Debug.LogWarning("⚠️ Label Progreso no encontrado");
        
        // Registrar eventos solo si existen
        if (botonAtras != null)
        {
            botonAtras.clicked += OnAtrasClick;
            Debug.Log("✅ Evento OnAtrasClick registrado");
        }
        
        if (mundo1 != null) mundo1.clicked += () => OnMundoClick(1);
        if (mundo2 != null) mundo2.clicked += () => OnMundoClick(2);
        if (mundo3 != null) mundo3.clicked += () => OnMundoClick(3);
        if (mundo4 != null) mundo4.clicked += () => OnMundoClick(4);
        if (mundo5 != null) mundo5.clicked += () => OnMundoClick(5);
        
        // Cargar progreso del jugador
        CargarProgreso();
        
        // Actualizar UI
        ActualizarUI();
        
        Debug.Log("✅ Pantalla de Niveles cargada correctamente");
    }
    
    private void CargarProgreso()
    {
        // Por defecto, los primeros 3 mundos están desbloqueados
        mundosDesbloqueados[0] = true; // Mundo 1
        mundosDesbloqueados[1] = true; // Mundo 2
        mundosDesbloqueados[2] = true; // Mundo 3
        mundosDesbloqueados[3] = false; // Mundo 4 - Bloqueado
        mundosDesbloqueados[4] = false; // Mundo 5 - Bloqueado
        
        // Cargar desde PlayerPrefs (opcional)
        for (int i = 0; i < mundosDesbloqueados.Length; i++)
        {
            mundosDesbloqueados[i] = PlayerPrefs.GetInt($"Mundo_{i + 1}_Desbloqueado", mundosDesbloqueados[i] ? 1 : 0) == 1;
        }
        
        Debug.Log($"📊 Mundos desbloqueados: 1={mundosDesbloqueados[0]}, 2={mundosDesbloqueados[1]}, 3={mundosDesbloqueados[2]}, 4={mundosDesbloqueados[3]}, 5={mundosDesbloqueados[4]}");
    }
    
    private void ActualizarUI()
    {
        // Contar mundos desbloqueados
        int mundosDesbloqueadosCount = 0;
        foreach (bool desbloqueado in mundosDesbloqueados)
        {
            if (desbloqueado) mundosDesbloqueadosCount++;
        }
        
        // Actualizar texto de progreso
        if (labelProgreso != null)
        {
            labelProgreso.text = $"Progreso: {mundosDesbloqueadosCount}/5 mundos";
        }
        
        // Aplicar estilos según estado
        if (mundo1 != null) ActualizarEstadoMundo(mundo1, mundosDesbloqueados[0]);
        if (mundo2 != null) ActualizarEstadoMundo(mundo2, mundosDesbloqueados[1]);
        if (mundo3 != null) ActualizarEstadoMundo(mundo3, mundosDesbloqueados[2]);
        if (mundo4 != null) ActualizarEstadoMundo(mundo4, mundosDesbloqueados[3]);
        if (mundo5 != null) ActualizarEstadoMundo(mundo5, mundosDesbloqueados[4]);
    }
    
    private void ActualizarEstadoMundo(Button botonMundo, bool desbloqueado)
    {
        if (desbloqueado)
        {
            // Remover clase bloqueado y añadir desbloqueado
            botonMundo.RemoveFromClassList("bloqueado");
            botonMundo.AddToClassList("desbloqueado");
            botonMundo.SetEnabled(true);
        }
        else
        {
            // Remover clase desbloqueado y añadir bloqueado
            botonMundo.RemoveFromClassList("desbloqueado");
            botonMundo.AddToClassList("bloqueado");
            botonMundo.SetEnabled(false);
        }
    }
    
    private void OnMundoClick(int numeroMundo)
    {
        Debug.Log($"🔥🔥🔥 MUNDO {numeroMundo} CLICKEADO 🔥🔥🔥");
        
        // Verificar si el mundo está desbloqueado
        if (!mundosDesbloqueados[numeroMundo - 1])
        {
            Debug.Log($"⚠️ Mundo {numeroMundo} está bloqueado");
            return;
        }
        
        Debug.Log($"🎮 Cargando Mundo {numeroMundo}...");
        
        // Guardar el mundo seleccionado en PlayerPrefs
        PlayerPrefs.SetInt("MundoSeleccionado", numeroMundo);
        PlayerPrefs.Save();
        
        // TEMPORAL: Cargar PlayScene para todos los mundos
        Debug.Log($"⚠️ TEMPORAL: Cargando PlayScene (Mundo {numeroMundo} aún no existe)");
        SceneManager.LoadScene("PlayScene");
        
        /* CUANDO CREES LAS ESCENAS, USA ESTO:
        switch (numeroMundo)
        {
            case 1:
                SceneManager.LoadScene("Mundo1_GranComedor");
                break;
            case 2:
                SceneManager.LoadScene("Mundo2_BosqueProhibido");
                break;
            case 3:
                SceneManager.LoadScene("Mundo3_TorreAstronomia");
                break;
            case 4:
                SceneManager.LoadScene("Mundo4_Mazmorras");
                break;
            case 5:
                SceneManager.LoadScene("Mundo5_CamaraFinal");
                break;
        }
        */
    }
    
    private void OnAtrasClick()
    {
        Debug.Log("🔥🔥🔥 BOTÓN ATRÁS CLICKEADO 🔥🔥🔥");
        Debug.Log("⬅️ Volviendo al menú principal...");
        SceneManager.LoadScene("MenuPrincipalT");
    }
    
    // Método público para desbloquear mundos (llamar desde otros scripts)
    public void DesbloquearMundo(int numeroMundo)
    {
        if (numeroMundo >= 1 && numeroMundo <= 5)
        {
            mundosDesbloqueados[numeroMundo - 1] = true;
            PlayerPrefs.SetInt($"Mundo_{numeroMundo}_Desbloqueado", 1);
            PlayerPrefs.Save();
            ActualizarUI();
            Debug.Log($"✅ Mundo {numeroMundo} desbloqueado!");
        }
    }
}