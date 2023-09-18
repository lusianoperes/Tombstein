using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProjectile : MonoBehaviour
{
    public float hitWidth;
    public float hitLength;
    public float size;
    public float lastingTime;
    public int damage;

    public bool hasPiercing;

    private IEnumerator LifeTime(float timeWhileAlive)
    {
        yield return new WaitForSeconds(timeWhileAlive);
        Destroy(gameObject);
    }

    void Start()
    {
        StartCoroutine(LifeTime(lastingTime));

    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.CompareTag("Enemy"))
        {   
            Enemy enemigo = col.gameObject.GetComponent<Enemy>();
            enemigo.RecieveDamage(damage);
            if(!(hasPiercing))
            {
                Destroy(gameObject);
            }
            
        }
    }

    public void Uniform_ResizeBullet(float sizeVar)
    {
        transform.localScale = new Vector3(transform.localScale.x + sizeVar, transform.localScale.y + sizeVar, transform.localScale.z + sizeVar);
    }
}
