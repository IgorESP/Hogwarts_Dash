using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

/// <summary>
/// Controlador visual para la selección de mundos
/// </summary>
public class NivelesUIController : MonoBehaviour
{
    private VisualElement root;
    private Button botonMundo1, botonMundo2, botonMundo3, botonMundo4, botonMundo5, botonAtras;
    private Label labelProgreso;
 
    private readonly string[] nombresEscenas = new string[] 
    { 
        "", "EscenaComedor", "EscenaGryffindor", "EscenaSlytherin", "EscenaHufflepuff", "EscenaRavenclaw" 
    };
 
    void Start()
    {
        root = GetComponent<UIDocument>().rootVisualElement;
        
        if (NivelesManager.Instance != null) NivelesManager.Instance.SincronizarAccesibilidad(root);
 
        ObtenerReferencias();
        ConfigurarEventos();
        ActualizarUI();
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
    }
 
    private void ConfigurarEventos() 
    {
        botonMundo1.clicked += () => SeleccionarMundo(1);
        botonMundo2.clicked += () => SeleccionarMundo(2);
        botonMundo3.clicked += () => SeleccionarMundo(3);
        botonMundo4.clicked += () => SeleccionarMundo(4);
        botonMundo5.clicked += () => SeleccionarMundo(5);
        if (botonAtras != null) botonAtras.clicked += () => SceneManager.LoadScene("MenuPrincipalT");
    }
 
    private void ActualizarUI() 
    {
        for (int i = 1; i <= 5; i++)
        {
            Button b = root.Q<Button>($"Mundo_{i}");
            ActualizarMundo(i, b);
        }
        ActualizarProgresoGeneral();
    }
 
    private void ActualizarMundo(int num, Button b) 
    {
        if (b == null) return;
        
        bool unlocked = NivelesManager.Instance.MundoEstaDesbloqueado(num);
        b.SetEnabled(unlocked);
        b.EnableInClassList("bloqueado", !unlocked);
        b.EnableInClassList("desbloqueado", unlocked);
        
        // Actualizar estrellas y % en la tarjeta
        int stars = NivelesManager.Instance.ObtenerEstrellasTotalMundo(num);
        b.Query<Label>().ForEach(l => {
            if (l.text.Contains("/3")) l.text = $"{stars}/3";
            else if (l.text.Contains("%")) l.text = unlocked ? "100%" : "0%"; // Basado en mundo completo
        });
    }
 
    private void ActualizarProgresoGeneral() 
    {
        int completados = 0;
        for (int i = 1; i <= 5; i++) if (NivelesManager.Instance.ObtenerEstrellasTotalMundo(i) > 0) completados++;
        if (labelProgreso != null) labelProgreso.text = $"Progreso: {completados}/5 mundos";
    }
 
    private void SeleccionarMundo(int num) 
    {
        Debug.Log($"[NivelesUI] Cargando: {nombresEscenas[num]}");
        PlayerPrefs.SetInt("MundoSeleccionado", num);
        PlayerPrefs.Save();
        SceneManager.LoadScene(nombresEscenas[num]);
    }
 
    void Update() 
    { 
        if (Input.GetKeyDown(KeyCode.F5)) ActualizarUI();
    }
}