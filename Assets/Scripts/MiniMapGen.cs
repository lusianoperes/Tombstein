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
    private GameObject[] salaAGenerar;
    private GameObject[] Puentes;
    private List<GameObject> SalasPasadas = new List<GameObject>();

    private bool MapaAbierto = false;

    public Material Completada;
    private void Start()
    {
        Iconos = Resources.LoadAll<GameObject>("Prefabs/Egipto/Layouts/MiniMap/Iconos");
        salaAGenerar = Resources.LoadAll<GameObject>("Prefabs/Egipto/Layouts/MiniMap/Salas");
        Puentes = Resources.LoadAll<GameObject>("Prefabs/Egipto/Layouts/MiniMap/Puentes");
    }
    private void Update()
    {
        if (Input.GetKeyDown("m"))
        {
            if (!MapaAbierto)
            {
                MiniMapCamera.transform.position = new Vector3(Spawn.transform.position.x, 600, Spawn.transform.position.z);
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
        int tipo = 0;
        GameObject Sala;

        for (int i = 0; i < mapArray.Count; i++)
        {
            Sala = Instantiate(salaAGenerar[size].gameObject, new Vector3((mapArray[i].GetComponent<Room>().valorDeCelda - mapArray[i].GetComponent<Room>().valorDeCelda / 10 * 10) * 500 + 3000, 0, (mapArray[i].GetComponent<Room>().valorDeCelda / 10) * 500 + 3000), Quaternion.identity);
            GenerarPuente(Sala,mapArray[i], mapArray);
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

            switch (mapArray[i].GetComponent<Room>().tipoDeSala)
            {
                case Room.TipoDeSala.Jefe:
                    tipo = 1;
                    break;
                case Room.TipoDeSala.Minijefe:
                    tipo = 2;
                    break;
                case Room.TipoDeSala.Npc:
                    tipo = 3;
                    break;

                default: break;
            }

            if (mapArray[i].GetComponent<Room>().tipoDeSala == Room.TipoDeSala.Jefe || mapArray[i].GetComponent<Room>().tipoDeSala == Room.TipoDeSala.Npc || mapArray[i].GetComponent<Room>().tipoDeSala == Room.TipoDeSala.Minijefe)
            {
                Instantiate(Iconos[tipo].gameObject, new Vector3((mapArray[i].GetComponent<Room>().valorDeCelda - mapArray[i].GetComponent<Room>().valorDeCelda / 10 * 10) * 500 + 3000, 500, (mapArray[i].GetComponent<Room>().valorDeCelda / 10) * 500 + 3000), Quaternion.identity);
            }

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

        Player.transform.position = new Vector3(SalaActual.transform.position.x, 551, SalaActual.transform.position.z);
        MiniMapCamera.transform.position = new Vector3(SalaActual.transform.position.x,600, SalaActual.transform.position.z); 
    }

    public void GenerarPuente(GameObject Sala, GameObject SalaReal, List<GameObject> mapArray)
    {

        for (int k = 0; k < mapArray.Count; k++)
        {
            GameObject Puente;
            if (!SalasPasadas.Contains(mapArray[k]))
            {

                if (SalaReal.GetComponent<Room>().valorDeCelda == mapArray[k].GetComponent<Room>().valorDeCelda + 10)
                {
                    Puente = Instantiate(Puentes[0].gameObject, new Vector3(Sala.transform.position.x, Sala.transform.position.y, Sala.transform.position.z - 200), Quaternion.identity);
                    Puente.transform.Rotate(0.0f, 90.0f, 0.0f, Space.Self);
                    SalasPasadas.Add(SalaReal);
                }
                if (SalaReal.GetComponent<Room>().valorDeCelda == mapArray[k].GetComponent<Room>().valorDeCelda - 10)
                {
                    Puente = Instantiate(Puentes[0].gameObject, new Vector3(Sala.transform.position.x, Sala.transform.position.y, Sala.transform.position.z + 200), Quaternion.identity);
                    Puente.transform.Rotate(0.0f, 90.0f, 0.0f, Space.Self);
                    SalasPasadas.Add(SalaReal);
                }
                if (SalaReal.GetComponent<Room>().valorDeCelda == mapArray[k].GetComponent<Room>().valorDeCelda + 1)
                {
                    Instantiate(Puentes[0].gameObject, new Vector3(Sala.transform.position.x - 200, Sala.transform.position.y, Sala.transform.position.z), Quaternion.identity);
                    SalasPasadas.Add(SalaReal);
                }
                if (SalaReal.GetComponent<Room>().valorDeCelda == mapArray[k].GetComponent<Room>().valorDeCelda - 1)
                {
                    Instantiate(Puentes[0].gameObject, new Vector3(Sala.transform.position.x + 200, Sala.transform.position.y, Sala.transform.position.z), Quaternion.identity);
                    SalasPasadas.Add(SalaReal);
                }
            }
        }

    }

}
