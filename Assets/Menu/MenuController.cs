using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.Audio;



public class MenuController : MonoBehaviour
{
    public GameObject menu1;//Archivos
    
    public GameObject menu2;//general

    public GameObject menu3;//Seleccion 
    
    public GameObject menu4;//Opciones

    public GameObject menu5;//Audio
    public GameObject menu6;//graficos
   public AudioSource audioSource1; // Reference to the first Audio Source
   public AudioSource audioSource2; // Reference to the second Audio Source
    
    public GameObject botones_archivo; 

    public Listas lista_Personajes;
   
    public GameObject esqueleto1; 
    public GameObject esqueleto12; 
   
   
    public TextMeshProUGUI TMP_nombre;
    public TextMeshProUGUI TMP_descripcion;
    public TextMeshProUGUI TMP_puntos;
  

    public int contadorDePersonajes = 0;

    public int menuActual = 0; 

    public int archivo = 0; //no hay ninguno seleccionado 
    
   // private int contador_personajes = 0 ; 

    /*
    private void awake(){
        if (MenuController.Instance == null)
        {
            MenuController.Instance = this;
            DontDestroyOnload(this.MenuController);
        }
        else{
            Destroy(MenuController);
        }
    }
    */

    public Slider volumeFX;
    public Slider volumeMaster;
    //public Toggle mute;
    public AudioMixer mixer;
   // public AudioSource fxSource;
    //public AudioClip clickSound;

    private float lastVolume;
    
     private void Awake()
    {
        volumeFX.onValueChanged.AddListener(ChangeVolumeFX);
        volumeMaster.onValueChanged.AddListener(ChangeVolumeMaster);
    }


    private void Start()
    {
        // Set menu1 and menu2 inactive at the beginning
        menu1.SetActive(false);
        menu3.SetActive(false);
        menu2.SetActive(false);
        //botones_archivo.SetActive(false);
        menu4.SetActive(false);
        menu5.SetActive(false);
          menu6.SetActive(false);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) || Input.anyKeyDown)
        {
            if (menuActual == 0)
            {   
                ActivateMenu1();
                menuActual = 1;  
            }
        }
    }

    private void ActivateMenu1()
    {
        // Set menu0 inactive, menu1 active
        gameObject.SetActive(false);
        menu1.SetActive(true);
        
        botones_archivo.SetActive(true);
    
        //audioSource1.Play(); 
       
    }

    private void ActivateMenu2()
    {
        // Set menu1 inactive, menu2 active
        menu1.SetActive(false);
        menu2.SetActive(true);
        menu3.SetActive(false);
        menu4.SetActive(false);
        botones_archivo.SetActive(false);
        menuActual = 2;
        audioSource2.Play();
    }
   
        private void ActivateMenu3()
        {
            menuActual = 3; 
            menu3.SetActive(true);
            menu2.SetActive(false);
            menu1.SetActive(false);          
         }

    
    public void cargarArchivo(int archivo_Aguardar)
    {
        archivo = archivo_Aguardar;
        menuActual = 2; 
        ActivateMenu2();
         
    }

    public void SeleccionDePersonajes(){
        //SceneManager.LoadScene("Partida");
        //Faltaria agregar cuando tengamos manejo de archivo de la partida guardada 
        // En esta funcion se cargara el nivel 
        // a partir de la variable:  
        // ARCHIVO  
        //ya que será la toma de decision del personaje
        menuActual = 3;
        ActivateMenu3();  
        ActualizarInformacion();
     
    }

      public void ActualizarInformacion()
    {

        Ficha personaje = lista_Personajes.ObtenerPersonaje(contadorDePersonajes);
        //personaje.objeto_Jugador.SetActive(true);
        
        // personaje.sprite= lista_Personajes[contadorDePersonajes].Objeto_Jugador;
        /*Personaje personajesopo = lista_Personajes[contador_personajes];
        personajesopo.objetoimagen();
        personajesopo.Objeto_Jugador.SetActive(True);
       
        //lista_Personajes.contador_personajes.Objeto_Jugador.SetActive(true);
*/        
 
         if(contadorDePersonajes == 0 ){        
        esqueleto1.SetActive(true);
        esqueleto12.SetActive(false);
        
        }
        else {
            esqueleto12.SetActive(true);
            esqueleto1.SetActive(false);
        }
       
    /*
        for(int i =0; i <= lista_Personajes.contadorDePersonajes; i++)
        {
            if(i== contadorDePersonajes){
                lista_Personajes.personajes[contadorDePersonajes].objeto_Jugador.SetActive(true);
            }
            else{
                 lista_Personajes.personajes[i].objeto_Jugador.SetActive(false);
                 }
        }
*/
        TMP_nombre.text = personaje.nombre_Personaje;
        TMP_descripcion.text = personaje.descripcion_Personaje;
        TMP_puntos.text =  personaje.Puntos_Base;
        Guardar();
    }  



 public void AnteriorPersonaje()
    {   
        
       // Ficha personaje = lista_Personajes.ObtenerPersonaje(contadorDePersonajes);
       // personaje.objeto_Jugador.SetActive(false);
    
        contadorDePersonajes--;

        if (contadorDePersonajes < 0)
        {
          
            contadorDePersonajes = lista_Personajes.contadorDePersonajes - 1;
          
        }
        
        ActualizarInformacion();
       
    }
    public void SiguientePersonaje()
    {   
        /*Personaje personajesopo = lista_Personajes[contador_personajes];
        personajesopo.objetoimagen();
        personajesopo.Objeto_Jugador.SetActive(False);
              // lista_Personajes.contadorDePersonajes.Objeto_Jugador.SetActive(false);
        */
        //Ficha personaje = lista_Personajes.ObtenerPersonaje(contadorDePersonajes);
        //personaje.objeto_Jugador.SetActive(false);
       
        contadorDePersonajes++;
        if (contadorDePersonajes >= lista_Personajes.contadorDePersonajes)
        {
            
            contadorDePersonajes = 0;
        }

        ActualizarInformacion();
        
    }

 public void Guardar()
    {
        PlayerPrefs.SetInt("contadorDePersonajes", contadorDePersonajes);
    }



    public void  salirDelJuego()
    {
        Application.Quit();
    }

    public void  Volver()
    {   
       if(menuActual == 5 ||   menuActual == 6){
        Opciones();
       }else {
        if(menuActual == 3 ||   menuActual == 4)
        {
            ActivateMenu2();
        }
        else
        {
            if(menuActual == 2)
            {
                ActivateMenu1();
            }
            else{
                        Application.Quit();
            }
        }
        }
    }

    public void CargarNivel()
    {
        contadorDePersonajes = PlayerPrefs.GetInt("contadorDePersonajes");
        SceneManager.LoadScene("Partida_Falsa");
    }
    
    public void Opciones(){
       
            menu3.SetActive(false);
            menu2.SetActive(false);
            menu1.SetActive(false);          
            menu4.SetActive(true);
            menu5.SetActive(false);
            menu6.SetActive(false);
            menuActual = 4;
    }  

    public void Audio(){
            menu3.SetActive(false);
            menu2.SetActive(false);
            menu1.SetActive(false);          
            menu4.SetActive(false);
            menu5.SetActive(true);
            menuActual = 5;

    }
     
    public void graficos(){
       
            menu3.SetActive(false);
            menu2.SetActive(false);
            menu1.SetActive(false);          
            menu4.SetActive(false);
            menu5.SetActive(false);
            menu6.SetActive(true);
            menuActual = 6;
    }  

    public void ChangeVolumeMaster(float v)
    {
        mixer.SetFloat("VolMaster", v);
    }
    public void ChangeVolumeFX(float v)
    {
        mixer.SetFloat("VolFX", v);
    }

}   