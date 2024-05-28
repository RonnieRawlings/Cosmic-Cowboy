// Author - Ronnie Rawlings.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerLink : MonoBehaviour
{
    /// <summary> method <c>OpenLink</c> opens a given link in the default web browser. </summary>
    public void OpenLink(string link)
    {
        Application.OpenURL(link);
    }
}
