using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PlayerController : MonoBehaviour
{
    public Rigidbody PlayerRigidbody;
    public Inventory PlayerInventory;
    public GameObject inventarioVisual;
    public GameObject panelPausa;
    public InventoryManager inventoryManager;
    public EffectManager effectManager;
    public Vector3 moveInput;
    public GameObject equipamiento;

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

    private Weapon primaria;
    private Weapon secundaria;
    private Melee equipedMelee;
    private Distance equipedDistance;


    public bool hasWeapon = false;
    public bool actualizeSecondary = false;

    public PickableObject objetoMasCercanoAnterior = null;
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
        inventoryManager = inventarioVisual.GetComponent<InventoryManager>();
    }

    private void Update()
    {
        ManejoDash();
        MovimientoCursor();
        AgarrarObjeto();
        AbrirCerrarInventario();
        CambiarConsumible();
        UsarConsumible();

        if (PlayerInventory.primaryWeapon != null && Input.GetMouseButton(0) && !(isDoinSomething) && !(primaryOnCooldown))
        {
            primaria = PlayerInventory.primaryWeapon.GetComponent<Weapon>();
            equipedMelee = primaria as Melee;

            equipedMelee.StartCoroutine(equipedMelee.DoAttack());

            StartCoroutine(CooldownPrimary(PlayerInventory.primaryWeapon.GetComponent<Weapon>().cooldown));
        }
        if (actualizeSecondary)
        {
            secundaria = PlayerInventory.secondaryWeapon.GetComponent<Weapon>();
            equipedDistance = secundaria as Distance;
        }
        if (hasWeapon)
        {
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
                    else if (!(Input.GetMouseButton(1)) && isDoinSomething && chargedStarted)
                    {

                        equipedDistance.StartCoroutine(equipedDistance.TriggerHoldable_DoFire(timeCounterA));
                        StartCoroutine(CooldownSecondary(secundaria.GetComponent<Weapon>().cooldown));
                        timeCounterA = 0;
                        chargedStarted = false;
                    }
                    break;

                default:
                    //no tiene arma.
                    break;
            }
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
                //EfectoPasivo efectoAux = new EfectoPasivo(PlayerInventory.consumibles[1].efectoPasivo);
                /*EfectoPasivo efectoAux= new EfectoPasivo();
                efectoAux.duracionEfecto = PlayerInventory.consumibles[1].efectoPasivo.duracionEfecto;
                efectoAux.coeficienteEfecto = PlayerInventory.consumibles[1].efectoPasivo.coeficienteEfecto;
                efectoAux.effectSprite = PlayerInventory.consumibles[1].efectoPasivo.effectSprite;
                efectoAux.effectType = PlayerInventory.consumibles[1].efectoPasivo.effectType;
                efectoAux.DarEfectoAlJugador_finished = false;
                efectoAux.MostrarEfectoVisualmente_finished = false;
                Debug.Log(gameObject.GetComponent<Jugador>());*/
                StartCoroutine(PlayerInventory.consumibles[1].efectoPasivo.AplicarEfecto(gameObject.GetComponent<Jugador>()));
                //activar collider de slot parent
                equipamiento.transform.Find("Consumibles").Find("SelectedConsumible").GetComponent<BoxCollider2D>().enabled = true;
                //eliminar consumible visualmente
                //Destroy(equipamiento.transform.Find("Consumibles").Find("SelectedConsumible").GetChild(1).gameObject);
                //eliminar consumible lógicamente
                //PlayerInventory.consumibles[1] = null;
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

                CambiarOpacidadHUD(equipamiento.transform.GetChild(0).GetComponent<Image>(), 0.2f);
                CambiarOpacidadHUD(equipamiento.transform.GetChild(0).transform.GetChild(0).GetComponent<Image>(), 0.2f);
                CambiarOpacidadHUD(equipamiento.transform.GetChild(1).GetComponent<Image>(), 0.2f);
                CambiarOpacidadHUD(equipamiento.transform.GetChild(1).transform.GetChild(0).GetComponent<Image>(), 0.2f);
                CambiarOpacidadHUD(equipamiento.transform.GetChild(2).transform.GetChild(0).GetComponent<Image>(), 0.2f);
                CambiarOpacidadHUD(equipamiento.transform.GetChild(2).transform.GetChild(0).transform.GetChild(0).GetComponent<Image>(), 0.2f);
                CambiarOpacidadHUD(equipamiento.transform.GetChild(2).transform.GetChild(1).GetComponent<Image>(), 0.2f);
                CambiarOpacidadHUD(equipamiento.transform.GetChild(2).transform.GetChild(1).transform.GetChild(0).GetComponent<Image>(), 0.2f);
                CambiarOpacidadHUD(equipamiento.transform.GetChild(2).transform.GetChild(2).GetComponent<Image>(), 0.2f);
                CambiarOpacidadHUD(equipamiento.transform.GetChild(2).transform.GetChild(2).transform.GetChild(0).GetComponent<Image>(), 0.2f);

                if (equipamiento.transform.GetChild(0).transform.childCount == 2)
                {
                    CambiarOpacidadHUD(equipamiento.transform.GetChild(0).transform.GetChild(1).GetComponent<Image>(), 0.2f);
                }
                if (equipamiento.transform.GetChild(1).transform.childCount == 2)
                {
                    CambiarOpacidadHUD(equipamiento.transform.GetChild(1).transform.GetChild(1).GetComponent<Image>(), 0.2f);
                }
                if (equipamiento.transform.GetChild(2).transform.GetChild(0).transform.childCount == 2)
                {
                    CambiarOpacidadHUD(equipamiento.transform.GetChild(2).transform.GetChild(0).transform.GetChild(1).GetComponent<Image>(), 0.2f);
                }
                if (equipamiento.transform.GetChild(2).transform.GetChild(1).transform.childCount == 2)
                {
                    CambiarOpacidadHUD(equipamiento.transform.GetChild(2).transform.GetChild(1).transform.GetChild(1).GetComponent<Image>(), 0.2f);
                }
                if (equipamiento.transform.GetChild(2).transform.GetChild(2).transform.childCount == 2)
                {
                    CambiarOpacidadHUD(equipamiento.transform.GetChild(2).transform.GetChild(2).transform.GetChild(1).GetComponent<Image>(), 0.2f);
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
            objetoMasCercano.gameObject.AddComponent<Outline>();
            objetoMasCercanoAnterior = objetoMasCercano;
        }
        else if (objetoMasCercano == null && objetoMasCercanoAnterior != null)
        {
            if (objetoMasCercanoAnterior.gameObject.GetComponent<Outline>() != null)
            {
                Outline outline = objetoMasCercanoAnterior.gameObject.GetComponent<Outline>();
                Destroy(outline);
            }
            objetoMasCercanoAnterior = null;
        }
        else if (objetoMasCercano != objetoMasCercanoAnterior)
        {
            Outline outline = objetoMasCercanoAnterior.gameObject.GetComponent<Outline>();
            Destroy(outline);
            objetoMasCercano.gameObject.AddComponent<Outline>();
            objetoMasCercanoAnterior = objetoMasCercano;
        }

        if (Input.GetKeyDown(KeyCode.E))
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