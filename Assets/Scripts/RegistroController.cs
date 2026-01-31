using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class RegistroUIController : MonoBehaviour
{
    private UIDocument uiDocument;
    private VisualElement root;
    private TextField campoNombre, campoApellido, campoContrasena, campoConfirmar;
    private Button botonCrear, botonVolver;
    private Label mensajeError;
    
    void Start()
    {
        uiDocument = GetComponent<UIDocument>();
        if (uiDocument == null) return;
        
        root = uiDocument.rootVisualElement;
        campoNombre = root.Q<TextField>("Campo_Nombre");
        campoApellido = root.Q<TextField>("Campo_Apellido");
        campoContrasena = root.Q<TextField>("Campo_Contrasena");
        campoConfirmar = root.Q<TextField>("Campo_Confirmar");
        botonCrear = root.Q<Button>("Boton_Crear");
        botonVolver = root.Q<Button>("Boton_Volver");
        mensajeError = root.Q<Label>("Mensaje_Error");
        
        ForzarTextoNegro();
        OcultarError();
        
        if (botonCrear != null) botonCrear.clicked += OnCrearClick;
        if (botonVolver != null) botonVolver.clicked += OnVolverClick;
        
        // Limpiar error al escribir
        campoNombre?.RegisterValueChangedCallback(evt => OcultarError());
        campoApellido?.RegisterValueChangedCallback(evt => OcultarError());
    }
    
    private void ForzarTextoNegro()
    {
        var campos = new[] { campoNombre, campoApellido, campoContrasena, campoConfirmar };
        foreach (var campo in campos)
        {
            if (campo != null) campo.style.color = new StyleColor(Color.black);
        }
    }

    private void OnVolverClick()
    {
        SceneManager.LoadScene("LoginScene"); 
    }

    private void MostrarError(string mensaje)
    {
        if (mensajeError != null)
        {
            mensajeError.text = mensaje;
            mensajeError.style.display = DisplayStyle.Flex;
            mensajeError.style.color = Color.red;
        }
    }

    private void OcultarError()
    {
        if (mensajeError != null)
            mensajeError.style.display = DisplayStyle.None;
    }
    
    private void OnCrearClick()
    {
        string nombre = campoNombre?.value ?? "";
        string apellido = campoApellido?.value ?? "";
        string contrasena = campoContrasena?.value ?? "";
        string confirmar = campoConfirmar?.value ?? "";
        
        if (string.IsNullOrEmpty(nombre) || string.IsNullOrEmpty(apellido) || 
            string.IsNullOrEmpty(contrasena) || string.IsNullOrEmpty(confirmar))
        {
            MostrarError("Todos los campos son obligatorios");
            return;
        }
        
        if (contrasena != confirmar)
        {
            MostrarError("Las contraseñas no coinciden");
            return;
        }

        if (!ValidarRequisitosContrasena(contrasena))
        {
            MostrarError("La contraseña debe tener: Mayús, minús, Número y Símbolo");
            return;
        }
        
        string usuario = $"{nombre}.{apellido}".ToLower();
        PlayerPrefs.SetString($"User_{usuario}_Pass", contrasena);
        PlayerPrefs.Save();
        
        Debug.Log($"¡REGISTRO EXITOSO! Usuario: {usuario}");
        SceneManager.LoadScene("MenuPrincipalT");
    }

    private bool ValidarRequisitosContrasena(string contrasena)
    {
        if (contrasena.Length < 8) return false;
        bool hasUpper = false, hasLower = false, hasDigit = false, hasSymbol = false;
        foreach (char c in contrasena)
        {
            if (char.IsUpper(c)) hasUpper = true;
            if (char.IsLower(c)) hasLower = true;
            if (char.IsDigit(c)) hasDigit = true;
            if (!char.IsLetterOrDigit(c)) hasSymbol = true;
        }
        return hasUpper && hasLower && hasDigit && hasSymbol;
    }
}