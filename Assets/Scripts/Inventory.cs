using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    
    public GameObject inventario;

    public Weapon primaryWeapon;
    public Weapon secondaryWeapon;

    public Consumible[] consumibles; 

    public Reliquia[] reliquias; 

    public BodyPart[] headPart;
    public BodyPart[] armPart;
    public BodyPart[] torsoPart;
    public BodyPart[] legPart;
    public Armor headProtection;
    public Armor torsoProtection;
    public Armor legProtection;
    public Armor feetProtection;

    public Item[] objetos; 
     
    void Start()
    {
        consumibles = new Consumible[3];
        reliquias = new Reliquia[5];
        objetos = new Item[25];
        headPart = new BodyPart[3];
        armPart = new BodyPart[3];
        torsoPart = new BodyPart[3];
        legPart = new BodyPart[3];
        
    }
   
}
