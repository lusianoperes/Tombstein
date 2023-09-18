using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public bool InventarioAbierto = false;
    public GameObject inventario;

    public Weapon primaryWeapon;
    public Weapon secondaryWeapon;

    public List<Consumible> consumibles; //En el awake limitar el largo (inicial 3)

    public List<Reliquia> reliquias; //En el awake limitar el largo (inicial 5)

    public BodyPart headPart;
    public BodyPart leftArmPart;
    public BodyPart rightArmPart;
    public BodyPart torsoPart;
    public BodyPart leftLegPart;
    public BodyPart rightLegPart;

    public List<Item> objetos; //En el awake limitar el largo (inicial grid 9x9)
     
   /* void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (!InventarioAbierto)
            {
                InventarioAbierto = true;
                // Asegúrate de que inventarioObject no sea nulo antes de usarlo
                if (objetos != null)
                {
                    inventario.SetActive(InventarioAbierto);
                }

                // Cambié GetComponenet a GetComponent
                GameObject.FindGameObjectWithTag("GenerarEventosInventario").GetComponent<InventoryController>().showInventory();
            }
            else
            {
                // Asegúrate de que inventarioObject no sea nulo antes de usarlo
                if (objetos != null)
                {
                    inventario.SetActive(false);
                }
                InventarioAbierto = false;
            }
        }
    }*/
}
