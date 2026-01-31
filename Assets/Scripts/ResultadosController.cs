using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class ResultadosController : MonoBehaviour
{
    private VisualElement root;
    private VisualElement panelResultados;
    
    // Labels
    private Label labelPorcentaje;
    private Label labelIntentos;
    private Label labelSnitches;
    
    // Estrellas
    private VisualElement estrella1;
    private VisualElement estrella2;
    private VisualElement estrella3;
    
    // Botones
    private Button botonReintentar;
    private Button botonVolver;
    
    void Start()
    {
        root = GetComponent<UIDocument>().rootVisualElement;
        
        // Obtener referencias
        panelResultados = root.Q<VisualElement>("PanelResultados");
        labelPorcentaje = root.Q<Label>("Label_Porcentaje");
        labelIntentos = root.Q<Label>("Label_Intentos");
        labelSnitches = root.Q<Label>("Label_Snitches");
        
        estrella1 = root.Q<VisualElement>("Estrella1");
        estrella2 = root.Q<VisualElement>("Estrella2");
        estrella3 = root.Q<VisualElement>("Estrella3");
        
        botonReintentar = root.Q<Button>("Boton_Reintentar");
        botonVolver = root.Q<Button>("Boton_Volver");
        
        // Verificar que todo se encontró
        if (panelResultados == null) Debug.LogError(" No se encontró PanelResultados");
        if (labelPorcentaje == null) Debug.LogError(" No se encontró Label_Porcentaje");
        if (labelIntentos == null) Debug.LogError("No se encontró Label_Intentos");
        if (labelSnitches == null) Debug.LogError(" No se encontró Label_Snitches");
        
        // Configurar eventos
        if (botonReintentar != null) botonReintentar.clicked += Reintentar;
        if (botonVolver != null) botonVolver.clicked += VolverAlMenu;
        
        // Mantener oculto al inicio
        if (panelResultados != null)
        {
            panelResultados.style.display = DisplayStyle.None;
        }
        
        Debug.Log("ResultadosController inicializado");
    }
    
    /// <summary>
    /// Muestra la pantalla de resultados con las estadísticas
    /// </summary>
    public void MostrarResultados(float porcentaje, int intentos, int snitches, int estrellas)
    {
        Debug.Log($"Mostrando resultados: {porcentaje}%, {intentos} intentos, {snitches} snitches, {estrellas} estrellas");
        
        // Actualizar textos
        if (labelPorcentaje != null) labelPorcentaje.text = $"{porcentaje:F0}%";
        if (labelIntentos != null) labelIntentos.text = intentos.ToString();
        if (labelSnitches != null) labelSnitches.text = snitches.ToString();
        
        // Actualizar estrellas
        ActualizarEstrellas(estrellas);
        
        // Mostrar panel
        if (panelResultados != null)
        {
            panelResultados.style.display = DisplayStyle.Flex;
        }
        
        // Pausar el juego
        Time.timeScale = 0f;
    }
    
    private void ActualizarEstrellas(int cantidad)
    {
        // Todas apagadas por defecto (opacity 0.3)
        if (estrella1 != null) estrella1.style.opacity = 0.3f;
        if (estrella2 != null) estrella2.style.opacity = 0.3f;
        if (estrella3 != null) estrella3.style.opacity = 0.3f;
        
        // Encender según cantidad
        if (cantidad >= 1 && estrella1 != null) estrella1.style.opacity = 1f;
        if (cantidad >= 2 && estrella2 != null) estrella2.style.opacity = 1f;
        if (cantidad >= 3 && estrella3 != null) estrella3.style.opacity = 1f;
    }
    
    private void Reintentar()
    {
        Debug.Log("Reintentando nivel...");
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    private void VolverAlMenu()
    {
        Debug.Log("Volviendo al menú de niveles...");
        Time.timeScale = 1f;
        SceneManager.LoadScene("PlayScene"); // ⚠️ Cambia por el nombre de tu escena de niveles
    }
}