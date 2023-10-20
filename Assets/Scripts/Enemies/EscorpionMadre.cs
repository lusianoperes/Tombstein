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

    [Header("Enemies")]
    public GameObject fiveScorpions;
    public GameObject tenScorpions;
    protected bool isFiveScorpionsInCooldown = false;
    protected bool isTenScorpionsInCooldown = false;

    public override void Start(){
        player = GameObject.Find("Jugador").transform;
        //Valores default de atributos
        enemyMaxHp = 1000;
        enemyCurrentHp = enemyMaxHp;
        enemySpeed = 0;
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
        isDoingSomething = true;
        if (enemyCurrentHp > 700) {
            SpawnAttack(fiveScorpions, transform.position);
            StartCoroutine(setFiveScorpionsCooldown());
        } else if (enemyCurrentHp > 500) {
            SpawnAttack(tenScorpions, transform.position);
            StartCoroutine(setTenScorpionsCooldown());
        } else {
            enemySpeed = 2.75f;
            enemyRange = 4;
            navMeshAgent.SetDestination(transform.position);
            Vector3 attackSpawn = transform.position + transform.forward * (transform.localScale.z);
            if (!isStingInCooldown) {
                yield return new WaitForSeconds(stingCasting);
                SpawnAttack(sting, attackSpawn);
                yield return new WaitForSeconds(sting.GetComponent<EnemyAttack>().lastingTime);
                StartCoroutine(setStingCooldown());
            } else {
                yield return new WaitForSeconds(baseAttackCasting);
                SpawnAttack(baseAttack,attackSpawn);
                yield return new WaitForSeconds(baseAttack.GetComponent<EnemyAttack>().lastingTime);
                SpawnAttack(baseAttack,attackSpawn);
                yield return new WaitForSeconds(baseAttack.GetComponent<EnemyAttack>().lastingTime);
                StartCoroutine(setBaseAttackCooldown());
            }
            StartCoroutine(setCooldownBetweenAttacks());
        }
        isDoingSomething = false;
    }

    public override bool isInCooldown(){
        return (isBaseAttackInCooldown && isStingInCooldown) || betweenAttacks || isFiveScorpionsInCooldown || isTenScorpionsInCooldown;
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

    protected IEnumerator setFiveScorpionsCooldown(){
        isFiveScorpionsInCooldown = true;
        yield return new WaitForSeconds(10);
        isFiveScorpionsInCooldown = false;
    }

    protected IEnumerator setTenScorpionsCooldown(){
        isTenScorpionsInCooldown = true;
        yield return new WaitForSeconds(10);
        isTenScorpionsInCooldown = false;
    }
}
