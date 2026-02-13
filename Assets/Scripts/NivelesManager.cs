using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UIElements;

/// <summary>
/// Progresión y lógica de desbloqueo de niveles y mundos (5 mundos temáticos)
/// </summary>
public class NivelesManager : MonoBehaviour
{
    public static NivelesManager Instance { get; private set; }
    
    // Número total de mundos disponibles
    private const int TOTAL_MUNDOS = 5;

    // Umbrales para calcular estrellas según porcentaje completado
    private const float UMBRAL_1_ESTRELLA = 40f;
    private const float UMBRAL_2_ESTRELLAS = 70f;
    private const float UMBRAL_3_ESTRELLAS = 90f;
    
    void Awake()
    {
        // Implementación del patrón Singleton
        if (Instance == null) 
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("[Niveles] Sistema de progresión activo");
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
    /// Configura el estado inicial del juego la primera vez que se ejecuta. 
    /// </summary>
    private void InicializarNiveles() 
    {
        // Si nunca se ha jugado antes, desbloquear el primer mundo y nivel
        if (!PlayerPrefs.HasKey("Nivel_1_1_Desbloqueado")) 
        {
            Debug.Log("[Niveles] Configurando acceso inicial al Gran Comedor");

            DesbloquearNivel(1, 1);
            DesbloquearMundo(1);

            PlayerPrefs.Save();
        }
    }
    
    /// <summary> 
    /// Aplica ajustes de accesibilidad a la UI (modo daltónico y tamaño de letra). 
    /// </summary>
    public void SincronizarAccesibilidad(VisualElement root) 
    {
        if (root == null) return;
        
        // Activar modo daltónico si está habilitado
        if (PlayerPrefs.GetInt("ModoDaltonico", 0) == 1) 
            root.AddToClassList("modo-daltonico");

        // Ajustar tamaño de letra global
        float tam = PlayerPrefs.GetFloat("TamanoLetra", 100f);
        root.Query<Label>().ForEach(l => l.style.fontSize = 24f * (tam / 100f));
    }

    // ============================================================ 
    // DESBLOQUEO DE NIVELES Y MUNDOS 
    // ============================================================
    
    public void DesbloquearNivel(int m, int n) 
    { 
        PlayerPrefs.SetInt($"Nivel_{m}_{n}_Desbloqueado", 1); 
        PlayerPrefs.Save(); 
    }
    public bool EstaDesbloqueado(int m, int n) => 
        PlayerPrefs.GetInt($"Nivel_{m}_{n}_Desbloqueado", 0) == 1;
    
    public void DesbloquearMundo(int m) 
    { 
        PlayerPrefs.SetInt($"Mundo_{m}_Desbloqueado", 1); 
        PlayerPrefs.Save(); 
    }
    public bool MundoEstaDesbloqueado(int m) => 
        PlayerPrefs.GetInt($"Mundo_{m}_Desbloqueado", 0) == 1;
    
    // ============================================================ 
    // REGISTRO DE PROGRESO Y ESTRELLAS 
    // ============================================================

    /// <summary>
    /// Registra la finalización de un nivel y actualiza récords
    /// </summary>
    public void CompletarNivel(int m, int n, float porcentaje) 
    {
        // Calcular estrellas según el porcentaje alcanzado
        int estrellas = CalcularEstrellas(porcentaje);

        // Leer récord anterior
        float record = PlayerPrefs.GetFloat($"Nivel_{m}_{n}_Porcentaje", 0f);
        
        // Si el nuevo porcentaje es mejor, actualizar récord
        if (porcentaje > record) 
        {
            PlayerPrefs.SetInt($"Nivel_{m}_{n}_Estrellas", estrellas);
            PlayerPrefs.SetFloat($"Nivel_{m}_{n}_Porcentaje", porcentaje);

            Debug.Log($"[Niveles] ¡Nuevo récord! Mundo {m}: {porcentaje:F1}% ({estrellas}⭐)");
        }
        
        // Marcar nivel como completado
        PlayerPrefs.SetInt($"Nivel_{m}_{n}_Completado", 1);
        PlayerPrefs.Save();
        
        // Desbloquear siguiente mundo y su primer nivel si corresponde
        if (m < TOTAL_MUNDOS) 
        {
            DesbloquearMundo(m + 1);
            DesbloquearNivel(m + 1, 1);
        }
    }

    /// <summary> 
    /// Determina cuántas estrellas obtiene el jugador según su porcentaje. 
    /// </summary>
    private int CalcularEstrellas(float p) 
    {
        if (p >= UMBRAL_3_ESTRELLAS) return 3;
        if (p >= UMBRAL_2_ESTRELLAS) return 2;
        if (p >= UMBRAL_1_ESTRELLA) return 1;
        return 0;
    }

    /// <summary> 
    /// Obtiene las estrellas del primer nivel del mundo (para mostrar progreso global).
    /// </summary>
    public int ObtenerEstrellasTotalMundo(int m) => 
        PlayerPrefs.GetInt($"Nivel_{m}_1_Estrellas", 0);
    
    /// <summary> 
    /// Devuelve 100% si el primer nivel del mundo está completado. 
    /// </summary>
    public float ObtenerPorcentajeMundo(int m) => 
        PlayerPrefs.GetInt($"Nivel_{m}_1_Completado", 0) == 1 ? 100f : 0f;

    // ============================================================ 
    // RESETEO COMPLETO DEL PROGRESO 
    // ============================================================

    public void ResetearProgreso() 
    { 
        PlayerPrefs.DeleteAll(); 
        PlayerPrefs.Save(); 
        Debug.Log("[Niveles] Progreso borrado por completo"); 
    }
}