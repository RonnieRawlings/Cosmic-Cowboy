// Author - Ronnie Rawlings.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FungusGradient : MonoBehaviour
{
    // Fungus gradient panel obj + image comp.
    [SerializeField] private GameObject fungusGradient;
    private Image gradientImage;

    // Has the setting been changed, is coroutine already running.
    private bool settingChange = false;
    private bool isCoroutineRunning = false;

    /// <summary> method <c>CheckForFungus</c> applies the graident effect if Fungus is active, reverts when not. </summary>
    public void CheckForFungus()
    {
        // When Fungus obj is found, enable gradient.
        if ((GameObject.Find("SayDialog") || GameObject.Find("MenuDialog")) && !settingChange && !isCoroutineRunning)
        {
            settingChange = true;
            StartCoroutine(ApplyChange());
        }
        else if (settingChange && !GameObject.Find("SayDialog") && !GameObject.Find("MenuDialog") && !isCoroutineRunning)
        {
            settingChange = false;
            StartCoroutine(RevertChange());
        }
    }

    /// <summary> coroutine <c>ApplyChange</c> fades in the graident UI panel in for when Fungus is playing. </summary>
    public IEnumerator ApplyChange()
    {
        isCoroutineRunning = true;

        float elapsedTime = 0;
        float duration = 0.8f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(0, 125f / 255f, elapsedTime / duration);
            gradientImage.color = new Color(gradientImage.color.r, gradientImage.color.g, gradientImage.color.b, alpha);
            yield return null;
        }

        isCoroutineRunning = false;
    }

    /// <summary> coroutine <c>RevertChange</c> fades out the gradient panel in for when Fungus has stopped. </summary>
    public IEnumerator RevertChange()
    {
        isCoroutineRunning = true;

        float elapsedTime = 0;
        float duration = 0.8f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(125f / 255f, 0, elapsedTime / duration);
            gradientImage.color = new Color(gradientImage.color.r, gradientImage.color.g, gradientImage.color.b, alpha);
            yield return null;
        }

        isCoroutineRunning = false;
    }

    // Called once before first update. 
    private void Start()
    {
        gradientImage = fungusGradient.GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckForFungus();
    }
}