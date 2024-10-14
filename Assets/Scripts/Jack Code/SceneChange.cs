using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChange : MonoBehaviour
{
    public string sceneName;

    private void Update()
    {
        if (Input.anyKeyDown)
        {
            SceneManager.LoadScene(sceneName);
        }
    }


}
