using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryController : MonoBehaviour
{   
    public List<Item> slots;
    Text text;
    
    private int run_slots_max = 25;

    void Start()
    {
        slots = new List<Item>();
    }

    public List<Item> getSlots()
    {
        return this.slots;
    }
    /*
    public void SetSlot(GameObject slot, int cant)
    {
        bool exist = false;
        for(int i = 0; i < slots.Count; i++)
        {
            if(slots[i] != null)
            {
                if (slots[i].tag == slot.tag)
                {
                    int already_cant = slots[i].GetComponent<Atribut>().getCantidad(); // Sin argumentos
                    slots[i].GetComponent<Atribut>().setCantidad(already_cant + cant);
                    exist = true;
                }
            }
        }

    }
    */

    public void showInventory()
    {
        Component[] inventory = GameObject.FindGameObjectWithTag("Inventario").GetComponentsInChildren<Transform>();

        //if (removeItems(inventory))
        {
            foreach (Item slot in slots)
            {
                if (slot != null)
                {
                    bool slotUsed = false;

                    foreach (Transform child in inventory)
                    {
                        if (child.tag == "Slot" && child.transform.childCount <= 1 && !slotUsed)
                        {
                            Item item = Instantiate(slot, child.transform.position, Quaternion.identity);
                            item.transform.SetParent(child, false);
                            item.transform.localPosition = Vector3.zero;
                            item.name = item.name.Replace("Clone", ""); // Fix the name.
                            text = item.GetComponentInChildren<Text>();
                            int cant = 1;
                            text.text = cant.ToString();

                            slotUsed = true;
                        }
                    }
                }
            }
        }
    }
    /*
    public bool removeItems(Component[] inventario)
    {
        for (int i = 1; i < inventario.Length; i++)
        {
            GameObject child = inventario[i].gameObject;
            if (child.tag == "Slot" && child.transform.childCount > 0)
            {
                for (int a = 0; a < child.transform.childCount; a++)
                {
                    Destroy(child.transform.GetChild(a).gameObject);
                }
            }
        }
        

        return true;
    }
    */
}

