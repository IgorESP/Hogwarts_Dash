using UnityEngine;

/// <summary> 
/// Selecciona y reproduce automáticamente la música correcta 
/// según la escena del nivel que esté cargada. 
/// </summary>
public class MusicaNivel : MonoBehaviour
{
    void Start()
    {   
        // Obtener el nombre de la escena actual
        string escena = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        // Seleccionar la música según el nombre de la escena
        switch (escena)
        {
            case "EscenaComedor":
                // Música del Comedor
                AudioManager.Instance.ReproducirMusicaComedor();
                break;

            case "EscenaGryffindor":
                // Música de Gryffindor
                AudioManager.Instance.ReproducirMusicaGryffindor();
                break;

            case "EscenaHufflepuff":
                // Música de Hufflepuff
                AudioManager.Instance.ReproducirMusicaHufflepuff();
                break;

            case "EscenaSlytherin":
                // Música de Slytherin
                AudioManager.Instance.ReproducirMusicaSlytherin();
                break;

            case "EscenaRavenclaw":
                // Música de Ravenclaw
                AudioManager.Instance.ReproducirMusicaRavenclaw();
                break;
        }
    }
}
