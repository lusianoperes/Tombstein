using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Jugador : MonoBehaviour
{
    public EffectManager effectManager;
    public PlayerController playerMovement;
    public GameManage gameManager;
    public int maxHp;
    public int currentHp;
    public float Fuerza;
    public float Destreza;
    public float Armadura;
    public float Estamina;
    public int Agilidad;

    public float headCurrentCooldown;
    public float armsCurrentCooldown;
    public float torsoCurrentCooldown;
    public float legsCurrentCooldown;
    

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
                maxHp = 750;
                playerMovement.Speed = 7f;
                Fuerza = 2f;
                Destreza = 10f;
                Armadura = 10f;
                Estamina = 5f;
                Agilidad = 2;
                break;
            case 1:
                maxHp = 200;
                playerMovement.Speed = 12f;
                Fuerza = 1f;
                Destreza = 30f;
                Armadura = 0f;
                Estamina = 8f;
                Agilidad = 5;
                break;
            case 2:
                maxHp = 150;
                playerMovement.Speed = 20f;
                Fuerza = 1.2f;
                Destreza = 20f;
                Armadura = 0f;
                Estamina = 20f;
                Agilidad = 1;
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
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManage>();
        currentHp=maxHp;
    }


   
    public void AgregarEfectoPasivo(EfectoPasivo efecto)
    {
        effectManager.efectosPasivos.Add(efecto);
    }

    public void ModificarVelocidad(float porcentajeVelocidad)
    {
        playerMovement.SpeedMultiple += porcentajeVelocidad;
    }
    public void ModificarArmadura(float armadura)
    {
        Armadura += armadura;
    }
    public void Curar(float vida)
    {
        currentHp += (int)vida;
        if(currentHp > maxHp)
        {
            currentHp = maxHp;
        }
    }
    public void ModificarFuerza(float fuerza)
    {
        Fuerza += fuerza;
    }
    public void ModificarDestreza(float destreza)
    {
        Destreza += destreza;
    }
    public void ModificarEstamina(float estamina)
    {
        Estamina += estamina;
    }
    public void ModificarAgilidad(float agilidad)
    {
        Agilidad += (int)agilidad;
    }
    /*public void ModificarCooldownBodypart(float bodypartCooldown)
    {
        headCurrentCooldown = headCurrentCooldown * (float)(bodypartCooldown * 0.01);
        armsCurrentCooldown = armsCurrentCooldown * (float)(bodypartCooldown * 0.01);
        torsoCurrentCooldown = torsoCurrentCooldown * (float)(bodypartCooldown * 0.01);
        legsCurrentCooldown = legsCurrentCooldown * (float)(bodypartCooldown * 0.01);
    }*/

    
    public void RecibirDanio(int damage)
    {
        currentHp -= damage;
        Debug.Log("recibiste " + damage);
        if(currentHp <= 0)
        {
            Debug.Log("Murio");
            gameManager.PlayerDied();

        }
    }

}
