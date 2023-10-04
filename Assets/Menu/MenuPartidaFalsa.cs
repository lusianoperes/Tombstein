using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using TMPro;


public class MenuPartidaFalsa : MonoBehaviour
{
    public TextMeshProUGUI TMP_nombre;
    public Listas lista_Personajes;
    public Image imagen;
    // Start is called before the first frame update

    public int valor = 2;
        
    void Start()
    {   
    
        int valorRecibido = PlayerPrefs.GetInt("posicionPersonaje"); 
        Debug.Log(valorRecibido);
        valor = valorRecibido;
        Ficha personaje = lista_Personajes.ObtenerPersonaje(valorRecibido);
        TMP_nombre.text =  "El personaje que eligio es   "+ personaje.nombre_Personaje;
        imagen.sprite = personaje.imagen_personaje;
         
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
