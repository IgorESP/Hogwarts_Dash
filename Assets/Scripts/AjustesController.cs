using System;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

/// <summary>
/// Controlador de la interfaz de ajustes (Opciones)
/// </summary>
public class AjustesController : MonoBehaviour
{
    private VisualElement root;
    
    // Referencias a elementos de UI
    private Slider sliderMusica, sliderSFX, sliderTamanoLetra;
    private Label labelMusica, labelSFX, labelTamanoLetra, mensajeVacio, labelPerfilNiveles, labelPerfilSnitches, labelPerfilIntentos;
    private Button botonRanking, botonPerfil, botonAtras, botonCerrarSesion, botonCreditos, botonTutorial, botonManual, botonCerrarRanking, botonCerrarPerfil, botonCerrarCreditos, botonCerrarManual, botonCerrarTutorial;
    private VisualElement panelRankingOverlay, panelPerfilOverlay, panelCreditosOverlay, panelManualOverlay, panelTutorialOverlay;
    private ScrollView listaRanking, contenidoManual, contenidoTutorial;

    void Start() 
    {
        Debug.Log("[Ajustes] Iniciando controlador de ajustes");

        // Obtenemos el root del UIDocument
        var uiDoc = GetComponent<UIDocument>();
        if (uiDoc != null) root = uiDoc.rootVisualElement;

        ObtenerReferencias();
        RegistrarEventos();
        CargarConfiguracion();
        AñadirSonidoABotones();
    }

    /// <summary> 
    /// Obtiene referencias a todos los elementos del UXML 
    /// </summary>
    private void ObtenerReferencias()
    {
        if (root == null) return;

        // Audio y Accesibilidad
        sliderMusica = root.Q<Slider>("Slider_Musica");
        sliderSFX = root.Q<Slider>("Slider_SFX");
        labelMusica = root.Q<Label>("Label_Musica");
        labelSFX = root.Q<Label>("Label_SFX");
        sliderTamanoLetra = root.Q<Slider>("Slider_TamanoLetra");
        labelTamanoLetra = root.Q<Label>("Label_TamanoLetra");

        // Botones principales
        botonRanking = root.Q<Button>("Boton_Ranking");
        botonAtras = root.Q<Button>("Boton_Atras");
        botonCerrarSesion = root.Q<Button>("Boton_CerrarSesion");
        botonCreditos = root.Q<Button>("Boton_Creditos");
        botonPerfil = root.Q<Button>("Boton_Perfil");
        botonTutorial = root.Q<Button>("Boton_Tutorial"); 
        botonManual = root.Q<Button>("Boton_Manual");

        // Paneles y Ranking
        panelRankingOverlay = root.Q<VisualElement>("PanelRankingOverlay");
        listaRanking = root.Q<ScrollView>("ListaRanking");
        mensajeVacio = root.Q<Label>("MensajeVacio");
        botonCerrarRanking = root.Q<Button>("Boton_CerrarRanking");

        // Perfil
        panelPerfilOverlay = root.Q<VisualElement>("PanelPerfilOverlay");
        botonCerrarPerfil = root.Q<Button>("Boton_CerrarPerfil");
        labelPerfilNiveles = root.Q<Label>("Label_PerfilNiveles");
        labelPerfilSnitches = root.Q<Label>("Label_PerfilSnitches");
        labelPerfilIntentos = root.Q<Label>("Label_PerfilIntentos");

        // Manual de usuario
        panelManualOverlay = root.Q<VisualElement>("PanelManualOverlay");
        contenidoManual = root.Q<ScrollView>("ContenidoManual");
        botonCerrarManual = root.Q<Button>("Boton_CerrarManual");

        // Tutorial
        panelTutorialOverlay = root.Q<VisualElement>("PanelTutorialOverlay");
        contenidoTutorial = root.Q<ScrollView>("ContenidoTutorial");
        botonCerrarTutorial = root.Q<Button>("Boton_CerrarTutorial");

        // Créditos
        panelCreditosOverlay = root.Q<VisualElement>("PanelCreditosOverlay");
        botonCerrarCreditos = root.Q<Button>("Boton_CerrarCreditos");
    }

    /// <summary> 
    /// Registra todos los eventos de botones y sliders 
    /// </summary>
    private void RegistrarEventos()
    {
        // Sliders
        if (sliderMusica != null) sliderMusica.RegisterValueChangedCallback(OnMusicaChanged);
        if (sliderSFX != null) sliderSFX.RegisterValueChangedCallback(OnSFXChanged);
        if (sliderTamanoLetra != null) sliderTamanoLetra.RegisterValueChangedCallback(OnTamanoLetraChanged);

        // Botones principales
        if (botonRanking != null) botonRanking.clicked += OnRankingClick;
        if (botonCerrarRanking != null) botonCerrarRanking.clicked += CerrarRanking;

        if (botonCreditos != null) botonCreditos.clicked += OnCreditosClick;
        if (botonCerrarCreditos != null) botonCerrarCreditos.clicked += CerrarCreditos;

        if (botonTutorial != null) botonTutorial.clicked += OnTutorialClick;
        if (botonCerrarTutorial != null) botonCerrarTutorial.clicked += CerrarTutorial;

        if (botonManual != null) botonManual.clicked += OnManualClick;
        if (botonCerrarManual != null) botonCerrarManual.clicked += CerrarManual;

        if (botonPerfil != null) botonPerfil.clicked += OnPerfilClick;
        if (botonCerrarPerfil != null) botonCerrarPerfil.clicked += CerrarPerfil;

        if (botonCerrarSesion != null) botonCerrarSesion.clicked += OnCerrarSesionClick;

        // Botón volver atrás
        if (botonAtras != null) 
            botonAtras.clicked += () => {
                Debug.Log("[Ajustes] Volviendo al menú principal");
                SceneManager.LoadScene("MenuPrincipalT");
            };
    }

    // --------------------------------------------------------- 
    // ---------------------- TUTORIAL -------------------------
    // ---------------------------------------------------------
    
    /// <summary> 
    /// Abre el panel del tutorial y genera su contenido dinámicamente. 
    /// </summary>
    private void OnTutorialClick()
    {
        if (panelTutorialOverlay != null)
            panelTutorialOverlay.style.display = DisplayStyle.Flex;

        if (contenidoTutorial != null)
        {
            contenidoTutorial.Clear();

            // Título
            contenidoTutorial.Add(new Label("TUTORIAL HOGWARTS DASH")
            {
                style = { fontSize = 28, unityFontStyleAndWeight = FontStyle.Bold, color = Color.cyan, marginBottom = 15 }
            });

            AgregarTitulo("Mecánicas de Juego");

            AgregarTarjeta("Salto Base", "Evita los obstáculos del suelo con un salto preciso.", "Tutorial/salto", "TAP / ESPACIO");
            AgregarTarjeta("Orbe Místico", "Tócala en el aire para impulsarte nuevamente.", "Tutorial/orbe", "TAP EN EL AIRE");
            AgregarTarjeta("Jump Pad", "Plataformas que te lanzan automáticamente.", "Tutorial/salto_jump_pad", "PASIVO");

            AgregarTitulo("Vehículos Disponibles");

            AgregarTarjeta("El Cubo", "Tu forma principal. Salto simple y efectivo.", "Tutorial/portal_cubo");
            AgregarTarjeta("Erizo Mágico", "Cambia la gravedad y se pega a superficies.", "Tutorial/bola");
            AgregarTarjeta("La Escoba", "Mantén para subir, suelta para caer.", "Tutorial/escoba");
            AgregarTarjeta("Gorro Mágico", "Permite múltiples saltos cortos en el aire.", "Tutorial/ufo");

            AgregarTitulo("Coleccionables");

            AgregarTarjeta("La Snitch Dorada","Encuentra las 3 snitch doradas en cada nivel para demostrar tu maestría.","Tutorial/snitch");
        }
    }

    /// <summary> 
    /// Añade un título centrado dentro del tutorial. 
    /// </summary>
    private void AgregarTitulo(string texto)
    {
        contenidoTutorial.Add(new Label(texto)
        {
            style = { fontSize = 26, unityFontStyleAndWeight = FontStyle.Bold, color = Color.yellow, marginTop = 25 }
        });
    }

    /// <summary> 
    /// Crea una tarjeta visual con imagen, título, descripción y controles opcionales. 
    /// </summary>
    private void AgregarTarjeta(string titulo, string descripcion, string rutaImagen, string control = "")
    {
        var card = new VisualElement(); 
        card.style.flexDirection = FlexDirection.Column; 
        card.style.backgroundColor = new Color(0.12f, 0.12f, 0.18f, 0.95f); 
        card.style.borderTopWidth = 3; 
        card.style.borderBottomWidth = 3; 
        card.style.borderLeftWidth = 3; 
        card.style.borderRightWidth = 3; 
        card.style.SetBorderColor(new Color(0.8f, 0.7f, 0.2f)); 
        card.style.marginBottom = 25; 
        card.style.paddingBottom = 20; 
        card.style.paddingTop = 20; 
        card.style.paddingLeft = 20; 
        card.style.paddingRight = 20; 
        card.style.SetBorderRadius(18);

        // Centrar contenido
        card.style.alignItems = Align.Center;
        card.style.justifyContent = Justify.Center;

        // Imagen
        AgregarImagen(rutaImagen, 180, card);

        // Título
        card.Add(new Label(titulo)
        {
            style = { fontSize = 22, unityFontStyleAndWeight = FontStyle.Bold, color = Color.cyan, marginTop = 10 }
        });

        // Descripción
        card.Add(new Label(descripcion)
        {
            style = { fontSize = 18, color = Color.white, whiteSpace = WhiteSpace.Normal }
        });

        // Controles
        if (!string.IsNullOrEmpty(control))
        {
            var ctrl = new Label(control);
            ctrl.style.backgroundColor = new Color(0, 1, 1, 0.1f);
            ctrl.style.color = Color.cyan;
            ctrl.style.unityFontStyleAndWeight = FontStyle.Bold;
            ctrl.style.marginTop = 10;
            ctrl.style.paddingLeft = 10;
            ctrl.style.paddingRight = 10;
            ctrl.style.SetBorderRadius(10);
            card.Add(ctrl);
        }

        contenidoTutorial.Add(card);
    }

    /// <summary> 
    /// Añade una imagen centrada dentro de un contenedor. 
    /// </summary>
    private void AgregarImagen(string ruta, int height = 200, VisualElement parent = null)
    {
        Texture2D tex = Resources.Load<Texture2D>(ruta);
        if (tex == null)
        {
            Debug.LogWarning($"[Tutorial] No se encontró la imagen: {ruta}");
            return;
        }

        var img = new VisualElement();
        img.style.height = height;
        img.style.width = 300;
        img.style.backgroundImage = new StyleBackground(tex);
        img.style.unityBackgroundScaleMode = ScaleMode.ScaleToFit;
        img.style.marginBottom = 15;

        if (parent != null) parent.Add(img);
        else contenidoTutorial.Add(img);
    }

    /// <summary> 
    /// Cierra el panel del tutorial. 
    /// </summary>
    private void CerrarTutorial()
    {
        if (panelTutorialOverlay != null)
            panelTutorialOverlay.style.display = DisplayStyle.None;
    }

    // --------------------------------------------------------- 
    // ----------------------- MANUAL -------------------------- 
    // ---------------------------------------------------------

    /// <summary> 
    /// Abre el panel del manual y genera su contenido dinámicamente. 
    /// </summary>
    private void OnManualClick()
    {
        if (panelManualOverlay != null)
            panelManualOverlay.style.display = DisplayStyle.Flex;

        if (contenidoManual != null)
        {
            contenidoManual.Clear();

            // Título
            contenidoManual.Add(new Label("Manual de Usuario - Hogwarts Dash")
            {
                style = { fontSize = 28, unityFontStyleAndWeight = FontStyle.Bold, color = Color.yellow, marginBottom = 15 }
            });

            // Secciones
            AgregarSeccion("1. Introducción",
                "El presente manual describe el funcionamiento del videojuego Hogwarts Dash, un juego de plataformas rítmico desarrollado en Unity para PC y Android. \n" +
                "En él, el jugador controla un cubo mágico que debe superar niveles inspirados en los escenarios de Hogwarts, saltando y esquivando obstáculos. \n" +
                "El objetivo principal es completar cada nivel, recolectando snitchs doradas y desbloquear nuevos niveles y skins."
            );

            AgregarSeccion("2. Controles del juego",
                "En PC, el jugador utilizará el ratón para navegar por los menús. Durante los niveles podrá usar el clic izquierdo o la barra espaciadora para saltar. \n" +
                "En Android, todos los controles serán con el dedo. Durante los niveles tendrá que pulsar la pantalla cada vez que quiera saltar."
            );

            AgregarSeccion("3. Menús",
                "Menú principal: elegir skins, niveles o ajustes.\n" +
                "Menú pausa: reanudar, reiniciar o volver al menú.\n" +
                "Menú ajustes: audio, accesibilidad, créditos, ranking, tutorial y manual."
            );
            
            AgregarSeccion("3.1. Menú principal",
                "En el menú principal encontrarás 3 botones. \n" +
                "Empezando por la izquierda al pulsar este botón mostrará otra escena con los diferentes colores de skins que puedes seleccionar (rojo, verde, amarillo y azul). \n" +
                "El botón central te mostrará la escena para elegir el nivel de juego. \n" +
                "Por último, el botón de la derecha te mostrará los ajustes del juego."
            );
            
            AgregarImagen("Manual/Menu_principal");

            AgregarSeccion("3.2. Menú pausa",
                "Para abrir el menú de pausa tendrás que hacer clic dentro de cualquier nivel en la parte superior derecha. \n" + 
                "Te mostrará un menú parecido al de abajo en el cual tendrás varias opciones. \n" +
                "Empezando con el botón con el logo de una casa, nos llevará al menú principal. Siguiendo con el botón del medio con el logo de play, reanuda la partida que estés haciendo en el mismo lugar. Por último, encontraremos un botón para reiniciar el nivel desde el inicio. \n" +
                "En la parte inferior encontrarás 2 barras deslizantes para cambiar el volumen de la música y de los efectos."
            );

            AgregarImagen("Manual/Menu_pausa");

            AgregarSeccion("3.3. Menú ajustes",
                "Aquí encontrarás muchas opciones para configurar o mirar. \n" +
                "Empezando de arriba hacia abajo, primero encontraremos los audios. En el cual podrás subir o bajar el volumen de la música y de los efectos. Después encontraremos la parte de accesibilidad. En la que podremos aumentar o disminuir la el tamaño de la letra. \n" +
                "Luego encontraremos varios botones para informarnos sobre diferentes temas. En créditos encontrarás los assets utilizados, la música utilizada y los desarrolladores del videojuego. El ranking te mostrará los 10 mejores jugadores. En tutorial te mostrará una ventana con los conocimientos básicos que necesitas saber para jugar. El manual de usuario te mostrará en una ventana el documento que estás leyendo ahora mismo. Por último, un botón para cerrar sesión la cuenta y volver al menú de login/registro."
            );
            
            AgregarImagen("Manual/Menu_ajustes");

            AgregarSeccion("4. Elegir nivel de juego",
                "Aquí podrás seleccionar el nivel que quieras. Eso sí tendrás que ir desbloqueando uno a uno para jugar a todos. En total encontrarás 5 niveles con temáticas de hogwarts (El comedor, Gryffindor, Slytherin, Hufflepuff y Ravenclaw)."
            );

            AgregarImagen("Manual/Nivel_juego");

            AgregarSeccion("5. Cambiar skins de personaje",
                "En esta sección podrás ver los diferentes personajes con los diferentes colores. Solo tendrás que seleccionar la casa (color) que más te guste y te mostrarán en la parte inferior el resultado."
            );

            AgregarImagen("Manual/Skins_personajes");

            AgregarSeccion("6. Contacto y créditos",
                "Desarrollado por el equipo HOGWARTS DASH. Para consultas o soporte técnico, contacte con el desarrollador responsable del proyecto."
            );
        }
    }

    /// <summary> 
    ///  Añade una sección de texto al manual.
    /// </summary>
    private void AgregarSeccion(string titulo, string contenido)
    {
        contenidoManual.Add(new Label(titulo)
        {
            style = { fontSize = 24, unityFontStyleAndWeight = FontStyle.Bold, color = Color.yellow, marginTop = 20 }
        });

        contenidoManual.Add(new Label(contenido)
        {
            style = { fontSize = 18, color = Color.white, whiteSpace = WhiteSpace.Normal, marginBottom = 10 }
        });
    }

    /// <summary> 
    ///  Añade una imagen al manual.
    /// </summary>
    private void AgregarImagen(string ruta)
    {
        Texture2D tex = Resources.Load<Texture2D>(ruta);
        if (tex == null)
        {
            Debug.LogWarning($"[Manual] No se encontró la imagen: {ruta}");
            return;
        }

        var img = new VisualElement();
        img.style.height = 400;
        img.style.backgroundImage = new StyleBackground(tex);
        img.style.unityBackgroundScaleMode = ScaleMode.ScaleToFit;
        img.style.marginBottom = 20;

        contenidoManual.Add(img);
    }

    /// <summary> 
    ///  Cierra el panel del manual.
    /// </summary>
    private void CerrarManual()
    {
        if (panelManualOverlay != null)
            panelManualOverlay.style.display = DisplayStyle.None;
    }

    // --------------------------------------------------------- 
    // ----------------------- AUDIO --------------------------- 
    // ---------------------------------------------------------

    /// <summary> 
    /// Cambia el volumen de la música 
    /// </summary>
    private void OnMusicaChanged(ChangeEvent<float> evt) 
    {
        float vol = evt.newValue;
        if (labelMusica != null) labelMusica.text = vol.ToString("F0") + "%";
        PlayerPrefs.SetFloat("VolumenMusica", vol);
        if (AudioManager.Instance != null) AudioManager.Instance.SetVolumenMusica(vol / 100f);
    }

    /// <summary> 
    /// Cambia el volumen de efectos 
    /// </summary>
    private void OnSFXChanged(ChangeEvent<float> evt) 
    {
        float vol = evt.newValue;
        if (labelSFX != null) labelSFX.text = vol.ToString("F0") + "%";
        PlayerPrefs.SetFloat("VolumenSFX", vol);
        if (AudioManager.Instance != null) AudioManager.Instance.SetVolumenSFX(vol / 100f);
    }

    // --------------------------------------------------------- 
    // -------------------- ACCESIBILIDAD ---------------------- 
    // ---------------------------------------------------------

    /// <summary> 
    /// Cambia el tamaño de letra y lo aplica a la UI 
    /// </summary>
    private void OnTamanoLetraChanged(ChangeEvent<float> evt) 
    {
        float tam = evt.newValue;
        if (labelTamanoLetra != null) labelTamanoLetra.text = tam.ToString("F0") + "%";
        PlayerPrefs.SetFloat("TamanoLetra", tam);
        AplicarTamanoLetraUI(tam);
    }

    /// <summary> 
    /// Aplica el tamaño de letra a todos los labels según su categoría 
    /// </summary>
    private void AplicarTamanoLetraUI(float porcentaje) 
    {
        float escala = porcentaje / 100f;
        root.Query<Label>().ForEach(label => {
            if (label.name != "Label_Musica" && label.name != "Label_SFX" && label.name != "Label_TamanoLetra")
            {
                float baseSize = DeterminarTamanoBase(label);
                label.style.fontSize = baseSize * escala;
            }
        });
    }

    /// <summary> 
    /// Determina un tamaño base según el tipo de texto y su posición en la jerarquía. 
    /// </summary>
    private float DeterminarTamanoBase(Label label)
    {
        string texto = label.text;
        if (string.IsNullOrEmpty(texto)) return 22f;
        
        // 1. Títulos de Pantalla (En UXML 50px)
        if (texto.Contains("CONFIGURACIÓN")) return 50f;
        
        // 2. Títulos de Paneles Flotantes (Perfil, Ranking, etc. en UXML 32px-38px)
        if (texto.Contains("TOP 10") || texto.Contains("MI PERFIL") || texto.Contains("MANUAL") || texto.Contains("TUTORIAL")) return 36f;
        
        // 3. Cabeceras de Sección (AUDIO, ACCESIBILIDAD, ASSETS, etc. en UXML 30px)
        if (label.name == "Titulo") return 50f; // Caso especial por nombre
        if (label.parent?.name == "Header") return 50f;

        // Si es una cabecera de sección dentro del scroll (30px)
        if (texto == "AUDIO" || texto == "ACCESIBILIDAD" || texto == "ASSETS UTILIZADOS" || texto == "MÚSICA" || texto == "EQUIPO DE DESARROLLO") return 30f;
        
        // 4. Botones de Opción (CRÉDITOS, RANKING, etc. en UXML 26px)
        if (label.parent is Button || (label.parent != null && label.parent.parent is Button)) return 26f;

        // 5. Etiquetas de Sliders y Contenido (En UXML 22px)
        if (texto.Contains("Música:") || texto.Contains("SFX:") || texto.Contains("Tamaño")) return 22f;

        // Default para el resto del contenido del manual/créditos
        return 22f;
    }

    // --------------------------------------------------------- 
    // ----------------------- RANKING ------------------------- 
    // ---------------------------------------------------------

    /// <summary> 
    /// Solicita el ranking al servidor y lo muestra 
    /// </summary>
    private void OnRankingClick() 
    {
        Debug.Log("[Ajustes] Solicitando datos de ranking...");

        if (panelRankingOverlay != null) 
            panelRankingOverlay.style.display = DisplayStyle.Flex;
        
        if (ApiManager.Instance != null)
        {
            ApiManager.Instance.ObtenerRanking((exito, lista) => {
                if (exito && lista != null) MostrarRankingEnUI(lista);
                else MostrarMensajeVacio();
            });
        }
    }

    /// <summary> 
    /// Cierra el panel de ranking 
    /// </summary>
    private void CerrarRanking() 
    {
        if (panelRankingOverlay != null) 
            panelRankingOverlay.style.display = DisplayStyle.None;
    }

    /// <summary> 
    /// Muestra el ranking recibido en la UI
    /// </summary>
    private void MostrarRankingEnUI(ApiManager.RankingEntry[] ranking) 
    {
        if (listaRanking == null) return;
        
        listaRanking.Clear();
        if (mensajeVacio != null) mensajeVacio.style.display = DisplayStyle.None;

        for (int i = 0; i < ranking.Length; i++) 
        {
            var entrada = ranking[i];

            // Fila contenedora
            var fila = new VisualElement();
            fila.style.flexDirection = FlexDirection.Row;
            fila.style.justifyContent = Justify.SpaceBetween;
            fila.style.paddingTop = 12;
            fila.style.marginBottom = 8;

            // Medalla o número
            string medalla = i == 0 ? "🥇" : i == 1 ? "🥈" : i == 2 ? "🥉" : (i + 1) + ".";

            fila.Add(new Label(medalla) { style = { minWidth = 50, fontSize = 24, color = Color.white } });
            fila.Add(new Label(entrada.username) { style = { flexGrow = 1, fontSize = 20, color = Color.grey } });
            fila.Add(new Label(entrada.puntuacion + " snitchs") { style = { fontSize = 22, color = Color.yellow } });
            
            listaRanking.Add(fila);
        }
    }

    /// <summary> 
    /// Muestra un mensaje cuando no hay ranking disponible 
    /// </summary>
    private void MostrarMensajeVacio() 
    {
        if (mensajeVacio != null) 
            mensajeVacio.style.display = DisplayStyle.Flex;
    }

    // --------------------------------------------------------- 
    // ----------------------- PERFIL -------------------------- 
    // ---------------------------------------------------------

    private void OnPerfilClick()
    {
        if (panelPerfilOverlay != null)
            panelPerfilOverlay.style.display = DisplayStyle.Flex;

        string playerId = PlayerPrefs.GetString("player_id", "");
        if (string.IsNullOrEmpty(playerId))
        {
            Debug.LogWarning("[Perfil] No hay player_id guardado");
            return;
        }

        ApiManager.Instance.ObtenerJugador(playerId, (exito, datos) =>
        {
            if (!exito || datos == null)
            {
                Debug.LogWarning("[Perfil] Error obteniendo datos del jugador");
                return;
            }

            var labelUsername = root.Q<Label>("Label_PerfilUsername");
            var labelFecha    = root.Q<Label>("Label_PerfilFecha");

            if (labelUsername != null) labelUsername.text = datos.username;
            if (labelPerfilNiveles != null) labelPerfilNiveles.text = $"{datos.niveles_completados}/5";
            if (labelPerfilSnitches != null) labelPerfilSnitches.text = datos.snitch_totales.ToString();
            if (labelPerfilIntentos != null) labelPerfilIntentos.text = datos.total_intentos.ToString();
            if (labelFecha != null) {
            if (datos.created_at != null) {
                DateTime fecha = DateTime.Parse(datos.created_at);
                labelFecha.text = fecha.ToString("dd/MM/yyyy");
            } else {
                labelFecha.text = "-";
            }
        }
        });
    }

    private void CerrarPerfil()
    {
        if (panelPerfilOverlay != null)
            panelPerfilOverlay.style.display = DisplayStyle.None;
    }

    // --------------------------------------------------------- 
    // ----------------------- CREDITOS ------------------------- 
    // ---------------------------------------------------------

    /// <summary> 
    /// Abre el panel de créditos 
    /// </summary>
    private void OnCreditosClick() 
    {
        if (panelCreditosOverlay != null) 
            panelCreditosOverlay.style.display = DisplayStyle.Flex;
    }

    /// <summary> 
    /// Cierra el panel de créditos 
    /// </summary>
    private void CerrarCreditos() 
    {
        if (panelCreditosOverlay != null) panelCreditosOverlay.style.display = DisplayStyle.None;
    }

    /// <summary> 
    /// Carga los valores guardados de audio y accesibilidad 
    /// </summary>
    private void CargarConfiguracion() 
    {
        float vMus = PlayerPrefs.GetFloat("VolumenMusica", 75f);
        if (sliderMusica != null) sliderMusica.value = vMus;
        
        float vSFX = PlayerPrefs.GetFloat("VolumenSFX", 75f);
        if (sliderSFX != null) sliderSFX.value = vSFX;

        float tLet = PlayerPrefs.GetFloat("TamanoLetra", 100f);
        if (sliderTamanoLetra != null) sliderTamanoLetra.value = tLet;

        // Aplica el volumen al AudioManager
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetVolumenMusica(vMus / 100f);
            AudioManager.Instance.SetVolumenSFX(vSFX / 100f);
        }
        
        Invoke("AplicarTamanoInicial", 0.1f);
    }

    private void AplicarTamanoInicial() 
    {
        AplicarTamanoLetraUI(PlayerPrefs.GetFloat("TamanoLetra", 100f));
    }

    // --------------------------------------------------------- 
    // ----------------------- CERRAR SESION ------------------------- 
    // ---------------------------------------------------------

    /// <summary>
    /// Cierra sesión y vuelve a la pantalla de login 
    /// </summary>
    private void OnCerrarSesionClick() 
    {
        Debug.Log("[Ajustes] Cerrando sesión y borrando credenciales persistentes");

        PlayerPrefs.DeleteKey("player_id");
        PlayerPrefs.DeleteKey("username");
        PlayerPrefs.DeleteKey("nombre");
        PlayerPrefs.Save();

        SceneManager.LoadScene("LoginScene");
    }

    /// <summary> 
    /// Añade sonido de clic a todos los botones del menú de ajustes. 
    /// </summary>
    private void AñadirSonidoABotones()
    {
        if (root == null) return;

        var botones = root.Query<Button>().ToList();

        foreach (var b in botones)
        {
            b.clicked += () => AudioManager.Instance.PlayBoton();
        }
    }

}