using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EfectoPasivo : MonoBehaviour
{
    public float duracionEfecto;
    public float coeficienteEfecto;
    public enum EffectType
    {
        armorBoost,
        armorDebuff,
        instantHealth,
        healthRegeneration,
        strenghtBoost,
        strenghtDebuff,
        dexterityBoost,
        dexterityDebuff,
        staminaBoost,
        staminaDebuff,
        agilityBoost,
        agilityDebuff,
        speedBoost,
        speedDebuff,
        bodyPartCooldownReduction,
        poison
    }
    public EffectType effectType;

    public IEnumerator AplicarEfecto(Jugador jugador)
    {
        switch (effectType)
        {
            case EffectType.armorBoost :
                jugador.ModificarArmadura(coeficienteEfecto);
                yield return new WaitForSeconds(duracionEfecto);
                jugador.ModificarArmadura(-coeficienteEfecto);
                break;
            case EffectType.armorDebuff :
                jugador.ModificarArmadura(-coeficienteEfecto);
                yield return new WaitForSeconds(duracionEfecto);
                jugador.ModificarArmadura(coeficienteEfecto);
                break;
            case EffectType.instantHealth:
                jugador.Curar(coeficienteEfecto);
                break;
            case EffectType.healthRegeneration:
                for (int i = 0; i < duracionEfecto; i++)
                {
                    jugador.Curar(coeficienteEfecto);
                    yield return new WaitForSeconds(1);
                }
                break;
            case EffectType.strenghtBoost:
                jugador.ModificarFuerza(coeficienteEfecto);
                yield return new WaitForSeconds(duracionEfecto);
                jugador.ModificarFuerza(-coeficienteEfecto);
                break;
            case EffectType.strenghtDebuff:
                jugador.ModificarFuerza(-coeficienteEfecto);
                yield return new WaitForSeconds(duracionEfecto);
                jugador.ModificarFuerza(coeficienteEfecto);
                break;
            case EffectType.dexterityBoost:
                jugador.ModificarDestreza(coeficienteEfecto);
                yield return new WaitForSeconds(duracionEfecto);
                jugador.ModificarDestreza(-coeficienteEfecto);
                break;
            case EffectType.dexterityDebuff:
                jugador.ModificarDestreza(-coeficienteEfecto);
                yield return new WaitForSeconds(duracionEfecto);
                jugador.ModificarDestreza(coeficienteEfecto);
                break;
            case EffectType.staminaBoost:
                jugador.ModificarEstamina(coeficienteEfecto);
                yield return new WaitForSeconds(duracionEfecto);
                jugador.ModificarEstamina(-coeficienteEfecto);
                break;
            case EffectType.staminaDebuff:
                jugador.ModificarEstamina(-coeficienteEfecto);
                yield return new WaitForSeconds(duracionEfecto);
                jugador.ModificarEstamina(coeficienteEfecto);
                break;
            case EffectType.agilityBoost:
                jugador.ModificarAgilidad(coeficienteEfecto);
                yield return new WaitForSeconds(duracionEfecto);
                jugador.ModificarAgilidad(-coeficienteEfecto);
                break;
            case EffectType.agilityDebuff:
                jugador.ModificarAgilidad(-coeficienteEfecto);
                yield return new WaitForSeconds(duracionEfecto);
                jugador.ModificarAgilidad(coeficienteEfecto);
                break;
            case EffectType.speedBoost:
                jugador.ModificarVelocidad(coeficienteEfecto);
                yield return new WaitForSeconds(duracionEfecto);
                jugador.ModificarVelocidad(-coeficienteEfecto);
                break;
            case EffectType.speedDebuff:
                jugador.ModificarVelocidad(-coeficienteEfecto);
                yield return new WaitForSeconds(duracionEfecto);
                jugador.ModificarVelocidad(coeficienteEfecto);
                break;
            case EffectType.bodyPartCooldownReduction:
                jugador.ModificarCooldownBodypart(coeficienteEfecto);
                yield return new WaitForSeconds(duracionEfecto);
                jugador.ModificarCooldownBodypart(1 / coeficienteEfecto);
                break;
            case EffectType.poison:
                for (int i = 0; i < duracionEfecto; i++)
                {
                    jugador.RecibirDanio((int)coeficienteEfecto);
                    yield return new WaitForSeconds(1);
                }
                break;
            default:

                break;
        }
    }
}
