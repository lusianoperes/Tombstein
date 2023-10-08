using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    public int damage;
    public float lastingTime;
    public bool isProjectile; //La hitbox corresponde a un disparo o no
    public bool hasKnockback; //La hitbox causa retroceso al jugador
    public float knockbackForce;
    public float knockbackTime;
    protected Vector3 knockbackDirection;
    protected bool hasDoneDamage = false;
    protected bool hasAppliedKnockback = false;
    //Faltan añadir efectos
    
    protected IEnumerator LifeTime(float timeWhileAlive)
    {
        yield return new WaitForSeconds(timeWhileAlive);
        Destroy(gameObject);
    }
    
    protected void Start()
    {   
        //Uniform_ResizeAttack(size);
        StartCoroutine(LifeTime(lastingTime));
    }
    
    protected void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player") && !hasDoneDamage)
        {
            Jugador player = other.gameObject.GetComponent<Jugador>();
            player.RecibirDanio(damage);
            hasDoneDamage = true;
            if (hasKnockback && !hasAppliedKnockback) { //Aplicar knockback al jugador
                other.gameObject.GetComponent<PlayerController>().ApplyKnockback(knockbackDirection, knockbackTime, knockbackForce);
                hasAppliedKnockback = true;
            }
        }
        if (isProjectile) {
            Destroy(gameObject);
        }
    }

    public void setKnockbackDirection (Vector3 direction) {
        knockbackDirection = direction;
    }
    
    public void Uniform_ResizeAttack(float sizeVar)
    {
        transform.localScale = new Vector3(transform.localScale.x * sizeVar, transform.localScale.y * sizeVar, transform.localScale.z * sizeVar);
    }
}
