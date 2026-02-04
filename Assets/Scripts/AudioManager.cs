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
        else { Destroy(gameObject); return; }
        
        // Configurar fuentes si no están asignadas
        if (musicaSource == null) { musicaSource = gameObject.AddComponent<AudioSource>(); musicaSource.loop = true; }
        if (sfxSource == null) sfxSource = gameObject.AddComponent<AudioSource>();
    }
    
    void Start()
    {
        CargarVolumenes();
        ReproducirMusicaMenu();
    }
    
    private void CargarVolumenes()
    {
        volumenMusica = PlayerPrefs.GetFloat("VolumenMusica", 80f) / 100f;
        volumenSFX = PlayerPrefs.GetFloat("VolumenSFX", 80f) / 100f;
        AplicarVolumenes();
    }
    
    private void AplicarVolumenes()
    {
        if (musicaSource != null) musicaSource.volume = volumenMusica;
        if (sfxSource != null) sfxSource.volume = volumenSFX;
    }

    // ====================================
    // CONTROL DE REPRODUCCIÓN
    // ====================================

    public void ReproducirMusicaMenu()
    {
        if (musicaMenu != null && musicaSource != null && musicaSource.clip != musicaMenu)
        {
            Debug.Log("[Audio] Cambiando a música de menú");
            musicaSource.clip = musicaMenu;
            musicaSource.Play();
        }
    }
    
    public void ReproducirMusicaJuego()
    {
        if (musicaJuego != null && musicaSource != null && musicaSource.clip != musicaJuego)
        {
            Debug.Log("[Audio] Cambiando a música de nivel");
            musicaSource.clip = musicaJuego;
            musicaSource.Play();
        }
    }

    public void SetVolumenMusica(float volumen)
    {
        volumenMusica = Mathf.Clamp01(volumen);
        if (musicaSource != null) musicaSource.volume = volumenMusica;
    }
    
    public void SetVolumenSFX(float volumen)
    {
        volumenSFX = Mathf.Clamp01(volumen);
        if (sfxSource != null) sfxSource.volume = volumenSFX;
    }
    
    /// <summary>
    /// Reproduce un efecto de sonido puntual
    /// </summary>
    public void ReproducirSFX(AudioClip clip)
    {
        if (clip != null && sfxSource != null) sfxSource.PlayOneShot(clip);
    }
}