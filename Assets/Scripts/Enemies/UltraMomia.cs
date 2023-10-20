using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JetBrains.Annotations;
using TMPro;
using UnityEngine.AI;

public class UltraMomia : Enemy
{
    public override void Start(){
        player = GameObject.Find("Jugador").transform;
        //Valores default de atributos
        enemyMaxHp = 1200;
        enemyCurrentHp = enemyMaxHp;
        enemySpeed = 0;
        enemyRange = 100;
        baseAttackCooldown = 5;
        baseAttackCasting = 0.5f;
        //
        TextoVida.GetComponent<TextMeshProUGUI>().text = enemyMaxHp+"";
    }

    public override IEnumerator Attack()
    {
        LookAtTarget(player);
        isDoingSomething = true;
        //Spawnea 5 momias débiles
        yield return new WaitForSeconds(10);
        //Spawnea 4 momias débiles y 1 Momia pesada
        yield return new WaitForSeconds(10);
        //Spawnea 2 supermomias
        yield return new WaitForSeconds(15);
        //Spawnea 3 momias pesadas y 1 Supermomia
        yield return new WaitForSeconds(15);
        //Spawnea 2 supermomias 5 enemigos débiles y 2 momias pesadas
        yield return new WaitForSeconds(20);
        isDoingSomething = false;
    }
}
