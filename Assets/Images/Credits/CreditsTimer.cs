using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditsTimer : MonoBehaviour
{
    public float timer = 0f;
    public float delay = 90f;
    public string sceneName = "NextScene";

    void Update()
    {
        timer += Time.deltaTime;
        Debug.Log("Timer: " + timer); //
        if (timer >= delay)
        {
            SceneManager.LoadScene(sceneName);
        }
    }

}
