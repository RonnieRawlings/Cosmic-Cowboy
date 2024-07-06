// Author - Jack/Ronnie.

using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    /// <summary> method <c>LoadScene</c> uses SceneManagement to load the given scene. </summary>
    /// <param name="sceneName">the given scenes name.</param>
    public void LoadScene(string sceneName)
    {
        // Loads the given scene.
        SceneManager.LoadScene(sceneName);

        // Resets any level leftovers.
        BattleInfo.ResetLevelInfo();
    }

    /// <summary> method <c>QuitGame</c> closes the application. Only functional in build. </summary>
    public void QuitGame()
    {
        Application.Quit();
    }
}