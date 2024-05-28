// Author - Ronnie Rawlings.

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ApplyOutline : MonoBehaviour
{
    // Outline width.
    [SerializeField] private float width;

    /// <summary> method <c>SetOutline</c> applies an outline to this objs text. </summary>
    public void SetOutline()
    {
        GetComponent<TextMeshProUGUI>().outlineWidth = width;
    }

    // Called once when active & enabled.
    void OnEnable()
    {
        // Applies an outline to text.
        SetOutline();
    }
}
