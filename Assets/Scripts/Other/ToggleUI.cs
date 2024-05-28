using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleUI : MonoBehaviour
{
    public GameObject uiElement; // Reference to your UI element

    private void Update()
    {
        if (InputManager.playerControls.Basic.Tab.WasPressedThisFrame() || InputManager.playerControls.Basic.Escape.WasPressedThisFrame() 
            && !GameObject.Find("SayDialog") && !GameObject.Find("MenuDialog"))
        {
            ToggleUIElement();
        }
    }

    void ToggleUIElement()
    {
        if (uiElement != null)
        {
            uiElement.SetActive(!uiElement.activeSelf);
            BattleInfo.gamePaused = uiElement.activeSelf;
        }
    }
}