using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class NivelesUIController : MonoBehaviour
{
    private VisualElement root;
 
    private Button botonMundo1;
    private Button botonMundo2;
    private Button botonMundo3;
    private Button botonMundo4;
    private Button botonMundo5;
 
    private Button botonAtras;
    private Label labelProgreso;
 
    private readonly string[] nombresEscenas = new string[]
    {
        "",  
        "EscenaGryffindor",    
        "EscenaSlytherin",     
        "EscenaRavenclaw",     
        "EscenaHufflepuff",    
        "EscenaFinal"          
    };
 
    void Start()
    {
        root = GetComponent<UIDocument>().rootVisualElement;
 
        ObtenerReferencias();
 
        ConfigurarEventos();
 
        ActualizarUI();
 
        Debug.Log(" NivelesUIController inicializado");
    }
 
    private void ObtenerReferencias()
    {
        botonMundo1 = root.Q<Button>("Mundo_1");
        botonMundo2 = root.Q<Button>("Mundo_2");
        botonMundo3 = root.Q<Button>("Mundo_3");
        botonMundo4 = root.Q<Button>("Mundo_4");
        botonMundo5 = root.Q<Button>("Mundo_5");
        botonAtras = root.Q<Button>("Boton_Atras");
        labelProgreso = root.Q<Label>("Progreso");
 
        if (botonMundo1 == null) Debug.LogError(" No se encontró Mundo_1");
        if (botonMundo2 == null) Debug.LogError("No se encontró Mundo_2");
        if (botonMundo3 == null) Debug.LogError("No se encontró Mundo_3");
        if (botonMundo4 == null) Debug.LogError("No se encontró Mundo_4");
        if (botonMundo5 == null) Debug.LogError("No se encontró Mundo_5");
    }
 
    private void ConfigurarEventos()
    {
        botonMundo1.clicked += () => SeleccionarMundo(1);
        botonMundo2.clicked += () => SeleccionarMundo(2);
        botonMundo3.clicked += () => SeleccionarMundo(3);
        botonMundo4.clicked += () => SeleccionarMundo(4);
        botonMundo5.clicked += () => SeleccionarMundo(5);
 
        if (botonAtras != null)
        {
            botonAtras.clicked += VolverAlMenu;
        }
    }
 
    private void ActualizarUI()
    {
        Debug.Log("Actualizando UI de niveles...");
 
        ActualizarMundo(1, botonMundo1);
        ActualizarMundo(2, botonMundo2);
        ActualizarMundo(3, botonMundo3);
        ActualizarMundo(4, botonMundo4);
        ActualizarMundo(5, botonMundo5);
 
        ActualizarProgresoGeneral();
    }
 
    private void ActualizarMundo(int numeroMundo, Button boton)
    {
        if (boton == null) return;
 
        bool estaDesbloqueado = NivelesManager.Instance.MundoEstaDesbloqueado(numeroMundo);
 
        int estrellas = NivelesManager.Instance.ObtenerEstrellasTotalMundo(numeroMundo);
        float porcentaje = NivelesManager.Instance.ObtenerPorcentajeMundo(numeroMundo);
 
        Debug.Log($"Mundo {numeroMundo}: Desbloqueado={estaDesbloqueado}, Estrellas={estrellas}, Porcentaje={porcentaje}%");
 
        if (estaDesbloqueado)
        {
            boton.RemoveFromClassList("bloqueado");
            boton.AddToClassList("desbloqueado");
            boton.SetEnabled(true);
        }
        else
        {
            boton.RemoveFromClassList("desbloqueado");
            boton.AddToClassList("bloqueado");
            boton.SetEnabled(false);
        }
 
        ActualizarEstadisticasEnTarjeta(boton, estrellas, porcentaje, estaDesbloqueado);
    }
 
    private void ActualizarEstadisticasEnTarjeta(Button boton, int estrellas, float porcentaje, bool desbloqueado)
    {
        var labels = boton.Query<Label>().ToList();
 
        foreach (var label in labels)
        {
            if (label.text.Contains("/3"))
            {
                label.text = desbloqueado ? $"{estrellas}/3" : "0/3";
            }
            else if (label.text.Contains("%"))
            {
                label.text = desbloqueado ? $"{porcentaje:F0}%" : "0%";
            }
        }
    }
 
    private void ActualizarProgresoGeneral()
    {
        if (labelProgreso == null) return;
 
        int mundosDesbloqueados = 0;
        for (int i = 1; i <= 5; i++)
        {
            if (NivelesManager.Instance.MundoEstaDesbloqueado(i))
            {
                mundosDesbloqueados++;
            }
        }
 
        labelProgreso.text = $"Progreso: {mundosDesbloqueados}/5 mundos";
        Debug.Log($" Progreso general: {mundosDesbloqueados}/5 mundos");
    }
 
    private void SeleccionarMundo(int numeroMundo)
    {
        bool estaDesbloqueado = NivelesManager.Instance.MundoEstaDesbloqueado(numeroMundo);
 
        if (!estaDesbloqueado)
        {
            Debug.Log($"Mundo {numeroMundo} está bloqueado");
            return;
        }
 
        Debug.Log($"Seleccionado: Mundo {numeroMundo}");
 
        PlayerPrefs.SetInt("MundoSeleccionado", numeroMundo);
        PlayerPrefs.Save();
 
        string nombreEscena = nombresEscenas[numeroMundo];
        Debug.Log($" Cargando escena: {nombreEscena}");
        SceneManager.LoadScene(nombreEscena);
    }
 
    private void VolverAlMenu()
    {
        Debug.Log("Volviendo al menú principal");
        SceneManager.LoadScene("menuprincipalT"); 
    }
 
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F5))
        {
            Debug.Log(" Refrescando UI...");
            ActualizarUI();
        }
    }
}
