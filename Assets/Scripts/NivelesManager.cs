using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UIElements;

/// <summary>
/// Gestor principal del sistema de niveles.
/// 5 MUNDOS: Comedor → Gryffindor → Slytherin → Hufflepuff → Ravenclaw
/// MÁXIMO 3 ESTRELLAS POR MUNDO (1 nivel por mundo)
/// </summary>
public class NivelesManager : MonoBehaviour
{
    public static NivelesManager Instance { get; private set; }
    
    // ============================================
    // CONSTANTES DE CONFIGURACIÓN
    // ============================================
    
    private const int TOTAL_MUNDOS = 5;           // 5 mundos totales
    private const int NIVELES_POR_MUNDO = 1;      // 1 nivel por mundo (cada mundo es un nivel completo)
    
    // Umbrales de porcentaje para obtener estrellas
    private const float UMBRAL_1_ESTRELLA = 40f;   // 40% o más = 1 estrella
    private const float UMBRAL_2_ESTRELLAS = 70f;  // 70% o más = 2 estrellas
    private const float UMBRAL_3_ESTRELLAS = 90f;  // 90% o más = 3 estrellas
    
    // ============================================
    // CLASE DE DATOS DE NIVEL
    // ============================================
    
    [System.Serializable]
    public class DatosNivel 
    {
        public int mundo;
        public int nivel;
        public bool desbloqueado;
        public bool completado;
        public int estrellas;
        public float porcentaje;
        public int intentos;
        
        public DatosNivel(int mundo, int nivel) 
        {
            this.mundo = mundo;
            this.nivel = nivel;
            this.desbloqueado = false;
            this.completado = false;
            this.estrellas = 0;
            this.porcentaje = 0f;
            this.intentos = 0;
        }
    }
    
    // ============================================
    // INICIALIZACIÓN
    // ============================================
    
    private void Awake()
    {
        if (Instance == null) 
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("[NivelesManager] Instancia creada - 5 mundos con 3 estrellas máximo cada uno");
        }
        else 
        { 
            Destroy(gameObject);
        }
    }
    
    void Start() 
    { 
        InicializarNiveles(); 
    }
    
    /// <summary>
    /// Inicializa el sistema de niveles.
    /// Desbloquea Mundo 1 (EscenaComedor) automáticamente la primera vez.
    /// </summary>
    private void InicializarNiveles() 
    {
        if (!PlayerPrefs.HasKey("Nivel_1_1_Desbloqueado")) 
        {
            Debug.Log("[NivelesManager] Primera vez - Desbloqueando Mundo 1 (EscenaComedor)");
            
            // Desbloquear Mundo 1, Nivel 1 (EscenaComedor - PRIMER NIVEL)
            DesbloquearNivel(1, 1);
            DesbloquearMundo(1);
            PlayerPrefs.Save();
        }
    }
    
    // ============================================
    // FUNCIONES DE ACCESIBILIDAD
    // ============================================
    
    public void SincronizarAccesibilidad(VisualElement root) 
    {
        if (root == null) return;
        
        // Modo daltónico
        if (PlayerPrefs.GetInt("ModoDaltonico", 0) == 1) 
        {
            root.AddToClassList("modo-daltonico");
            Debug.Log("[NivelesManager] Modo daltónico activado");
        }
        else
        {
            root.RemoveFromClassList("modo-daltonico");
        }

        // Tamaño de letra
        float porcentaje = PlayerPrefs.GetFloat("TamanoLetra", 100f);
        float escala = porcentaje / 100f;
        
        root.Query<Label>().ForEach(label => 
        {
            label.style.fontSize = 24f * escala;
        });
        
        Debug.Log($"[NivelesManager] Tamaño letra: {porcentaje}%");
    }
    
    // ============================================
    // DESBLOQUEO DE NIVELES Y MUNDOS
    // ============================================
    
    public void DesbloquearNivel(int mundo, int nivel) 
    {
        string key = $"Nivel_{mundo}_{nivel}_Desbloqueado";
        PlayerPrefs.SetInt(key, 1);
        PlayerPrefs.Save();
        Debug.Log($"[NivelesManager] Desbloqueado: Mundo {mundo}, Nivel {nivel}");
    }
    
    public bool EstaDesbloqueado(int mundo, int nivel) 
    {
        string key = $"Nivel_{mundo}_{nivel}_Desbloqueado";
        return PlayerPrefs.GetInt(key, 0) == 1;
    }
    
    public void DesbloquearMundo(int mundo) 
    { 
        string key = $"Mundo_{mundo}_Desbloqueado";
        PlayerPrefs.SetInt(key, 1); 
        PlayerPrefs.Save();
        Debug.Log($"[NivelesManager] Mundo {mundo} desbloqueado");
    }
    
    public bool MundoEstaDesbloqueado(int mundo) 
    {
        string key = $"Mundo_{mundo}_Desbloqueado";
        return PlayerPrefs.GetInt(key, 0) == 1;
    }

    // ============================================
    // COMPLETAR NIVELES Y CALCULAR ESTRELLAS
    // ============================================

    /// <summary>
    /// Completa un nivel y calcula las estrellas según el porcentaje de monedas.
    /// MÁXIMO 3 ESTRELLAS POR MUNDO.
    /// </summary>
    public void CompletarNivel(int mundo, int nivel, float porcentajeAciertos) 
    {
        int estrellasObtenidas = CalcularEstrellas(porcentajeAciertos);
        float porcentajeAnterior = ObtenerPorcentaje(mundo, nivel);
        
        // Solo actualizar si es mejor
        if (porcentajeAciertos > porcentajeAnterior) 
        {
            PlayerPrefs.SetInt($"Nivel_{mundo}_{nivel}_Estrellas", estrellasObtenidas);
            PlayerPrefs.SetFloat($"Nivel_{mundo}_{nivel}_Porcentaje", porcentajeAciertos);
            Debug.Log($"[NivelesManager] Nuevo record: Mundo {mundo} - {porcentajeAciertos}% - {estrellasObtenidas} estrellas");
        }
        
        PlayerPrefs.SetInt($"Nivel_{mundo}_{nivel}_Completado", 1);
        PlayerPrefs.Save();
        
        // Desbloquear siguiente mundo
        DesbloquearSiguienteMundo(mundo);
    }

    /// <summary>
    /// Calcula estrellas según porcentaje.
    /// 40% = 1⭐ | 70% = 2⭐ | 90% = 3⭐
    /// </summary>
    private int CalcularEstrellas(float porcentaje) 
    {
        if (porcentaje >= UMBRAL_3_ESTRELLAS) return 3;
        if (porcentaje >= UMBRAL_2_ESTRELLAS) return 2;
        if (porcentaje >= UMBRAL_1_ESTRELLA) return 1;
        return 0;
    }

    /// <summary>
    /// Desbloquea el siguiente mundo después de completar uno.
    /// ORDEN: Comedor (1) → Gryffindor (2) → Slytherin (3) → Hufflepuff (4) → Ravenclaw (5)
    /// </summary>
    private void DesbloquearSiguienteMundo(int mundoActual) 
    {
        // Si no es el último mundo, desbloquear el siguiente
        if (mundoActual < TOTAL_MUNDOS)
        {
            int siguienteMundo = mundoActual + 1;
            DesbloquearMundo(siguienteMundo);
            DesbloquearNivel(siguienteMundo, 1);
            
            Debug.Log($"[NivelesManager] Mundo {mundoActual} completado → Desbloqueado Mundo {siguienteMundo}");
        }
        else
        {
            Debug.Log("[NivelesManager] ¡JUEGO COMPLETADO! Mundo 5 (Ravenclaw) es el último nivel");
        }
    }

    // ============================================
    // CONSULTAS DE PROGRESO
    // ============================================

    public bool EstaCompletado(int mundo, int nivel) 
    {
        return PlayerPrefs.GetInt($"Nivel_{mundo}_{nivel}_Completado", 0) == 1;
    }
    
    public int ObtenerEstrellas(int mundo, int nivel) 
    {
        return PlayerPrefs.GetInt($"Nivel_{mundo}_{nivel}_Estrellas", 0);
    }
    
    public float ObtenerPorcentaje(int mundo, int nivel) 
    {
        return PlayerPrefs.GetFloat($"Nivel_{mundo}_{nivel}_Porcentaje", 0f);
    }
    
    public int ObtenerIntentos(int mundo, int nivel) 
    {
        return PlayerPrefs.GetInt($"Nivel_{mundo}_{nivel}_Intentos", 0);
    }
    
    /// <summary>
    /// Porcentaje de completado del mundo.
    /// Como solo hay 1 nivel por mundo, es 100% si está completado, 0% si no.
    /// </summary>
    public float ObtenerPorcentajeMundo(int mundo) 
    {
        bool completado = EstaCompletado(mundo, 1);
        return completado ? 100f : 0f;
    }

    /// <summary>
    /// Obtiene las estrellas totales de un mundo.
    /// Como hay 1 nivel por mundo, las estrellas del mundo = estrellas del nivel.
    /// MÁXIMO 3 ESTRELLAS POR MUNDO.
    /// </summary>
    public int ObtenerEstrellasTotalMundo(int mundo) 
    {
        return ObtenerEstrellas(mundo, 1);
    }
    
    // ============================================
    // FUNCIONES DE DEBUG
    // ============================================
    
    /// <summary>
    /// Resetea todo el progreso (útil para testing).
    /// </summary>
    public void ResetearProgreso()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        Debug.Log("[NivelesManager] Progreso reseteado - Reinicia el juego");
    }
    
    /// <summary>
    /// Desbloquea todos los mundos (útil para testing).
    /// </summary>
    public void DesbloquearTodo()
    {
        for (int i = 1; i <= TOTAL_MUNDOS; i++)
        {
            DesbloquearMundo(i);
            DesbloquearNivel(i, 1);
        }
        Debug.Log("[NivelesManager] Todos los mundos desbloqueados (DEBUG)");
    }
}