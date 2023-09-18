using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movimientoydash : MonoBehaviour {
    public float velocidad = 5f; // Velocidad de movimiento
    public float velocidadD = 10f; // Velocidad del Dash
    public float duracionD = 0.5f; // Duracion del Dash
    public float determinarCooldown = 2f; // Cooldown del Dash
    private float cooldownD;
    private bool cooldownActivo = false;
    private bool dasheando = false;
    private Vector3 direccionD;


    void Update() {
        // Movimiento WASD (eje X y eje Y)
        float movimientoHorizontal = Input.GetAxis("Horizontal"); 
        float movimientoVertical = Input.GetAxis("Vertical");

        Vector3 movimiento = new Vector3(movimientoHorizontal, 0f, movimientoVertical) * velocidad * Time.deltaTime;
        transform.Translate(movimiento);

        // Activar el Dash (Hay apretar la Barra Espaciadora, que no se este dasheando y que el cooldown se encuentre desactivado)
        if(Input.GetKeyDown(KeyCode.Space) && !dasheando && !cooldownActivo) {
            dasheando = true;
            cooldownActivo = true;
            direccionD = movimiento.normalized;
            StartCoroutine(Dash());
            cooldownD = determinarCooldown;
        }

        // Control del cooldown
        if (cooldownActivo) {
            cooldownD-= Time.deltaTime;
            if (cooldownD <= 0) {
                cooldownActivo = false;
            }
        }

        // Cambia el movimiento cuando se esta dasheando
        if(!dasheando) {
            transform.Translate(movimiento * velocidad * Time.deltaTime);
        }

        // Esta funcion se utiliza para la activacion del Dash 
        IEnumerator Dash() {
            float tiempoInicio = Time.time;
            while (Time.time < tiempoInicio + duracionD)
            {
                transform.Translate(direccionD * velocidadD * Time.deltaTime);
                yield return null;
            }
            dasheando = false;
        }
    }
}
