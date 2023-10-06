using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    public int damage;
    public float lastingTime;
    public bool isProjectile; //La hitbox corresponde a un disparo o no
    public bool hasKnockback; //La hitbox causa retroceso al jugador
    protected Transform enemy;
    protected bool hasAppliedKnockback = false;
    
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
        if(other.CompareTag("Player"))
        {
            Jugador player = other.gameObject.GetComponent<Jugador>();
            player.RecibirDanio(damage);
            if (hasKnockback && !hasAppliedKnockback) { //Aplicar knockback al jugador
                player.ApplyKnockback(enemy);
                hasAppliedKnockback = true;
            }
        }
    }

    public void setEnemy (Transform position) { //Para saber donde esta el enemigo que lo creo (soluciona bugs del knockback)
        enemy = position;
    }
    
    public void Uniform_ResizeAttack(float sizeVar)
    {
        transform.localScale = new Vector3(transform.localScale.x * sizeVar, transform.localScale.y * sizeVar, transform.localScale.z * sizeVar);
    }
}
