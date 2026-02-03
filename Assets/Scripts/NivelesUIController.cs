using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

/// <summary>
/// Controlador de la interfaz de seleccion de mundos/niveles.
/// 5 MUNDOS: Comedor -> Gryffindor -> Slytherin -> Hufflepuff -> Ravenclaw
/// </summary>
public class NivelesUIController : MonoBehaviour
{
    // Referencias a elementos de la UI
    private VisualElement root;
    private Button botonMundo1, botonMundo2, botonMundo3, botonMundo4, botonMundo5, botonAtras;
    private Label labelProgreso;
 
    // ============================================
    // ORDEN CORRECTO DE LOS 5 MUNDOS
    // ============================================
    // Mundo 1: EscenaComedor (PRIMER NIVEL)
    // Mundo 2: EscenaGryffindor
    // Mundo 3: EscenaSlytherin
    // Mundo 4: EscenaHufflepuff
    // Mundo 5: EscenaRavenclaw (ULTIMO NIVEL)
    // ============================================
    private readonly string[] nombresEscenas = new string[] 
    { 
        "",                     // Indice 0 - vacio
        "EscenaComedor",        // Mundo 1 - Gran Comedor (PRIMER NIVEL)
        "EscenaGryffindor",     // Mundo 2 - Gryffindor
        "EscenaSlytherin",      // Mundo 3 - Slytherin
        "EscenaHufflepuff",     // Mundo 4 - Hufflepuff
        "EscenaRavenclaw"       // Mundo 5 - Ravenclaw (ULTIMO NIVEL)
    };
 
    void Start()
    {
        root = GetComponent<UIDocument>().rootVisualElement;
        
        // Aplicar configuracion de accesibilidad
        if (NivelesManager.Instance != null) 
        {
            NivelesManager.Instance.SincronizarAccesibilidad(root);
        }
 
        ObtenerReferencias();
        ConfigurarEventos();
        ActualizarUI();
    }
 
    /// <summary>
    /// Obtiene las referencias a todos los botones y labels de la UI.
    /// </summary>
    private void ObtenerReferencias() 
    {
        // Obtener los 5 botones de mundos
        botonMundo1 = root.Q<Button>("Mundo_1");
        botonMundo2 = root.Q<Button>("Mundo_2");
        botonMundo3 = root.Q<Button>("Mundo_3");
        botonMundo4 = root.Q<Button>("Mundo_4");
        botonMundo5 = root.Q<Button>("Mundo_5");
        
        // Boton de volver atras
        botonAtras = root.Q<Button>("Boton_Atras");
        
        // Label que muestra el progreso total
        labelProgreso = root.Q<Label>("Progreso");
    }
 
    /// <summary>
    /// Configura los eventos de clic de todos los botones.
    /// </summary>
    private void ConfigurarEventos() 
    {
        // Asignar eventos de clic a cada boton de mundo
        botonMundo1.clicked += () => SeleccionarMundo(1);
        botonMundo2.clicked += () => SeleccionarMundo(2);
        botonMundo3.clicked += () => SeleccionarMundo(3);
        botonMundo4.clicked += () => SeleccionarMundo(4);
        botonMundo5.clicked += () => SeleccionarMundo(5);
        
        // Evento del boton atras
        if (botonAtras != null) 
        {
            botonAtras.clicked += VolverAlMenu;
        }
    }
 
    /// <summary>
    /// Actualiza toda la interfaz: estados de mundos y progreso general.
    /// </summary>
    private void ActualizarUI() 
    {
        // Actualizar cada uno de los 5 mundos
        ActualizarMundo(1, botonMundo1);
        ActualizarMundo(2, botonMundo2);
        ActualizarMundo(3, botonMundo3);
        ActualizarMundo(4, botonMundo4);
        ActualizarMundo(5, botonMundo5);
        
        // Actualizar el label de progreso general
        ActualizarProgresoGeneral();
    }
 
    /// <summary>
    /// Actualiza el estado visual de un mundo especifico.
    /// Cambia clases CSS segun este desbloqueado o no.
    /// </summary>
    /// <param name="numeroMundo">Numero del mundo (1-5)</param>
    /// <param name="boton">Boton del mundo en la UI</param>
    private void ActualizarMundo(int numeroMundo, Button boton) 
    {
        if (boton == null) return;
        
        // Consultar estado del mundo en NivelesManager
        bool estaDesbloqueado = NivelesManager.Instance.MundoEstaDesbloqueado(numeroMundo);
        int estrellas = NivelesManager.Instance.ObtenerEstrellasTotalMundo(numeroMundo);
        float porcentaje = NivelesManager.Instance.ObtenerPorcentajeMundo(numeroMundo);
 
        // Actualizar clases CSS segun el estado
        if (estaDesbloqueado) 
        {
            // Mundo desbloqueado: colores normales, clickeable
            boton.RemoveFromClassList("bloqueado");
            boton.AddToClassList("desbloqueado");
            boton.SetEnabled(true);
        } 
        else 
        {
            // Mundo bloqueado: opaco, no clickeable
            boton.RemoveFromClassList("desbloqueado");
            boton.AddToClassList("bloqueado");
            boton.SetEnabled(false);
        }
        
        // Actualizar textos de estrellas y porcentaje
        ActualizarEstadisticasEnTarjeta(boton, estrellas, porcentaje, estaDesbloqueado);
    }
 
    /// <summary>
    /// Actualiza los textos de estrellas y porcentaje dentro de una tarjeta de mundo.
    /// MAXIMO 3 ESTRELLAS POR MUNDO.
    /// </summary>
    /// <param name="boton">Boton del mundo</param>
    /// <param name="estrellas">Estrellas obtenidas (0-3)</param>
    /// <param name="porcentaje">Porcentaje de completado (0-100)</param>
    /// <param name="desbloqueado">Si el mundo esta desbloqueado</param>
    private void ActualizarEstadisticasEnTarjeta(Button boton, int estrellas, float porcentaje, bool desbloqueado) 
    {
        // Obtener todos los labels dentro del boton
        var labels = boton.Query<Label>().ToList();
        
        // Recorrer y actualizar segun el contenido
        foreach (var label in labels) 
        {
            // Actualizar label de estrellas (busca "/3" o "/30")
            if (label.text.Contains("/3") || label.text.Contains("/30")) 
            {
                // MAXIMO 3 ESTRELLAS POR MUNDO
                label.text = desbloqueado ? $"{estrellas}/3" : "0/3";
            }
            // Actualizar label de porcentaje (busca "%")
            else if (label.text.Contains("%")) 
            {
                label.text = desbloqueado ? $"{porcentaje:F0}%" : "0%";
            }
        }
    }
 
    /// <summary>
    /// Actualiza el label de progreso general mostrando cuantos mundos están COMPLETADOS.
    /// Progreso: 0/5 -> 1/5 -> 2/5 -> 3/5 -> 4/5 -> 5/5
    /// </summary>
    private void ActualizarProgresoGeneral() 
    {
        if (labelProgreso == null) return;
        
        // Contar mundos COMPLETADOS (no desbloqueados)
        int mundosCompletados = 0;
        for (int i = 1; i <= 5; i++) 
        {
            // Un mundo está completado si tiene al menos 1 estrella
            if (NivelesManager.Instance.ObtenerEstrellasTotalMundo(i) > 0) 
            {
                mundosCompletados++;
            }
        }
        
        // Actualizar texto: 0/5, 1/5, 2/5, 3/5, 4/5, 5/5
        labelProgreso.text = $"Progreso: {mundosCompletados}/5 mundos";
        
        Debug.Log($"[NivelesUI] Progreso actualizado: {mundosCompletados}/5 mundos completados");
    }
 
    /// <summary>
    /// Se ejecuta cuando el jugador hace clic en un mundo.
    /// Guarda el mundo seleccionado y carga su escena.
    /// </summary>
    /// <param name="numeroMundo">Numero del mundo seleccionado (1-5)</param>
    private void SeleccionarMundo(int numeroMundo) 
    {
        // Verificar que el mundo este desbloqueado
        if (!NivelesManager.Instance.MundoEstaDesbloqueado(numeroMundo)) 
        {
            Debug.Log($"[NivelesUI] Mundo {numeroMundo} esta bloqueado");
            return;
        }
        
        // Guardar el mundo seleccionado en PlayerPrefs
        PlayerPrefs.SetInt("MundoSeleccionado", numeroMundo);
        PlayerPrefs.Save();
        
        Debug.Log($"[NivelesUI] Cargando mundo {numeroMundo}: {nombresEscenas[numeroMundo]}");
        
        // Cargar la escena del mundo
        SceneManager.LoadScene(nombresEscenas[numeroMundo]);
    }
 
    /// <summary>
    /// Vuelve al menu principal.
    /// </summary>
    private void VolverAlMenu() 
    {
        Debug.Log("[NivelesUI] Volviendo al menu principal");
        SceneManager.LoadScene("MenuPrincipalT");
    }
 
    /// <summary>
    /// Update se ejecuta cada frame.
    /// F5 actualiza manualmente la UI (util para testing).
    /// </summary>
    void Update() 
    { 
        // Presionar F5 para refrescar la UI manualmente
        if (Input.GetKeyDown(KeyCode.F5)) 
        {
            Debug.Log("[NivelesUI] Refrescando UI manualmente (F5)");
            ActualizarUI();
        }
    }
}