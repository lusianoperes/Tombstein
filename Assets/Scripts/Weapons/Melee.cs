using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Melee : Weapon
{
    public PlayerController player;
    public GameObject meleeHitbox;
    public IEnumerator DoAttack()
    {   
        //Setea ataque
        player.isDoinSomething = true;
        meleeHitbox.GetComponent<MeleeBox>().ResizeBox();
        meleeHitbox.SetActive(false);
        hitsound.Play();
        
        
        //Comienza Ataque
        yield return new WaitForSeconds(delayBefore);

        //Ataque Ocurriendo. Aplica Daño.
        meleeHitbox.SetActive(true);
        
        yield return new WaitForSeconds(timeDuring);
        
        
        //Termina Ataque
        meleeHitbox.SetActive(false);
        //Vuelve a reposo
        yield return new WaitForSeconds(delayAfter);
        
        //Fin del ataque
        player.isDoinSomething = false;
    }


}
