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
        // Obtener referencias
        uiDocument = GetComponent<UIDocument>();
        root = uiDocument.rootVisualElement;
        
        // Obtener botones
        botonAtras = root.Q<Button>("Boton_Atras");
        mundo1 = root.Q<Button>("Mundo_1");
        mundo2 = root.Q<Button>("Mundo_2");
        mundo3 = root.Q<Button>("Mundo_3");
        mundo4 = root.Q<Button>("Mundo_4");
        mundo5 = root.Q<Button>("Mundo_5");
        
        // Obtener label de progreso
        labelProgreso = root.Q<Label>("Progreso");
        
        // Registrar eventos
        botonAtras.clicked += OnAtrasClick;
        mundo1.clicked += () => OnMundoClick(1);
        mundo2.clicked += () => OnMundoClick(2);
        mundo3.clicked += () => OnMundoClick(3);
        mundo4.clicked += () => OnMundoClick(4);
        mundo5.clicked += () => OnMundoClick(5);
        
        // Cargar progreso del jugador
        CargarProgreso();
        
        // Actualizar UI
        ActualizarUI();
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
        labelProgreso.text = $"Progreso: {mundosDesbloqueadosCount}/5 mundos";
        
        // Aplicar estilos según estado
        ActualizarEstadoMundo(mundo1, mundosDesbloqueados[0]);
        ActualizarEstadoMundo(mundo2, mundosDesbloqueados[1]);
        ActualizarEstadoMundo(mundo3, mundosDesbloqueados[2]);
        ActualizarEstadoMundo(mundo4, mundosDesbloqueados[3]);
        ActualizarEstadoMundo(mundo5, mundosDesbloqueados[4]);
    }
    
    private void ActualizarEstadoMundo(Button botonMundo, bool desbloqueado)
    {
        if (desbloqueado)
        {
            botonMundo.SetEnabled(true);
        }
        else
        {
            botonMundo.SetEnabled(false);
        }
    }
    
    private void OnMundoClick(int numeroMundo)
    {
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
        // Cuando crees las escenas reales, cambia esto
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