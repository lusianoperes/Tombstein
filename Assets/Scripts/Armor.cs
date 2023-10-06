using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Armor : Item
{
    public enum ArmorpartType 
    {   
        HeadPart,
        TorsoPart,
        LegPart,
        FeetPart,
    }
    public ArmorpartType armorpartType;
    public float armorProtection;
    public int extraHp;
    public float fuerzaExtra;
    public float destrezaExtra;
    public float estaminaExtra;
    public float velocidadExtra;
    public int agilidadExtra;

}
