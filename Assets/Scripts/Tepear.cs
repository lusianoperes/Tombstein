using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tepear : MonoBehaviour
{
    public GameObject plano;
    public GameObject Sala;
    public GameObject MiniMapPlano;
    public GameManage gameManage;

    private void OnCollisionEnter(Collision collision)
    {
        
        if (collision.collider.CompareTag("Player"))
        {
            gameManage.TeleportarJugador(plano,Sala,MiniMapPlano);
        }
    }
}
