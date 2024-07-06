// Author - Ronnie Rawlings.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchLoadingChar : MonoBehaviour
{
    /// <summary> method <c>ChooseRandChar</c> enables a random character for the loading screen. </summary>
    private void ChooseRandChar()
    {
        // Enables a rand child of obj (char).
        transform.GetChild(Random.Range(0, transform.childCount)).gameObject.SetActive(true);
    }

    void Awake()
    {
        ChooseRandChar();
    }
}
