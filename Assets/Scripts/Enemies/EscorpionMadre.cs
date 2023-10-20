using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JetBrains.Annotations;
using TMPro;
using UnityEngine.AI;

public class EscorpionMadre : Enemy
{
    [Header("Sting")]
    public int stingCooldown;
    public float stingCasting;
    public GameObject sting;    
    protected bool isStingInCooldown = false;
    protected bool betweenAttacks = false;

    public override void Start(){
        player = GameObject.Find("Jugador").transform;
        //Valores default de atributos
        enemyMaxHp = 1000;
        enemyCurrentHp = enemyMaxHp;
        enemySpeed = 2.75f;
        enemyRange = 100;
        baseAttackCooldown = 5;
        baseAttackCasting = 0.5f;
        stingCooldown = 5;
        stingCasting = 0.25f;
        //
        TextoVida.GetComponent<TextMeshProUGUI>().text = enemyMaxHp+"";
    }

    public override IEnumerator Attack()
    {
        LookAtTarget(player);
        if (enemyCurrentHp > 700) {
            //crea 5 escorpiones
        } else if (enemyCurrentHp > 500) {
            //crea 10 escorpiones
        } else {
            enemyRange = 2;
            navMeshAgent.SetDestination(transform.position);
            if (!isStingInCooldown) {
                isDoingSomething = true;
                yield return new WaitForSeconds(stingCasting);
                SpawnAttack(sting);
                yield return new WaitForSeconds(sting.GetComponent<EnemyAttack>().lastingTime);
                StartCoroutine(setStingCooldown());
            } else {
                isDoingSomething = true;
                yield return new WaitForSeconds(baseAttackCasting);
                SpawnAttack(baseAttack);
                yield return new WaitForSeconds(baseAttack.GetComponent<EnemyAttack>().lastingTime);
                SpawnAttack(baseAttack);
                yield return new WaitForSeconds(baseAttack.GetComponent<EnemyAttack>().lastingTime);
                StartCoroutine(setBaseAttackCooldown());
            }
            StartCoroutine(setCooldownBetweenAttacks());
            isDoingSomething = false;
        }
    }

    public override bool isInCooldown(){
        return (isBaseAttackInCooldown && isStingInCooldown) || betweenAttacks;
    }

    protected IEnumerator setStingCooldown(){
        isStingInCooldown = true;
        yield return new WaitForSeconds(stingCooldown);
        isStingInCooldown = false;
    }

    protected IEnumerator setCooldownBetweenAttacks(){
        betweenAttacks = true;
        yield return new WaitForSeconds(0.25f);
        betweenAttacks = false;
    }
}
