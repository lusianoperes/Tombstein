using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMapGen : MonoBehaviour
{
    public Canvas salaMini;
    public GameManage GameManage;
    public Camera MiniMapCamera;
    private GameObject[] Iconos;
    private GameObject Spawn;
    private GameObject Player;
    public Material Completada;
    GameObject[] salaAGenerar;
    private bool MapaAbierto = false;

    private void Start()
    {
        Iconos = Resources.LoadAll<GameObject>("Prefabs/Egipto/Layouts/MiniMap/Iconos");
        salaAGenerar = Resources.LoadAll<GameObject>("Prefabs/Egipto/Layouts/MiniMap/Salas");
    }
    private void Update()
    {
        if (Input.GetKeyDown("m"))
        {
            if (!MapaAbierto)
            {
                ActualizarMiniMapa(Spawn, GameManage.SalaActual);
                MiniMapCamera.GetComponent<Camera>().orthographicSize = 2750;
            }
            else
            {
                ActualizarMiniMapa(GameManage.MiniSalaActual, GameManage.SalaActual);
                MiniMapCamera.GetComponent<Camera>().orthographicSize = 789;

            }
            MapaAbierto = !MapaAbierto;
            salaMini.transform.GetChild(0).gameObject.SetActive(MapaAbierto);
            salaMini.transform.GetChild(1).gameObject.SetActive(!MapaAbierto);
        }
    }

    public List<GameObject> InstanciarMiniMapa(ref List<GameObject> mapArray, ref List<GameObject> MinimapArray, ref GameObject MiniSalaActual)
    {
        int size = 0;
        GameObject Sala;
        for (int i = 0; i < mapArray.Count; i++)
        {
            switch (mapArray[i].GetComponent<Room>().SalaSize)
            {
                case 12:
                    size = 3;
                    break;
                case 10:
                    size = 2;
                    break;
                case 75:
                    size = 1;
                    break;
                case 5:
                    size = 0;
                    break;

                default: break;
            }
      
            if(mapArray[i].GetComponent<Room>().tipoDeSala == Room.TipoDeSala.Jefe)
            {
                Instantiate(Iconos[1].gameObject, new Vector3((mapArray[i].GetComponent<Room>().valorDeCelda - mapArray[i].GetComponent<Room>().valorDeCelda / 10 * 10) * 500 + 3000, 500, (mapArray[i].GetComponent<Room>().valorDeCelda / 10) * 500 + 3000), Quaternion.identity);
            }
            if (mapArray[i].GetComponent<Room>().tipoDeSala == Room.TipoDeSala.Npc)
            {
                Instantiate(Iconos[3].gameObject, new Vector3((mapArray[i].GetComponent<Room>().valorDeCelda - mapArray[i].GetComponent<Room>().valorDeCelda / 10 * 10) * 500 + 3000, 500, (mapArray[i].GetComponent<Room>().valorDeCelda / 10) * 500 + 3000), Quaternion.identity);
            }
            if (mapArray[i].GetComponent<Room>().tipoDeSala == Room.TipoDeSala.Minijefe)
            {
                Instantiate(Iconos[2].gameObject, new Vector3((mapArray[i].GetComponent<Room>().valorDeCelda - mapArray[i].GetComponent<Room>().valorDeCelda / 10 * 10) * 500 + 3000, 500, (mapArray[i].GetComponent<Room>().valorDeCelda / 10) * 500 + 3000), Quaternion.identity);
            }

            Sala = Instantiate(salaAGenerar[size].gameObject, new Vector3((mapArray[i].GetComponent<Room>().valorDeCelda - mapArray[i].GetComponent<Room>().valorDeCelda / 10 * 10) * 500 + 3000, 0, (mapArray[i].GetComponent<Room>().valorDeCelda / 10) * 500 + 3000), Quaternion.identity);
            MinimapArray.Add(Sala);
            if (mapArray[i].GetComponent<Room>().valorDeCelda == 44)
            {
                Player = Instantiate(Iconos[0].gameObject, new Vector3((mapArray[i].GetComponent<Room>().valorDeCelda - mapArray[i].GetComponent<Room>().valorDeCelda / 10 * 10) * 500 + 3000, 500, (mapArray[i].GetComponent<Room>().valorDeCelda / 10) * 500 + 3000), Quaternion.identity);
                Spawn = Sala;
                Debug.Log(Sala);
                MiniSalaActual = Sala;
            }
        }
        return MinimapArray;
    }

    public void ActualizarMiniMapa(GameObject SalaActual, GameObject SalaReal)
    {
        if (SalaReal.GetComponent<Room>().IsClear)
        {
            SalaActual.GetComponent<MeshRenderer>().material = Completada;
        }

        Player.transform.position = new Vector3(SalaActual.transform.position.x, 500, SalaActual.transform.position.z);
        MiniMapCamera.transform.position = new Vector3(SalaActual.transform.position.x,551, SalaActual.transform.position.z); 
    }

}
