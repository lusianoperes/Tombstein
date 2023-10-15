using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;

public class HudManager : MonoBehaviour
{
    public Jugador Player;
    public PlayerController playercontrolerRef;
    public GameObject RellenoVida;
    public GameObject TextoVida;
    public GameObject rellenoDashes;
    public GameObject textCantDashes;
   
    void Start()
    {
        Player = FindObjectOfType<Jugador>();
        RellenoVida = GameObject.Find("RellenoVida");
        TextoVida = GameObject.Find("CantidadVida");
        ActualizarBarraDeVida(Player,RellenoVida,TextoVida);
        ActualizarDashes(playercontrolerRef,rellenoDashes,textCantDashes);
    }

    void Update()
    {
        ActualizarBarraDeVida(Player,RellenoVida,TextoVida); //Esto no tendria que ejecutarse todo el tiempo, sino cuando el jugador se cura o recibe daño
        ActualizarDashes(playercontrolerRef,rellenoDashes,textCantDashes);
    }

    public void ActualizarBarraDeVida(Jugador player, GameObject rellenovida, GameObject TextoVida)
    {
        float maxWidth = 288.0633f;

        TextoVida.GetComponent<TextMeshProUGUI>().text = player.currentHp + " / " + player.maxHp;

        rellenovida.GetComponent<RectTransform>().sizeDelta = new Vector2(player.currentHp * maxWidth / player.maxHp, rellenovida.GetComponent<RectTransform>().sizeDelta.y);

    }

    public void ActualizarDashes(PlayerController playercontroler, GameObject rellenoDashes, GameObject textCantDashes)
    {
        float maxWidth = 288.0633f;

        textCantDashes.GetComponent<TextMeshProUGUI>().text = "" + playercontroler.cantActualDashes;
        

        rellenoDashes.GetComponent<RectTransform>().sizeDelta = new Vector2(playercontroler.cantActualDashes * maxWidth / playercontroler.maxDashes, rellenoDashes.GetComponent<RectTransform>().sizeDelta.y);

    }
}
