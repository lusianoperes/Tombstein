using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryVisualManager : MonoBehaviour
{
    public List<Slot> slots;
    public List<Sprite> lista;

    
    /*
    public void Update(){
        for(int i = 0; i<Mathf.Min(slots.Count, lista.Count); i++){
            if(lista[i]){
                Slot slotToUpdate = slots.Find(slot => slot.id == i);
                if (slotToUpdate != null)
                {
                    // Actualiza el Slot con la nueva Sprite
                    slotToUpdate.UpdateSlot(lista[i]);
                }
                slots[i].UpdateSlot(lista[i]);
            }
        }
    }
    */
    

    // public void UpdateSlot(){

    //     icon = item.imagenAsociada;
    // }

    
}
