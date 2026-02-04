using UnityEngine;

/// <summary>
/// Efecto de flotado vertical para objetos (portales, powerups, etc)
/// </summary>
public class FlotarObjeto : MonoBehaviour
{
    [Header("Configuración de Flote")]
    [Tooltip("Cuánto sube y baja")]
    public float amplitud = 0.5f;
    
    [Tooltip("Qué tan rápido oscila")]
    public float frecuencia = 1f;

    private Vector3 posicionInicial;

    void Start()
    {
        // Guardar posición inicial del objeto
        posicionInicial = transform.position;
    }

    /// <summary>
    /// Movimiento sinusoidal vertical
    /// </summary>
    void Update()
    {
        // Calcular nueva posición Y usando función seno
        float nuevaY = posicionInicial.y + Mathf.Sin(Time.time * frecuencia) * amplitud;
       
        // Aplicar solo al eje Y, mantener X y Z fijos
        transform.position = new Vector3(posicionInicial.x, nuevaY, posicionInicial.z);
    }
}
