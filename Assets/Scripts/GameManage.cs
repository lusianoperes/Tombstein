using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManage : MonoBehaviour
{
    public DungeonManager dungeonManager;
    public int erasRecorridas = 0;
    public Room.Etapa etapa = Room.Etapa.Primera;
    public List<GameObject> mapArray;
    public GameObject jugador;
    public GameObject camara;
    public MiniMapGen MiniMapGen;
    void Start()
    {
        InstanciarMapa(dungeonManager, ref mapArray);
        mapArray = dungeonManager.IngresarPuertasScripts(ref mapArray, this);
        MiniMapGen.GenerateMiniMap(mapArray);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void InstanciarMapa(DungeonManager dungeonManager, ref List<GameObject> mapArray)
    {
        mapArray = dungeonManager.GenerarListaRooms(etapa, erasRecorridas);
        
        
        for (int i = 0; i < mapArray.Count; i++)
        {
            mapArray[i] = Instantiate(mapArray[i], new Vector3((mapArray[i].GetComponent<Room>().valorDeCelda - mapArray[i].GetComponent<Room>().valorDeCelda / 10 * 10) * 300, 0, (mapArray[i].GetComponent<Room>().valorDeCelda / 10) * 300), Quaternion.identity);
            if (mapArray[i].GetComponent<Room>().valorDeCelda == 44)
            {
                jugador.transform.position = new Vector3(mapArray[i].transform.position.x, 10, mapArray[i].transform.position.z);
                camara.GetComponent<CameraMovement>().room = mapArray[i].transform.GetChild(0).transform;
            }
        }
    }

    public void TeleportarJugador(GameObject plano, GameObject piso)
    {
        camara.GetComponent<CameraMovement>().room = piso.transform;
        jugador.transform.position = new Vector3(plano.transform.position.x, plano.transform.position.y, plano.transform.position.z);
    }
}
