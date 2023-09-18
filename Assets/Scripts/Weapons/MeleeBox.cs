using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeBox : MonoBehaviour
{
    public GameObject jugador;
    
    public void ResizeBox()
    {
        Weapon primaria = jugador.GetComponent<Inventory>().primaryWeapon;
        transform.parent.localScale = new Vector3(primaria.weaponWidth, transform.parent.localScale.y , primaria.weaponRange);
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.CompareTag("Enemy"))
        {   
            Weapon primaria = jugador.GetComponent<Inventory>().primaryWeapon;
            Enemy enemigo = col.gameObject.GetComponent<Enemy>();
            enemigo.RecieveDamage(primaria.weaponDamage);
        }
        else
        {

        }
    }
    
}