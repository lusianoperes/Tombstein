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
    private int enemyCurrentHp; 
    public float enemySpeed;

    [Header("Attack")]
    public int attackDamage;
    public float attackRange;
    public int attackCooldown;
    public int attackSpeed;
    public float attackCasting; //Tiempo de ejecución de ataque
    public float attackDuration; //Tiempo de vida de la hitbox
    public GameObject attack1;
    ///////////////////////////////
    public bool melee; //Borrar, los ataques deben ser melee o a rango no todo el enemigo (esto para generalizar)
    ///////////////////////////////

    [Header("Life Bar")]
    public GameObject RellenoVida;
    public GameObject TextoVida;

    private NavMeshAgent navMeshAgent;
    private Transform player;
    private float lastAttack = 0;
    private float pathUpdateDelay = 0.2f;
    private float pathUpdateDeadline;
    private float escapeDistance = 2f;


    //agregados pata. para que se cuenten los enemigos que se matan y los mande a gamemanage
    public GameManage GameManage;

    private void Awake() {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    void Start()
    {
        player = GameObject.Find("Jugador").transform;
        enemyCurrentHp = enemyMaxHp;
        navMeshAgent.speed = enemySpeed;
        navMeshAgent.stoppingDistance = attackRange;
        TextoVida.GetComponent<TextMeshProUGUI>().text = enemyMaxHp+"";
        StartCoroutine(EnemyAI());
    }

    private enum EnemyState
    {
        Idle,
        Chasing,
        Attacking,
        Repositioning
    }
    private EnemyState currentState = EnemyState.Idle;

    public virtual IEnumerator EnemyAI()
    {
        while (true)
        {
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
                    navMeshAgent.stoppingDistance = attackRange;
                    Move(player);
                    currentState = EnemyState.Idle;
                    break;

                case EnemyState.Attacking: //Logica de Ataque
                    Attack(attack1, !melee);
                    yield return new WaitForSeconds(attackCasting);
                    lastAttack = Time.time;
                    currentState = EnemyState.Idle;
                    break;

                case EnemyState.Repositioning: //Logica de Escape (que hace despues de atacar)
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
                    currentState = EnemyState.Idle;
                    break;
            }
            yield return null;
        }
    }
    
    void Update()
    {

    }

    public virtual void Move(Transform target){
        if (Time.time >= pathUpdateDeadline) {
            pathUpdateDeadline = Time.time + pathUpdateDelay;
            navMeshAgent.SetDestination(target.position);
        }
    }

    private void Attack(GameObject attackPrefab, bool isProjectile)
    {
        Vector3 attackSpawn = transform.position + transform.forward * transform.localScale.x;
        GameObject attackInstantiated = Instantiate(attackPrefab, attackSpawn, Quaternion.identity);
        attackInstantiated.transform.rotation = transform.rotation;
        EnemyAttack attackStats = attackInstantiated.GetComponent<EnemyAttack>();
    
        attackStats.damage = attackDamage;
        attackStats.lastingTime = attackDuration;
        //attackStats.Uniform_ResizeAttack(attackSize);

        if (isProjectile)
        {
            Rigidbody rb = attackInstantiated.GetComponent<Rigidbody>();
            rb.AddForce(transform.forward * attackSpeed, ForceMode.VelocityChange);
        }
    }

    private void LookAtTarget (Transform target) {
        Vector3 lookPos = target.position - transform.position;
        lookPos.y = 0;
        Quaternion rotation = Quaternion.LookRotation(lookPos);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 0.2f);
    }

    public void Die()
    {   
        Debug.Log("Mataste a " + enemyName);
        GameManage.Counter --;
        if (GameManage.Counter == 0)
        {
            GameManage.MakeClear();
        }
        
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
