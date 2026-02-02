using UnityEngine;

/// <summary>
/// Sistema centralizado de audio para todo el juego
/// Gestiona música de fondo y efectos de sonido
/// </summary>
public class AudioManager : MonoBehaviour
{
    // ====================================
    // SINGLETON
    // ====================================
    
    public static AudioManager Instance { get; private set; }
    
    // ====================================
    // AUDIO SOURCES
    // ====================================
    
    [Header("Audio Sources")]
    [Tooltip("AudioSource para la música de fondo")]
    public AudioSource musicaSource;
    
    [Tooltip("AudioSource para efectos de sonido")]
    public AudioSource sfxSource;
    
    // ====================================
    // MÚSICA DE FONDO
    // ====================================
    
    [Header("Música de Fondo")]
    [Tooltip("Clip de música del menú principal")]
    public AudioClip musicaMenu;
    
    [Tooltip("Clip de música durante el juego")]
    public AudioClip musicaJuego;
    
    // ====================================
    // VOLÚMENES
    // ====================================
    
    private float volumenMusica = 0.8f;
    private float volumenSFX = 0.8f;
    
    // ====================================
    // INICIALIZACIÓN
    // ====================================
    
    void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("AudioManager creado");
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        // Crear AudioSources si no existen
        if (musicaSource == null)
        {
            musicaSource = gameObject.AddComponent<AudioSource>();
            musicaSource.loop = true;
            musicaSource.playOnAwake = false;
        }
        
        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.loop = false;
            sfxSource.playOnAwake = false;
        }
    }
    
    void Start()
    {
        // Cargar volúmenes guardados
        CargarVolumenes();
        
        // Reproducir música de menú al inicio
        ReproducirMusicaMenu();
    }
    
    // ====================================
    // CARGAR VOLÚMENES
    // ====================================
    
    private void CargarVolumenes()
    {
        volumenMusica = PlayerPrefs.GetFloat("VolumenMusica", 80f) / 100f;
        volumenSFX = PlayerPrefs.GetFloat("VolumenSFX", 80f) / 100f;
        
        AplicarVolumenes();
        
        Debug.Log($"Volúmenes cargados - Música: {volumenMusica * 100}%, SFX: {volumenSFX * 100}%");
    }
    
    private void AplicarVolumenes()
    {
        if (musicaSource != null)
            musicaSource.volume = volumenMusica;
        
        if (sfxSource != null)
            sfxSource.volume = volumenSFX;
    }
    
    // ====================================
    // CONTROL DE MÚSICA
    // ====================================
    
    public void ReproducirMusicaMenu()
    {
        if (musicaMenu != null && musicaSource != null)
        {
            if (musicaSource.clip != musicaMenu)
            {
                musicaSource.clip = musicaMenu;
                musicaSource.Play();
                Debug.Log("Reproduciendo música de menú");
            }
        }
    }
    
    public void ReproducirMusicaJuego()
    {
        if (musicaJuego != null && musicaSource != null)
        {
            if (musicaSource.clip != musicaJuego)
            {
                musicaSource.clip = musicaJuego;
                musicaSource.Play();
                Debug.Log("Reproduciendo música de juego");
            }
        }
    }
    
    public void PausarMusica()
    {
        if (musicaSource != null && musicaSource.isPlaying)
        {
            musicaSource.Pause();
        }
    }
    
    public void ReanudarMusica()
    {
        if (musicaSource != null && !musicaSource.isPlaying)
        {
            musicaSource.UnPause();
        }
    }
    
    public void DetenerMusica()
    {
        if (musicaSource != null)
        {
            musicaSource.Stop();
        }
    }
    
    // ====================================
    // CONTROL DE VOLUMEN
    // ====================================
    
    public void SetVolumenMusica(float volumen)
    {
        volumenMusica = Mathf.Clamp01(volumen);
        
        if (musicaSource != null)
            musicaSource.volume = volumenMusica;
        
        Debug.Log($"🎵 Volumen música: {volumenMusica * 100}%");
    }
    
    public void SetVolumenSFX(float volumen)
    {
        volumenSFX = Mathf.Clamp01(volumen);
        
        if (sfxSource != null)
            sfxSource.volume = volumenSFX;
        
        Debug.Log($"🔊 Volumen SFX: {volumenSFX * 100}%");
    }
    
    // ====================================
    // REPRODUCIR EFECTOS DE SONIDO
    // ====================================
    
    public void ReproducirSFX(AudioClip clip)
    {
        if (clip != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }
    
    public void ReproducirSFX(AudioClip clip, float volumenExtra)
    {
        if (clip != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(clip, volumenExtra);
        }
    }
    
    // ====================================
    // GETTERS
    // ====================================
    
    public float GetVolumenMusica()
    {
        return volumenMusica;
    }
    
    public float GetVolumenSFX()
    {
        return volumenSFX;
    }
}