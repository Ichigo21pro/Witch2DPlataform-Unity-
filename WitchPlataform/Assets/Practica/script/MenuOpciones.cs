using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuOpciones : MonoBehaviour
{
   public void BotonPractica()
    { 

        SceneManager.LoadScene("Practica");
    }

    public void Salir()
    {
        Application.Quit();
    }
}
