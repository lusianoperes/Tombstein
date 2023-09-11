using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tepear : MonoBehaviour
{
    public GameObject plano;
    public GameObject piso;
    public GameManage gameManage;

    //toda la parte de abajo debe ser adaptada para los layouts
    private void OnCollisionEnter(Collision collision)
    {
        
        if (collision.collider.CompareTag("Player"))
        {
            gameManage.TeleportarJugador(plano,piso);
        }
    }
}
