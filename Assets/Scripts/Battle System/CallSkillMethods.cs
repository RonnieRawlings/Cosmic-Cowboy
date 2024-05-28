// Author - Ronnie Rawlings.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CallSkillMethods : MonoBehaviour
{
    // Ref to currently selected class.
    [SerializeField] public string selectedClass;

    // Amount stat should change by.
    public int statChange; 

    public void ChangeStatChangeVar(int newStatChange)
    {
        statChange = newStatChange;
    }

    /// <summary> method <c>ChangeSelectedClass</c> allows player to change their mind before confirming class. </summary>
    public void ChangeSelectedClass(string className)
    {
        selectedClass = className;
    }

    /// <summary> method <c>SetClass</c> calls the static SetPlayerClass method, giving selectedClass as chosenClass. </summary>
    public void SetClass()
    {
        SkillsAndClasses.SetPlayerClass(selectedClass);
    }

    /// <summary> method <c>ChangePlayerStat</c> allows buttons to change the given stat by statChange amount, set before. </summary>
    public void ChangePlayerStat(string statName)
    {
        // Prevents point over spend.
        if (SkillsAndClasses.remainingPoints == 0 && statChange == 1) { return; }

        SkillsAndClasses.ApplyStatIncrease(statName, statChange);
    }
}
