using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMapGen : MonoBehaviour
{
    public Canvas salaMini;
    public GameManage GameManage;
    private GameObject salaAGenerar;
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
            AgregarIconos(0, mapArray[i]);

            if(mapArray[i].GetComponent<Room>().valorDeCelda == 44)
            {
                Spawn = mapArray[i];
            }

            if(mapArray[i].GetComponent<Room>().tipoDeSala == Room.TipoDeSala.Npc)
            {
                AgregarIconos(3, mapArray[i]);
            }

            if (mapArray[i].GetComponent<Room>().tipoDeSala == Room.TipoDeSala.Jefe)
            {
                AgregarIconos(4, mapArray[i]);
            }

            if (mapArray[i].GetComponent<Room>().tipoDeSala == Room.TipoDeSala.Minijefe)
            {
                AgregarIconos(5, mapArray[i]);
            }

            if(mapArray[i].GetComponent<Room>().SalaSize == 10)
            {
                salaMini.transform.GetChild(2).GetChild(0).GetComponent<RectTransform>().localScale = new Vector3(0.1795932f, 0.1795932f, 0.1795932f);
            }

            if (mapArray[i].GetComponent<Room>().SalaSize == 75)
            {
                salaMini.transform.GetChild(2).GetChild(0).GetComponent<RectTransform>().localScale = new Vector3(0.124028f, 0.124028f, 0.124028f);
            }

            if (mapArray[i].GetComponent<Room>().SalaSize == 75)
            {
                salaMini.transform.GetChild(2).GetChild(0).GetComponent<RectTransform>().localScale = new Vector3(0.07677455f, 0.07677455f, 0.07677455f);
            }
        }
        salaAGenerar = null;
        ActualizarMiniMapa(Spawn);

    }
    public void ActualizarMiniMapa(GameObject SalaActual)
    {
        Destroy(salaAGenerar);
        if (SalaActual.GetComponent<Room>().IsClear)
        {
            AgregarIconos(2, SalaActual);
        }
        AgregarIconos(1, SalaActual);
    }

    public void AgregarIconos(int NumeroDeSala, GameObject sala)
    {
        salaAGenerar = Instantiate(salaMini.transform.GetChild(2).GetChild(NumeroDeSala).gameObject, new Vector2(salaMini.transform.GetChild(2).GetChild(0).GetComponent<RectTransform>().position.x + (40 * (sala.GetComponent<Room>().valorDeCelda - sala.GetComponent<Room>().valorDeCelda / 10 * 10) + 60), salaMini.transform.GetChild(2).GetChild(0).GetComponent<RectTransform>().position.y + (40 * (sala.GetComponent<Room>().valorDeCelda / 10)) - 15), Quaternion.identity);
        salaAGenerar.transform.parent = salaMini.transform.GetChild(1);
    }
}
