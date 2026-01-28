using UnityEngine;


public class FlotarObjeto : MonoBehaviour
{
    [Header("Configuración de Flote")]
    public float amplitud = 0.5f; // Cuánto sube y baja
    public float frecuencia = 1f;  // Qué tan rápido lo hace


    private Vector3 posicionInicial;


    void Start()
    {
        // Guardamos la posición donde pusiste el portal en el mapa
        posicionInicial = transform.position;
    }


    void Update()
    {
        // Calculamos la nueva posición usando el tiempo y el seno
        float nuevaY = posicionInicial.y + Mathf.Sin(Time.time * frecuencia) * amplitud;
       
        // Aplicamos solo al eje Y
        transform.position = new Vector3(posicionInicial.x, nuevaY, posicionInicial.z);
    }
}
