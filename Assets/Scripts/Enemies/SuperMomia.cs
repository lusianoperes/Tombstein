using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JetBrains.Annotations;
using TMPro;
using UnityEngine.AI;

public class SuperMomia : Enemy
{
    public override void Start(){
        player = GameObject.Find("Jugador").transform;
        //Valores default de atributos
        enemyMaxHp = 300;
        enemyCurrentHp = enemyMaxHp;
        enemySpeed = 6;
        enemyRange = 2;
        baseAttackCooldown = 3;
        baseAttackCasting = 0.5f;
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
        DashTo(player.position, 3, 8);
        yield return new WaitForSeconds(baseAttack.GetComponent<EnemyAttack>().lastingTime);
        LookAtTargetWithoutSlerp(player);
        SpawnAttack(baseAttack);
        DashTo(player.position, 3, 8);
        yield return new WaitForSeconds(baseAttack.GetComponent<EnemyAttack>().lastingTime);
        LookAtTargetWithoutSlerp(player);
        SpawnAttack(baseAttack);
        DashTo(player.position, 3, 8);
        yield return new WaitForSeconds(baseAttack.GetComponent<EnemyAttack>().lastingTime);
        LookAtTargetWithoutSlerp(player);
        SpawnAttack(baseAttack);
        DashTo(player.position, 3, 8);
        yield return new WaitForSeconds(baseAttack.GetComponent<EnemyAttack>().lastingTime);
        StartCoroutine(setBaseAttackCooldown());

        isDoingSomething = false;
    }
}