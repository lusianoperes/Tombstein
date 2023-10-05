using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    
    public string itemName;
    public string itemDescription;
    public Sprite imagenAsociada;
    
    public GameObject fullPlayerReference;
    public PlayerController playerControllerReference;

    void Awake()
    {
        fullPlayerReference = GameObject.Find("Jugador");
        playerControllerReference = fullPlayerReference.GetComponent<PlayerController>();
    }
}
