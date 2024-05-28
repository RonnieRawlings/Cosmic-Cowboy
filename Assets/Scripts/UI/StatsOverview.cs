// Author - Ronnie Rawlings.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatsOverview : MonoBehaviour
{
    // Ref to script containing current selected class.
    [SerializeField] private CallSkillMethods selectedClass;

    // Ref to each skill number in overview.
    [SerializeField] private List<Text> skillNumbers = new List<Text>();

    /// <summary> method <c>SetOverview</c> used to keep the overview list of stats up-to-date. </summary>
    public void SetOverview()
    {
        // Creates updatedStats list with class stats included.
        Dictionary<string, int> updatedStats = SkillsAndClasses.ReturnPlayerClassValues(selectedClass.selectedClass);

        // Loops through each skill number in overview.
        foreach (Text childText in skillNumbers)
        {
            childText.text = updatedStats[childText.transform.parent.name].ToString();
        }
    }

    // Called once when active & enabled. 
    void OnEnable()
    {
        SetOverview();
    }
}
