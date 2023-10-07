using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Rigidbody PlayerRigidbody;
    public Inventory PlayerInventory;
    public Vector3 moveInput;

    public float Speed = 7f; // Velocidad base del personaje
    public float SpeedMultiple = 1.0f; // Velocidad adicional para mi personaje
    public float fuerzaDash; //= 25f; // Fuerza del Dash
    public float duracionDash = 0.25f; // Duración del Dash
    public float determinarCooldown = 1f; // Cooldown del Dash
    private bool cooldownActivo = false;
    private bool dasheando = false;
    private Vector3 direccionDash;
    public float velocidadRotacion = 20f; //Para que el jugador gire con el cursor
    
    public bool isDoinSomething = false;
    public bool primaryOnCooldown = false;
    public bool secondaryOnCooldown = false;
    public bool chargedStarted = false;
    public float timeCounterA = 0f;
    public float timeCounterB = 0f;
    private float knockbackCounter = 0; //Contador del knockback


    IEnumerator CooldownPrimary(float cOOLdown)
    {
        primaryOnCooldown = true;
        yield return new WaitForSeconds(cOOLdown);
        primaryOnCooldown = false;
    }
    IEnumerator CooldownSecondary(float cOOLdown)
    {
                secondaryOnCooldown = true;
                yield return new WaitForSeconds(cOOLdown);
                secondaryOnCooldown = false;
    }
    

    private void Awake()
    {
        PlayerRigidbody = GetComponent<Rigidbody>();
        PlayerInventory = GetComponent<Inventory>();
    }

    private void Update()
    {
        ManejoDash();
        MovimientoCursor();
        

        if(Input.GetMouseButton(0) && !(isDoinSomething) && !(primaryOnCooldown))
        {   
            Weapon primaria = PlayerInventory.primaryWeapon.GetComponent<Weapon>();
            Melee equipedMelee = primaria as Melee;
            
            equipedMelee.StartCoroutine(equipedMelee.DoAttack());
    
            StartCoroutine(CooldownPrimary(PlayerInventory.primaryWeapon.GetComponent<Weapon>().cooldown));
        }
        Weapon secundaria = PlayerInventory.secondaryWeapon.GetComponent<Weapon>();
        Distance equipedDistance = secundaria as Distance;
        switch (equipedDistance.distanceType)
        {
            case Distance.DistanceType.triggerOnly:
                if(Input.GetMouseButton(1) && !(isDoinSomething) && !(secondaryOnCooldown))
                {              
                    equipedDistance.StartCoroutine(equipedDistance.TriggerOnly_DoFire());
                    StartCoroutine(CooldownSecondary(secundaria.GetComponent<Weapon>().cooldown));
                }       
                break;

            case Distance.DistanceType.triggerHoldable:
                if(Input.GetMouseButton(1) && !(isDoinSomething) && !(secondaryOnCooldown))
                {   
                    isDoinSomething = true;   
                    chargedStarted = true;
                }
                else if(Input.GetMouseButton(1) && isDoinSomething && chargedStarted)
                {
                    
                    if(timeCounterA < secundaria.GetComponent<Weapon>().weaponRange)
                    {
                        timeCounterA += Time.deltaTime;
                    }

                }
                else if(!(Input.GetMouseButton(1)) && isDoinSomething && chargedStarted)
                {   
                    
                    equipedDistance.StartCoroutine(equipedDistance.TriggerHoldable_DoFire(timeCounterA));
                    StartCoroutine(CooldownSecondary(secundaria.GetComponent<Weapon>().cooldown));
                    timeCounterA = 0;
                    chargedStarted = false;
                }  
                break;
                /*
            case Distance.DistanceType.toggleOnly:
                if(Input.GetMouseButton(1) && !(isDoinSomething) && !(secondaryOnCooldown))
                {              
                    equipedDistance.StartCoroutine(equipedDistance.DoFire());
                    StartCoroutine(CooldownSecondary(secundaria.GetComponent<Weapon>().cooldown));
                }  
                break;
            case Distance.DistanceType.toggleHoldable:
                if(Input.GetMouseButton(1) && !(isDoinSomething) && !(secondaryOnCooldown))
                {              
                    equipedDistance.StartCoroutine(equipedDistance.DoFire());
                    StartCoroutine(CooldownSecondary(secundaria.GetComponent<Weapon>().cooldown));
                }  
                break;*/
            default:
                //no tiene arma.
                break;
        }
        

    }
    private void FixedUpdate()
    {
        MovimientoPersonaje();
    }

    public void MovimientoPersonaje()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        if (knockbackCounter <= 0) //Si esta aplicandose un knockback no se puede mover
        {
            moveInput = new Vector3(h, 0f, v); //.normalized;
        } else {
            knockbackCounter -= Time.deltaTime;
        }
        
        Vector3 currentVelocity = PlayerRigidbody.velocity;

        Vector3 newVelocity = new Vector3(moveInput.x * Speed * SpeedMultiple, currentVelocity.y, moveInput.z * Speed * SpeedMultiple);

        if (!dasheando)
        {
            PlayerRigidbody.velocity = newVelocity;
        }
    }

    public void ManejoDash()
    {
        fuerzaDash = Speed * 3.5f * SpeedMultiple;
        // Activar el Dash (Hay que presionar la Barra Espaciadora, que no se esté dasheando y que el cooldown se encuentre desactivado)
        if (Input.GetKeyDown(KeyCode.Space) && !dasheando && !cooldownActivo && moveInput != Vector3.zero)
        {
            StartCoroutine(Dash());
        }
    }

    public void MovimientoCursor()
    {
        // Obtener la posición del cursor en pantalla
        Vector3 posicionCursor = Input.mousePosition;

        // Convertir la posición del cursor a una posición en el mundo
        Ray rayo = Camera.main.ScreenPointToRay(posicionCursor);
        RaycastHit hit;

        if (Physics.Raycast(rayo, out hit))
        {
            // Calcular la dirección hacia la posición del cursor en el eje X
            Vector3 direccion = hit.point - transform.position;
            direccion.y = 0f;

            // Calcular la rotación deseada para mirar hacia la dirección
            Quaternion rotacionDeseada = Quaternion.LookRotation(direccion);

            // Interpolar gradualmente hacia la nueva rotación
            transform.rotation = Quaternion.Lerp(transform.rotation, rotacionDeseada, velocidadRotacion * Time.deltaTime);
        }
    }

    private IEnumerator Dash()
    {
        dasheando = true;


        // Almacenar la velocidad actual del jugador
        Vector3 velocidadOriginal = PlayerRigidbody.velocity;

        // Calcular la dirección del dash
        direccionDash = moveInput.normalized;

        // Aplicar una fuerza momentánea para el dash
        PlayerRigidbody.AddForce(direccionDash.normalized * fuerzaDash, ForceMode.Impulse);

        yield return new WaitForSeconds(duracionDash);
        
        dasheando = false;


        // Restaurar la velocidad original después del dash
        PlayerRigidbody.velocity = velocidadOriginal;

        // Activar el cooldown del dash
        cooldownActivo = true;

        yield return new WaitForSeconds(determinarCooldown);


        // Desactivar el cooldown y permitir otro dash
        cooldownActivo = false;
        
    }
    
    public void ApplyKnockback (Vector3 direction, float time, float force) { //Aplicar knockback al jugador
        knockbackCounter = time;
        moveInput = direction * force;
    }
}