using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;

public class RegistroUIController : MonoBehaviour
{
    private VisualElement root;
    private TextField campoNom, campoApe, campoPass, campoConf;
    private Button botonCrear, botonVolver;
    private Label mensajeError;

    void Start() {
        var uiDoc = GetComponent<UIDocument>();
        root = uiDoc.rootVisualElement;

        // --- ACCESIBILIDAD ---
        if (PlayerPrefs.GetInt("ModoDaltonico", 0) == 1) root.AddToClassList("modo-daltonico-activado");
        float escala = PlayerPrefs.GetFloat("TamanoLetra", 100f) / 100f;
        root.Query<Label>().ForEach(l => l.style.fontSize = 25f * escala);

        campoNom = root.Q<TextField>("Campo_Nombre");
        campoApe = root.Q<TextField>("Campo_Apellido");
        campoPass = root.Q<TextField>("Campo_Contrasena");
        campoConf = root.Q<TextField>("Campo_Confirmar");
        botonCrear = root.Q<Button>("Boton_Crear");
        botonVolver = root.Q<Button>("Boton_Volver");
        mensajeError = root.Q<Label>("Mensaje_Error");

        botonCrear.clicked += OnCrearClick;
        botonVolver.clicked += () => SceneManager.LoadScene("LoginScene");
    }

    private void OnCrearClick() {
        if (campoNom.value.Length < 2) { MostrarError("Nombre muy corto"); return; }
        if (campoApe.value.Length < 2) { MostrarError("Apellido muy corto"); return; }
        if (campoPass.value.Length < 6) { MostrarError("Contraseña mínimo 6 caracteres"); return; }
        if (!Regex.IsMatch(campoPass.value, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$")) {
            MostrarError("Falta Mayúscula, Minúscula o Número"); return;
        }
        if (campoPass.value != campoConf.value) { MostrarError("No coinciden"); return; }

        botonCrear.SetEnabled(false);
        string user = campoNom.value.ToLower().Replace(" ", "");
        ApiManager.Instance.Registrar(user, campoNom.value, campoApe.value, campoPass.value, (exito, msg) => {
            botonCrear.SetEnabled(true);
            if (exito) SceneManager.LoadScene("LoginScene");
            else MostrarError(msg);
        });
    }

    private void MostrarError(string msg) {
        if (mensajeError != null) { mensajeError.text = "✗ " + msg; mensajeError.style.display = DisplayStyle.Flex; }
    }
}