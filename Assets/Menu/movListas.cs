using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movListas : MonoBehaviour
{
    // Variables públicas que se pueden configurar desde el editor de Unity
    public Vector3 startingPos;  // La posición inicial del objeto.
    public Transform finalPos;   // La posición final a la que se moverá el objeto.
    public Vector3 initialScale; // La escala inicial del objeto.
    public Vector3 finalScale;   // La escala final del objeto.
    public float timeDuration = 2f; // La duración total del movimiento y la animación.

    // Variables privadas para el seguimiento del tiempo y la evaluación de las curvas de animación.
    private float elapsedTime = 0f;

    // Curvas de animación configurables desde el editor de Unity.
    [SerializeField] private AnimationCurve curve;  // Curva para el movimiento de posición.
    [SerializeField] private AnimationCurve curve2; // Curva para el cambio de escala.

    void Start()
    {
        // Al comienzo del juego, se capturan la posición inicial y la escala inicial del objeto.
        startingPos = transform.position;
        initialScale = transform.localScale;
    }

    void Update()
    {
        // Se sigue el tiempo transcurrido.
        elapsedTime += Time.deltaTime;

        // Se calcula el porcentaje actual de tiempo transcurrido en relación con la duración total.
        float currentPercentage = elapsedTime / timeDuration;

        // Se utiliza Lerp para interpolar suavemente la posición inicial y final del objeto
        // basándose en la curva de animación 'curve'.
        transform.position = Vector3.Lerp(startingPos, new Vector3(finalPos.position.x, finalPos.position.y, 0), curve.Evaluate(currentPercentage));

        // Se utiliza Lerp para interpolar suavemente la escala inicial y final del objeto
        // basándose en la curva de animación 'curve2'.
        transform.localScale = Vector3.Lerp(initialScale, finalScale, curve2.Evaluate(currentPercentage));
    }
}