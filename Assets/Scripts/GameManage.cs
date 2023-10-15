using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;

public class GameManage : MonoBehaviour
{
    public DungeonManager dungeonManager;
    public MiniMapGen miniMapGen;

    public Room.Etapa etapa = Room.Etapa.Primera;
    public GameObject jugador;
    public GameObject camara;

    public GameObject MiniSalaActual;
    public GameObject SalaActual;

    public List<GameObject> mapArray;
    public List<GameObject> MinimapArray = new List<GameObject>();

    public menuPausaFunciones hud;

    public int Counter = 0;
    public int erasRecorridas = 0;
    void Start()
    {
        InstanciarMapa(dungeonManager, ref mapArray);
        hud = GameObject.Find("Hud").GetComponent<menuPausaFunciones>();
        MinimapArray =  miniMapGen.InstanciarMiniMapa(ref mapArray, ref MinimapArray, ref MiniSalaActual);
        mapArray = dungeonManager.SeteadoDePuertasGeneral(ref mapArray, this, ref MinimapArray); //
        miniMapGen.ActualizarMiniMapa(MiniSalaActual,SalaActual);
        
        MakeClear();
    }

    void Update()
    {
    }
    public void InstanciarMapa(DungeonManager dungeonManager, ref List<GameObject> mapArray)
    {
        mapArray = dungeonManager.GenerarListaRooms(etapa, erasRecorridas);
        for (int i = 0; i < mapArray.Count; i++)
        {
            mapArray[i] = Instantiate(mapArray[i], new Vector3((mapArray[i].GetComponent<Room>().valorDeCelda - mapArray[i].GetComponent<Room>().valorDeCelda / 10 * 10) * 300, 0, (mapArray[i].GetComponent<Room>().valorDeCelda / 10) * 300), Quaternion.identity); //salas realaes
            mapArray[i].gameObject.SetActive(false);

            NavMeshSurface surfaceComponent = mapArray[i].GetComponent<NavMeshSurface>();
            if (surfaceComponent != null)
            {
                surfaceComponent.enabled = true;
            }
            
            Transform enemigos = mapArray[i].transform.Find("Enemigos");
            if (enemigos != null)
            {
                foreach (Transform child in enemigos)
                {
                    UnityEngine.AI.NavMeshAgent navMeshAgent = child.GetComponent<UnityEngine.AI.NavMeshAgent>();
                    if (navMeshAgent != null)
                    {
                        navMeshAgent.enabled = true;
                    }
                    
                    Enemy enemyComponent = child.GetComponent<Enemy>();
                    if (enemyComponent != null)
                    {
                        enemyComponent.enabled = true;
                    }
                }
            }
            if (mapArray[i].GetComponent<Room>().valorDeCelda == 44)
            {
                mapArray[i].gameObject.SetActive(true);
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
        Debug.Log("makeclear llamado");
        SalaActual.GetComponent<Room>().IsClear = true; 
        dungeonManager.OpenTheDoor(SalaActual);
        miniMapGen.ActualizarMiniMapa(MiniSalaActual, SalaActual);
    }
    public void TeleportarJugador(GameObject plano, GameObject Sala, GameObject MiniMapPlano)
    {
        SalaActual.SetActive(false);
        Sala.SetActive(true);
        SalaActual = Sala;
        MiniSalaActual = MiniMapPlano;
        Counter = 0;
        miniMapGen.ActualizarMiniMapa(MiniMapPlano, SalaActual);
        if (!Sala.GetComponent<Room>().IsClear)
        {
            Counter = Sala.transform.Find("Enemigos").childCount;
            Debug.Log(Counter);
            if(Counter == 0)
            {
                MakeClear();
            }
        }
        else
        {
            MakeClear();
        }
        camara.GetComponent<CameraMovement>().room = Sala.transform.GetChild(0).transform;
        jugador.transform.position = new Vector3(plano.transform.position.x, plano.transform.position.y, plano.transform.position.z);
    }

    public void PlayerDied()
    {
        hud.DeathMenu();
    }
}
