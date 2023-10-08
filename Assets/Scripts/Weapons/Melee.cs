using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Melee : Weapon
{

    public GameObject weaponHitBox;
    
    
    public IEnumerator DoAttack()
    {   
        
        playerControllerReference.isDoinSomething = true; //Setea ataque
        
        yield return new WaitForSeconds(delayBefore); //Comienza Ataque

        hitsound.Play();
        var meleeHitBox = Instantiate(weaponHitBox, fullPlayerReference.transform.Find("MeleeSpawn").position, Quaternion.identity); //Disparo Ocurriendo. Aplica Daño.
        meleeHitBox.transform.GetChild(0).GetComponent<MeleeBox>().weaponData = this;

       
        
        yield return new WaitForSeconds(timeDuring);//Termina Ataque
        
       
        yield return new WaitForSeconds(delayAfter); //Volviendo a reposo
        
        
        playerControllerReference.isDoinSomething = false; //Fin del ataque
    }


}
////////////// TOCA REAHACER TODOasdaslnf  

