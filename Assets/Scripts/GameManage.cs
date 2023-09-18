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
    public GameObject SalaActual;
    public MiniMapGen miniMapGen;
    public int Counter = 0;
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
                SalaActual = mapArray[i];
                mapArray[i].GetComponent<Room>().IsClear = true;
                jugador.transform.position = new Vector3(mapArray[i].transform.position.x, 10, mapArray[i].transform.position.z);
                camara.GetComponent<CameraMovement>().room = mapArray[i].transform.GetChild(0).transform;
            }
        }
    }

    public List<GameObject> FindGameObjectByTag(GameObject SalaTag, string tag)
    {
        int ChildrenQuantity = SalaTag.transform.childCount;
        List<GameObject> SalasWithTag = new List<GameObject>();
        for (int i=0;i<ChildrenQuantity;i++)
        {
            if (SalaTag.transform.GetChild(i).tag == tag)
            {
                SalasWithTag.Add(SalaTag.transform.GetChild(i).gameObject);
            }

        }
        return SalasWithTag;
    }

    public void MakeClear()
    {
        SalaActual.GetComponent<Room>().IsClear = true;
        dungeonManager.IngresarPuertasScriptsSolo(ref mapArray, SalaActual,this);
        Debug.Log("Masacraste a todos");
    }

    public void TeleportarJugador(GameObject plano, GameObject Sala)
    {
        SalaActual = Sala;
        Counter = 0;

        miniMapGen.ActualizarMiniMapa(SalaActual);

        if (!Sala.GetComponent<Room>().IsClear)
        {
            List<GameObject> SalasWithTag = new List<GameObject>();
            SalasWithTag = FindGameObjectByTag(Sala, "Enemy");
            Counter = SalasWithTag.Count;
            if(Counter == 0)
            {
                MakeClear();
            }
            else
            {
                dungeonManager.SetearObjetosDefault(Sala);
                Debug.Log("hora de matar monstruos");

                for (int i = 0; i < Counter; i++)
                {
                    SalasWithTag[i].GetComponent<Enemy>().GameManage = this;
                }
            }
        }

        camara.GetComponent<CameraMovement>().room = Sala.transform.GetChild(0).transform;
        jugador.transform.position = new Vector3(plano.transform.position.x, plano.transform.position.y, plano.transform.position.z);
    }
}
