using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Jugador : MonoBehaviour
{
    public EffectManager effectManager;
    public PlayerController playerMovement;
    public int maxHp;
    public int currentHp;
    public float Fuerza;
    public float Destreza;
    public float Armadura;
    public float Estamina;
    public int Agilidad;
    public float finalStrength;
    
    public enum Personaje
    {
        Fredi,
        Pickle,
        Vagabond,
        Ringo
    }

    private Personaje personajeSelect;
    
    public void SetJugador(int persPos)
    {
        
        switch (persPos)
        {
            case 0:
                maxHp = 800;
                playerMovement.Speed = 6f;
                Fuerza = 50f;
                Destreza = 10f;
                Armadura = 0f;
                Estamina = 2f;
                Agilidad = 2;
                break;
            case 1:
                maxHp = 200;
                playerMovement.Speed = 12f;
                Fuerza = 10f;
                Destreza = 30f;
                Armadura = 0f;
                Estamina = 10f;
                Agilidad = 5;
                break;
            case 2:
                maxHp = 150;
                playerMovement.Speed = 20f;
                Fuerza = 15f;
                Destreza = 20f;
                Armadura = 0f;
                Estamina = 20f;
                Agilidad = 3;
                break;
            default:
                maxHp = 200;
                playerMovement.Speed = 6f;
                break;
        }
    }

    public void Start()
    {
        playerMovement = this.GetComponent<PlayerController>();
        SetJugador(PlayerPrefs.GetInt("posicionPersonaje"));
        currentHp=maxHp;
    }


    public void Update()
    {
        effectManager.ApplyEffects(this);
        
        

    }

    public void AgregarEfectoPasivo(IEfectoPasivo efecto)
    {
        effectManager.efectosPasivos.Add(efecto);
    }

    public void ModificarVelocidad(float porcentajeVelocidad)
    {
        playerMovement.SpeedMultiple += porcentajeVelocidad;
    }
    
    public void RecibirDanio(int damage)
    {
        currentHp -= damage;
    }

}
