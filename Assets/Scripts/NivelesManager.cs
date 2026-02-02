using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UIElements; // Añadido para accesibilidad

public class NivelesManager : MonoBehaviour
{
    public static NivelesManager Instance { get; private set; }
    
    // --- FUNCIÓN AÑADIDA PARA ACCESIBILIDAD ---
    public void SincronizarAccesibilidad(VisualElement root) {
        if (root == null) return;
        // Modo Daltónico
         if (PlayerPrefs.GetInt("ModoDaltonico", 0) == 1) {
            root.AddToClassList("modo-daltonico-activado");
        }

        // 2. Solo cambia el tamaño si el usuario movió el slider
        float porcentaje = PlayerPrefs.GetFloat("TamanoLetra", 100f);
        float escala = porcentaje / 100f;
        root.Query<Label>().ForEach(label => {
            label.style.fontSize = 24f * escala; // 24px es el tamaño base
        });
    }

    private void Awake()
    {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else { Destroy(gameObject); }
    }
    
    private const int TOTAL_MUNDOS = 4;
    private const int NIVELES_POR_MUNDO = 10;
    private const float UMBRAL_1_ESTRELLA = 40f;
    private const float UMBRAL_2_ESTRELLAS = 70f;
    private const float UMBRAL_3_ESTRELLAS = 90f;
    
    [System.Serializable]
    public class DatosNivel {
        public int mundo; public int nivel; public bool desbloqueado; public bool completado;
        public int estrellas; public float porcentaje; public int intentos;
        public DatosNivel(int mundo, int nivel) {
            this.mundo = mundo; this.nivel = nivel; this.desbloqueado = false;
            this.completado = false; this.estrellas = 0; this.porcentaje = 0f; this.intentos = 0;
        }
    }
    
    void Start() { InicializarNiveles(); }
    
    private void InicializarNiveles() {
        if (!PlayerPrefs.HasKey("Nivel_1_1_Desbloqueado")) {
            DesbloquearNivel(1, 1);
            PlayerPrefs.SetInt("Mundo_1_Desbloqueado", 1);
            PlayerPrefs.Save();
        }
    }
    
    public void DesbloquearNivel(int mundo, int nivel) {
        PlayerPrefs.SetInt($"Nivel_{mundo}_{nivel}_Desbloqueado", 1);
        PlayerPrefs.Save();
    }
    
    public bool EstaDesbloqueado(int mundo, int nivel) => PlayerPrefs.GetInt($"Nivel_{mundo}_{nivel}_Desbloqueado", 0) == 1;
    public void DesbloquearMundo(int mundo) { PlayerPrefs.SetInt($"Mundo_{mundo}_Desbloqueado", 1); PlayerPrefs.Save(); }
    public bool MundoEstaDesbloqueado(int mundo) => PlayerPrefs.GetInt($"Mundo_{mundo}_Desbloqueado", 0) == 1;

    public void CompletarNivel(int mundo, int nivel, float porcentajeAciertos) {
        int estrellasObtenidas = CalcularEstrellas(porcentajeAciertos);
        float porcentajeAnterior = ObtenerPorcentaje(mundo, nivel);
        if (porcentajeAciertos > porcentajeAnterior) {
            PlayerPrefs.SetInt($"Nivel_{mundo}_{nivel}_Estrellas", estrellasObtenidas);
            PlayerPrefs.SetFloat($"Nivel_{mundo}_{nivel}_Porcentaje", porcentajeAciertos);
        }
        PlayerPrefs.SetInt($"Nivel_{mundo}_{nivel}_Completado", 1);
        PlayerPrefs.Save();
        DesbloquearSiguienteNivel(mundo, nivel);
    }

    private int CalcularEstrellas(float p) => p >= 90 ? 3 : p >= 70 ? 2 : p >= 40 ? 1 : 0;

    private void DesbloquearSiguienteNivel(int mundo, int nivel) {
        if (nivel == NIVELES_POR_MUNDO) {
            if (mundo < TOTAL_MUNDOS) { DesbloquearMundo(mundo + 1); DesbloquearNivel(mundo + 1, 1); }
        } else DesbloquearNivel(mundo, nivel + 1);
    }

    public bool EstaCompletado(int m, int n) => PlayerPrefs.GetInt($"Nivel_{m}_{n}_Completado", 0) == 1;
    public int ObtenerEstrellas(int m, int n) => PlayerPrefs.GetInt($"Nivel_{m}_{n}_Estrellas", 0);
    public float ObtenerPorcentaje(int m, int n) => PlayerPrefs.GetFloat($"Nivel_{m}_{n}_Porcentaje", 0f);
    public int ObtenerIntentos(int m, int n) => PlayerPrefs.GetInt($"Nivel_{m}_{n}_Intentos", 0);
    
    public float ObtenerPorcentajeMundo(int mundo) {
        int comp = 0;
        for (int i = 1; i <= NIVELES_POR_MUNDO; i++) if (EstaCompletado(mundo, i)) comp++;
        return (comp / (float)NIVELES_POR_MUNDO) * 100f;
    }

    public int ObtenerEstrellasTotalMundo(int mundo) {
        int tot = 0;
        for (int i = 1; i <= NIVELES_POR_MUNDO; i++) tot += ObtenerEstrellas(mundo, i);
        return tot;
    }
}