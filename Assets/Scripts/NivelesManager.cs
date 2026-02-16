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
    public void CompletarNivel(int m, int n, float porcentaje, int snitches) 
    {
        // 1. Guardar récord de porcentaje si es mejor
        float recordPerc = PlayerPrefs.GetFloat($"Nivel_{m}_{n}_Porcentaje", 0f);
        if (porcentaje > recordPerc) 
        {
            PlayerPrefs.SetFloat($"Nivel_{m}_{n}_Porcentaje", porcentaje);
            Debug.Log($"[Niveles] ¡Nuevo récord de porcentaje! Mundo {m}: {porcentaje:F1}%");
        }

        // 2. Guardar récord de snitches si es mejor
        int recordSnit = PlayerPrefs.GetInt($"Nivel_{m}_{n}_Snitches", 0);
        if (snitches > recordSnit)
        {
            PlayerPrefs.SetInt($"Nivel_{m}_{n}_Snitches", snitches);
            Debug.Log($"[Niveles] ¡Nuevo récord de snitches! Mundo {m}: {snitches}/3");
        }

        // 3. Calcular estrellas según el porcentaje alcanzado (Lógica original)
        int estrellas = CalcularEstrellas(porcentaje);
        int recordEst = PlayerPrefs.GetInt($"Nivel_{m}_{n}_Estrellas", 0);
        if (estrellas > recordEst)
        {
            PlayerPrefs.SetInt($"Nivel_{m}_{n}_Estrellas", estrellas);
        }
        
        // Marcar nivel como completado SOLO si llegó al 100%
        if (porcentaje >= 99f) 
        {
            PlayerPrefs.SetInt($"Nivel_{m}_{n}_Completado", 1);
            
            // Desbloquear siguiente mundo y su primer nivel si corresponde
            if (m < TOTAL_MUNDOS) 
            {
                DesbloquearMundo(m + 1);
                DesbloquearNivel(m + 1, 1);
            }
        }
        
        PlayerPrefs.Save();
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
    /// Obtiene las snitches del primer nivel del mundo.
    /// </summary>
    public int ObtenerSnitchesTotalMundo(int m) => 
        PlayerPrefs.GetInt($"Nivel_{m}_1_Snitches", 0);

    /// <summary> 
    /// Obtiene las estrellas del primer nivel del mundo (basado en porcentaje).
    /// </summary>
    public int ObtenerEstrellasTotalMundo(int m) => 
        PlayerPrefs.GetInt($"Nivel_{m}_1_Estrellas", 0);
    
    /// <summary> 
    /// Devuelve el porcentaje récord del primer nivel del mundo. 
    /// </summary>
    public float ObtenerPorcentajeMundo(int m) => 
        PlayerPrefs.GetFloat($"Nivel_{m}_1_Porcentaje", 0f);

    /// <summary>
    /// Indica si el mundo ha sido completado al 100%
    /// </summary>
    public bool MundoEstaCompletado(int m) => 
        PlayerPrefs.GetInt($"Nivel_{m}_1_Completado", 0) == 1;

    // ============================================================ 
    // AGREGACIÓN DE ESTADÍSTICAS GLOBALES (Perfil de Usuario)
    // ============================================================

    public int ObtenerTotalSnitches()
    {
        int total = 0;
        for (int i = 1; i <= TOTAL_MUNDOS; i++)
            total += ObtenerSnitchesTotalMundo(i);
        return total;
    }

    public int ObtenerTotalNivelesCompletados()
    {
        int total = 0;
        for (int i = 1; i <= TOTAL_MUNDOS; i++)
            if (MundoEstaCompletado(i)) total++;
        return total;
    }

    public int ObtenerTotalIntentos() => 
        PlayerPrefs.GetInt("TotalIntentosGlobal", 0);

    public void RegistrarIntentoGlobal()
    {
        int total = ObtenerTotalIntentos() + 1;
        PlayerPrefs.SetInt("TotalIntentosGlobal", total);
        PlayerPrefs.Save();
    }

    // ============================================================ 
    // RESETEO COMPLETO DEL PROGRESO 
    // ============================================================

    public void ResetearProgreso() 
    { 
        PlayerPrefs.DeleteAll(); 
        InicializarNiveles(); // Re-desbloquear el primer nivel
        PlayerPrefs.Save(); 
        Debug.Log("[Niveles] Progreso borrado y re-inicializado"); 
    }

    /// <summary>
    /// Limpia los récords de un nivel específico (útil para desarrollo y arreglar bugs de datos)
    /// </summary>
    public void LimpiarRécordNivel(int m, int n)
    {
        PlayerPrefs.DeleteKey($"Nivel_{m}_{n}_Porcentaje");
        PlayerPrefs.DeleteKey($"Nivel_{m}_{n}_Snitches");
        PlayerPrefs.DeleteKey($"Nivel_{m}_{n}_Estrellas");
        PlayerPrefs.DeleteKey($"Nivel_{m}_{n}_Completado");
        PlayerPrefs.Save();
        Debug.Log($"[Niveles] Récord limpiado para Mundo {m} Nivel {n}");
    }
}