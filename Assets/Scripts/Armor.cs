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
    public int armorProtection;
    
}
