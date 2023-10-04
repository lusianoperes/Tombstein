using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    public int damage;
    public float lastingTime;
    
    private IEnumerator LifeTime(float timeWhileAlive)
    {
        yield return new WaitForSeconds(timeWhileAlive);
        Destroy(gameObject);
    }
    
    void Start()
    {   
        //Uniform_ResizeAttack(size);
        StartCoroutine(LifeTime(lastingTime));

    }
    
    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            other.gameObject.GetComponent<Jugador>().RecibirDanio(damage);
        }
    }
    
    
    public void Uniform_ResizeAttack(float sizeVar)
    {
        transform.localScale = new Vector3(transform.localScale.x * sizeVar, transform.localScale.y * sizeVar, transform.localScale.z * sizeVar);
    }
}
