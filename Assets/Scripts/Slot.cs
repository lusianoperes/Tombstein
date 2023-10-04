using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slot : MonoBehaviour
{
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
    void Start()
    {
        
    }
//.parent.GetSiblingIndex
    // Update is called once per frame
    void Update()
    {
        
    }
}
