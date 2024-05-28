// Author - Ronnie Rawlings.

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ActionPointUI : MonoBehaviour
{
    /// <summary> method  <c>UpdateAPUI</c> displays the current player AP they have remaining for the turn. </summary>
    public void UpdateAPUI()
    {
        // Displays up-to-date AP total.
        transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = BattleInfo.currentActionPoints.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        // Displays current player AP.
        UpdateAPUI();
    }
}
