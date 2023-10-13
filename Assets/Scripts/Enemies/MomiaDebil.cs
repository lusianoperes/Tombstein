using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JetBrains.Annotations;
using TMPro;
using UnityEngine.AI;

public class MomiaDebil : Enemy
{
    public override void Start(){
        player = GameObject.Find("Jugador").transform;
        TextoVida.GetComponent<TextMeshProUGUI>().text = enemyMaxHp+"";
        //Valores default de atributos
        enemyMaxHp = 100;
        enemyCurrentHp = enemyMaxHp;
        enemySpeed = 3.5f;
        enemyRange = 2;
        baseAttackCooldown = 5;
        baseAttackCasting = 0.25f;
    }

    public override IEnumerator Attack()
    {
        LookAtTarget(player);
        navMeshAgent.SetDestination(transform.position);
        isDoingSomething = true;
        yield return new WaitForSeconds(baseAttackCasting);
        SpawnAttack(baseAttack);
        yield return new WaitForSeconds(baseAttack.GetComponent<EnemyAttack>().lastingTime);
        StartCoroutine(setBaseAttackCooldown());
        isDoingSomething = false;
    }
}
