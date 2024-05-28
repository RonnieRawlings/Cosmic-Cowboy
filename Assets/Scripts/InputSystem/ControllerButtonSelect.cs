// Author - Ronnie Rawlings.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ControllerButtonSelect : MonoBehaviour
{
    public GameObject activeMenu; // Holds the currently active menu.

    /// <summary> method <c>UpdateActiveMenu</c> Allows buttons to change the active menu var, prevents controller issues. </summary>
    public void UpdateActiveMenu(GameObject newActiveMenu)
    {
        activeMenu = newActiveMenu;
    }

    /// <summary> method <c>ChangeMenu</c> Changes which button starts selected on current menu. </summary>
    public void ChangeMenu(GameObject changeMenu)
    {
        EventSystem.current.SetSelectedGameObject(null); // Makes sure the EventSystem currently selected is empty.
        EventSystem.current.SetSelectedGameObject(changeMenu); // Sets currently selected button.
    }

    private void OnEnable()
    {
        ChangeMenu(activeMenu);
    }

    private bool lastCheck = false;

    private void Update()
    {
        if (activeMenu.name == "Shoot Button" && !InputManager.actionBarMode) 
        {
            EventSystem.current.SetSelectedGameObject(null);
            return; 
        }

        bool currentCheck;
        if (InputManager.isArcade) { currentCheck = InputManager.isArcade; }
        else { currentCheck = InputManager.IsUsingController; }
       
        if (currentCheck && !lastCheck)
        {
            UpdateActiveMenu(activeMenu);
        }

        if (EventSystem.current.currentSelectedGameObject == null)
        {
            ChangeMenu(activeMenu);
        }

        lastCheck = currentCheck; // Store the current state for the next frame
    }
}
