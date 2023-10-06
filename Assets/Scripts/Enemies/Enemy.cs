using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JetBrains.Annotations;
using TMPro;
using UnityEngine.AI;

public class Enemy : MonoBehaviour {
    [Header("General")]
    public string enemyName;
    public int enemyMaxHp;
    protected int enemyCurrentHp; 
    public float enemySpeed;

    [Header("Attack")]
    public int attackDamage;
    public float attackRange;
    public int attackCooldown;
    public int attackSpeed;
    public float attackCasting; //Tiempo de ejecución de ataque
    public float attackDuration; //Tiempo de vida de la hitbox
    public GameObject attack1;
    protected bool isDoingSomething;

    [Header("Life Bar")]
    public GameObject RellenoVida;
    public GameObject TextoVida;

    protected NavMeshAgent navMeshAgent;
    protected Transform player;
    protected float lastAttack = 0;
    protected float pathUpdateDelay = 0.2f;
    protected float pathUpdateDeadline;
    protected float escapeDistance = 2f;

    protected void Awake() {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    protected void Start()
    {
        player = GameObject.Find("Jugador").transform;
        enemyCurrentHp = enemyMaxHp;
        navMeshAgent.speed = enemySpeed;
        navMeshAgent.stoppingDistance = attackRange;
        TextoVida.GetComponent<TextMeshProUGUI>().text = enemyMaxHp+"";
    }

    protected enum EnemyState
    {
        Idle,
        Chasing,
        Attacking,
        Repositioning
    }
    protected EnemyState currentState = EnemyState.Idle;

    protected void Update() //Todo lo que estaba en EnemyAI (o algo así) ahora está en el update
    {
        if(!isDoingSomething){
            switch (currentState)
            {
                case EnemyState.Idle: //Logica de cambio de estados
                    bool inRange = Vector3.Distance(transform.position, player.position) <= attackRange;
                    if (Time.time - lastAttack >= attackCooldown || lastAttack == 0) {
                        if (inRange) {
                            currentState = EnemyState.Attacking;
                        }
                        else {
                            currentState = EnemyState.Chasing;
                        }
                    } else {
                        currentState = EnemyState.Repositioning;
                    }
                    break;

                case EnemyState.Chasing: //Logica de Movimiento
                    Move(player);
                    currentState = EnemyState.Idle;
                    break;

                case EnemyState.Attacking: //Logica de Ataque
                    StartCoroutine(Attack());
                    currentState = EnemyState.Idle;
                    break;

                case EnemyState.Repositioning: //Logica de Escape (que hace despues de atacar)
                    Reposition();
                    currentState = EnemyState.Idle;
                    break;
            }
        }
    }

    public virtual IEnumerator Attack() //Separado en una funcion para generalizar
    {
        SpawnAttack(attack1);
        isDoingSomething = true;
        yield return new WaitForSeconds(attackCasting);
        lastAttack = Time.time;
        isDoingSomething = false;
    }

    public virtual void Reposition() //Separado en una funcion para generalizar
    {
        LookAtTarget(player);
        Vector3 directionToPlayer = player.position - transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;

        if (distanceToPlayer <= 5 /*Aca iria el rango del jugador*/)
        {
            Vector3 escapeDirection = -directionToPlayer.normalized;
            Vector3 escapeDestination = transform.position + escapeDirection * escapeDistance;
            navMeshAgent.SetDestination(escapeDestination);
            navMeshAgent.stoppingDistance = 0;
        }
    }

    public virtual void Move(Transform target){
        navMeshAgent.stoppingDistance = attackRange;
        if (Time.time >= pathUpdateDeadline) {
            pathUpdateDeadline = Time.time + pathUpdateDelay;
            navMeshAgent.SetDestination(target.position);
        }
    }

    public void SpawnAttack(GameObject attackPrefab)
    {
        Vector3 attackSpawn = transform.position + transform.forward * transform.localScale.x;
        GameObject attackInstantiated = Instantiate(attackPrefab, attackSpawn, Quaternion.identity);

        attackInstantiated.transform.rotation = transform.rotation;
        EnemyAttack attackStats = attackInstantiated.GetComponent<EnemyAttack>();
    
        attackStats.damage = attackDamage;
        attackStats.lastingTime = attackDuration;
        attackStats.setEnemy(transform);
        //attackStats.Uniform_ResizeAttack(attackSize);

        if (attackStats.isProjectile)
        {
            Rigidbody rb = attackInstantiated.GetComponent<Rigidbody>();
            rb.AddForce(transform.forward * attackSpeed, ForceMode.VelocityChange);
        } else {
            attackInstantiated.transform.parent = transform; //La hitbox ahora es hijo del enemigo para que esta lo siga
        }
    }

    protected void LookAtTarget (Transform target) {
        Vector3 lookPos = target.position - transform.position;
        lookPos.y = 0;
        Quaternion rotation = Quaternion.LookRotation(lookPos);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 0.2f);
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
