using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Escorpion : Enemy
{
    [Header("Sting")]
    public int stingCooldown;
    public float stingCasting;
    public GameObject sting;    
    protected bool isStingInCooldown = false;
    protected bool betweenAttacks = false;

    public override IEnumerator Attack()
    {
        LookAtTarget(player);
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
