using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

/// <summary>
/// Controlador visual para la selección de mundos
/// </summary>
public class NivelesUIController : MonoBehaviour
{
    private VisualElement root;
    private Button botonMundo1, botonMundo2, botonMundo3, botonMundo4, botonMundo5, botonAtras, botonReset;
    private Label labelProgreso;
 
    // Array que mapea cada mundo con su escena correspondiente
    private readonly string[] nombresEscenas = new string[] 
    { 
        "", "EscenaComedor", "EscenaGryffindor", "EscenaSlytherin", "EscenaHufflepuff", "EscenaRavenclaw" 
    };
 
    void Start()
    {
        // Obtener el root del UXML
        root = GetComponent<UIDocument>().rootVisualElement;
        
        // Aplicar accesibilidad global (modo daltónico, tamaño de letra, etc.)
        if (NivelesManager.Instance != null) 
            NivelesManager.Instance.SincronizarAccesibilidad(root);
    
        // Buscar elementos del UXML
        ObtenerReferencias();

        // Registrar eventos de botones
        ConfigurarEventos();

        // Actualizar estados visuales según progreso
        ActualizarUI();

        AñadirSonidoABotones();
    }
    
    /// <summary> 
    /// Obtiene referencias a los botones y labels del UXML. 
    /// </summary>
    private void ObtenerReferencias() 
    {
        botonMundo1 = root.Q<Button>("Mundo_1");
        botonMundo2 = root.Q<Button>("Mundo_2");
        botonMundo3 = root.Q<Button>("Mundo_3");
        botonMundo4 = root.Q<Button>("Mundo_4");
        botonMundo5 = root.Q<Button>("Mundo_5");

        botonAtras = root.Q<Button>("Boton_Atras");
        botonReset = root.Q<Button>("Boton_Reset_Progreso");
        labelProgreso = root.Q<Label>("Progreso");
    }
    
    /// <summary> 
    /// Registra los eventos de clic para cada botón. 
    /// </summary>
    private void ConfigurarEventos() 
    {
        botonMundo1.clicked += () => SeleccionarMundo(1);
        botonMundo2.clicked += () => SeleccionarMundo(2);
        botonMundo3.clicked += () => SeleccionarMundo(3);
        botonMundo4.clicked += () => SeleccionarMundo(4);
        botonMundo5.clicked += () => SeleccionarMundo(5);

        if (botonAtras != null) 
            botonAtras.clicked += () =>
                {
                    if (AudioManager.Instance != null)
                        AudioManager.Instance.musicaSource.Stop();
                    SceneManager.LoadScene("MenuPrincipalT");
                };

        if (botonReset != null)
            botonReset.clicked += () => 
            {
                if (NivelesManager.Instance != null)
                {
                    NivelesManager.Instance.ResetearProgreso();
                    ActualizarUI();
                    Debug.Log("[NivelesUI] Progreso reseteado por el usuario.");
                }
            };
    }
    
    /// <summary> 
    /// Actualiza la UI completa: estados de mundos, estrellas y progreso general. 
    /// </summary>
    private void ActualizarUI() 
    {
        for (int i = 1; i <= 5; i++)
        {
            Button b = root.Q<Button>($"Mundo_{i}");
            bool unlocked = NivelesManager.Instance.MundoEstaDesbloqueado(i);
            Debug.Log($"[NivelesUI] Mundo {i} estado: {(unlocked ? "DESBLOQUEADO" : "BLOQUEADO")}");
            ActualizarMundo(i, b);
        }
        ActualizarProgresoGeneral();
    }
    
    /// <summary> 
    /// Actualiza el estado visual de un mundo concreto: 
    /// - Bloqueado / desbloqueado 
    /// - Estrellas obtenidas 
    /// - Porcentaje completado 
    /// </summary>
    private void ActualizarMundo(int num, Button b) 
    {
        if (b == null) return;
        
        bool unlocked = NivelesManager.Instance.MundoEstaDesbloqueado(num);

        // Activar o desactivar el botón según si está desbloqueado
        b.SetEnabled(unlocked);

        // Aplicar clases CSS para estilos visuales
        b.EnableInClassList("bloqueado", !unlocked);
        b.EnableInClassList("desbloqueado", unlocked);
        
        // Actualizar estrellas y porcentaje dentro de la tarjeta del mundo
        int snitches = NivelesManager.Instance.ObtenerSnitchesTotalMundo(num);
        float pRecord = NivelesManager.Instance.ObtenerPorcentajeMundo(num);
        
        Debug.Log($"[NivelesUI] Mundo {num} - Récord leido: {pRecord}% - Snitches: {snitches}");

        b.Query<Label>().ForEach(l => 
        {
            // Actualizar snitches (ej: "2/3")
            if (l.text.Contains("/3")) 
                l.text = $"{snitches}/3";
            
            // Actualizar porcentaje real (ej: "54%")
            else if (l.text.Contains("%")) 
                l.text = $"{Mathf.RoundToInt(pRecord)}%";
        });
    }
    
    /// <summary> 
    /// Calcula cuántos mundos tienen al menos 1 estrella y actualiza el texto de progreso. 
    /// </summary>
    private void ActualizarProgresoGeneral() 
    {
        int completados = 0;

        for (int i = 1; i <= 5; i++) 
            if (NivelesManager.Instance.MundoEstaCompletado(i)) 
                completados++;
        
        if (labelProgreso != null) 
            labelProgreso.text = $"Progreso: {completados}/5 mundos";
    }
    
    /// <summary> 
    /// Guarda el mundo seleccionado y carga la escena correspondiente. 
    /// </summary>
    private void SeleccionarMundo(int num) 
    {
        Debug.Log($"[NivelesUI] Cargando: {nombresEscenas[num]}");

        PlayerPrefs.SetInt("MundoSeleccionado", num);
        PlayerPrefs.Save();

        if (ProgressManager.Instance != null)
            ProgressManager.Instance.IniciarPartida();

        // Música segun el mundo
        switch (num) 
        { 
            case 1: 
                AudioManager.Instance.ReproducirMusicaComedor(); 
                break; 

            case 2: 
                AudioManager.Instance.ReproducirMusicaGryffindor(); 
                break; 

            case 3: 
                AudioManager.Instance.ReproducirMusicaSlytherin(); 
                break; 
            
            case 4: 
                AudioManager.Instance.ReproducirMusicaHufflepuff(); 
                break; 
            
            case 5: 
                AudioManager.Instance.ReproducirMusicaRavenclaw(); 
                break; 
        }

        SceneManager.LoadScene(nombresEscenas[num]);
    }
    
    /// <summary> 
    /// Permite refrescar la UI manualmente pulsando F5 (útil en desarrollo). 
    /// </summary>
    void Update() 
    { 
        if (Input.GetKeyDown(KeyCode.F5)) 
            ActualizarUI();

        // DEPURACIÓN: Borrar récord del mundo 1 con la tecla DELETE (Supr)
        if (Input.GetKeyDown(KeyCode.Delete))
        {
            NivelesManager.Instance.LimpiarRécordNivel(1, 1);
            ActualizarUI();
        }
    }

    /// <summary> 
    /// Añade sonido de clic a todos los botones del menú de niveles. 
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