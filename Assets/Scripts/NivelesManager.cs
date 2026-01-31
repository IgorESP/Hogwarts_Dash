using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Sistema completo de gestión de niveles
/// - Desbloqueo automático
/// - Sistema de estrellas (1-3)
/// - Porcentaje de completado
/// - Guardado en PlayerPrefs
/// </summary>
public class NivelesManager : MonoBehaviour
{
    // ====================================
    // SINGLETON PATTERN
    // ====================================
    public static NivelesManager Instance { get; private set; }
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("NivelesManager inicializado");
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    // ====================================
    // CONSTANTES DE CONFIGURACIÓN
    // ====================================
    private const int TOTAL_MUNDOS = 4;
    private const int NIVELES_POR_MUNDO = 10;
    
    // Umbrales para estrellas (porcentaje de aciertos)
    private const float UMBRAL_1_ESTRELLA = 40f;  // 40% = 1 estrella
    private const float UMBRAL_2_ESTRELLAS = 70f; // 70% = 2 estrellas
    private const float UMBRAL_3_ESTRELLAS = 90f; // 90% = 3 estrellas
    
    // ====================================
    // ESTRUCTURA DE DATOS DE NIVEL
    // ====================================
    [System.Serializable]
    public class DatosNivel
    {
        public int mundo;           // 1-4
        public int nivel;           // 1-10
        public bool desbloqueado;   // ¿Está disponible?
        public bool completado;     // ¿Se ha pasado?
        public int estrellas;       // 0-3
        public float porcentaje;    // 0-100 (mejor intento)
        public int intentos;        // Veces jugado
        
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
    
    // ====================================
    // INICIALIZACIÓN
    // ====================================
    void Start()
    {
        InicializarNiveles();
    }
    
    private void InicializarNiveles()
    {
        Debug.Log("Inicializando sistema de niveles...");
        
        // Si es primera vez, desbloquear Mundo 1 - Nivel 1
        if (!PlayerPrefs.HasKey("Nivel_1_1_Desbloqueado"))
        {
            Debug.Log("Primera vez - Desbloqueando Mundo 1 - Nivel 1");
            DesbloquearNivel(1, 1);
            
            // Desbloquear Mundo 1 también
            PlayerPrefs.SetInt("Mundo_1_Desbloqueado", 1);
            PlayerPrefs.Save();
        }
    }
    
    // ====================================
    // DESBLOQUEAR NIVEL
    // ====================================
    public void DesbloquearNivel(int mundo, int nivel)
    {
        string key = $"Nivel_{mundo}_{nivel}_Desbloqueado";
        PlayerPrefs.SetInt(key, 1);
        PlayerPrefs.Save();
        Debug.Log($"Nivel {mundo}-{nivel} desbloqueado");
    }
    
    public bool EstaDesbloqueado(int mundo, int nivel)
    {
        string key = $"Nivel_{mundo}_{nivel}_Desbloqueado";
        return PlayerPrefs.GetInt(key, 0) == 1;
    }
    
    // ====================================
    // DESBLOQUEAR MUNDO
    // ====================================
    public void DesbloquearMundo(int mundo)
    {
        string key = $"Mundo_{mundo}_Desbloqueado";
        PlayerPrefs.SetInt(key, 1);
        PlayerPrefs.Save();
        Debug.Log($"🌍 Mundo {mundo} desbloqueado");
    }
    
    public bool MundoEstaDesbloqueado(int mundo)
    {
        string key = $"Mundo_{mundo}_Desbloqueado";
        return PlayerPrefs.GetInt(key, 0) == 1;
    }
    
    // ====================================
    // COMPLETAR NIVEL (LLAMAR DESDE EL JUEGO)
    // ====================================
    public void CompletarNivel(int mundo, int nivel, float porcentajeAciertos)
    {
        Debug.Log($"🎯 Completando nivel {mundo}-{nivel} con {porcentajeAciertos}% de aciertos");
        
        // Calcular estrellas según porcentaje
        int estrellasObtenidas = CalcularEstrellas(porcentajeAciertos);
        
        // Obtener datos anteriores
        int estrellasAnteriores = ObtenerEstrellas(mundo, nivel);
        float porcentajeAnterior = ObtenerPorcentaje(mundo, nivel);
        int intentosAnteriores = ObtenerIntentos(mundo, nivel);
        
        // Solo actualizar si es mejor que el intento anterior
        bool esNuevoRecord = porcentajeAciertos > porcentajeAnterior;
        
        // Guardar datos
        string keyCompletado = $"Nivel_{mundo}_{nivel}_Completado";
        string keyEstrellas = $"Nivel_{mundo}_{nivel}_Estrellas";
        string keyPorcentaje = $"Nivel_{mundo}_{nivel}_Porcentaje";
        string keyIntentos = $"Nivel_{mundo}_{nivel}_Intentos";
        
        PlayerPrefs.SetInt(keyCompletado, 1);
        PlayerPrefs.SetInt(keyIntentos, intentosAnteriores + 1);
        
        // Solo actualizar estrellas y porcentaje si es mejor
        if (esNuevoRecord)
        {
            PlayerPrefs.SetInt(keyEstrellas, estrellasObtenidas);
            PlayerPrefs.SetFloat(keyPorcentaje, porcentajeAciertos);
            Debug.Log($"⭐ ¡NUEVO RÉCORD! {estrellasObtenidas} estrellas ({porcentajeAciertos}%)");
        }
        else
        {
            Debug.Log($"📊 Intento registrado. Récord anterior: {estrellasAnteriores} estrellas ({porcentajeAnterior}%)");
        }
        
        PlayerPrefs.Save();
        
        // Desbloquear siguiente nivel
        DesbloquearSiguienteNivel(mundo, nivel);
        
        // Verificar si el mundo está completo
        VerificarMundoCompletado(mundo);
    }
    
    // ====================================
    // CALCULAR ESTRELLAS
    // ====================================
    private int CalcularEstrellas(float porcentajeAciertos)
    {
        if (porcentajeAciertos >= UMBRAL_3_ESTRELLAS)
            return 3;
        else if (porcentajeAciertos >= UMBRAL_2_ESTRELLAS)
            return 2;
        else if (porcentajeAciertos >= UMBRAL_1_ESTRELLA)
            return 1;
        else
            return 0; // No pasó el nivel
    }
    
    // ====================================
    // DESBLOQUEAR SIGUIENTE NIVEL
    // ====================================
    private void DesbloquearSiguienteNivel(int mundo, int nivel)
    {
        // Si es el último nivel del mundo, desbloquear primer nivel del siguiente mundo
        if (nivel == NIVELES_POR_MUNDO)
        {
            if (mundo < TOTAL_MUNDOS)
            {
                DesbloquearMundo(mundo + 1);
                DesbloquearNivel(mundo + 1, 1);
                Debug.Log($"🎉 ¡Mundo {mundo + 1} desbloqueado!");
            }
            else
            {
                Debug.Log($"🏆 ¡FELICIDADES! Has completado TODOS los niveles");
            }
        }
        else
        {
            // Desbloquear siguiente nivel en el mismo mundo
            DesbloquearNivel(mundo, nivel + 1);
        }
    }
    
    // ====================================
    // VERIFICAR MUNDO COMPLETADO
    // ====================================
    private void VerificarMundoCompletado(int mundo)
    {
        int nivelesCompletados = 0;
        
        for (int i = 1; i <= NIVELES_POR_MUNDO; i++)
        {
            if (EstaCompletado(mundo, i))
            {
                nivelesCompletados++;
            }
        }
        
        float porcentajeMundo = (nivelesCompletados / (float)NIVELES_POR_MUNDO) * 100f;
        
        Debug.Log($"🌍 Mundo {mundo}: {nivelesCompletados}/{NIVELES_POR_MUNDO} niveles completados ({porcentajeMundo}%)");
        
        if (nivelesCompletados == NIVELES_POR_MUNDO)
        {
            Debug.Log($"🎊 ¡MUNDO {mundo} COMPLETADO AL 100%!");
        }
    }
    
    // ====================================
    // OBTENER DATOS DE NIVEL
    // ====================================
    public bool EstaCompletado(int mundo, int nivel)
    {
        string key = $"Nivel_{mundo}_{nivel}_Completado";
        return PlayerPrefs.GetInt(key, 0) == 1;
    }
    
    public int ObtenerEstrellas(int mundo, int nivel)
    {
        string key = $"Nivel_{mundo}_{nivel}_Estrellas";
        return PlayerPrefs.GetInt(key, 0);
    }
    
    public float ObtenerPorcentaje(int mundo, int nivel)
    {
        string key = $"Nivel_{mundo}_{nivel}_Porcentaje";
        return PlayerPrefs.GetFloat(key, 0f);
    }
    
    public int ObtenerIntentos(int mundo, int nivel)
    {
        string key = $"Nivel_{mundo}_{nivel}_Intentos";
        return PlayerPrefs.GetInt(key, 0);
    }
    
    // ====================================
    // OBTENER PROGRESO DEL MUNDO
    // ====================================
    public float ObtenerPorcentajeMundo(int mundo)
    {
        int nivelesCompletados = 0;
        
        for (int i = 1; i <= NIVELES_POR_MUNDO; i++)
        {
            if (EstaCompletado(mundo, i))
            {
                nivelesCompletados++;
            }
        }
        
        return (nivelesCompletados / (float)NIVELES_POR_MUNDO) * 100f;
    }
    
    public int ObtenerEstrellasTotalMundo(int mundo)
    {
        int estrellasTotales = 0;
        
        for (int i = 1; i <= NIVELES_POR_MUNDO; i++)
        {
            estrellasTotales += ObtenerEstrellas(mundo, i);
        }
        
        return estrellasTotales;
    }
    
    // ====================================
    // RESET (PARA TESTING)
    // ====================================
    public void ResetearTodo()
    {
        Debug.LogWarning("RESETEANDO TODO EL PROGRESO");
        
        for (int mundo = 1; mundo <= TOTAL_MUNDOS; mundo++)
        {
            for (int nivel = 1; nivel <= NIVELES_POR_MUNDO; nivel++)
            {
                PlayerPrefs.DeleteKey($"Nivel_{mundo}_{nivel}_Desbloqueado");
                PlayerPrefs.DeleteKey($"Nivel_{mundo}_{nivel}_Completado");
                PlayerPrefs.DeleteKey($"Nivel_{mundo}_{nivel}_Estrellas");
                PlayerPrefs.DeleteKey($"Nivel_{mundo}_{nivel}_Porcentaje");
                PlayerPrefs.DeleteKey($"Nivel_{mundo}_{nivel}_Intentos");
            }
            PlayerPrefs.DeleteKey($"Mundo_{mundo}_Desbloqueado");
        }
        
        PlayerPrefs.Save();
        
        // Reinicializar
        InicializarNiveles();
    }
    
    // ====================================
    // DEBUG - DESBLOQUEAR TODO
    // ====================================
    [ContextMenu("Desbloquear Todo (Debug)")]
    public void DesbloquearTodo()
    {
        Debug.Log("Desbloqueando todos los niveles (DEBUG)");
        
        for (int mundo = 1; mundo <= TOTAL_MUNDOS; mundo++)
        {
            DesbloquearMundo(mundo);
            for (int nivel = 1; nivel <= NIVELES_POR_MUNDO; nivel++)
            {
                DesbloquearNivel(mundo, nivel);
            }
        }
    }
}