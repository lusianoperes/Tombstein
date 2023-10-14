using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class EfectoPasivo : MonoBehaviour
{
    public float duracionEfecto;
    public float coeficienteEfecto;
    public Sprite effectSprite;

    public bool DarEfectoAlJugador_finished = false;
    public bool MostrarEfectoVisualmente_finished = false;
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
        //bodyPartCooldownReduction,
        poison
    }
    public EffectType effectType;

    public IEnumerator AplicarEfecto(Jugador jugador, GameObject objetoVisual)
    {
        jugador.effectManager.efectosPasivos.Add(this);
        Debug.Log("antes de dar efecto");

        StartCoroutine(DarEfectoAlJugador(jugador));
        StartCoroutine(MostrarEfectoVisualmente(jugador));
        
        yield return new WaitUntil(() => DarEfectoAlJugador_finished && MostrarEfectoVisualmente_finished);

        Debug.Log("Ambas corutinas han terminado");
        Destroy(objetoVisual);
    }

    public IEnumerator DarEfectoAlJugador(Jugador jugador)
    {
        Debug.Log("jugador");
        switch (effectType)
        {
            case EffectType.armorBoost:
                jugador.ModificarArmadura(coeficienteEfecto);
                yield return new WaitForSeconds(duracionEfecto);
                jugador.ModificarArmadura(-coeficienteEfecto);
                break;
            case EffectType.armorDebuff:
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
            /*case EffectType.bodyPartCooldownReduction:
                jugador.ModificarCooldownBodypart(coeficienteEfecto);
                yield return new WaitForSeconds(duracionEfecto);
                jugador.ModificarCooldownBodypart(1 / coeficienteEfecto);
                break;*/
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
        DarEfectoAlJugador_finished = true;
        yield break;
    }
    public IEnumerator MostrarEfectoVisualmente(Jugador jugador)
    {   
        Debug.Log("mostrar");
        float maxWidth = 129.14f;
        GameObject effectHolder = Instantiate(Resources.Load<GameObject>("Prefabs/EffectHolder"), jugador.effectManager.gameObject.transform);
        float duracion = duracionEfecto;
        effectHolder.transform.GetChild(0).GetComponent<Image>().sprite = effectSprite;

        while (duracion >= 0)
        {
            Debug.Log(duracion);
            effectHolder.transform.GetChild(1).GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(duracion * maxWidth / duracionEfecto, effectHolder.transform.GetChild(1).GetChild(0).GetComponent<RectTransform>().rect.height);
            duracion -= Time.deltaTime;
            Debug.Log(duracion);
            yield return null;
            Debug.Log("paso el tiempo pa");

        }
        MostrarEfectoVisualmente_finished = true;
        Destroy(effectHolder);
        yield break;
    }
}
