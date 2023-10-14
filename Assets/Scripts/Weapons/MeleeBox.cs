using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeBox : MonoBehaviour
{
    public int damage = 0;
    public Melee weaponData;
    private float lastingTime = 0.1f;
    
    private IEnumerator LifeTime(float timeWhileAlive)
    {
        yield return new WaitForSeconds(timeWhileAlive);
        Destroy(transform.parent.gameObject);
    }

    void Start(){
        InitializeBoxValues(weaponData.weaponDamage, weaponData.fullPlayerReference, weaponData.attackDuration);
        StartCoroutine(LifeTime(lastingTime));
    }
    public void InitializeBoxValues(int finalDamge, GameObject playerRef, float duringTime)
    {      
        
        Transform meleePoint = playerRef.transform.Find("MeleeSpawn");
        damage = (int)Mathf.Round((finalDamge * playerRef.GetComponent<Jugador>().Fuerza));
        transform.parent.rotation = meleePoint.rotation;
        lastingTime = duringTime;
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