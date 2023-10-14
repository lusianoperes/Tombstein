using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Mathf;

public class Distance : Weapon
{
    public float bulletSpeed;
    public PlayerProjectile bulletPrefab;


    public enum DistanceType
    {
        triggerOnly, 
        triggerHoldable,
        toggleOnly,
        toggleHoldable
    }
    
    
    /*
    triggerOnly:
    int weaponDamage -- Daño de la bala
    float weaponWidth -- No se usa
    float weaponRange -- No se usa
    
    float delayBefore -- Inicio del disparo post click
    float timeDuring -- Duración de bala en el aire
    
    float delayAfter -- Delay interno del disparo
    float cooldown  -- Cada cuanto se puede disparar (cadencia)
----<----<----<----------<----------<----<-----------<---------<-------------
    triggerHoldable:
    int weaponDamage --- Daño base de la bala

    float weaponWidth --- No se Usa  --------> La cantidad de multiplicaciones de daño, forma en la que ocurren y demás variables
    float weaponRange --- Tiempo hasta el escalado. ---------------> se van a manejar localmente en la función DoFire especifica del arma.
    
    float delayBefore --- Inicio del disparo post click
    float timeDuring --- Duración de bala en el aire

    float delayAfter --- Delay Interno del disparo
    float cooldown --- Cada cuanto se puede iniciar un disparo (cadencia)
----<-----------------<--------------<--------------<--------------------
    toggleHoldable:
    int weaponDamage --- Daño base de la bala

    float weaponWidth --- No se Usa  --------> La cantidad de multiplicaciones de daño, forma en la que ocurren y demás variables
    float weaponRange --- Tiempo hasta el escalado. ---------------> se van a manejar localmente en la función DoFire especifica del arma.
    
    float delayBefore --- Inicio del disparo post click
    float timeDuring --- Duración de bala en el aire

    float delayAfter --- Delay Interno del disparo
    float cooldown --- Cada cuanto se puede disparar (cadencia)
-------<<----------------<------<<-----------<--------<---------------------
    */
   // fullPlayerReference.transform.Find("BulletSpawn");//canon point

    public DistanceType distanceType;
    
    public virtual IEnumerator TriggerOnly_DoFire() 
    {
        

        playerControllerReference.isDoinSomething = true; //Comienza Disparo
        var canonPoint = fullPlayerReference.transform.Find("BulletSpawn");
        yield return new WaitForSeconds(delayBefore);
        
        hitsound.Play();
        var bullet = Instantiate(bulletPrefab, canonPoint.transform.position, Quaternion.identity); //Disparo Ocurriendo. Aplica Daño.
        PlayerProjectile bulletData = bullet.GetComponent<PlayerProjectile>();
        bulletData.damage = weaponDamage;
        bulletData.lastingTime = timeDuring;
        
        bullet.GetComponent<Rigidbody>().velocity = canonPoint.transform.forward * bulletSpeed;

        bullet.transform.rotation = canonPoint.transform.rotation;
        yield return new WaitForSeconds(delayAfter);
                
        
        playerControllerReference.isDoinSomething = false; //Fin del Disparo
    }

    public virtual IEnumerator TriggerHoldable_DoFire(float holdedTime) 
    {
        
        playerControllerReference.isDoinSomething = true; //Comienza Disparo
        var canonPoint = fullPlayerReference.transform.Find("BulletSpawn");
        yield return new WaitForSeconds(delayBefore); //Disparo Ocurriendo. Aplica Daño.

        hitsound.Play();
        var bullet = Instantiate(bulletPrefab, canonPoint.transform.position, Quaternion.identity);
        PlayerProjectile bulletData = bullet.GetComponent<PlayerProjectile>();
        
        bulletData.damage = (int)Mathf.Round(weaponDamage * (holdedTime));
        bulletData.size = holdedTime;
        bulletData.lastingTime = timeDuring;
        bulletData.Uniform_ResizeBullet(holdedTime*2);
        
        bullet.GetComponent<Rigidbody>().velocity = canonPoint.transform.forward * bulletSpeed;

        bullet.transform.rotation = canonPoint.transform.rotation;
        yield return new WaitForSeconds(delayAfter);
                
       
        playerControllerReference.isDoinSomething = false; //Fin del Disparo
    }


   /* public virtual IEnumerator ToggleHoldable_DoFire(float holdedTime) 
    {
        //Comienza Disparo
        playerControllerReference.isDoinSomething = true;
        //Disparo Ocurriendo. Aplica Daño.
        yield return new WaitForSeconds(delayBefore);

        hitsound.Play();
        var bullet = Instantiate(bulletPrefab, canonPoint.transform.position, Quaternion.identity);
        PlayerProjectile bulletData = bullet.GetComponent<PlayerProjectile>();
        
        bulletData.damage = weaponDamage * (holdedTime / 3);
        bulletData.lastingTime = timeDuring;
        
        bullet.GetComponent<Rigidbody>().velocity = canonPoint.transform.forward * bulletSpeed;

        bullet.transform.rotation = canonPoint.transform.rotation;
        yield return new WaitForSeconds(delayAfter);
                
        //Fin del Disparo
        playerControllerReference.isDoinSomething = false;
    }*/





}
