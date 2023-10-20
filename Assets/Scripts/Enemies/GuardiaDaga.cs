using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JetBrains.Annotations;
using TMPro;
using UnityEngine.AI;

public class GuardiaDaga : Enemy
{
    public override void Start(){
        player = GameObject.Find("Jugador").transform;
        //Valores default de atributos
        enemyMaxHp = 50;
        enemyCurrentHp = enemyMaxHp;
        enemySpeed = 3.5f;
        enemyRange = 2;
        baseAttackCooldown = 5;
        baseAttackCasting = 0.25f;
        //
        TextoVida.GetComponent<TextMeshProUGUI>().text = enemyMaxHp+"";
    }

    public override IEnumerator Attack()
    {
        LookAtTarget(player);
        navMeshAgent.SetDestination(transform.position);
        isDoingSomething = true;
        yield return new WaitForSeconds(baseAttackCasting);
        Vector3 attackSpawn = transform.position + transform.forward * (transform.localScale.z);
        SpawnAttack(baseAttack,attackSpawn);
        DashTo(player.position, 3, 8);
        yield return new WaitForSeconds(baseAttack.GetComponent<EnemyAttack>().lastingTime);
        StartCoroutine(setBaseAttackCooldown());
        isDoingSomething = false;
    }
}
