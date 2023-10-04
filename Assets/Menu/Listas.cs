using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Listas : ScriptableObject
{
    public List<Ficha> personajes;

    public int contadorDePersonajes
    {
        get
        {
            return personajes.Count;
        }
    }

    public Ficha ObtenerPersonaje(int index)
    {
        return personajes[index];
    }
}
