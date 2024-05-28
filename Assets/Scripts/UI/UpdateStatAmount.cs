// Author - Ronnie Rawlings.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdateStatAmount : MonoBehaviour
{
    /// <summary> method <c>ChangeStatText</c> updates the text to match the current stat. </summary>
    public void ChangeStatText(int statChange)
    {
        if ((SkillsAndClasses.remainingPoints == 0 && statChange == 1) || (int.Parse(GetComponent<Text>().text) + statChange) < 50) { return; }

        // Updates text with correct stat number.
        GetComponent<Text>().text = (int.Parse(GetComponent<Text>().text) + statChange).ToString();

        // Consumes a stat point.
        SkillsAndClasses.remainingPoints -= statChange;
    }

    // Called on script instance load.
    void Awake()
    {
        // Set starting value as player's skill value.
        string skillName = transform.parent.name;
        GetComponent<Text>().text = SkillsAndClasses.playerStats[skillName].ToString();
    }
}
