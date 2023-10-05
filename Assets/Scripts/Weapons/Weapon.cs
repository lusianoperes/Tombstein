using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : Item
{   
    
    public enum WeaponType
    {
        Primary,
        Secondary
    }
    public WeaponType weaponType;

    public AudioSource hitsound;      

    public int weaponDamage;
    public float attackDuration;
    public float weaponRange; //rango de arma en melee , tiempo maximo mantenido en holdableDistance
    
    public float delayBefore;
    public float timeDuring;
    public float delayAfter;
    public float cooldown;

}


