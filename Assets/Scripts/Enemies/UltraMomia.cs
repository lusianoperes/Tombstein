using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JetBrains.Annotations;
using TMPro;
using UnityEngine.AI;

public class UltraMomia : Enemy
{
    [Header("Enemies")]
    public GameObject firstWave;
    public GameObject secondWave;
    public GameObject thirdWave;
    public GameObject fourthWave;
    public GameObject fifthWave;

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
        SpawnAttack(firstWave, transform.position);
        yield return new WaitForSeconds(10);
        SpawnAttack(secondWave, transform.position);
        yield return new WaitForSeconds(10);
        SpawnAttack(thirdWave, transform.position);
        yield return new WaitForSeconds(15);
        SpawnAttack(fourthWave, transform.position);
        yield return new WaitForSeconds(15);
        SpawnAttack(fifthWave, transform.position);
        yield return new WaitForSeconds(20);
        isDoingSomething = false;
    }
}
