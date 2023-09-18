using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventario : MonoBehaviour
{
    private bool InventarioAbierto = false;

    // Cambié el nombre de la variable para evitar la colisión de nombres
    private GameObject inventarioObject;

    void Awake()
    {
        inventarioObject = GameObject.FindGameObjectWithTag("inventario-com");
        // Asegúrate de que inventarioObject no sea nulo antes de usarlo
        if (inventarioObject != null)
        {
            inventarioObject.SetActive(false);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {   
            Debug.Log("nig");
            if (!InventarioAbierto)
            {
                InventarioAbierto = true;
                // Asegúrate de que inventarioObject no sea nulo antes de usarlo
                if (inventarioObject != null)
                {
                    inventarioObject.SetActive(InventarioAbierto);
                }

                // Cambié GetComponenet a GetComponent
                GameObject.FindGameObjectWithTag("GenerarEventosInventario").GetComponent<InventoryController>().showInventory();
            }
            else
            {
                // Asegúrate de que inventarioObject no sea nulo antes de usarlo
                if (inventarioObject != null)
                {
                    inventarioObject.SetActive(false);
                }
                InventarioAbierto = false;
            }
        }
    }
}