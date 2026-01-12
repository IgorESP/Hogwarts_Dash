using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // Si usas TMPro, cambia a TMPro
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Configuración del Nivel")]
    public Transform playerTransform;
    public Transform endPoint; // Punto final para calcular %
    
    [Header("UI")]
    public Text progressText; // O TMPro.TMP_Text
    public GameObject gameOverPanel;
    public GameObject victoryPanel;

    private bool isGameOver = false;
    private float levelDistance;

    private void Awake()
    {
        // Singleton simple
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (playerTransform != null && endPoint != null)
        {
            levelDistance = Vector2.Distance(new Vector2(playerTransform.position.x, 0), new Vector2(endPoint.position.x, 0));
        }

        Time.timeScale = 1; // Asegurar tiempo normal al inicio
    }

    private void Update()
    {
        if (isGameOver)
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.touchCount > 0)
            {
                RestartLevel();
            }
            return;
        }

        UpdateProgress();
    }

    void UpdateProgress()
    {
        if (playerTransform != null && endPoint != null)
        {
            float currentDistance = Vector2.Distance(new Vector2(playerTransform.position.x, 0), new Vector2(endPoint.position.x, 0));
            float progress = 1f - (currentDistance / levelDistance);
            progress = Mathf.Clamp01(progress);

            if (progressText != null)
            {
                progressText.text = Mathf.FloorToInt(progress * 100) + "%";
            }
        }
    }

    public void PlayerDied()
    {
        if (isGameOver) return;

        isGameOver = true;
        Debug.Log("Game Over!");

        // Efecto de muerte (partículas) podrías instanciarlo aquí
        
        // Mostrar UI de muerte o simplemente reiniciar rápido estilo Geometry Dash
        // Aquí reiniciamos tras un breve delay
        StartCoroutine(RestartRoutine());
    }

    IEnumerator RestartRoutine()
    {
        yield return new WaitForSeconds(1f); // Espera 1 segundo
        RestartLevel();
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LevelComplete()
    {
        if (isGameOver) return;
        
        Debug.Log("Level Complete!");
        // Aquí cargarías el siguiente nivel o mostrarías menú victoria
        if(victoryPanel != null) victoryPanel.SetActive(true);
        Time.timeScale = 0; // Pausa
    }
}
