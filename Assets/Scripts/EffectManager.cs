using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Reflection;

public class EffectManager : MonoBehaviour
{
    public List<EfectoPasivo> efectosPasivos = new List<EfectoPasivo>();

    public GameObject jugadoroObj;
    public Jugador jugador;
    public void Start()
    {
        jugadoroObj = GameObject.Find("Jugador");
        jugador = jugadoroObj.GetComponent<Jugador>();
    }
    /*public void Update()
    {
        //ApplyEffects(jugador);
    }
    public void ApplyEffects(Jugador jugador)
    {
        foreach (EfectoPasivo efecto in efectosPasivos)
        {
            StartCoroutine(efecto.AplicarEfecto(jugador));
        }

        efectosPasivos.Clear();
    }*/
}
