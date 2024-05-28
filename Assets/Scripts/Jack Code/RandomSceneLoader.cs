using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RandomSceneLoader : MonoBehaviour
{
    public Button yourButton;
    public string[] scenes;

    void Start()
    {
        Button btn = yourButton.GetComponent<Button>();
        btn.onClick.AddListener(TaskOnClick);
    }

    void TaskOnClick()
    {
        int index = Random.Range(0, scenes.Length);
        SceneManager.LoadScene(scenes[index]);
    }
}