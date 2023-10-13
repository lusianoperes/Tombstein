using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
    public GameObject bodypartsHud;
    public GameObject jugadorObj;
    public GameObject headAbility;
    public GameObject torsoAbility;
    public GameObject armAbility;
    public GameObject legAbility;
    public Jugador jugador;
    public enum SlotType 
    {   
        Anything,
        Armor,
        Weapon,
        Consumible,
        BodyPart,
        Reliquia
    }
    public SlotType slotype;
    public Armor.ArmorpartType slotarmorType;
    public BodyPart.BodypartType slotbodyType;
    public Weapon.WeaponType weaponType;

    public void Start()
    {
        jugadorObj = GameObject.Find("Jugador");
        jugador = jugadorObj.GetComponent<Jugador>();
        bodypartsHud = jugadorObj.GetComponent<PlayerController>().bodypartsHud;
        headAbility = bodypartsHud.transform.GetChild(0).GetChild(0).gameObject;
        torsoAbility = bodypartsHud.transform.GetChild(0).GetChild(1).gameObject;
        legAbility = bodypartsHud.transform.GetChild(0).GetChild(2).gameObject;
        armAbility = bodypartsHud.transform.GetChild(0).GetChild(3).gameObject;
    }
    
    void Update()
    {
       ActualizarBodyPartsHud();
    }

    public void ActualizarBodyPartsHud()
    {
        
        //si se equipa una bodypart actualizar el hudde habilidades y asignarle el cooldown de la misma a la clase Jugador
        if(name == "HeadActiveSlot" && transform.childCount == 1)
        {
            headAbility.GetComponent<Image>().enabled = true;
            headAbility.GetComponent<Image>().sprite = transform.GetChild(0).GetComponent<DraggableObject>().itemPrefab.GetComponent<Item>().imagenAsociada;
            //jugador.headCurrentCooldown = transform.GetChild(0).GetComponent<DraggableObject>().itemPrefab.GetComponent<BodyPart>().useCooldown;
        }
        else if(name == "TorsoActiveSlot" && transform.childCount == 1)
        {
            torsoAbility.GetComponent<Image>().enabled = true;
            torsoAbility.GetComponent<Image>().sprite = transform.GetChild(0).GetComponent<DraggableObject>().itemPrefab.GetComponent<Item>().imagenAsociada;
            //jugador.torsoCurrentCooldown = transform.GetChild(0).GetComponent<DraggableObject>().itemPrefab.GetComponent<BodyPart>().useCooldown;
        }
        else if(name == "ArmActiveSlot" && transform.childCount == 1)
        {
            armAbility.GetComponent<Image>().enabled = true;
            armAbility.GetComponent<Image>().sprite = transform.GetChild(0).GetComponent<DraggableObject>().itemPrefab.GetComponent<Item>().imagenAsociada;
            //jugador.armsCurrentCooldown = transform.GetChild(0).GetComponent<DraggableObject>().itemPrefab.GetComponent<BodyPart>().useCooldown;
        }
        else if(name == "LegActiveSlot" && transform.childCount == 1)
        {
            legAbility.GetComponent<Image>().enabled = true;
            legAbility.GetComponent<Image>().sprite = transform.GetChild(0).GetComponent<DraggableObject>().itemPrefab.GetComponent<Item>().imagenAsociada;
            //jugador.legsCurrentCooldown = transform.GetChild(0).GetComponent<DraggableObject>().itemPrefab.GetComponent<BodyPart>().useCooldown;
        }

        //si se desequipa una bodypart eliminar la imagen
        if(name == "HeadActiveSlot" && transform.childCount == 0)
        {
            headAbility.GetComponent<Image>().enabled = false;
            //jugador.headCurrentCooldown = 0;
        }
        else if(name == "TorsoActiveSlot" && transform.childCount == 0)
        {
            torsoAbility.GetComponent<Image>().enabled = false;
            //jugador.torsoCurrentCooldown = 0;
        }
        else if(name == "ArmActiveSlot" && transform.childCount == 0)
        {
            armAbility.GetComponent<Image>().enabled = false;
            //jugador.armsCurrentCooldown = 0;
        }
        else if(name == "LegActiveSlot" && transform.childCount == 0)
        {
            legAbility.GetComponent<Image>().enabled = false;
            //jugador.legsCurrentCooldown = 0;
        }
    }
}
