using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMapGen : MonoBehaviour
{
    public Canvas salaMini;
    public GameManage GameManage;
    private GameObject test;
    private GameObject Spawn;
    private bool MapaAbierto = false;

    private void Start()
    {
        salaMini.gameObject.SetActive(false);
    }
    private void Update()
    {
        if (Input.GetKeyDown("m"))
        {
            MapaAbierto = !MapaAbierto;
            salaMini.gameObject.SetActive(MapaAbierto);
        }
    }
    public void GenerateMiniMap(List<GameObject> mapArray)
    {
        for (int i = 0; i < mapArray.Count; i++)
        {
            test = Instantiate(salaMini.transform.GetChild(2).GetChild(0).gameObject, new Vector2 (salaMini.transform.GetChild(2).GetChild(0).GetComponent<RectTransform>().position.x + (40 * (mapArray[i].GetComponent<Room>().valorDeCelda - mapArray[i].GetComponent<Room>().valorDeCelda / 10 * 10) + 60), salaMini.transform.GetChild(2).GetChild(0).GetComponent<RectTransform>().position.y + (40 * (mapArray[i].GetComponent<Room>().valorDeCelda / 10)) - 15), Quaternion.identity);
            test.SetActive(true);
            test.transform.parent = salaMini.transform.GetChild(1);

            if(mapArray[i].GetComponent<Room>().valorDeCelda == 44)
            {
                Spawn = mapArray[i];
            }
        }
        test = null;
        ActualizarMiniMapa(Spawn);

    }
    public void ActualizarMiniMapa(GameObject SalaActual)
    {

        Destroy(test);
        test = Instantiate(salaMini.transform.GetChild(2).GetChild(1).gameObject, new Vector2(salaMini.transform.GetChild(2).GetChild(0).GetComponent<RectTransform>().position.x + (40 * (SalaActual.GetComponent<Room>().valorDeCelda - SalaActual.GetComponent<Room>().valorDeCelda / 10 * 10) + 60), salaMini.transform.GetChild(2).GetChild(0).GetComponent<RectTransform>().position.y + (40 * (SalaActual.GetComponent<Room>().valorDeCelda / 10)) - 15), Quaternion.identity);
        test.transform.parent = salaMini.transform.GetChild(1);
    }
}
