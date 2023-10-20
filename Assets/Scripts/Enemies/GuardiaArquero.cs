using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JetBrains.Annotations;
using TMPro;
using UnityEngine.AI;

public class GuardiaArquero : Enemy
{
    public override void Start(){
        player = GameObject.Find("Jugador").transform;
        //Valores default de atributos
        enemyMaxHp = 50;
        enemyCurrentHp = enemyMaxHp;
        enemySpeed = 3.5f;
        enemyRange = 15;
        baseAttackCooldown = 5;
        baseAttackCasting = 3;
        //
        TextoVida.GetComponent<TextMeshProUGUI>().text = enemyMaxHp+"";
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
