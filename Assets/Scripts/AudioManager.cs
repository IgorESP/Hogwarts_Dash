using UnityEngine;

/// <summary>
/// Sistema centralizado de audio que sobrevive entre escenas (Singleton)
/// Controla música y efectos de sonido del juego.
/// </summary>
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    public AudioSource musicaSource;   // Música del juego
    public AudioSource sfxSource;      // Efectos de sonido

    [Header("Música de los niveles")]
    public AudioClip musicaComedor;
    public AudioClip musicaGryffindor;
    public AudioClip musicaHufflepuff;
    public AudioClip musicaSlytherin;
    public AudioClip musicaRavenclaw;

    [Header("Sonidos del juego")]
    public AudioClip sonidoMenu;
    public AudioClip sonidoMuerte;
    public AudioClip sonidoSnitch;
    public AudioClip sonidoVictoria;

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
            musicaSource.spatialBlend = 0f;
        }

        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.loop = false;
            sfxSource.playOnAwake = false;
            sfxSource.spatialBlend = 0f;
        }
    }

    void Start()
    {
        CargarVolumenes();
    }

    // ============================================================
    // VOLUMEN
    // ============================================================

    /// <summary> 
    /// Carga los volúmenes desde PlayerPrefs y los aplica. 
    /// </summary>
    private void CargarVolumenes()
    {
        volumenMusica = PlayerPrefs.GetFloat("VolumenMusica", 80f) / 100f;
        volumenSFX = PlayerPrefs.GetFloat("VolumenSFX", 80f) / 100f;
        AplicarVolumenes();
    }

    /// <summary> 
    /// Aplica los volúmenes cargados a los AudioSources. 
    /// El SFX siempre se deja a 1 porque PlayOneShot ya controla su volumen. 
    /// </summary>
    private void AplicarVolumenes()
    {
        musicaSource.volume = volumenMusica;
        sfxSource.volume = 1f;
    }

    /// <summary> 
    /// Cambia el volumen de la música y lo guarda. 
    /// </summary>
    public void SetVolumenMusica(float volumen)
    {
        volumenMusica = Mathf.Clamp01(volumen);
        musicaSource.volume = volumenMusica;

        PlayerPrefs.SetFloat("VolumenMusica", volumenMusica * 100f);
        PlayerPrefs.Save();
    }

    /// <summary> 
    /// Cambia el volumen de los efectos y lo guarda. 
    /// </summary>
    public void SetVolumenSFX(float volumen)
    {
        volumenSFX = Mathf.Clamp01(volumen);
        sfxSource.volume = 1f;

        PlayerPrefs.SetFloat("VolumenSFX", volumenSFX * 100f);
        PlayerPrefs.Save();
    }

    // ============================================================
    // EFECTOS DE SONIDO
    // ============================================================

    /// <summary> 
    /// Reproduce el sonido del menú (clic de botón). 
    /// </summary>
    public void PlayMenu() => ReproducirSFX(sonidoMenu);
    
    /// <summary> 
    /// Reproduce el sonido de muerte del jugador. 
    /// </summary>
    public void PlayMuerte()
    {
        if (sonidoMuerte != null)
            sfxSource.PlayOneShot(sonidoMuerte, volumenSFX);
    }
    
    public void PlaySnitch() => ReproducirSFX(sonidoSnitch);
    public void PlayVictoria() => ReproducirSFX(sonidoVictoria);

    /// <summary> 
    /// Reproduce cualquier efecto de sonido con el volumen SFX actual. 
    /// </summary>
    public void ReproducirSFX(AudioClip clip)
    {
        if (clip != null)
            sfxSource.PlayOneShot(clip, volumenSFX);
    }

    // ============================================================
    // MÚSICA
    // ============================================================

    /// <summary> 
    /// Sonido de clic para botones. 
    /// </summary>
    public void PlayBoton()
    {
        if (sonidoMenu != null)
            sfxSource.PlayOneShot(sonidoMenu, volumenSFX);
    }

    /// <summary> 
    /// Reproduce la música del Comedor. 
    /// </summary>
    public void ReproducirMusicaComedor()
    {
        if (musicaComedor == null) return;
        
        musicaSource.Stop();
        musicaSource.clip = musicaComedor;

        musicaSource.volume = volumenMusica;
        
        musicaSource.Play();
    }

    /// <summary> 
    /// Reproduce la música de Gryffindor. 
    /// </summary>
    public void ReproducirMusicaGryffindor()
    {
        if (musicaGryffindor == null) return;
        
        musicaSource.Stop();
        musicaSource.clip = musicaGryffindor;

        musicaSource.volume = volumenMusica;
        
        musicaSource.Play();
    }

    /// <summary> 
    /// Reproduce la música de Hufflepuff. 
    /// </summary>
    public void ReproducirMusicaHufflepuff()
    {
        if (musicaHufflepuff == null) return;
        
        musicaSource.Stop();
        musicaSource.clip = musicaHufflepuff;

        musicaSource.volume = volumenMusica;
        
        musicaSource.Play();
    }

    /// <summary> 
    /// Reproduce la música de Slytherin. 
    /// </summary>
    public void ReproducirMusicaSlytherin()
    {
        if (musicaSlytherin == null) return;

        musicaSource.Stop();
        musicaSource.clip = musicaSlytherin;

        musicaSource.volume = volumenMusica;
        
        musicaSource.Play();
    }

    /// <summary> 
    /// Reproduce la música de Ravenclaw. 
    /// </summary>
    public void ReproducirMusicaRavenclaw()
    {
        if (musicaRavenclaw == null) return;

        musicaSource.Stop();
        musicaSource.clip = musicaRavenclaw;

        musicaSource.volume = volumenMusica * 1.5f;

        musicaSource.Play();
    }

}
