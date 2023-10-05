using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Transform target; // Referencia al transform del jugador
    public Transform room; // Referencia al transform de la sala
    public Vector3 offset; // Desplazamiento de la cámara con respecto al jugador
    public float minX; // Límite mínimo en el eje X de movimiento de cámara
    public float maxX; // Límite máximo en el eje X de movimiento de cámara
    public float minZ; // Límite mínimo en el eje Z de movimiento de cámara
    public float maxZ; // Límite máximo en el eje Z de movimiento de cámara



    void Start()
    {
        offset = new Vector3(0, 50, -43);
    }

    void Update()
    {
        
        minX = room.position.x - room.localScale.x / 2 + -9.5f;
        maxX = room.position.x + room.localScale.x / 2 - -9.5f;
        maxZ = room.position.z + room.localScale.z / 2 - 27f;
        minZ = room.position.z - room.localScale.z / 2 + -58f;
    }


    void LateUpdate()
    {
        {
        //Más adelante tendríamos que agregar para que en caso de estar en una habitacion con un jefe
        //la cámara se pondría a seguir un punto intermedio entre el jugador y el jefe
        //Además la cámara también tiene que limitarse a la sala, es decir si el jugador llega al borde
        //de la misma esta tendria que no "apuntar" a un espacio donde no hay nada (misma forma q el isaac)
        // Obtén la posición actual de la cámara
        Vector3 desiredPosition = target.position + offset;
        
       /* if (desiredPosition.x < minX)   <--------------------- No anda bien con las salas :p
        {
            desiredPosition = new Vector3(minX,desiredPosition.y,desiredPosition.z);
        }
        else if (desiredPosition.x > maxX)
        {
            desiredPosition = new Vector3(maxX,desiredPosition.y,desiredPosition.z);
        }
        if (desiredPosition.z < minZ)
        {
            desiredPosition = new Vector3(desiredPosition.x,desiredPosition.y,minZ);
        }
        else if (desiredPosition.z > maxZ)
        {
            desiredPosition = new Vector3(desiredPosition.x,desiredPosition.y,maxZ);
        }*/
        
            transform.position = desiredPosition;
        
        
    }
        
    }
}
