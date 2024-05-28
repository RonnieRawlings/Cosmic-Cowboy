using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreenTips : MonoBehaviour
{
    public Text tipText;
    public string[] tips;
    private int index;

    void Start()
    {
        if (tips.Length > 0)
        {
            index = Random.Range(0, tips.Length);
            StartCoroutine(ShowTips());
        }
    }

    IEnumerator ShowTips()
    {
        while (true)
        {
            tipText.text = tips[index];
            yield return new WaitForSeconds(5);

            index = (index + 1) % tips.Length;
        }
    }
}