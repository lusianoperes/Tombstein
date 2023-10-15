using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;



public class PlayerController : MonoBehaviour
{
    public Rigidbody PlayerRigidbody;
    public Inventory PlayerInventory;
    public Jugador jugador;
    public GameObject inventarioVisual;
    public GameObject panelPausa;
    public InventoryManager inventoryManager;
    public EffectManager effectManager;
    public Vector3 moveInput;
    public GameObject equipamiento;
    public GameObject bodypartsHud;

    Animator animator;

    public float Speed = 7f; // Velocidad base del personaje
    public float SpeedMultiple = 1.0f; // Velocidad adicional para mi personaje
    public float DashMultiple = 1.0f; //multiplicador de velocidad para el dash (dash usa velocidad base mulitplicado por este número)
    public float fuerzaDash; //= 25f; // Fuerza del Dash
    public float duracionDash = 0.25f; // Duración del Dash
    public float dashCooldown; // Cooldown del Dash

    private bool dasheando = false;
    private Vector3 direccionDash;
    public int maxDashes; // Catidad máxima de Dashes --// se puede llamar a .Agilidad directamente
    public int cantActualDashes; // Cantidad actual de Dashes
    public bool sumandoDash = false; // Si está en proceso de sumar un Dash o no
    public bool reiniciarCantActualDashes = true; // Si es necesario reiniciar la cantidad de Dashes actuales o no
    public float velocidadRotacion = 20f; //Para que el jugador gire con el cursor

    public bool isDoinSomething = false;
    public bool primaryOnCooldown = false;
    public bool secondaryOnCooldown = false;
    public bool chargedStarted = false;
    public float timeCounterA = 0f;
    public float timeCounterB = 0f;
    public bool headOnCooldown = false;
    public bool torsoOnCooldown = false;
    public bool armOnCooldown = false;
    public bool legOnCooldown = false;

    private Weapon primaria;
    private Weapon secundaria;
    private Melee equipedMelee;
    private Distance equipedDistance;


    public PickableObject objetoMasCercanoAnterior = null;
    public bool forceGrab = false;
    GameObject luz = null;

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
        jugador = GetComponent<Jugador>();
        inventoryManager = inventarioVisual.GetComponent<InventoryManager>();
        animator = GetComponentsInChildren<Animator>()[0];
        
    }

    private void Start()
    {
        forceGrab = true;
    }

    private void Update()
    {
        ManejoDash();
        MovimientoCursor();
        AgarrarObjeto();
        AbrirCerrarInventario();
        CambiarConsumible();
        UsarConsumible();
        HabilidadesBodyParts();

        if (PlayerInventory.primaryWeapon != null && Input.GetMouseButton(0) && !(isDoinSomething) && !(primaryOnCooldown))
        {
            animator.SetTrigger("Pegar");
            primaria = PlayerInventory.primaryWeapon.GetComponent<Weapon>();
            equipedMelee = primaria as Melee;

            equipedMelee.StartCoroutine(equipedMelee.DoAttack());

            StartCoroutine(CooldownPrimary(PlayerInventory.primaryWeapon.GetComponent<Weapon>().cooldown));
        }

        if(PlayerInventory.secondaryWeapon != null && Input.GetMouseButton(1))
        {
            secundaria = PlayerInventory.secondaryWeapon.GetComponent<Weapon>();
            equipedDistance = secundaria as Distance;
            switch (equipedDistance.distanceType)
            {
            case Distance.DistanceType.triggerOnly:
                if (Input.GetMouseButton(1) && !(isDoinSomething) && !(secondaryOnCooldown))
                {
                    equipedDistance.StartCoroutine(equipedDistance.TriggerOnly_DoFire());
                    StartCoroutine(CooldownSecondary(secundaria.GetComponent<Weapon>().cooldown));
                }
                break;

            case Distance.DistanceType.triggerHoldable:
                if (Input.GetMouseButton(1) && !(isDoinSomething) && !(secondaryOnCooldown))
                {
                    isDoinSomething = true;
                    chargedStarted = true;
                }
                else if (Input.GetMouseButton(1) && isDoinSomething && chargedStarted)
                {

                    if (timeCounterA < secundaria.GetComponent<Weapon>().weaponRange)
                    {
                        timeCounterA += Time.deltaTime;
                    }

                }
                break;
        
            default:
                break;
            }
        }
        else if (!(Input.GetMouseButton(1)) && isDoinSomething && chargedStarted && PlayerInventory.secondaryWeapon != null)
        {
                    Debug.Log("disparó");
                    equipedDistance.StartCoroutine(equipedDistance.TriggerHoldable_DoFire(timeCounterA));
                    StartCoroutine(CooldownSecondary(secundaria.GetComponent<Weapon>().cooldown));
                    timeCounterA = 0;
                    chargedStarted = false;
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

        if (h != 0.0f || v!=0.0f)
        {
            animator.SetBool("IsRunning", true);
        }
        else
        {
            animator.SetBool("IsRunning", false);
        }

        moveInput = new Vector3(h, 0f, v); //.normalized;

        Vector3 currentVelocity = PlayerRigidbody.velocity;

        Vector3 newVelocity = new Vector3(moveInput.x * Speed * SpeedMultiple, currentVelocity.y, moveInput.z * Speed * SpeedMultiple);

        if (!dasheando)
        {
            PlayerRigidbody.velocity = newVelocity;
        }
    }

    public void ManejoDash()
    {   
        maxDashes = jugador.Agilidad;
        
        dashCooldown = 10 / jugador.Estamina;

        fuerzaDash = Speed * DashMultiple;

        // Iguala la cantidad de Dashes a su capacidad máxima
        if(reiniciarCantActualDashes)
        {
            reiniciarCantActualDashes = false;
            cantActualDashes = jugador.Agilidad;
        }

        // Activar el Dash (Hay que presionar la Barra Espaciadora, que no se esté dasheando y que el cooldown se encuentre desactivado)
        if (Input.GetKeyDown(KeyCode.Space) && !dasheando && cantActualDashes > 0 && moveInput != Vector3.zero)
        {
            StartCoroutine(Dash());
        }

        // Activar SumarDashes cuando no tenga la cantidad máxima de dashes y que no esté en proceso de sumarlo
        if (cantActualDashes < maxDashes && !sumandoDash)
        {
             StartCoroutine(SumarDash());
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

        animator.SetTrigger("Dash");
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

        // Se le resta un dash
        cantActualDashes--;
    }

    private IEnumerator SumarDash()
    {
        // Determinar que se está sumando el Dash
        sumandoDash = true;
        // Cooldown de la recarga del Dash
        yield return new WaitForSeconds(dashCooldown);
        // Aumenta la cantidad de Dashes actuales
        cantActualDashes++;
        // Determinar que terminó de sumar el Dash
        sumandoDash = false;


    }

    public void CambiarOpacidadHUD(Image imagen, float valor)
    {

      Color colorImagen = imagen.color;
      colorImagen.a = valor;
      imagen.color = colorImagen;

    }


    public void UsarConsumible()
    {
        if (Input.GetKeyDown(KeyCode.C) && !inventoryManager.isInventoryOpen)
        {

            //añadir efecto del consumible al effectmanager
            if (PlayerInventory.consumibles[1] != null)
            {
                Debug.Log("efecto consmuidosdoosos");
                StartCoroutine(PlayerInventory.consumibles[1].efectoPasivo.AplicarEfecto(gameObject.GetComponent<Jugador>(), equipamiento.transform.Find("Consumibles").Find("SelectedConsumible").GetChild(1).gameObject));
                //activar collider de slot parent
                equipamiento.transform.Find("Consumibles").Find("SelectedConsumible").GetComponent<BoxCollider2D>().enabled = true;
                //eliminar consumible visualmente
                equipamiento.transform.Find("Consumibles").Find("SelectedConsumible").GetChild(1).parent = null;
                //eliminar consumible lógicamente
                PlayerInventory.consumibles[1] = null;
            }

        }
    }

    public void CambiarConsumible()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            GameObject auxLeft = null;
            GameObject auxMid = null;
            GameObject auxRight = null;
            //Intercambiar visualmente
            if (equipamiento.transform.Find("Consumibles").Find("LeftConsumible").childCount > 1)
            {
                auxLeft = equipamiento.transform.Find("Consumibles").Find("LeftConsumible").GetChild(1).gameObject;
            }
            if (equipamiento.transform.Find("Consumibles").Find("SelectedConsumible").childCount > 1)
            {
                auxMid = equipamiento.transform.Find("Consumibles").Find("SelectedConsumible").GetChild(1).gameObject;
            }
            if (equipamiento.transform.Find("Consumibles").Find("RightConsumible").childCount > 1)
            {
                auxRight = equipamiento.transform.Find("Consumibles").Find("RightConsumible").GetChild(1).gameObject;
            }

            if (auxLeft != null)
            {
                auxLeft.transform.parent = equipamiento.transform.Find("Consumibles").Find("SelectedConsumible").transform;
                auxLeft.transform.localPosition = Vector3.zero;
            }
            if (auxMid != null)
            {
                auxMid.transform.parent = equipamiento.transform.Find("Consumibles").Find("RightConsumible").transform;
                auxMid.transform.localPosition = Vector3.zero;
            }
            if (auxRight != null)
            {
                auxRight.transform.parent = equipamiento.transform.Find("Consumibles").Find("LeftConsumible").transform;
                auxRight.transform.localPosition = Vector3.zero;
            }


            //Intercambiar lógicamente
            Consumible auxiliar = null;
            auxiliar = PlayerInventory.consumibles[0];
            PlayerInventory.consumibles[0] = PlayerInventory.consumibles[2];
            PlayerInventory.consumibles[2] = PlayerInventory.consumibles[1];
            PlayerInventory.consumibles[1] = auxiliar;

        }
    }


    public void AbrirCerrarInventario()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (inventoryManager.isInventoryOpen)
            {
                inventoryManager.isInventoryOpen = false;
                inventarioVisual.SetActive(false);
                panelPausa.SetActive(false);

                CambiarOpacidadHUD(equipamiento.transform.GetChild(0).GetComponent<Image>(), 0.3f);
                CambiarOpacidadHUD(equipamiento.transform.GetChild(0).transform.GetChild(0).GetComponent<Image>(), 0.3f);
                CambiarOpacidadHUD(equipamiento.transform.GetChild(1).GetComponent<Image>(), 0.3f);
                CambiarOpacidadHUD(equipamiento.transform.GetChild(1).transform.GetChild(0).GetComponent<Image>(), 0.3f);
                CambiarOpacidadHUD(equipamiento.transform.GetChild(2).transform.GetChild(0).GetComponent<Image>(), 0.3f);
                CambiarOpacidadHUD(equipamiento.transform.GetChild(2).transform.GetChild(0).transform.GetChild(0).GetComponent<Image>(), 0.3f);
                CambiarOpacidadHUD(equipamiento.transform.GetChild(2).transform.GetChild(1).GetComponent<Image>(), 0.3f);
                CambiarOpacidadHUD(equipamiento.transform.GetChild(2).transform.GetChild(1).transform.GetChild(0).GetComponent<Image>(), 0.3f);
                CambiarOpacidadHUD(equipamiento.transform.GetChild(2).transform.GetChild(2).GetComponent<Image>(), 0.5f);
                CambiarOpacidadHUD(equipamiento.transform.GetChild(2).transform.GetChild(2).transform.GetChild(0).GetComponent<Image>(), 0.5f);

                if (equipamiento.transform.GetChild(0).transform.childCount == 2)
                {
                    CambiarOpacidadHUD(equipamiento.transform.GetChild(0).transform.GetChild(1).GetComponent<Image>(), 0.7f);
                }
                if (equipamiento.transform.GetChild(1).transform.childCount == 2)   
                {
                    CambiarOpacidadHUD(equipamiento.transform.GetChild(1).transform.GetChild(1).GetComponent<Image>(), 0.7f);
                }
                if (equipamiento.transform.GetChild(2).transform.GetChild(0).transform.childCount == 2)
                {
                    CambiarOpacidadHUD(equipamiento.transform.GetChild(2).transform.GetChild(0).transform.GetChild(1).GetComponent<Image>(), 0.7f);
                }
                if (equipamiento.transform.GetChild(2).transform.GetChild(1).transform.childCount == 2)
                {
                    CambiarOpacidadHUD(equipamiento.transform.GetChild(2).transform.GetChild(1).transform.GetChild(1).GetComponent<Image>(), 0.7f);
                }
                if (equipamiento.transform.GetChild(2).transform.GetChild(2).transform.childCount == 2)
                {
                    CambiarOpacidadHUD(equipamiento.transform.GetChild(2).transform.GetChild(2).transform.GetChild(1).GetComponent<Image>(), 0.8f);
                }
                

                if (inventarioVisual.GetComponent<InventoryManager>().currentDraggedItem != null)
                {
                    inventarioVisual.GetComponent<InventoryManager>().currentDraggedItem.transform.SetParent(inventarioVisual.GetComponent<InventoryManager>().currentDraggedItem.GetComponent<DraggableObject>().slotParent.transform);
                    inventarioVisual.GetComponent<InventoryManager>().currentDraggedItem.transform.localPosition = Vector3.zero;
                }

            }
            else
            {

                CambiarOpacidadHUD(equipamiento.transform.GetChild(0).GetComponent<Image>(), 1f);
                CambiarOpacidadHUD(equipamiento.transform.GetChild(0).transform.GetChild(0).GetComponent<Image>(), 1f);
                CambiarOpacidadHUD(equipamiento.transform.GetChild(1).GetComponent<Image>(), 1f);
                CambiarOpacidadHUD(equipamiento.transform.GetChild(1).transform.GetChild(0).GetComponent<Image>(), 1f);
                CambiarOpacidadHUD(equipamiento.transform.GetChild(2).transform.GetChild(0).GetComponent<Image>(), 1f);
                CambiarOpacidadHUD(equipamiento.transform.GetChild(2).transform.GetChild(0).transform.GetChild(0).GetComponent<Image>(), 1f);
                CambiarOpacidadHUD(equipamiento.transform.GetChild(2).transform.GetChild(1).GetComponent<Image>(), 1f);
                CambiarOpacidadHUD(equipamiento.transform.GetChild(2).transform.GetChild(1).transform.GetChild(0).GetComponent<Image>(), 1f);
                CambiarOpacidadHUD(equipamiento.transform.GetChild(2).transform.GetChild(2).GetComponent<Image>(), 1f);
                CambiarOpacidadHUD(equipamiento.transform.GetChild(2).transform.GetChild(2).transform.GetChild(0).GetComponent<Image>(), 1f);

                if (equipamiento.transform.GetChild(0).transform.childCount == 2)
                {
                    CambiarOpacidadHUD(equipamiento.transform.GetChild(0).transform.GetChild(1).GetComponent<Image>(), 1f);
                }
                if (equipamiento.transform.GetChild(1).transform.childCount == 2)
                {
                    CambiarOpacidadHUD(equipamiento.transform.GetChild(1).transform.GetChild(1).GetComponent<Image>(), 1f);
                }
                if (equipamiento.transform.GetChild(2).transform.GetChild(0).transform.childCount == 2)
                {
                    CambiarOpacidadHUD(equipamiento.transform.GetChild(2).transform.GetChild(0).transform.GetChild(1).GetComponent<Image>(), 1f);
                }
                if (equipamiento.transform.GetChild(2).transform.GetChild(1).transform.childCount == 2)
                {
                    CambiarOpacidadHUD(equipamiento.transform.GetChild(2).transform.GetChild(1).transform.GetChild(1).GetComponent<Image>(), 1f);
                }
                if (equipamiento.transform.GetChild(2).transform.GetChild(2).transform.childCount == 2)
                {
                    CambiarOpacidadHUD(equipamiento.transform.GetChild(2).transform.GetChild(2).transform.GetChild(1).GetComponent<Image>(), 1f);
                }
        

                inventoryManager.isInventoryOpen = true;
                inventarioVisual.SetActive(true);
                panelPausa.SetActive(true);
            }
        }
    }


    public void HabilidadesBodyParts()
    {
        if (Input.GetKeyDown(KeyCode.R) && PlayerInventory.headPart[1] != null && !headOnCooldown)
        {
            Debug.Log("usaste partecabesa");
            StartCoroutine(PlayerInventory.headPart[1].UsarParte());
            StartCoroutine(CooldownEnHudBodyparts(bodypartsHud.transform.GetChild(0).Find("HeadAbility").GetChild(0).gameObject, PlayerInventory.headPart[1].useCooldown));
        }
        if (Input.GetKeyDown(KeyCode.T) && PlayerInventory.torsoPart[1] != null && !torsoOnCooldown && !isDoinSomething)
        {
            StartCoroutine(PlayerInventory.torsoPart[1].UsarParte());
            StartCoroutine(CooldownEnHudBodyparts(bodypartsHud.transform.GetChild(0).Find("TorsoAbility").GetChild(0).gameObject, PlayerInventory.torsoPart[1].useCooldown));
        }
        if (Input.GetKeyDown(KeyCode.Q) && PlayerInventory.armPart[1] != null && !armOnCooldown && !isDoinSomething)
        {
            StartCoroutine(PlayerInventory.armPart[1].UsarParte());
            StartCoroutine(CooldownEnHudBodyparts(bodypartsHud.transform.GetChild(0).Find("ArmAbility").GetChild(0).gameObject, PlayerInventory.armPart[1].useCooldown));
        }
        if ((Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) && PlayerInventory.legPart[1] != null && !legOnCooldown)
        {
            StartCoroutine(PlayerInventory.legPart[1].UsarParte());
            StartCoroutine(CooldownEnHudBodyparts(bodypartsHud.transform.GetChild(0).Find("LegAbility").GetChild(0).gameObject, PlayerInventory.legPart[1].useCooldown));
        }
    }

    public IEnumerator CooldownEnHudBodyparts(GameObject textoBP, float cooldown)
    {
        switch(textoBP.name)
        {
            case "HeadCooldownText":
            headOnCooldown = true;
            break;
            case "TorsoCooldownText":
            torsoOnCooldown = true;
            isDoinSomething = true;
            break;
            case "ArmCooldownText":
            armOnCooldown = true;
            isDoinSomething = true;
            break;
            case "LegCooldownText":
            legOnCooldown = true;
            break;
        }

        textoBP.GetComponent<TextMeshProUGUI>().enabled = true;
        for(int i = 0; i < cooldown; i++)
        {
            textoBP.GetComponent<TextMeshProUGUI>().text = (cooldown - i).ToString();
            yield return new WaitForSeconds(1);
        }
        textoBP.GetComponent<TextMeshProUGUI>().enabled = false;
        
        switch(textoBP.name)
        {
            case "HeadCooldownText":
            headOnCooldown = false;
            break;
            case "TorsoCooldownText":
            torsoOnCooldown = false;
            isDoinSomething = false;
            break;
            case "ArmCooldownText":
            armOnCooldown = false;
            isDoinSomething = false;
            break;
            case "LegCooldownText":
            legOnCooldown = false;
            break;
        }
    }


    public void AgarrarObjeto()
    {
        Vector3 lightPosition = Vector3.zero;
        float radioDeAlcance = 3f;
        Collider[] colliders = Physics.OverlapSphere(transform.position, radioDeAlcance);
        float distanciaMinima = Mathf.Infinity;
        PickableObject objetoMasCercano = null;

        foreach (Collider collider in colliders)
        {
            PickableObject pickableObject = collider.GetComponent<PickableObject>();

            if (pickableObject != null)
            {
                float distancia = Vector3.Distance(transform.position, collider.transform.position);

                if (distancia < distanciaMinima)
                {
                    distanciaMinima = distancia;
                    objetoMasCercano = pickableObject;
                }
            }
        }

        if (objetoMasCercano != null && objetoMasCercanoAnterior == null)
        {   
            Outline outlineNew = objetoMasCercano.gameObject.GetComponent<Outline>();
            outlineNew.outlineColor = Color.magenta;
            objetoMasCercanoAnterior = objetoMasCercano;
            outlineNew.needsUpdate = true;
            Debug.Log("coloreo magenta");
        }
        else if (objetoMasCercano == null && objetoMasCercanoAnterior != null)
        {
            Outline outline = objetoMasCercanoAnterior.gameObject.GetComponent<Outline>();
            outline.outlineColor = Color.white;
            outline.needsUpdate = true;
            objetoMasCercanoAnterior = null;
            Debug.Log("coloreo blanco al lejano");
        }
        else if (objetoMasCercano != objetoMasCercanoAnterior)
        {
            Outline outline = objetoMasCercanoAnterior.gameObject.GetComponent<Outline>();
            outline.outlineColor = Color.white;
            outline.needsUpdate = true;

            Outline outlineNew = objetoMasCercano.gameObject.GetComponent<Outline>();
            outlineNew.outlineColor = Color.magenta;
            outlineNew.needsUpdate = true;
            objetoMasCercanoAnterior = objetoMasCercano;
            
        }

        if (Input.GetKeyDown(KeyCode.E) || forceGrab)
        {

            if (objetoMasCercano != null)
            {
                int index = IndexDeInventario();
                if (index == -1)
                {
                    Debug.Log("No hay espacio en el inventario");
                }
                else
                {
                    PlayerInventory.objetos[index] = objetoMasCercano.gameObject.GetComponent<Item>();
                    inventoryManager.AgregarObjetoInventario(index, objetoMasCercano.gameObject);
                }
            }
            forceGrab = false;
        }

    }
    public int IndexDeInventario()
    {
        for (int i = 0; i < PlayerInventory.objetos.Length; i++)
        {
            if (PlayerInventory.objetos[i] == null)
            {
                return i;
            }

        }
        return -1;
    }


}