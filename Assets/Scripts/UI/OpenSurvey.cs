// Author - Ronnie Rawlings.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenSurvey : MonoBehaviour
{
    public void OpenSurveryShort()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            Application.OpenURL("https://forms.gle/vySyHMXBGxPXmvgD8");
        }
    }

    // Update is called once per frame
    void Update()
    {
        OpenSurveryShort();
    }
}
