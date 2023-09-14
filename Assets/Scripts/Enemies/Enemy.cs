using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JetBrains.Annotations;
using TMPro;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{   
    public string enemyName;
    

    public bool melee; //si el ataque sedispara o solo se crea
    
    public int enemyMaxHp; //old vidaEnemigo
    public int enemyCurentHp; 
    public int enemyDamage; //danioAtaqueEnemigo

    public float disAtaqueYEnemigo = 2f;// Donde se crea la caja de daño.
    public float velAtaqueDistancia = 2f;
    
    public float attackSize = 1f;
    public float cadenciaAtaqueEnemigo = 2f;  //cada cuanto de sipara el ataque
    public float duracionAtaque = 2f; //tiempo que dura creado elataque
    public float attackDistance; //distancia a la cual el enemigo no debe acercarse mas. Donde inicia el ataque
    
    private float distanciaAlJugador; // distancia actual del jugador
    private float proximoAtaqueEnemigo = 0f; //cooldown actualizable de ataque
        
    public GameObject attackBox;
    
    private GameObject jugador;
    private Transform PlayerTarget;
    
    private NavMeshAgent enemy;
    public LayerMask raycastLayer;
    
    
    
    public GameObject RellenoVida;
    public GameObject TextoVida;

    //agregados pata. para que se cuenten los enemigos que se matan y los mande a gamemangae
    public GameManage GameManage;

    void Start()
    {
        enemy = GetComponent<NavMeshAgent>();
        jugador = GameObject.Find("Jugador");
        PlayerTarget = jugador.transform ;
        enemy.stoppingDistance = attackDistance;

    }
    
    void Update()
    {
        Vector3 directionToPlayer = (PlayerTarget.position - transform.position).normalized;
        RaycastHit hit;
        
        if (Physics.Raycast(transform.position, directionToPlayer, out hit, 20f))
        {
            if (hit.collider.gameObject == PlayerTarget.gameObject)
            {
                distanciaAlJugador = Vector3.Distance(transform.position, PlayerTarget.position);

                if (distanciaAlJugador <= attackDistance && Time.time >= proximoAtaqueEnemigo)
                { 
                        Vector3 targetPostition = new Vector3( PlayerTarget.position.x, transform.position.y, PlayerTarget.position.z ) ;
                        transform.LookAt( targetPostition ) ;
                        Atacar();
                        proximoAtaqueEnemigo = Time.time + cadenciaAtaqueEnemigo;
                        
                }
            }
            
            else
            {
                enemy.stoppingDistance = attackDistance;
            }
        }
        enemy.SetDestination(PlayerTarget.position);
    }
    
    public void Atacar()
    {
        Vector3 posicionEnemigo = transform.position;
        Vector3 direccionMirada = transform.forward;
        Vector3 posicionSpawn = posicionEnemigo + direccionMirada * disAtaqueYEnemigo;

        var ataqueHitbox = Instantiate(attackBox, posicionSpawn, Quaternion.identity, null);
        EnemyAttack ataqueHitBData = ataqueHitbox.GetComponent<EnemyAttack>();
        ataqueHitBData.damage = enemyDamage;
        ataqueHitBData.lastingTime = duracionAtaque;
        //ataqueHitBData.Uniform_ResizeAttack(attackSize);


        if(!melee)
        {
            Rigidbody rb = ataqueHitbox.GetComponent<Rigidbody>();

            rb.AddForce(direccionMirada * velAtaqueDistancia, ForceMode.VelocityChange);
        }
    }
    
    
    
    public void Die()
    {   
        Debug.Log("Mataste a " + enemyName);
        Destroy(gameObject);
        GameManage.Counter--;
        if (GameManage.Counter == 0)
            GameManage.MakeClear();
    }

    public void RecieveDamage(int damage)
    {
        enemyCurentHp -= damage;
        
        float maxWidth = 288.0633f;

        TextoVida.GetComponent<TextMeshProUGUI>().text = enemyCurentHp + " / " + enemyMaxHp;
        RellenoVida.GetComponent<RectTransform>().sizeDelta = new Vector2(enemyCurentHp * maxWidth / enemyMaxHp, RellenoVida.GetComponent<RectTransform>().sizeDelta.y);

        if(enemyCurentHp <= 0)
        {
            Die();
        }

    }

    

    
}
