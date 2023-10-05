using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeBox : MonoBehaviour
{
    public int damage = 0;
    
    private IEnumerator LifeTime(float timeWhileAlive)
    {
        yield return new WaitForSeconds(timeWhileAlive);
        Destroy(transform.parent.gameObject);
    }

    void Start(){
        Transform FirstP = transform.parent;
        Melee weaponData = FirstP.transform.parent.GetComponent<Melee>();
        InitializeBoxValues(weaponData.weaponDamage, weaponData.fullPlayerReference.transform.Find("MeleeSpawn") );
        StartCoroutine(LifeTime(weaponData.attackDuration));
    }
    public void InitializeBoxValues(int finalDamge, Transform meleePoint)
    {
        damage = finalDamge;
        transform.parent.rotation = meleePoint.rotation;
    }

    void OnTriggerEnter(Collider col)
    {   
        if (col.gameObject.CompareTag("Enemy"))
        {   
            Enemy enemigo = col.gameObject.GetComponent<Enemy>();
            enemigo.RecieveDamage(damage);
        }
    }
    
}// hacer que el rescalado sea por medio del padre, el instanciado es el padre, luego se accede ahijo y se edita