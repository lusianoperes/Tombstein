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
    public float enemyRange;

    [Header("Life Bar")]
    public GameObject RellenoVida;
    public GameObject TextoVida;

    [Header("Base Attack")]
    public int baseAttackCooldown;
    public int baseAttackSpeed;
    public float baseAttackCasting; //Tiempo de ejecución de ataque
    public GameObject baseAttack;    
    protected bool isBaseAttackInCooldown = false;

    protected NavMeshAgent navMeshAgent;
    protected Transform player;
    protected bool isDoingSomething = false;
    protected float pathUpdateDelay = 0.2f;
    protected float pathUpdateDeadline;
    protected float escapeDistance = 2f;
    protected bool isDashing = false;

    protected void Awake() {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    public virtual void Start()
    {
        //navMeshAgent.radius = transform.localScale.x > transform.localScale.z? transform.localScale.x : transform.localScale.z;
        player = GameObject.Find("Jugador").transform;
        enemyCurrentHp = enemyMaxHp;
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

    protected void Update()
    {
        if(!isDoingSomething){
            switch (currentState)
            {
                case EnemyState.Idle: //Logica de cambio de estados
                    bool inRange = Vector3.Distance(transform.position, player.position) <= enemyRange + transform.localScale.z/2;
                    if (!isInCooldown()) {
                        if (inRange) {
                            if (isLookingAtTarget(player)) {
                                currentState = EnemyState.Attacking;
                            } else {
                                MoveOrLookAtTarget(player);
                            }
                        } else {
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
        if (!isDashing){
            LookAtTarget(player);
            //navMeshAgent.SetDestination(transform.position);
            isDoingSomething = true;
            yield return new WaitForSeconds(baseAttackCasting);
            SpawnAttack(baseAttack);
            DashTo(player.position, 5, 8); //Ejemplo de un dash hacia el jugador mientras ataca
            if (!baseAttack.GetComponent<EnemyAttack>().isProjectile){
                yield return new WaitForSeconds(baseAttack.GetComponent<EnemyAttack>().lastingTime);
            }
            StartCoroutine(setBaseAttackCooldown());
            isDoingSomething = false;
        }
    }

    public virtual bool isInCooldown(){
        return isBaseAttackInCooldown;
    }

    protected IEnumerator setBaseAttackCooldown() {
        isBaseAttackInCooldown = true;
        yield return new WaitForSeconds(baseAttackCooldown);
        isBaseAttackInCooldown = false;
    }

    public virtual void Reposition() //Separado en una funcion para generalizar
    {
        navMeshAgent.speed = enemySpeed;
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
        navMeshAgent.speed = enemySpeed;
        navMeshAgent.stoppingDistance = enemyRange;
        if (Time.time >= pathUpdateDeadline) {
            pathUpdateDeadline = Time.time + pathUpdateDelay;
            navMeshAgent.SetDestination(target.position);
        }
    }

    protected void DashTo (Vector3 position, float displacement, float speed)
    {
        Vector3 direction = (position - transform.position).normalized * displacement + transform.position;
        LookAtTarget(player);
        StartCoroutine(dash (transform.position, direction, speed));
    }

    protected IEnumerator dash (Vector3 startPosition, Vector3 target, float speed)
    {
        isDashing = true;
        navMeshAgent.enabled = false;
        float journeyLength = Vector3.Distance(startPosition, target);
        float startTime = Time.time;
        float fractionOfJourney;
        while (true)
        {
            float journeyTime = Time.time - startTime;
            float distanceCovered = journeyTime * speed;
            fractionOfJourney = distanceCovered / journeyLength;

            transform.position = Vector3.Lerp(startPosition, target, fractionOfJourney);

            if (fractionOfJourney >= 1.0f) {
                transform.position = target;
                isDashing = false;
                navMeshAgent.enabled = true;
                yield break;
            } else if (HasCollidedWithObstacle()) {
                isDashing = false;
                navMeshAgent.enabled = true;
                yield break;
            }

            yield return null;
        }
        
        transform.position = target;
        isDashing = false;
    }

    private bool HasCollidedWithObstacle()
    {
        Vector3 raycastOrigin = transform.position + Vector3.up * 0.5f;
        Vector3 raycastDirection = transform.forward;
        float raycastDistance = 0.3f;
        RaycastHit hit;
        return Physics.Raycast(raycastOrigin, raycastDirection, out hit, raycastDistance);
    }

    public void SpawnAttack(GameObject attackPrefab)
    {
        Vector3 attackSpawn = transform.position + transform.forward * transform.localScale.z;
        GameObject attackInstantiated = Instantiate(attackPrefab, attackSpawn, Quaternion.identity);

        attackInstantiated.transform.rotation = transform.rotation;
        EnemyAttack attackStats = attackInstantiated.GetComponent<EnemyAttack>();

        Vector3 knockbackDirection = (player.position - transform.position).normalized;
        knockbackDirection.y = 0;
        attackStats.setKnockbackDirection(knockbackDirection);
        //attackStats.Uniform_ResizeAttack(attackSize);

        if (attackStats.isProjectile)
        {
            Rigidbody rb = attackInstantiated.GetComponent<Rigidbody>();
            rb.AddForce(transform.forward * baseAttackSpeed, ForceMode.VelocityChange);
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

    protected bool isLookingAtTarget (Transform target) {
        Vector3 directionToTarget = (target.position - transform.position).normalized;
        directionToTarget.x = transform.forward.x;
        directionToTarget.z = transform.forward.z;
        float distanceToTarget = Vector3.Distance(transform.position, target.position);
        RaycastHit hit;

        if (Physics.Raycast(transform.position, directionToTarget, out hit, distanceToTarget)) {
            if (hit.collider.gameObject == target.gameObject) {
                return true;
            }
        }
        return false;
    }

    protected void MoveOrLookAtTarget (Transform target) {
        Vector3 directionToTarget = (target.position - transform.position).normalized;
        float distanceToTarget = Vector3.Distance(transform.position, target.position);
        RaycastHit hit;

        if (Physics.Raycast(transform.position, directionToTarget, out hit, distanceToTarget)) {
            if (hit.collider.gameObject == target.gameObject) {
                LookAtTarget(target);
            } else {
                navMeshAgent.SetDestination(target.position);
                navMeshAgent.stoppingDistance = 0;
            }
        }
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
