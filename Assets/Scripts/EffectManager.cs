using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Reflection;

public class EffectManager : MonoBehaviour
{
    public List<IEfectoPasivo> efectosPasivos = new List<IEfectoPasivo>();

    public void ApplyEffects(Jugador jugador)
    {
        foreach (IEfectoPasivo efecto in efectosPasivos)
        {
            StartCoroutine(efecto.AplicarEfecto(jugador));
        }

        efectosPasivos.Clear();
    }
}

public interface IEfectoPasivo
{
    float duracionEfecto { get; set; }
    IEnumerator AplicarEfecto(Jugador jugador);

}

public class ModificacionVelocidad : IEfectoPasivo
{
    //Los enemigos te pueden enviar un -0.3 de velocidad lo que hace que speedMultiple de jugador pase de 1 a 0.7
    // y los efectos positivos de velocidad un 0.3 que lo vuelve 1.03
    public float duracionEfecto { get; set; }
    public float porcentajeVelocidad;

    public ModificacionVelocidad(float multiplicadoreVel, float duracion)
    {
        porcentajeVelocidad = multiplicadoreVel;
        duracionEfecto = duracion;
    }

    public IEnumerator AplicarEfecto(Jugador jugador)
    {
        jugador.ModificarVelocidad(porcentajeVelocidad);

        yield return new WaitForSeconds(duracionEfecto);

        jugador.ModificarVelocidad(-porcentajeVelocidad);
    }
}
