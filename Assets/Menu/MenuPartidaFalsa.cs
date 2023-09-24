using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuPartidaFalsa : ScriptableObject
{
     public Text texto_se_guardo;
    int valorRecibido = PlayerPrefs.GetInt("contadorDePersonajes");
    // Start is called before the first frame update

  public int valor;
        
    void Start()
    {   
      valor = valorRecibido;
         texto_se_guardo.text =  "El personaje que eligio es   "+ valorRecibido.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
