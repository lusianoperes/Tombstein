using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class InventoryManager : MonoBehaviour
{
    public GameObject JugadorObj;
    public Inventory PlayerInventory;
    public GameObject slots;
    public bool isInventoryOpen = false;
    public GameObject Armadura;
    public GameObject Fuerza;
    public GameObject Destreza;
    public GameObject Agilidad;
    public GameObject Velocidad;
    public GameObject Estamina;
    public GameObject DisplayArmadura;
    public GameObject DisplayOrganos;
    public GameObject Descriptor;
    public GameObject nombreDescriptor;
    public GameObject descripcionDescriptor;
    public GameObject spriteDescriptor;
    public GameObject currentDraggedItem = null;

    public void Awake()
    {
        PlayerInventory = JugadorObj.GetComponent<Inventory>();
    }


    public void Update()
    {

        actualizar_visuals();

    }

    public void actualizar_visuals()
    {

        Fuerza.GetComponent<TMP_Text>().text = $"Fuerza:\n{JugadorObj.GetComponent<Jugador>().Fuerza}";

        Armadura.GetComponent<TMP_Text>().text = $"Armadura:\n{JugadorObj.GetComponent<Jugador>().Armadura}";

        Destreza.GetComponent<TMP_Text>().text = $"Destreza:\n{JugadorObj.GetComponent<Jugador>().Destreza}";

        Agilidad.GetComponent<TMP_Text>().text = $"Agilidad:\n{JugadorObj.GetComponent<Jugador>().Agilidad}";

        Velocidad.GetComponent<TMP_Text>().text = $"Velocidad:\n{JugadorObj.GetComponent<PlayerController>().Speed * JugadorObj.GetComponent<PlayerController>().SpeedMultiple}";

        Estamina.GetComponent<TMP_Text>().text = $"Estamina:\n{JugadorObj.GetComponent<Jugador>().Estamina}";

    }

    public void AgregarObjetoInventario(int index, GameObject itemPrefab)
    {
        GameObject slot = null;
        GameObject spriteHolder = null;

        slot = slots.transform.GetChild(index).gameObject;
        spriteHolder = Instantiate(Resources.Load<GameObject>("Prefabs/SpriteHolder"), slot.transform);
        spriteHolder.GetComponent<Image>().sprite = PlayerInventory.objetos[index].imagenAsociada;
        spriteHolder.GetComponent<DraggableObject>().slotParent = slot;
        itemPrefab.transform.parent = spriteHolder.transform;
        spriteHolder.GetComponent<DraggableObject>().itemPrefab = itemPrefab;
        spriteHolder.GetComponent<BoxCollider2D>().size = spriteHolder.transform.parent.GetComponent<RectTransform>().sizeDelta;
        spriteHolder.transform.parent.GetComponent<BoxCollider2D>().enabled = false;
        spriteHolder.GetComponent<RectTransform>().sizeDelta = new Vector2(spriteHolder.transform.parent.GetComponent<RectTransform>().rect.width, spriteHolder.transform.parent.GetComponent<RectTransform>().rect.height); //ajustar sprite al slot

    }

    public void AbrirArmaduras()
    {
        DisplayArmadura.SetActive(true);
        DisplayOrganos.SetActive(false);
    }
    public void AbrirÓrganos()
    {
        DisplayArmadura.SetActive(false);
        DisplayOrganos.SetActive(true);
    }
}
