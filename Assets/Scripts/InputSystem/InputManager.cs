// Author - Ronnie Rawlings.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    // Allows access to all controls.
    public static PlayerControls playerControls;
    public static bool IsUsingController { get; private set; }

    public static bool actionBarMode;

    InputManager()
    {
        // Starting joystick mode.
        actionBarMode = false;
    }

    private void Awake()
    {
        // Sets up player controls.
        playerControls = new PlayerControls();
        playerControls.Enable();
    }

    void Update()
    {
        // Check for mouse movement or left click
        if (Input.GetAxisRaw("Mouse X") != 0 || Input.GetAxisRaw("Mouse Y") != 0 || Input.GetMouseButton(0))
        {
            IsUsingController = false;
        }
        // Check for any controller button press
        else if (Input.GetKey(KeyCode.JoystickButton0) || Input.GetKey(KeyCode.JoystickButton1) || Mathf.Abs(Input.GetAxis("Horizontal")) > 0.2f 
            || Mathf.Abs(Input.GetAxis("Vertical")) > 0.2f)
        {
            IsUsingController = true;
        }
    }

    void OnDisable()
    {
        playerControls.Disable();
    }
}