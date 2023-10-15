using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LigthRange_Size_AutoAdjust : MonoBehaviour
{   
    new private Light light;
    private float originalRange;

    void Start()
    {
        light = GetComponent<Light>();
        originalRange = light.range;
    }

    void Update()
    {
        var parentScale = transform.parent.localScale;
        light.range = originalRange * Mathf.Max(parentScale.x, parentScale.y, parentScale.z);
    }
}

