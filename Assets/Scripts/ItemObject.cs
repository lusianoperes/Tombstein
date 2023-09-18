using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemObject : MonoBehaviour
{
    public GameObject obj;
    public int cant =1;

    public InventoryVisualManager inventoryvisman;

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            InventoryController inventoryController = GameObject.FindGameObjectWithTag("GenerarEventosInventario").GetComponent<InventoryController>();
            
            List<Item> inventario = inventoryController.getSlots();
        
           


            Weapon arma = this.GetComponent<Weapon>();
            
            inventoryController.slots.Add(arma); //array listItem

            int posicion = inventoryController.slots.IndexOf(arma);
 
            inventoryvisman.lista.Add(arma.imagenAsociada);
            InventorySlot[] slotsEncontrados = Resources.FindObjectsOfTypeAll<InventorySlot>();
            /*
            Image imagen = slotsEncontrados[15].gameObject.GetComponent<Image>();
            imagen.sprite = arma.imagenAsociada;
            */
            bool guardada = false;
            foreach(InventorySlot slot in slotsEncontrados){
                
                Image imagen = slot.gameObject.GetComponent<Image>();
                if(imagen.sprite == null && !(guardada)){
                    slot.GetComponent<DraggableItem>().objeto = arma;
                    imagen.sprite = arma.imagenAsociada;
                    guardada = true;
                }
            }





            //slot.UpdateSlot()

            /*
            for(int i = 0; i<25;i++)
            {
                if(inventario[i].GetComponent<Slot>.empty)
                {
                    inventario[i].GetComponent<Slot>.icon = gameObject.GetComponent<Atribut>.imagenAsociada;
                }
            }
            */

            


            gameObject.SetActive(false);
        }
    }
}