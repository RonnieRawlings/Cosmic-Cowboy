using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    public void LoadMainMenu()
    {
        SceneManager.LoadScene(1);
    }

    public void LoadCredits()
    {
        SceneManager.LoadScene(2);
    }

    public void LoadGallery()
    {
        SceneManager.LoadScene(3);
    }

    public void LoadLevel01()
    {
        BattleInfo.ResetLevelInfo();
        SceneManager.LoadScene(4);
    }

    public void LoadLevel02()
    {
        BattleInfo.ResetLevelInfo();
        SceneManager.LoadScene(5);
    }

    public void SkillSelect()
    {
        BattleInfo.ResetLevelInfo();
        SceneManager.LoadScene(12);
    }

    public void ThankYou()
    {
        BattleInfo.ResetLevelInfo();
        SceneManager.LoadScene("ThankYou");
    }

    public void quitGame()
    {
        Application.Quit();
    }
}