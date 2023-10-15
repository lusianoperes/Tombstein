using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class menuPausaFunciones : MonoBehaviour
{
    [SerializeField] private GameObject menuPausa;
    [SerializeField] private GameObject menuOpcionesAudio;
    [SerializeField] private GameObject menuOpcionesGraficos;
    [SerializeField] private GameObject menuDeath;

    [SerializeField] private Slider musicaSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private TMP_Dropdown opcionesGraficos;

    [SerializeField] private Text valorMusica;
    [SerializeField] private Text valorSFX;
    private bool pausaActivada = false;
    private int volumenMusica;
    private int volumenSFX;
    private int elegirGraficos;



    private void Update(){
        if(Input.GetKeyDown(KeyCode.Escape)){
            if(pausaActivada){
                Reanudar();
            }
            else{
                Pausa();
            }
        }

        volumenMusica = (int)musicaSlider.value;
        volumenSFX = (int)sfxSlider.value;
        elegirGraficos = opcionesGraficos.value;

        valorMusica.text = volumenMusica.ToString();
        valorSFX.text = volumenSFX.ToString();
    } 

    public void Pausa(){
        pausaActivada = true;
        menuPausa.SetActive(true);
        Time.timeScale = 0f;
    }

    public void Reanudar(){
        pausaActivada = false;
        menuPausa.SetActive(false);
        Time.timeScale = 1f;
    }

    public void Opciones(){
        menuPausa.SetActive(false);
        menuOpcionesAudio.SetActive(true);
    }

    public void Graficos(){
        menuOpcionesAudio.SetActive(false);
        menuOpcionesGraficos.SetActive(true);
    }

    public void VolverMenuPausa(){
        menuOpcionesAudio.SetActive(false);
        menuPausa.SetActive(true);
    }

    public void VolverMenuAudio(){
        menuOpcionesGraficos.SetActive(false);
        menuOpcionesAudio.SetActive(true);
    }

    public void VolverMenuPrincipal(){
        menuPausa.SetActive(false);
        Time.timeScale = 1f;
        //guardar datos etc
        SceneManager.LoadScene("MENU");
    }

    public void DeathMenu(){
        menuDeath.SetActive(true);
        Time.timeScale = 0f;
    }

    public void Reiniciar(){
        menuPausa.SetActive(false);
        menuDeath.SetActive(false);
        Time.timeScale = 1f;
        //guardar datos etc
        SceneManager.LoadScene("Etapa1");
    }
}
