using UnityEngine;

/// <summary>
/// Sistema centralizado de audio que sobrevive entre escenas (Singleton)
/// </summary>
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    
    [Header("Audio Sources")]
    public AudioSource musicaSource;
    public AudioSource sfxSource;
    
    [Header("Clips de Música")]
    public AudioClip musicaMenu;
    public AudioClip musicaJuego;
    
    private float volumenMusica = 0.8f;
    private float volumenSFX = 0.8f;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("[Audio] Iniciando AudioManager persistente");
        }
        else { 
            Destroy(gameObject); 
            return; 
        }
        
        // Si no se asignaron fuentes desde el inspector, se crean automáticamente
        if (musicaSource == null) { 
            musicaSource = gameObject.AddComponent<AudioSource>(); 
            musicaSource.loop = true; 
        }

        if (sfxSource == null) 
            sfxSource = gameObject.AddComponent<AudioSource>();
    }
    
    void Start()
    {
        // Cargar volúmenes guardados en PlayerPrefs
        CargarVolumenes();

        // Reproducir música del menú al iniciar
        ReproducirMusicaMenu();
    }
    
    /// <summary> 
    /// Carga los volúmenes guardados y los aplica. 
    /// </summary>
    private void CargarVolumenes()
    {
        volumenMusica = PlayerPrefs.GetFloat("VolumenMusica", 80f) / 100f;
        volumenSFX = PlayerPrefs.GetFloat("VolumenSFX", 80f) / 100f;
        AplicarVolumenes();
    }
    
    /// <summary> 
    /// Aplica los volúmenes actuales a las fuentes de audio. 
    /// </summary>
    private void AplicarVolumenes()
    {
        if (musicaSource != null) musicaSource.volume = volumenMusica;
        if (sfxSource != null) sfxSource.volume = volumenSFX;
    }

    // ====================================
    // CONTROL DE REPRODUCCIÓN
    // ====================================

    /// <summary> 
    /// Reproduce la música del menú si no está ya sonando. 
    /// </summary>
    public void ReproducirMusicaMenu()
    {
        if (musicaMenu != null && musicaSource != null && musicaSource.clip != musicaMenu)
        {
            Debug.Log("[Audio] Cambiando a música de menú");
            musicaSource.clip = musicaMenu;
            musicaSource.Play();
        }
    }
    
    /// <summary> 
    /// Reproduce la música del juego si no está ya sonando. 
    /// </summary>
    public void ReproducirMusicaJuego()
    {
        if (musicaJuego != null && musicaSource != null && musicaSource.clip != musicaJuego)
        {
            Debug.Log("[Audio] Cambiando a música de nivel");
            musicaSource.clip = musicaJuego;
            musicaSource.Play();
        }
    }

    // ==================================== 
    // CONTROL DE VOLÚMENES 
    // ====================================

    /// <summary> 
    /// Ajusta el volumen de la música (0–1). 
    /// </summary>
    public void SetVolumenMusica(float volumen)
    {
        volumenMusica = Mathf.Clamp01(volumen);
        if (musicaSource != null) musicaSource.volume = volumenMusica;
    }
    
    /// <summary> 
    /// Ajusta el volumen de los efectos (0–1). 
    /// </summary>
    public void SetVolumenSFX(float volumen)
    {
        volumenSFX = Mathf.Clamp01(volumen);
        if (sfxSource != null) sfxSource.volume = volumenSFX;
    }
    
    /// <summary> 
    /// Reproduce un efecto de sonido puntual sin interrumpir la música. 
    /// </summary>
    public void ReproducirSFX(AudioClip clip)
    {
        if (clip != null && sfxSource != null) sfxSource.PlayOneShot(clip);
    }
}