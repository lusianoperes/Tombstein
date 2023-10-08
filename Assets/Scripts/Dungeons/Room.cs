using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{   
    public int valorDeCelda;
    public int SalaSize;
    public bool IsClear = false;
    public enum Epoca
    {
        Egipto
    }
    public Epoca epoca;

    public enum Etapa
    {
        Primera
        //Segunda,
        //Tercera

    }

    public Etapa etapa;

    public enum TipoDeSala
    {
        Spawn,
        Enemigos,
        Obstaculos,
        Minijefe,
        Npc,
        Jefe
    }

    public TipoDeSala tipoDeSala;
    public DoorReferences doorReferences;


}