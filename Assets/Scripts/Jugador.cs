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

    public float finalDamage;

    public enum Personaje
    {
        Fredi,
        Pickle,
        Vagabond,
        Ringo
    }

    public Personaje personajeSelect;

    public void SetJugador(Personaje pers)
    {
        
        switch (pers)
        {
            case Personaje.Pickle:
                maxHp = 800;
                playerMovement.Speed = 6f;
                break;
            case Personaje.Vagabond:
                maxHp = 200;
                playerMovement.Speed = 7f;
                break;
            case Personaje.Ringo:
                maxHp = 150;
                playerMovement.Speed = 7f;   
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
        SetJugador(personajeSelect);
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

    public void ApplyKnockback (Transform enemy) {
        StartCoroutine(knockback(enemy));
    }

    protected IEnumerator knockback (Transform enemy) //Pendiente de ver que no parezca un tp a otra posicion
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        Vector3 knockbackDirection = (transform.position - enemy.position).normalized;
        knockbackDirection.y = 0;
        rb.AddForce(100 * knockbackDirection, ForceMode.Impulse);
        yield return new WaitForSeconds(5);
        rb.velocity = Vector3.zero;
    }
}
