using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class TiempoArch : MonoBehaviour
{
    public Text tiempo_arch1;
    public Text tiempo_arch2;
    public Text tiempo_arch3;
    public Text tiempo_arch4;
   
    void Start()
    {
        tiempo_arch1.text =  "01:01:01hs";
        tiempo_arch2.text =  "02:22:22hs";
        tiempo_arch3.text =  "03:33:33hs";
        tiempo_arch4.text =  "04:44:44hs";
        // se usará en el futuro para 
        // tomar la informacion de las horas jugadas
    }

    void Update()
    {
        
    }
}
