using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DungeonManager : MonoBehaviour
{
    public int numeroDeSalas;
    public int numeroDeEndrooms;
    public int floorplancount = 0;
    public int[] floorplan = new int[100];
    public List<int> endrooms = new List<int>();
    public List<GameObject> mapArray;
    public GameManage gameManage;

    public List<GameObject> GenerarListaRooms(Room.Etapa etapa, int erasRecorridas)
    {
        numeroDeSalas = DefinirCantidadSalas(etapa, erasRecorridas);

        while (floorplancount < numeroDeSalas || (endrooms.Count <= 6 && endrooms.Count >= 4))
        {
            endrooms.Clear();
            floorplancount = 0;
            floorplan = GenerarMapa(ref floorplancount, ref endrooms);
        }
        mapArray = ContenidoMapa(floorplan, endrooms);
        return mapArray;
    }

    public int[] GenerarMapa(ref int floorplancount, ref List<int> endrooms)
    {

        Queue<int> colaCeldas = new Queue<int>();

        int[] floorplan = new int[100];

        int i;
        int x;
        bool created;

        for (i = 0; i < 100; i++)
        {
            floorplan[i] = -1;
        }

        VisitarCelda(44, ref floorplan, numeroDeSalas, ref colaCeldas, ref floorplancount);

        while (floorplancount <= numeroDeSalas && colaCeldas.Count > 0)
        {
            i = colaCeldas.Dequeue();
            x = i % 10;
            created = false;
            if (i < 70)
            {
                created = VisitarCelda(i + 10, ref floorplan, numeroDeSalas, ref colaCeldas, ref floorplancount) || created;
            }
            if (i > 20)
            {
                created = VisitarCelda(i - 10, ref floorplan, numeroDeSalas, ref colaCeldas, ref floorplancount) || created;
            }
            if (x > 1)
            {
                created = VisitarCelda(i - 1, ref floorplan, numeroDeSalas, ref colaCeldas, ref floorplancount) || created;
            }
            if (x < 9)
            {
                created = VisitarCelda(i + 1, ref floorplan, numeroDeSalas, ref colaCeldas, ref floorplancount) || created;
            }

            if (!created)
            {
                endrooms.Add(i);
            }

        }

        return floorplan;

    }

    public bool VisitarCelda(int i, ref int[] floorplan, int numeroDeSalas, ref Queue<int> colaCeldas, ref int floorplancount)
    {

        if (floorplan[i] != -1)
        {
            return false;
        }

        int vecinos = ContarVecinos(i, ref floorplan);

        if (vecinos > 1)
        {
            return false;
        }
        if (floorplancount >= numeroDeSalas)
        {
            return false;
        }
        if (UnityEngine.Random.value < 0.5f && i != 44) //revisar
        {
            return false;
        }

        colaCeldas.Enqueue(i);
        floorplan[i] = i;
        floorplancount += 1;

        return true;
    }
    public int ContarVecinos(int i, ref int[] floorplan)
    {
        int x = i % 10;
        int vecinos = 0;

        if (i < 70)
        {
            if (floorplan[i + 10] != -1)
            {
                vecinos += 1;
            }
        }
        if (i > 20)
        {
            if (floorplan[i - 10] != -1)
            {
                vecinos += 1;
            }
        }
        if (x > 1)
        {
            if (floorplan[i - 1] != -1)
            {
                vecinos += 1;
            }
        }
        if (x < 9)
        {
            if (floorplan[i + 1] != -1)
            {
                vecinos += 1;
            }
        }

        return vecinos;
    }
    public int DefinirCantidadSalas(Room.Etapa etapa, int erasRecorridas)
    {
        if (etapa == Room.Etapa.Primera /*|| etapa == Room.Etapa.Segunda*/)
        {
            return 20;
        }
        else
        {
            return 5;
        }
    }

    public List<GameObject> ContenidoMapa(int[] floorplan, List<int> endrooms)
    {
        int i;

        GameObject[] prefabs = Resources.LoadAll<GameObject>("Prefabs/Egipto/Layouts/Salas");

        //GameObject room = new GameObject();

        List<GameObject> mapArray = new List<GameObject>();

        List<GameObject> filteredPrefabs = new List<GameObject>();

        bool jefePuesto = false;

        bool npcPuesta = false;

        GameObject room = null;

        for (i = 0; i < floorplan.Length; i++)
        {

            filteredPrefabs.Clear();

            if (floorplan[i] != -1)
            {
                room = null;

                if (floorplan[i] == 44)
                {

                    AsignarLayout(Room.TipoDeSala.Spawn, ref mapArray, floorplan, ref prefabs, ref filteredPrefabs, ref room, i);

                }
                else if (endrooms.Contains(floorplan[i]))
                {

                    if (floorplan[i] == EncontrarValorMasAlejado(44, endrooms) && !jefePuesto)
                    {
                        jefePuesto = true;

                        AsignarLayout(Room.TipoDeSala.Jefe, ref mapArray, floorplan, ref prefabs, ref filteredPrefabs, ref room, i);
                    }
                    else if (!npcPuesta)
                    {
                        npcPuesta = true;

                        AsignarLayout(Room.TipoDeSala.Npc, ref mapArray, floorplan, ref prefabs, ref filteredPrefabs, ref room, i);
                    }
                    else
                    {
                        AsignarLayout(Room.TipoDeSala.Minijefe, ref mapArray, floorplan, ref prefabs, ref filteredPrefabs, ref room, i);
                    }

                }
                else
                {
                    AsignarLayout(Room.TipoDeSala.Enemigos, ref mapArray, floorplan, ref prefabs, ref filteredPrefabs, ref room, i);
                }

            }

        }

        return mapArray;
    }

    public void AsignarLayout(Room.TipoDeSala tipoDeSala, ref List<GameObject> mapArray, int[] floorplan, ref GameObject[] prefabs, ref List<GameObject> filteredPrefabs, ref GameObject room, int i)
    {

        filteredPrefabs.Clear();
        room = null;
        int randomIndex = 0;

        foreach (GameObject prefab in prefabs)
        {

            if (tipoDeSala == Room.TipoDeSala.Enemigos || tipoDeSala == Room.TipoDeSala.Obstaculos)
            {
                if (prefab.GetComponent<Room>() != null && (prefab.GetComponent<Room>().tipoDeSala == Room.TipoDeSala.Enemigos || prefab.GetComponent<Room>().tipoDeSala == Room.TipoDeSala.Obstaculos))
                {
                    filteredPrefabs.Add(prefab);
                }
            }
            else
            {
                if (prefab.GetComponent<Room>() != null && prefab.GetComponent<Room>().tipoDeSala == tipoDeSala)
                {
                    filteredPrefabs.Add(prefab);
                }
            }

        }

        if (filteredPrefabs.Count > 0)
        {
            randomIndex = UnityEngine.Random.Range(0, filteredPrefabs.Count);
            room = Instantiate(filteredPrefabs[randomIndex]);
        }

        room.GetComponent<Room>().valorDeCelda = floorplan[i];

        mapArray.Add(room);
        Destroy(room);
    }

    public int EncontrarValorMasAlejado(int x, List<int> listaValores)
    {
        int[,] tabla = new int[,]
        {
            { 80, 81, 82, 83, 84, 85, 86, 87, 88 },
            { 70, 71, 72, 73, 74, 75, 76, 77, 78 },
            { 60, 61, 62, 63, 64, 65, 66, 67, 68 },
            { 50, 51, 52, 53, 54, 55, 56, 57, 58 },
            { 40, 41, 42, 43, 44, 45, 46, 47, 48 },
            { 30, 31, 32, 33, 34, 35, 36, 37, 38 },
            { 20, 21, 22, 23, 24, 25, 26, 27, 28 },
            { 10, 11, 12, 13, 14, 15, 16, 17, 18 },
            {  0,  1,  2,  3,  4,  5,  6,  7,  8 }
        };

        int distanciaMaxima = int.MinValue;
        int valorMasAlejado = 0;

        // Encontrar las coordenadas de x en la tabla
        int filaX = -1;
        int columnaX = -1;
        for (int i = 0; i < tabla.GetLength(0); i++)
        {
            for (int j = 0; j < tabla.GetLength(1); j++)
            {
                if (tabla[i, j] == x)
                {
                    filaX = i;
                    columnaX = j;
                    break;
                }
            }
            if (filaX != -1)
                break;
        }

        // Calcular la distancia posicional para cada valor en la lista
        foreach (int valor in listaValores)
        {
            // Encontrar las coordenadas del valor en la tabla
            int filaValor = -1;
            int columnaValor = -1;
            for (int i = 0; i < tabla.GetLength(0); i++)
            {
                for (int j = 0; j < tabla.GetLength(1); j++)
                {
                    if (tabla[i, j] == valor)
                    {
                        filaValor = i;
                        columnaValor = j;
                        break;
                    }
                }
                if (filaValor != -1)
                    break;
            }

            // Calcular la distancia posicional entre x y el valor
            int distancia = Math.Abs(filaValor - filaX) + Math.Abs(columnaValor - columnaX);

            // Actualizar el valor más alejado si la distancia es mayor a la distancia máxima registrada
            if (distancia > distanciaMaxima)
            {
                distanciaMaxima = distancia;
                valorMasAlejado = valor;
            }
        }

        return valorMasAlejado;
    }
    
    /*
    public void BuscarPorNombre(GameObject mapArrayi, GameObject mapArrayk, int k, string Direccion, string direccionContraria, GameManage gameManage, List<GameObject> MinimapPiso)
    {
        mapArrayi.transform.GetChild(0).Find("Lateral " + Direccion).Find("Puerta").gameObject.SetActive(false);
        mapArrayi.transform.GetChild(0).Find("Lateral " + Direccion).Find("Puerta Invisible").gameObject.SetActive(false);
        mapArrayi.transform.GetChild(0).Find("Lateral " + Direccion).Find("Marco").gameObject.SetActive(true);
        mapArrayi.transform.GetChild(0).Find("Lateral " + Direccion).Find("Relleno").GetComponent<BoxCollider>().enabled = false;

        if (!mapArrayi.transform.GetChild(0).Find("Lateral " + Direccion).Find("Spawnpoint").gameObject.TryGetComponent<Tepear>(out Tepear hinge))
            mapArrayi.transform.GetChild(0).Find("Lateral " + Direccion).Find("Spawnpoint").gameObject.AddComponent<Tepear>();

        mapArrayi.transform.GetChild(0).Find("Lateral " + Direccion).Find("Spawnpoint").gameObject.GetComponent<Tepear>().gameManage = gameManage;
        mapArrayi.transform.GetChild(0).Find("Lateral " + Direccion).Find("Spawnpoint").gameObject.GetComponent<Tepear>().plano = mapArrayk.transform.GetChild(0).Find("Lateral " + direccionContraria).Find("TPPlayer").gameObject;
        mapArrayi.transform.GetChild(0).Find("Lateral " + Direccion).Find("Spawnpoint").gameObject.GetComponent<Tepear>().Sala = mapArrayk;
        mapArrayi.transform.GetChild(0).Find("Lateral " + Direccion).Find("Spawnpoint").gameObject.GetComponent<Tepear>().MiniMapPlano = MinimapPiso[k];
    }

    public void SetearObjetosDefault(GameObject mapArrayi, bool SacarMarco)
    {
        string[] Direccion = new string[] {
            "inferior", "superior", "derecha", "izquierda"
        };

        for (int i=0;i<Direccion.Length;i++)
        {
            if (SacarMarco)
            {
                mapArrayi.transform.GetChild(0).Find("Lateral " + Direccion[i]).Find("Marco").gameObject.SetActive(false);
                mapArrayi.transform.GetChild(0).Find("Lateral " + Direccion[i]).Find("AntorchasMarcoOff").gameObject.SetActive(true);  
            }
            
            mapArrayi.transform.GetChild(0).Find("Lateral " + Direccion[i]).Find("Puerta").gameObject.SetActive(true);
            mapArrayi.transform.GetChild(0).Find("Lateral " + Direccion[i]).Find("Puerta Invisible").gameObject.SetActive(true);
            mapArrayi.transform.GetChild(0).Find("Lateral " + Direccion[i]).Find("Relleno").GetComponent<BoxCollider>().enabled = true;
        }
    }

    public List<GameObject> IngresarPuertasScripts(ref List<GameObject> mapArray, GameManage gameManage, ref List<GameObject> MinimapArray)
    {
        for (int i = 0; i< mapArray.Count; i++)
        {
            SetearObjetosDefault(mapArray[i],true);

            IngresarPuertasScriptsSolo(ref mapArray, mapArray[i], gameManage, ref MinimapArray);
        }

            return mapArray;
    }

    public void IngresarPuertasScriptsSolo(ref List<GameObject> mapArrayList,GameObject mapArray, GameManage gameManage, ref List<GameObject> MinimapArray)
    {
            SetearObjetosDefault(mapArray,true);

            for (int k = 0; k < mapArrayList.Count; k++)
            {

                if (mapArray.GetComponent<Room>().valorDeCelda == mapArrayList[k].GetComponent<Room>().valorDeCelda + 10)
                {

                    BuscarPorNombre(mapArray, mapArrayList[k],k, "inferior", "superior", gameManage, MinimapArray);

                }
                if (mapArray.GetComponent<Room>().valorDeCelda == mapArrayList[k].GetComponent<Room>().valorDeCelda - 10)
                {

                    BuscarPorNombre(mapArray, mapArrayList[k],k, "superior", "inferior", gameManage, MinimapArray);

                }
                if (mapArray.GetComponent<Room>().valorDeCelda == mapArrayList[k].GetComponent<Room>().valorDeCelda + 1)
                {

                    BuscarPorNombre(mapArray, mapArrayList[k], k, "izquierda", "derecha", gameManage, MinimapArray);

                }
                if (mapArray.GetComponent<Room>().valorDeCelda == mapArrayList[k].GetComponent<Room>().valorDeCelda - 1)
                {

                    BuscarPorNombre(mapArray, mapArrayList[k], k, "derecha", "izquierda", gameManage, MinimapArray);
                }
            }

    }

*/
//-------------------------------------------------------------------- >w<

    public void OpenTheDoor(GameObject sala)
    {   
        sala.GetComponent<Room>().doorReferences.closedDoorIzq.SetActive(false);
        sala.GetComponent<Room>().doorReferences.closedDoorTop.SetActive(false);
        sala.GetComponent<Room>().doorReferences.closedDoorDer.SetActive(false);
        sala.GetComponent<Room>().doorReferences.closedDoorDown.SetActive(false);
    }
    public void SetearSalaDefault(GameObject sala)
    {

        sala.GetComponent<Room>().doorReferences.marcoIzq.SetActive(false);
        sala.GetComponent<Room>().doorReferences.marcoTop.SetActive(false);
        sala.GetComponent<Room>().doorReferences.marcoDer.SetActive(false);
        sala.GetComponent<Room>().doorReferences.marcoDown.SetActive(false);

        sala.GetComponent<Room>().doorReferences.closedDoorIzq.SetActive(false);
        sala.GetComponent<Room>().doorReferences.closedDoorTop.SetActive(false);
        sala.GetComponent<Room>().doorReferences.closedDoorDer.SetActive(false);
        sala.GetComponent<Room>().doorReferences.closedDoorDown.SetActive(false);

        sala.GetComponent<Room>().doorReferences.DoorFillerIzq.SetActive(true);
        sala.GetComponent<Room>().doorReferences.DoorFillerTop.SetActive(true);
        sala.GetComponent<Room>().doorReferences.DoorFillerDer.SetActive(true);
        sala.GetComponent<Room>().doorReferences.DoorFillerDown.SetActive(true);
        
    }

    public void SeteadoDePuertasSala(ref List<GameObject> mapArrayList,GameObject sala, GameManage ggameManage, ref List<GameObject> MinimapArray)
    {
        SetearSalaDefault(sala);
        
        for(int k=0; k <mapArrayList.Count; k++){
            if (sala.GetComponent<Room>().valorDeCelda == mapArrayList[k].GetComponent<Room>().valorDeCelda + 10)//hay sala abajo
            {
                sala.GetComponent<Room>().doorReferences.marcoDown.SetActive(true);
                sala.GetComponent<Room>().doorReferences.DoorFillerDown.SetActive(false);
                sala.GetComponent<Room>().doorReferences.closedDoorDown.SetActive(true);
                
                sala.GetComponent<Room>().doorReferences.portalDown.GetComponent<Tepear>().gameManage = ggameManage;
                sala.GetComponent<Room>().doorReferences.portalDown.GetComponent<Tepear>().plano = mapArrayList[k].GetComponent<Room>().doorReferences.spawnTop;
                sala.GetComponent<Room>().doorReferences.portalDown.GetComponent<Tepear>().Sala = mapArrayList[k];
                sala.GetComponent<Room>().doorReferences.portalDown.GetComponent<Tepear>().MiniMapPlano = MinimapArray[k];
            }
        
            if (sala.GetComponent<Room>().valorDeCelda == mapArrayList[k].GetComponent<Room>().valorDeCelda - 10)//hay sala ariba
            {
                sala.GetComponent<Room>().doorReferences.marcoTop.SetActive(true);
                sala.GetComponent<Room>().doorReferences.DoorFillerTop.SetActive(false);
                sala.GetComponent<Room>().doorReferences.closedDoorTop.SetActive(true);
                
                sala.GetComponent<Room>().doorReferences.portalTop.GetComponent<Tepear>().gameManage = ggameManage;
                sala.GetComponent<Room>().doorReferences.portalTop.GetComponent<Tepear>().plano = mapArrayList[k].GetComponent<Room>().doorReferences.spawnDown;
                sala.GetComponent<Room>().doorReferences.portalTop.GetComponent<Tepear>().Sala = mapArrayList[k];
                sala.GetComponent<Room>().doorReferences.portalTop.GetComponent<Tepear>().MiniMapPlano = MinimapArray[k];
            }

            if (sala.GetComponent<Room>().valorDeCelda == mapArrayList[k].GetComponent<Room>().valorDeCelda + 1)//hay sala iz
            {
                sala.GetComponent<Room>().doorReferences.marcoIzq.SetActive(true);
                sala.GetComponent<Room>().doorReferences.DoorFillerIzq.SetActive(false);
                sala.GetComponent<Room>().doorReferences.closedDoorIzq.SetActive(true);
     
                
                sala.GetComponent<Room>().doorReferences.portalIzq.GetComponent<Tepear>().gameManage = ggameManage;
                sala.GetComponent<Room>().doorReferences.portalIzq.GetComponent<Tepear>().plano = mapArrayList[k].GetComponent<Room>().doorReferences.spawnDer;
                sala.GetComponent<Room>().doorReferences.portalIzq.GetComponent<Tepear>().Sala = mapArrayList[k];
                sala.GetComponent<Room>().doorReferences.portalIzq.GetComponent<Tepear>().MiniMapPlano = MinimapArray[k];
            }
        
            if (sala.GetComponent<Room>().valorDeCelda == mapArrayList[k].GetComponent<Room>().valorDeCelda - 1)//hay sala der
            {   
                sala.GetComponent<Room>().doorReferences.marcoDer.SetActive(true);
                sala.GetComponent<Room>().doorReferences.DoorFillerDer.SetActive(false);
                sala.GetComponent<Room>().doorReferences.closedDoorDer.SetActive(true);

                sala.GetComponent<Room>().doorReferences.portalDer.GetComponent<Tepear>().gameManage = ggameManage;
                sala.GetComponent<Room>().doorReferences.portalDer.GetComponent<Tepear>().plano = mapArrayList[k].GetComponent<Room>().doorReferences.spawnIzq;
                sala.GetComponent<Room>().doorReferences.portalDer.GetComponent<Tepear>().Sala = mapArrayList[k];
                sala.GetComponent<Room>().doorReferences.portalDer.GetComponent<Tepear>().MiniMapPlano = MinimapArray[k];
            }
        }
    }
    public List<GameObject> SeteadoDePuertasGeneral(ref List<GameObject> mapArrayList, GameManage gameManage, ref List<GameObject> MinimapArray)
    {
        for (int k = 0; k < mapArrayList.Count; k++)
        {   
            SeteadoDePuertasSala(ref mapArrayList, mapArrayList[k], gameManage, ref MinimapArray);
        }
        return mapArrayList;
    }


}








