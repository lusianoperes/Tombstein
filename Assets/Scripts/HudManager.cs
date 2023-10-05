using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;

public class HudManager : MonoBehaviour
{
    public Jugador Player;

    public GameObject RellenoVida;
    public GameObject TextoVida;
   
    void Start()
    {
        Player = FindObjectOfType<Jugador>();
        RellenoVida = GameObject.Find("RellenoVida");
        TextoVida = GameObject.Find("CantidadVida");

        ActualizarBarraDeVida(Player,RellenoVida,TextoVida);
    }

    void Update()
    {
        ActualizarBarraDeVida(Player,RellenoVida,TextoVida); //Esto no tendria que ejecutarse todo el tiempo, sino cuando el jugador se cura o recibe daño
    }

    public void ActualizarBarraDeVida(Jugador player, GameObject rellenovida, GameObject TextoVida)
    {
        float maxWidth = 288.0633f;

        TextoVida.GetComponent<TextMeshProUGUI>().text = player.currentHp + " / " + player.maxHp;

        rellenovida.GetComponent<RectTransform>().sizeDelta = new Vector2(player.currentHp * maxWidth / player.maxHp, rellenovida.GetComponent<RectTransform>().sizeDelta.y);

    }
}
