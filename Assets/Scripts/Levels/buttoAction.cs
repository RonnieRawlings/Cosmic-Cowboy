using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class buttoAction : MonoBehaviour
{

    public void LoadLoading()
    {
        SceneManager.LoadScene(3);
    }

    public void LoadCredits()
    {
        SceneManager.LoadScene(2);
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene(1);
    }
    public void LoadStartMenu()
    {
        SceneManager.LoadScene(4);
    }

    public void LoadGallery()
    {
        SceneManager.LoadScene(5);
    }

    public void LoadLevel01()
    {
        SceneManager.LoadScene(6);
    }


    public void quitGame()
    {
        Application.Quit();
    }
}
