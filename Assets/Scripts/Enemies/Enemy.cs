using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JetBrains.Annotations;
using TMPro;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{   
    public string enemyName;
    public int enemyMaxHp;
    private int enemyCurrentHp; 
    public float enemySpeed;
    public int attackDamage;
    public float attackRange;
    public int attackCooldown;
    public int attackSpeed;
    public float repositionDistance = 1.5f;
    public GameObject attackBox;
    public GameObject RellenoVida;
    public GameObject TextoVida;

    private NavMeshAgent enemy;
    private Transform player;
    private float currentCooldown = 0f;
    private GameObject attackHitbox;

    ///////////////////////////////
    public bool melee; //si el ataque sedispara o solo se crea
    ///////////////////////////////

    private enum EnemyState
    {
        Idle,
        MovingToPlayer,
        Attacking,
        Repositioning
    }
    private EnemyState currentState = EnemyState.Idle;

    void Start()
    {
        enemy = GetComponent<NavMeshAgent>();
        player = GameObject.Find("Jugador").transform;
        enemyCurrentHp = enemyMaxHp;
        enemy.speed = enemySpeed;
        TextoVida.GetComponent<TextMeshProUGUI>().text = enemyMaxHp+"";
    }

    IEnumerator EnemyAI()
    {
        while (true)
        {
            switch (currentState)
            {
                case EnemyState.Idle:
                    if (Vector3.Distance(transform.position, player.position) < attackRange)
                    {
                        currentState = EnemyState.Attacking;
                    }
                    else
                    {
                        currentState = EnemyState.MovingToPlayer;
                    }
                    break;

                case EnemyState.MovingToPlayer:
                    Move();
                    yield return new WaitUntil(() => !enemy.pathPending && enemy.remainingDistance <= attackRange);
                    if (Vector3.Distance(transform.position, player.position) <= attackRange)
                    {
                        currentState = EnemyState.Attacking;
                    }
                    break;

                case EnemyState.Attacking:
                    if (Vector3.Distance(transform.position, player.position) <= attackRange && isLookingTarget(player))
                    {
                        Attack(player);
                        currentState = EnemyState.Repositioning;
                    }
                    else
                    {
                        currentState = EnemyState.MovingToPlayer;
                    }
                    break;

                case EnemyState.Repositioning:
                    Vector3 oppositeDirection = transform.position - player.position;
                    Vector3 destination = transform.position + oppositeDirection.normalized * repositionDistance;
                    MoveTo(destination);
                    yield return new WaitUntil(() => !enemy.pathPending && enemy.remainingDistance < 2f);
                    currentState = EnemyState.Idle;
                    break;
            }
            yield return null;
        }
    }
    
    void Update()
    {
        StartCoroutine(EnemyAI());
    }

    public virtual void Move()
    {
        ChaseTarget(player);
        LookAtTarget(player);
    }

    public virtual void Attack(Transform target)
    {
        float targetDistance = Vector3.Distance(transform.position, target.position);

        if (targetDistance <= attackRange && Time.time >= currentCooldown)
        {
            LookAtTarget(player);
            SpawnAttackHitBox();
            currentCooldown = Time.time + attackCooldown;
        }
    }

    private void ChaseTarget(Transform target)
    {
        enemy.SetDestination(target.position);
        enemy.stoppingDistance = attackRange;
    }

    private bool MoveTo(Vector3 position){
        LookAtTarget(player);
        enemy.SetDestination(position);
        enemy.stoppingDistance = 0;
        return transform.position == position;
    }

    private bool isLookingTarget(Transform target)
    {
        Vector3 directionToTarget = (target.position - transform.position).normalized;
        RaycastHit hit;
        if (Physics.Raycast(transform.position, directionToTarget, out hit, 30)){
            return hit.collider.gameObject == target.gameObject;
        };
        return true;
    }

    private void LookAtTarget(Transform target)
    {
        Vector3 lookDirection = target.position - transform.position;
        lookDirection.y = 0f;
        transform.rotation = Quaternion.LookRotation(lookDirection);
    }

    private void SpawnAttackHitBox()
    {
        Vector3 spawnPosition = transform.position + transform.forward * 2f;

        attackHitbox = Instantiate(attackBox, spawnPosition, Quaternion.identity, null);
        attackHitbox.transform.rotation = transform.rotation;
        EnemyAttack ataqueHitBData = attackHitbox.GetComponent<EnemyAttack>();
    
        ataqueHitBData.damage = attackDamage;
        ataqueHitBData.lastingTime = 2f;
        //ataqueHitBData.Uniform_ResizeAttack(attackSize);

        if (!melee)
        {
            Rigidbody rb = attackHitbox.GetComponent<Rigidbody>();
            rb.AddForce(transform.forward * attackSpeed, ForceMode.VelocityChange);
        }
        currentCooldown = attackCooldown;
    }
    
    public void Die()
    {   
        Debug.Log("Mataste a " + enemyName);
        Destroy(gameObject);
    }

    public void RecieveDamage(int damage)
    {
        enemyCurrentHp -= damage;
        
        float maxWidth = 288.0633f;

        TextoVida.GetComponent<TextMeshProUGUI>().text = enemyCurrentHp + " / " + enemyMaxHp;
        RellenoVida.GetComponent<RectTransform>().sizeDelta = new Vector2(enemyCurrentHp * maxWidth / enemyMaxHp, RellenoVida.GetComponent<RectTransform>().sizeDelta.y);

        if(enemyCurrentHp <= 0)
        {
            Die();
        }
    }
}
