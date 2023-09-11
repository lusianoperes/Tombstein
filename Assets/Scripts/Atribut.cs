using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Atribut : MonoBehaviour
{   
    public int cantidad;
    

    public void setCantidad(int cantidad)
    {
        this.cantidad = cantidad;
    }

    public int getCantidad()
    {
        return this.cantidad;
    }
}
