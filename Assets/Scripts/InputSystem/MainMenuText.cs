// Author - Ronnie Rawlings.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MainMenuText : MonoBehaviour
{
    [SerializeField] private GameObject originalText, altText;

    void Update()
    {
        if (!InputManager.IsUsingController) { return; }

        if (EventSystem.current.currentSelectedGameObject == this.gameObject)
        {
            originalText.SetActive(false);
            altText.SetActive(true);
        }
        else
        {
            originalText.SetActive(true);
            altText.SetActive(false);
        }
    }
}
