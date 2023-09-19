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
    public int attackDamage;
    public float attackRange;
    public int attackCooldown;
    private float currentCooldown;
    public GameObject attackBox;
    public GameObject RellenoVida;
    public GameObject TextoVida;
    private NavMeshAgent enemy;
    private Transform playerTarget;

    public bool melee; //si el ataque sedispara o solo se crea
    
    void Start()
    {
        enemy = GetComponent<NavMeshAgent>();
        playerTarget = GameObject.Find("Jugador").transform;
        enemyCurrentHp = enemyMaxHp;
    }
    
    void Update()
    {
        Move();
        currentCooldown -= Time.deltaTime;
    }

    public virtual void Move()
    {
        //Configurar NavMeshAgent
        enemy.SetDestination(playerTarget.position);
        enemy.stoppingDistance = attackRange;

        //Ver si hay un objeto entre el jugador y el enemigo
        Vector3 directionToPlayer = (playerTarget.position - transform.position).normalized;
        RaycastHit hit;

        if (Physics.Raycast(transform.position, directionToPlayer, out hit, 20))
        {
            if (hit.collider.gameObject == playerTarget.gameObject)
            {
                Attack();
            }
        }
    }

    public virtual void Attack()
    {
        float targetDistance = Vector3.Distance(transform.position, playerTarget.position);

        if (targetDistance <= attackRange && currentCooldown <= 0)
        {
            //Mirar al jugador
            Vector3 targetPosition = new Vector3(playerTarget.position.x, transform.position.y, playerTarget.position.z);
            transform.LookAt(targetPosition);

            //Instanciar ataque y configurarlo
            Vector3 spawnPosition = transform.position + transform.forward * 2f;
            var ataqueHitbox = Instantiate(attackBox, spawnPosition, Quaternion.identity, null);
            EnemyAttack ataqueHitBData = ataqueHitbox.GetComponent<EnemyAttack>();
            ataqueHitBData.damage = attackDamage;
            ataqueHitBData.lastingTime = 2f;
            //ataqueHitBData.Uniform_ResizeAttack(attackSize);

            if(!melee)
            {
                Rigidbody rb = ataqueHitbox.GetComponent<Rigidbody>();
                rb.AddForce(transform.forward * 2f, ForceMode.VelocityChange);
            }
            currentCooldown = attackCooldown;
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
