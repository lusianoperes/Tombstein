using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableObject : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler, IPointerExitHandler, IPointerEnterHandler

{
    public GameObject slotParent;
    public GameObject itemPrefab;
    public GameObject jugador;
    public GameObject inventarioVisual;
    public GameObject bloqueDescripcion;
    public GameObject nombreItem;
    public GameObject descripcionItem;
    public GameObject spriteItem;
    private bool isDragging = false;
    private Vector2 offset;

    public void Start()
    {
        jugador = GameObject.Find("Jugador");
        inventarioVisual = GameObject.Find("Inventario");
        bloqueDescripcion = inventarioVisual.GetComponent<InventoryManager>().Descriptor;
        nombreItem = inventarioVisual.GetComponent<InventoryManager>().nombreDescriptor;
        descripcionItem = inventarioVisual.GetComponent<InventoryManager>().descripcionDescriptor;
        spriteItem = inventarioVisual.GetComponent<InventoryManager>().spriteDescriptor;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("Mouse encima de objeto");
        bloqueDescripcion.SetActive(true);
        spriteItem.GetComponent<Image>().sprite = itemPrefab.GetComponent<Item>().imagenAsociada;
        nombreItem.GetComponent<Text>().text = itemPrefab.GetComponent<Item>().itemName;
        descripcionItem.GetComponent<Text>().text = itemPrefab.GetComponent<Item>().itemDescription;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("Mouse fuera de objeto");
        bloqueDescripcion.SetActive(false);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (inventarioVisual.GetComponent<InventoryManager>().isInventoryOpen)
        {
            isDragging = true;
            inventarioVisual.GetComponent<InventoryManager>().currentDraggedItem = this.gameObject;
            offset = eventData.position - (Vector2)transform.position;
            transform.SetParent(inventarioVisual.transform.parent);
        }

    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isDragging && inventarioVisual.GetComponent<InventoryManager>().isInventoryOpen)
        {
            transform.position = eventData.position - offset;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (inventarioVisual.GetComponent<InventoryManager>().isInventoryOpen)
        {
            isDragging = false;

            GameObject dropObject = eventData.pointerCurrentRaycast.gameObject;

            Collider2D[] colliders = Physics2D.OverlapPointAll(eventData.position);

            Collider2D closestCollider = null;
            float closestDistance = float.MaxValue;
            Vector2 center = transform.position;

            foreach (Collider2D collider in colliders)
            {
                if (collider.gameObject != gameObject)
                {
                    Vector2 closestPoint = collider.ClosestPoint(center);
                    float distance = Vector2.Distance(center, closestPoint);

                    if (distance < closestDistance)
                    {
                        closestCollider = collider;
                        closestDistance = distance;
                    }
                }
            }

            if (closestCollider != null)
            {
                if (closestCollider.gameObject.GetComponent<DraggableObject>() != null)
                {
                    Debug.Log("SOLTADO EN OTRO OBJETO NASH");

                    if (closestCollider.transform.parent.GetComponent<Slot>().slotype == Slot.SlotType.Anything && slotParent.GetComponent<Slot>().slotype == Slot.SlotType.Anything)
                    {
                        Debug.Log("AMBOS ITEMS ESTAN EN SLOT ANYTHING");
                        IntercambioPosicionObjetos(gameObject, closestCollider.gameObject);

                        int ExTiradoIndex = slotParent.transform.GetSiblingIndex();
                        int ExAplastadoIndex = closestCollider.gameObject.GetComponent<DraggableObject>().slotParent.transform.GetSiblingIndex();
                        Item ExaplastadoAux = jugador.GetComponent<Inventory>().objetos[ExAplastadoIndex];
                        jugador.GetComponent<Inventory>().objetos[ExAplastadoIndex] = jugador.GetComponent<Inventory>().objetos[ExTiradoIndex];
                        jugador.GetComponent<Inventory>().objetos[ExTiradoIndex] = ExaplastadoAux;
                    }
                    else if (TipoDeItem(closestCollider.transform.GetChild(0).gameObject) == TipoDeItem(transform.GetChild(0).gameObject) &&
                    ((itemPrefab.GetComponent<Weapon>() == null || itemPrefab.GetComponent<Weapon>().weaponType == closestCollider.transform.GetChild(0).GetComponent<Weapon>().weaponType)
                    && (itemPrefab.GetComponent<Armor>() == null || itemPrefab.GetComponent<Armor>().armorpartType == closestCollider.transform.GetChild(0).GetComponent<Armor>().armorpartType)
                    && (itemPrefab.GetComponent<BodyPart>() == null || itemPrefab.GetComponent<BodyPart>().bodypartType == closestCollider.transform.GetChild(0).GetComponent<BodyPart>().bodypartType)))
                    {
                        Debug.Log("CUMPLE CONDICIONES DE INTERCAMBIO");
                        Item aplastadoAux = null;

                        IntercambioLogicoObjetos(jugador, this.gameObject, closestCollider.gameObject, ref aplastadoAux);
                        IntercambioPosicionObjetos(gameObject, closestCollider.gameObject);
                    }
                    else
                    {
                        Debug.Log("NO SE PUEDEN INTERCAMBIAR FORREEEAL");
                        transform.SetParent(slotParent.transform);
                        transform.localPosition = Vector3.zero;
                    }
                }
                else if (closestCollider.gameObject.GetComponent<Slot>() != null)
                {
                    Debug.Log("SOLTADO EN SLOT");
                    Item aplastadoAux = null;

                    if (itemPrefab.GetComponent<Armor>() != null && ((closestCollider.gameObject.GetComponent<Slot>().slotype == Slot.SlotType.Armor && itemPrefab.GetComponent<Armor>().armorpartType == closestCollider.gameObject.GetComponent<Slot>().slotarmorType) || closestCollider.gameObject.GetComponent<Slot>().slotype == Slot.SlotType.Anything))
                    {
                        if (slotParent.GetComponent<Slot>().slotype == Slot.SlotType.Anything)
                        {
                            //COPIAR CONTENIDO Y BORRAR EN EL LUGAR
                            aplastadoAux = jugador.GetComponent<Inventory>().objetos[slotParent.transform.GetSiblingIndex()];
                            jugador.GetComponent<Inventory>().objetos[slotParent.transform.GetSiblingIndex()] = null;
                        }
                        else
                        {
                            switch (itemPrefab.GetComponent<Armor>().armorpartType)
                            {
                                case Armor.ArmorpartType.HeadPart:
                                    aplastadoAux = jugador.GetComponent<Inventory>().headProtection;
                                    jugador.GetComponent<Inventory>().headProtection = null;
                                    break;

                                case Armor.ArmorpartType.TorsoPart:
                                    aplastadoAux = jugador.GetComponent<Inventory>().torsoProtection;
                                    jugador.GetComponent<Inventory>().torsoProtection = null;
                                    break;

                                case Armor.ArmorpartType.LegPart:
                                    aplastadoAux = jugador.GetComponent<Inventory>().legProtection;
                                    jugador.GetComponent<Inventory>().legProtection = null;
                                    break;

                                case Armor.ArmorpartType.FeetPart:
                                    aplastadoAux = jugador.GetComponent<Inventory>().feetProtection;
                                    jugador.GetComponent<Inventory>().feetProtection = null;
                                    break;

                                default:

                                    break;
                            }
                        }

                        //Movimiento visual ingame
                        transform.SetParent(closestCollider.transform);
                        slotParent.GetComponent<BoxCollider2D>().enabled = true; //activar collider de slot del que me fui
                        slotParent = closestCollider.gameObject;
                        slotParent.GetComponent<BoxCollider2D>().enabled = false; //desactivar collider de slot al que me voy
                        transform.localPosition = Vector3.zero;
                        GetComponent<BoxCollider2D>().size = transform.parent.GetComponent<RectTransform>().sizeDelta; //ajustar collider del sprite al tamaño del slot
                        GetComponent<RectTransform>().sizeDelta = new Vector2(transform.parent.GetComponent<RectTransform>().rect.width, transform.parent.GetComponent<RectTransform>().rect.height); //ajustar tamaño de sprite al slot
                    }
                    else if (itemPrefab.GetComponent<BodyPart>() != null && ((closestCollider.gameObject.GetComponent<Slot>().slotype == Slot.SlotType.BodyPart && itemPrefab.GetComponent<BodyPart>().bodypartType == closestCollider.gameObject.GetComponent<Slot>().slotbodyType) || closestCollider.gameObject.GetComponent<Slot>().slotype == Slot.SlotType.Anything))
                    {
                        if (slotParent.GetComponent<Slot>().slotype == Slot.SlotType.Anything)
                        {
                            //COPIAR CONTENIDO Y BORRAR EN EL LUGAR
                            aplastadoAux = jugador.GetComponent<Inventory>().objetos[slotParent.transform.GetSiblingIndex()];
                            jugador.GetComponent<Inventory>().objetos[slotParent.transform.GetSiblingIndex()] = null;
                        }
                        else
                        {
                            switch (itemPrefab.GetComponent<BodyPart>().bodypartType)
                            {
                                case BodyPart.BodypartType.HeadPart:
                                    aplastadoAux = jugador.GetComponent<Inventory>().headPart[slotParent.transform.GetSiblingIndex()];
                                    jugador.GetComponent<Inventory>().headPart[slotParent.transform.GetSiblingIndex()] = null;
                                    break;

                                case BodyPart.BodypartType.ArmPart:
                                    aplastadoAux = jugador.GetComponent<Inventory>().armPart[slotParent.transform.GetSiblingIndex()];
                                    jugador.GetComponent<Inventory>().armPart[slotParent.transform.GetSiblingIndex()] = null;
                                    break;

                                case BodyPart.BodypartType.TorsoPart:
                                    aplastadoAux = jugador.GetComponent<Inventory>().torsoPart[slotParent.transform.GetSiblingIndex()];
                                    jugador.GetComponent<Inventory>().torsoPart[slotParent.transform.GetSiblingIndex()] = null;
                                    break;

                                case BodyPart.BodypartType.LegPart:
                                    aplastadoAux = jugador.GetComponent<Inventory>().legPart[slotParent.transform.GetSiblingIndex()];
                                    jugador.GetComponent<Inventory>().legPart[slotParent.transform.GetSiblingIndex()] = null;
                                    break;

                                default:

                                    break;
                            }
                        }

                        //Movimiento visual ingame
                        transform.SetParent(closestCollider.transform);
                        slotParent.GetComponent<BoxCollider2D>().enabled = true; //activar collider de slot del que me fui
                        slotParent = closestCollider.gameObject;
                        slotParent.GetComponent<BoxCollider2D>().enabled = false; //desactivar collider de slot al que me voy
                        transform.localPosition = Vector3.zero;
                        GetComponent<BoxCollider2D>().size = transform.parent.GetComponent<RectTransform>().sizeDelta; //ajustar collider del sprite al tamaño del slot
                        GetComponent<RectTransform>().sizeDelta = new Vector2(transform.parent.GetComponent<RectTransform>().rect.width, transform.parent.GetComponent<RectTransform>().rect.height); //ajustar sprite al slot
                    }
                    else if (itemPrefab.GetComponent<Weapon>() != null && ((closestCollider.gameObject.GetComponent<Slot>().slotype == Slot.SlotType.Weapon && itemPrefab.GetComponent<Weapon>().weaponType == closestCollider.gameObject.GetComponent<Slot>().weaponType) || closestCollider.gameObject.GetComponent<Slot>().slotype == Slot.SlotType.Anything))
                    {
                        if (slotParent.GetComponent<Slot>().slotype == Slot.SlotType.Anything)
                        {
                            //COPIAR CONTENIDO Y BORRAR EN EL LUGAR
                            aplastadoAux = jugador.GetComponent<Inventory>().objetos[slotParent.transform.GetSiblingIndex()];
                            jugador.GetComponent<Inventory>().objetos[slotParent.transform.GetSiblingIndex()] = null;
                        }
                        else
                        {
                            switch (itemPrefab.GetComponent<Weapon>().weaponType)
                            {
                                case Weapon.WeaponType.Primary:
                                    aplastadoAux = jugador.GetComponent<Inventory>().primaryWeapon;
                                    jugador.GetComponent<Inventory>().primaryWeapon = null;
                                    break;

                                case Weapon.WeaponType.Secondary:
                                    aplastadoAux = jugador.GetComponent<Inventory>().secondaryWeapon;
                                    jugador.GetComponent<Inventory>().secondaryWeapon = null;
                                    break;

                                default:

                                    break;
                            }
                        }

                        //Movimiento visual ingame
                        transform.SetParent(closestCollider.transform);
                        slotParent.GetComponent<BoxCollider2D>().enabled = true; //activar collider de slot del que me fui
                        slotParent = closestCollider.gameObject;
                        slotParent.GetComponent<BoxCollider2D>().enabled = false; //desactivar collider de slot al que me voy
                        transform.localPosition = Vector3.zero;
                        GetComponent<BoxCollider2D>().size = transform.parent.GetComponent<RectTransform>().sizeDelta; //ajustar collider del sprite al tamaño del slot
                        GetComponent<RectTransform>().sizeDelta = new Vector2(transform.parent.GetComponent<RectTransform>().rect.width, transform.parent.GetComponent<RectTransform>().rect.height); //ajustar sprite al slot
                    }
                    else if (itemPrefab.GetComponent<Consumible>() != null && (closestCollider.gameObject.GetComponent<Slot>().slotype == Slot.SlotType.Consumible || closestCollider.gameObject.GetComponent<Slot>().slotype == Slot.SlotType.Anything))
                    {
                        if (slotParent.GetComponent<Slot>().slotype == Slot.SlotType.Anything)
                        {
                            //COPIAR CONTENIDO Y BORRAR EN EL LUGAR
                            aplastadoAux = jugador.GetComponent<Inventory>().objetos[slotParent.transform.GetSiblingIndex()];
                            jugador.GetComponent<Inventory>().objetos[slotParent.transform.GetSiblingIndex()] = null;
                        }
                        else
                        {
                            switch (slotParent.transform.name)
                            {
                                case "RightConsumible":
                                    aplastadoAux = jugador.GetComponent<Inventory>().consumibles[0];
                                    jugador.GetComponent<Inventory>().consumibles[0] = null;
                                    break;
                                case "LeftConsumible":
                                    aplastadoAux = jugador.GetComponent<Inventory>().consumibles[2];
                                    jugador.GetComponent<Inventory>().consumibles[2] = null;
                                    break;
                                case "SelectedConsumible":
                                    aplastadoAux = jugador.GetComponent<Inventory>().consumibles[1];
                                    jugador.GetComponent<Inventory>().consumibles[1] = null;
                                    break;
                                default:
                                    break;
                            }
                        }

                        //Movimiento visual ingame
                        transform.SetParent(closestCollider.transform);
                        slotParent.GetComponent<BoxCollider2D>().enabled = true; //activar collider de slot del que me fui
                        slotParent = closestCollider.gameObject;
                        slotParent.GetComponent<BoxCollider2D>().enabled = false; //desactivar collider de slot al que me voy
                        transform.localPosition = Vector3.zero;
                        GetComponent<BoxCollider2D>().size = transform.parent.GetComponent<RectTransform>().sizeDelta; //ajustar collider del sprite al tamaño del slot
                        GetComponent<RectTransform>().sizeDelta = new Vector2(transform.parent.GetComponent<RectTransform>().rect.width, transform.parent.GetComponent<RectTransform>().rect.height); //ajustar sprite al slot
                    }
                    else if (itemPrefab.GetComponent<Reliquia>() != null && (closestCollider.gameObject.GetComponent<Slot>().slotype == Slot.SlotType.Reliquia || closestCollider.gameObject.GetComponent<Slot>().slotype == Slot.SlotType.Anything))
                    {
                        if (slotParent.GetComponent<Slot>().slotype == Slot.SlotType.Anything)
                        {
                            //COPIAR CONTENIDO Y BORRAR EN EL LUGAR
                            aplastadoAux = jugador.GetComponent<Inventory>().objetos[slotParent.transform.GetSiblingIndex()];
                            jugador.GetComponent<Inventory>().objetos[slotParent.transform.GetSiblingIndex()] = null;
                        }
                        else
                        {
                            aplastadoAux = jugador.GetComponent<Inventory>().reliquias[slotParent.transform.GetSiblingIndex()];
                            jugador.GetComponent<Inventory>().reliquias[slotParent.transform.GetSiblingIndex()] = null;
                        }

                        //Movimiento visual ingame
                        transform.SetParent(closestCollider.transform);
                        slotParent.GetComponent<BoxCollider2D>().enabled = true; //activar collider de slot del que me fui
                        slotParent = closestCollider.gameObject;
                        slotParent.GetComponent<BoxCollider2D>().enabled = false; //desactivar collider de slot al que me voy
                        transform.localPosition = Vector3.zero;
                        GetComponent<BoxCollider2D>().size = transform.parent.GetComponent<RectTransform>().sizeDelta; //ajustar collider del sprite al tamaño del slot
                        GetComponent<RectTransform>().sizeDelta = new Vector2(transform.parent.GetComponent<RectTransform>().rect.width, transform.parent.GetComponent<RectTransform>().rect.height); //ajustar sprite al slot
                    }

                    //SEGUIR ASI Y LUEGO LLENAR EL SLOT CON EL CONTENIDO DE APLASTADO AUX
                    if (aplastadoAux != null)
                    {
                        if (closestCollider.gameObject.GetComponent<Slot>().slotype == Slot.SlotType.Anything)
                        {
                            jugador.GetComponent<Inventory>().objetos[closestCollider.transform.GetSiblingIndex()] = aplastadoAux;
                        }
                        else if (closestCollider.gameObject.GetComponent<Slot>().slotype == Slot.SlotType.Armor)
                        {
                            switch (closestCollider.gameObject.GetComponent<Slot>().slotarmorType)
                            {
                                case Armor.ArmorpartType.HeadPart:
                                    jugador.GetComponent<Inventory>().headProtection = aplastadoAux as Armor;
                                    break;

                                case Armor.ArmorpartType.TorsoPart:
                                    jugador.GetComponent<Inventory>().torsoProtection = aplastadoAux as Armor;
                                    break;

                                case Armor.ArmorpartType.LegPart:
                                    jugador.GetComponent<Inventory>().legProtection = aplastadoAux as Armor;
                                    break;

                                case Armor.ArmorpartType.FeetPart:
                                    jugador.GetComponent<Inventory>().feetProtection = aplastadoAux as Armor;
                                    break;

                                default:

                                    break;
                            }
                        }
                        else if (closestCollider.gameObject.GetComponent<Slot>().slotype == Slot.SlotType.BodyPart)
                        {
                            switch (closestCollider.gameObject.GetComponent<Slot>().slotbodyType)
                            {
                                case BodyPart.BodypartType.HeadPart:
                                    jugador.GetComponent<Inventory>().headPart[closestCollider.transform.GetSiblingIndex()] = aplastadoAux as BodyPart;
                                    break;

                                case BodyPart.BodypartType.ArmPart:
                                    jugador.GetComponent<Inventory>().armPart[closestCollider.transform.GetSiblingIndex()] = aplastadoAux as BodyPart;
                                    break;

                                case BodyPart.BodypartType.TorsoPart:
                                    jugador.GetComponent<Inventory>().torsoPart[closestCollider.transform.GetSiblingIndex()] = aplastadoAux as BodyPart;
                                    break;

                                case BodyPart.BodypartType.LegPart:
                                    jugador.GetComponent<Inventory>().legPart[closestCollider.transform.GetSiblingIndex()] = aplastadoAux as BodyPart;
                                    break;

                                default:

                                    break;
                            }
                        }
                        else if (closestCollider.gameObject.GetComponent<Slot>().slotype == Slot.SlotType.Weapon)
                        {
                            switch (closestCollider.GetComponent<Slot>().weaponType)
                            {
                                case Weapon.WeaponType.Primary:
                                    jugador.GetComponent<Inventory>().primaryWeapon = aplastadoAux as Weapon;
                                    break;

                                case Weapon.WeaponType.Secondary:
                                    jugador.GetComponent<Inventory>().secondaryWeapon = aplastadoAux as Weapon;
                                    break;

                                default:

                                    break;
                            }
                        }
                        else if (closestCollider.gameObject.GetComponent<Slot>().slotype == Slot.SlotType.Reliquia)
                        {
                            jugador.GetComponent<Inventory>().reliquias[closestCollider.transform.GetSiblingIndex()] = aplastadoAux as Reliquia;
                        }
                        else if (closestCollider.gameObject.GetComponent<Slot>().slotype == Slot.SlotType.Consumible)
                        {
                            switch (closestCollider.transform.name)
                            {
                                case "RightConsumible":
                                    jugador.GetComponent<Inventory>().consumibles[0] = aplastadoAux as Consumible;
                                    break;
                                case "LeftConsumible":
                                    jugador.GetComponent<Inventory>().consumibles[2] = aplastadoAux as Consumible;
                                    break;
                                case "SelectedConsumible":
                                    jugador.GetComponent<Inventory>().consumibles[1] = aplastadoAux as Consumible;
                                    break;
                                default:
                                    break;
                            }
                        }

                    }
                    else
                    {
                        Debug.Log("NO SE PUEDE GUARDAR EN ESE SLOT");
                        transform.SetParent(slotParent.transform);
                        transform.localPosition = Vector3.zero;
                    }
                }
                else if (closestCollider.gameObject.name == "Dropzone")
                {
                    Debug.Log("SOLTADO EN DROPZONE");
                    //borrarlo logicamente del inventario del jugador
                    if (slotParent.GetComponent<Slot>().slotype == Slot.SlotType.Anything)
                    {
                        jugador.GetComponent<Inventory>().objetos[slotParent.transform.GetSiblingIndex()] = null;
                    }
                    else if (slotParent.GetComponent<Slot>().slotype == Slot.SlotType.Armor)
                    {
                        switch (slotParent.GetComponent<Slot>().slotarmorType)
                        {
                            case Armor.ArmorpartType.HeadPart:
                                jugador.GetComponent<Inventory>().headProtection = null;
                                break;
                            case Armor.ArmorpartType.TorsoPart:
                                jugador.GetComponent<Inventory>().torsoProtection = null;
                                break;
                            case Armor.ArmorpartType.LegPart:
                                jugador.GetComponent<Inventory>().legProtection = null;
                                break;
                            case Armor.ArmorpartType.FeetPart:
                                jugador.GetComponent<Inventory>().feetProtection = null;
                                break;
                            default:
                                break;
                        }
                    }
                    else if (slotParent.GetComponent<Slot>().slotype == Slot.SlotType.Weapon)
                    {
                        switch (slotParent.GetComponent<Slot>().weaponType)
                        {
                            case Weapon.WeaponType.Primary:
                                jugador.GetComponent<Inventory>().primaryWeapon = null;
                                break;
                            case Weapon.WeaponType.Secondary:
                                jugador.GetComponent<Inventory>().secondaryWeapon = null;
                                break;
                            default:
                                break;
                        }
                    }
                    else if (slotParent.GetComponent<Slot>().slotype == Slot.SlotType.BodyPart)
                    {
                        switch (slotParent.GetComponent<Slot>().slotbodyType)
                        {
                            case BodyPart.BodypartType.HeadPart:
                                jugador.GetComponent<Inventory>().headPart[slotParent.transform.GetSiblingIndex()] = null;
                                break;

                            case BodyPart.BodypartType.ArmPart:
                                jugador.GetComponent<Inventory>().armPart[slotParent.transform.GetSiblingIndex()] = null;
                                break;

                            case BodyPart.BodypartType.TorsoPart:
                                jugador.GetComponent<Inventory>().torsoPart[slotParent.transform.GetSiblingIndex()] = null;
                                break;

                            case BodyPart.BodypartType.LegPart:
                                jugador.GetComponent<Inventory>().legPart[slotParent.transform.GetSiblingIndex()] = null;
                                break;

                            default:

                                break;
                        }
                    }
                    else if (slotParent.GetComponent<Slot>().slotype == Slot.SlotType.Reliquia)
                    {
                        jugador.GetComponent<Inventory>().reliquias[slotParent.transform.GetSiblingIndex()] = null;
                    }
                    else if (slotParent.GetComponent<Slot>().slotype == Slot.SlotType.Consumible)
                    {
                        switch (slotParent.transform.name)
                        {
                            case "RightConsumible":
                                jugador.GetComponent<Inventory>().consumibles[0] = null;
                                break;
                            case "LeftConsumible":
                                jugador.GetComponent<Inventory>().consumibles[2] = null;
                                break;
                            case "SelectedConsumible":
                                jugador.GetComponent<Inventory>().consumibles[1] = null;
                                break;
                            default:
                                break;
                        }
                    }
                    //poner el prefab pickable en el piso
                    itemPrefab.transform.SetParent(jugador.transform);
                    itemPrefab.transform.localPosition = Vector3.zero;
                    itemPrefab.transform.parent = null;
                    itemPrefab.SetActive(true);
                    //volver a activar collider del slot del que saque el item
                    slotParent.GetComponent<BoxCollider2D>().enabled = true;
                    //quitar visualmente el objeto del inventario
                    Destroy(gameObject);

                }
                else
                {
                    Debug.Log("SOLTADO EN OTRO LUHAR");
                    transform.SetParent(slotParent.transform);
                    transform.localPosition = Vector3.zero;
                }
            }
            else
            {
                Debug.Log("COLLIDER NULL");
                transform.SetParent(slotParent.transform);
                transform.localPosition = Vector3.zero;
            }
        }

        inventarioVisual.GetComponent<InventoryManager>().currentDraggedItem = null;
    }

    public void IntercambioPosicionObjetos(GameObject tirado, GameObject aplastado)
    {
        Debug.Log("INTERCAMBIO");
        tirado.transform.SetParent(aplastado.GetComponent<DraggableObject>().slotParent.transform);
        tirado.transform.localPosition = Vector3.zero;
        aplastado.transform.SetParent(tirado.GetComponent<DraggableObject>().slotParent.transform);
        tirado.GetComponent<DraggableObject>().slotParent = tirado.transform.parent.gameObject;
        aplastado.GetComponent<DraggableObject>().slotParent = aplastado.transform.parent.gameObject;
        aplastado.transform.localPosition = Vector3.zero;

        //ajustar colliders y sprites
        tirado.GetComponent<BoxCollider2D>().size = tirado.transform.parent.GetComponent<RectTransform>().sizeDelta;
        aplastado.GetComponent<BoxCollider2D>().size = aplastado.transform.parent.GetComponent<RectTransform>().sizeDelta;
        tirado.GetComponent<RectTransform>().sizeDelta = new Vector2(tirado.transform.parent.GetComponent<RectTransform>().rect.width, tirado.transform.parent.GetComponent<RectTransform>().rect.height);
        aplastado.GetComponent<RectTransform>().sizeDelta = new Vector2(aplastado.transform.parent.GetComponent<RectTransform>().rect.width, aplastado.transform.parent.GetComponent<RectTransform>().rect.height);
    }

    public Type TipoDeItem(GameObject item)
    {
        Component[] components = item.GetComponents(typeof(MonoBehaviour));

        foreach (Component component in components)
        {
            Type componentType = component.GetType();

            if (componentType == typeof(Weapon) ||
                componentType == typeof(Armor) ||
                componentType == typeof(Reliquia) ||
                componentType == typeof(Consumible) ||
                componentType == typeof(BodyPart))
            {
                return componentType;
            }
        }

        return null;
    }

    public void IntercambioLogicoObjetos(GameObject jugador, GameObject tirado, GameObject aplastado, ref Item aplastadoAux)
    {
        switch (aplastado.transform.parent.GetComponent<Slot>().slotype)
        {
            case Slot.SlotType.Anything:
                aplastadoAux = jugador.GetComponent<Inventory>().objetos[aplastado.transform.parent.GetSiblingIndex()];
                jugador.GetComponent<Inventory>().objetos[aplastado.transform.parent.GetSiblingIndex()] = TiradoToAplastado(aplastadoAux, jugador, tirado, aplastado);
                break;
            case Slot.SlotType.Weapon:
                switch (aplastado.transform.parent.GetComponent<Slot>().weaponType)
                {
                    case Weapon.WeaponType.Primary:
                        aplastadoAux = jugador.GetComponent<Inventory>().primaryWeapon;
                        jugador.GetComponent<Inventory>().primaryWeapon = TiradoToAplastado(aplastadoAux, jugador, tirado, aplastado) as Weapon;
                        break;
                    case Weapon.WeaponType.Secondary:
                        aplastadoAux = jugador.GetComponent<Inventory>().secondaryWeapon;
                        jugador.GetComponent<Inventory>().secondaryWeapon = TiradoToAplastado(aplastadoAux, jugador, tirado, aplastado) as Weapon;
                        break;
                    default:

                        break;
                }
                break;
            case Slot.SlotType.Armor:
                switch (aplastado.transform.parent.GetComponent<Slot>().slotarmorType)
                {
                    case Armor.ArmorpartType.HeadPart:
                        aplastadoAux = jugador.GetComponent<Inventory>().headProtection;
                        jugador.GetComponent<Inventory>().headProtection = TiradoToAplastado(aplastadoAux, jugador, tirado, aplastado) as Armor;
                        break;
                    case Armor.ArmorpartType.TorsoPart:
                        aplastadoAux = jugador.GetComponent<Inventory>().torsoProtection;
                        jugador.GetComponent<Inventory>().torsoProtection = TiradoToAplastado(aplastadoAux, jugador, tirado, aplastado) as Armor;
                        break;
                    case Armor.ArmorpartType.LegPart:
                        aplastadoAux = jugador.GetComponent<Inventory>().legProtection;
                        jugador.GetComponent<Inventory>().legProtection = TiradoToAplastado(aplastadoAux, jugador, tirado, aplastado) as Armor;
                        break;
                    case Armor.ArmorpartType.FeetPart:
                        aplastadoAux = jugador.GetComponent<Inventory>().feetProtection;
                        jugador.GetComponent<Inventory>().feetProtection = TiradoToAplastado(aplastadoAux, jugador, tirado, aplastado) as Armor;
                        break;
                    default:

                        break;
                }
                break;
            case Slot.SlotType.BodyPart:
                switch (aplastado.transform.parent.GetComponent<Slot>().slotbodyType)
                {
                    case BodyPart.BodypartType.HeadPart:
                        aplastadoAux = jugador.GetComponent<Inventory>().headPart[aplastado.transform.parent.transform.GetSiblingIndex()];
                        jugador.GetComponent<Inventory>().headPart[aplastado.transform.parent.transform.GetSiblingIndex()] = TiradoToAplastado(aplastadoAux, jugador, tirado, aplastado) as BodyPart;
                        break;
                    case BodyPart.BodypartType.ArmPart:
                        aplastadoAux = jugador.GetComponent<Inventory>().armPart[aplastado.transform.parent.transform.GetSiblingIndex()];
                        jugador.GetComponent<Inventory>().armPart[aplastado.transform.parent.transform.GetSiblingIndex()] = TiradoToAplastado(aplastadoAux, jugador, tirado, aplastado) as BodyPart;
                        break;
                    case BodyPart.BodypartType.TorsoPart:
                        aplastadoAux = jugador.GetComponent<Inventory>().torsoPart[aplastado.transform.parent.transform.GetSiblingIndex()];
                        jugador.GetComponent<Inventory>().torsoPart[aplastado.transform.parent.transform.GetSiblingIndex()] = TiradoToAplastado(aplastadoAux, jugador, tirado, aplastado) as BodyPart;
                        break;
                    case BodyPart.BodypartType.LegPart:
                        aplastadoAux = jugador.GetComponent<Inventory>().legPart[aplastado.transform.parent.transform.GetSiblingIndex()];
                        jugador.GetComponent<Inventory>().legPart[aplastado.transform.parent.transform.GetSiblingIndex()] = TiradoToAplastado(aplastadoAux, jugador, tirado, aplastado) as BodyPart;
                        break;
                    default:

                        break;
                }
                break;
            case Slot.SlotType.Reliquia:
                aplastadoAux = jugador.GetComponent<Inventory>().reliquias[aplastado.transform.parent.GetSiblingIndex()];
                jugador.GetComponent<Inventory>().reliquias[aplastado.transform.parent.GetSiblingIndex()] = TiradoToAplastado(aplastadoAux, jugador, tirado, aplastado) as Reliquia;
                break;
            case Slot.SlotType.Consumible:
                switch (aplastado.transform.parent.name)
                {
                    case "RightConsumible":
                        aplastadoAux = jugador.GetComponent<Inventory>().consumibles[0];
                        jugador.GetComponent<Inventory>().consumibles[0] = TiradoToAplastado(aplastadoAux, jugador, tirado, aplastado) as Consumible;
                        break;
                    case "LeftConsumible":
                        aplastadoAux = jugador.GetComponent<Inventory>().consumibles[2];
                        jugador.GetComponent<Inventory>().consumibles[2] = TiradoToAplastado(aplastadoAux, jugador, tirado, aplastado) as Consumible;
                        break;
                    case "SelectedConsumible":
                        aplastadoAux = jugador.GetComponent<Inventory>().consumibles[1];
                        jugador.GetComponent<Inventory>().consumibles[1] = TiradoToAplastado(aplastadoAux, jugador, tirado, aplastado) as Consumible;
                        break;
                    default:
                        break;
                }
                break;
            default:

                break;
        }

    }

    public Item TiradoToAplastado(Item aplastadoAux, GameObject jugador, GameObject tirado, GameObject aplastado)
    {
        Item copiaTirado = null;
        switch (tirado.GetComponent<DraggableObject>().slotParent.GetComponent<Slot>().slotype)
        {
            case Slot.SlotType.Anything:
                copiaTirado = jugador.GetComponent<Inventory>().objetos[tirado.GetComponent<DraggableObject>().slotParent.transform.GetSiblingIndex()];
                jugador.GetComponent<Inventory>().objetos[tirado.GetComponent<DraggableObject>().slotParent.transform.GetSiblingIndex()] = aplastadoAux;
                return copiaTirado;
                break;
            case Slot.SlotType.Weapon:
                switch (tirado.GetComponent<DraggableObject>().slotParent.GetComponent<Slot>().weaponType)
                {
                    case Weapon.WeaponType.Primary:
                        copiaTirado = jugador.GetComponent<Inventory>().primaryWeapon;
                        jugador.GetComponent<Inventory>().primaryWeapon = aplastadoAux as Weapon;
                        return copiaTirado;
                        break;
                    case Weapon.WeaponType.Secondary:
                        copiaTirado = jugador.GetComponent<Inventory>().secondaryWeapon;
                        jugador.GetComponent<Inventory>().secondaryWeapon = aplastadoAux as Weapon;
                        return copiaTirado;
                        break;
                    default:
                        return null;
                        break;
                }
                break;
            case Slot.SlotType.Armor:
                switch (tirado.GetComponent<DraggableObject>().slotParent.GetComponent<Slot>().slotarmorType)
                {
                    case Armor.ArmorpartType.HeadPart:
                        copiaTirado = jugador.GetComponent<Inventory>().headProtection;
                        jugador.GetComponent<Inventory>().headProtection = aplastadoAux as Armor;
                        return copiaTirado;
                        break;
                    case Armor.ArmorpartType.TorsoPart:
                        copiaTirado = jugador.GetComponent<Inventory>().torsoProtection;
                        jugador.GetComponent<Inventory>().torsoProtection = aplastadoAux as Armor;
                        return copiaTirado;
                        break;
                    case Armor.ArmorpartType.LegPart:
                        copiaTirado = jugador.GetComponent<Inventory>().legProtection;
                        jugador.GetComponent<Inventory>().legProtection = aplastadoAux as Armor;
                        return copiaTirado;
                        break;
                    case Armor.ArmorpartType.FeetPart:
                        copiaTirado = jugador.GetComponent<Inventory>().feetProtection;
                        jugador.GetComponent<Inventory>().feetProtection = aplastadoAux as Armor;
                        return copiaTirado;
                        break;
                    default:
                        return null;
                        break;
                }
                break;
            case Slot.SlotType.BodyPart:
                switch (tirado.GetComponent<DraggableObject>().slotParent.GetComponent<Slot>().slotbodyType)
                {
                    case BodyPart.BodypartType.HeadPart:
                        copiaTirado = jugador.GetComponent<Inventory>().headPart[tirado.GetComponent<DraggableObject>().slotParent.transform.GetSiblingIndex()];
                        jugador.GetComponent<Inventory>().headPart[tirado.GetComponent<DraggableObject>().slotParent.transform.GetSiblingIndex()] = aplastadoAux as BodyPart;
                        return copiaTirado;
                        break;
                    case BodyPart.BodypartType.ArmPart:
                        copiaTirado = jugador.GetComponent<Inventory>().armPart[tirado.GetComponent<DraggableObject>().slotParent.transform.GetSiblingIndex()];
                        jugador.GetComponent<Inventory>().armPart[tirado.GetComponent<DraggableObject>().slotParent.transform.GetSiblingIndex()] = aplastadoAux as BodyPart;
                        return copiaTirado;
                        break;
                    case BodyPart.BodypartType.TorsoPart:
                        copiaTirado = jugador.GetComponent<Inventory>().torsoPart[tirado.GetComponent<DraggableObject>().slotParent.transform.GetSiblingIndex()];
                        jugador.GetComponent<Inventory>().torsoPart[tirado.GetComponent<DraggableObject>().slotParent.transform.GetSiblingIndex()] = aplastadoAux as BodyPart;
                        return copiaTirado;
                        break;
                    case BodyPart.BodypartType.LegPart:
                        copiaTirado = jugador.GetComponent<Inventory>().legPart[tirado.GetComponent<DraggableObject>().slotParent.transform.GetSiblingIndex()];
                        jugador.GetComponent<Inventory>().legPart[tirado.GetComponent<DraggableObject>().slotParent.transform.GetSiblingIndex()] = aplastadoAux as BodyPart;
                        return copiaTirado;
                        break;
                    default:
                        return null;
                        break;
                }
                break;
            case Slot.SlotType.Reliquia:
                copiaTirado = jugador.GetComponent<Inventory>().reliquias[tirado.GetComponent<DraggableObject>().slotParent.transform.GetSiblingIndex()];
                jugador.GetComponent<Inventory>().reliquias[tirado.GetComponent<DraggableObject>().slotParent.transform.GetSiblingIndex()] = aplastadoAux as Reliquia;
                return copiaTirado;
                break;
            case Slot.SlotType.Consumible:
                switch (tirado.GetComponent<DraggableObject>().slotParent.name)
                {
                    case "RightConsumible":
                        copiaTirado = jugador.GetComponent<Inventory>().consumibles[0];
                        jugador.GetComponent<Inventory>().consumibles[0] = aplastadoAux as Consumible;
                        return copiaTirado;
                        break;
                    case "LeftConsumible":
                        copiaTirado = jugador.GetComponent<Inventory>().consumibles[2];
                        jugador.GetComponent<Inventory>().consumibles[2] = aplastadoAux as Consumible;
                        return copiaTirado;
                        break;
                    case "SelectedConsumible":
                        copiaTirado = jugador.GetComponent<Inventory>().consumibles[1];
                        jugador.GetComponent<Inventory>().consumibles[1] = aplastadoAux as Consumible;
                        return copiaTirado;
                        break;
                    default:
                        return null;
                        break;
                }
                break;
            default:
                return null;
                break;
        }
    }
}
