using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyPart : Item
{
    public float useCooldown;
    public enum BodypartType 
    {   
        HeadPart,
        ArmPart,
        TorsoPart,
        LegPart
    }
    public BodypartType bodypartType;
    
    public int armorProtection;
    public int extraHp;
    public float fuerzaExtra;
    public float destrezaExtra;
    public float estaminaExtra;
    public float velocidadExtra;
    public int agilidadExtra;

    public IEnumerator UsarParte()
    {
        
        Debug.Log("Has usado la parte del cuerpo");
        yield return null;

    }
}