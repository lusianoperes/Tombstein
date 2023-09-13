using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMapGen : MonoBehaviour
{
    public Canvas salaMini;
    public GameObject test;

    public void GenerateMiniMap(List<GameObject> mapArray)
    {
        //test = Instantiate(salaMini.transform.Find("Sala").gameObject,new Vector3(1*500,0, 1*1000), Quaternion.identity);
        //test.transform.parent = salaMini.transform;
        //Debug.Log(test.GetComponent<RectTransform>().localPosition.x);
        for (int i = 0; i < mapArray.Count; i++)

        {
            test = Instantiate(salaMini.transform.Find("Sala").gameObject,new Vector3((mapArray[i].GetComponent<Room>().valorDeCelda - mapArray[i].GetComponent<Room>().valorDeCelda / 10 * 10)*100, (mapArray[i].GetComponent<Room>().valorDeCelda / 10)*100, 0), Quaternion.identity);
            test.transform.parent = salaMini.transform;
            if (mapArray[i].GetComponent<Room>().tamañoSala == 10)
            {
                Debug.Log("pls");
            }
        }
    }
}
