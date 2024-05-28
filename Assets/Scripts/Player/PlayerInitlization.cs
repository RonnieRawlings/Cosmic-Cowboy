// Author - Ronnie Rawlings.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInitlization : MonoBehaviour
{
    /// <summary> method <c>ApplyStatBuffs</c> changes the player's base stats by their skill selection. </summary>
    public void ApplyStatBuffs()
    {
        // Adjusts health based on Endurance stat & it's related value.
        if (SkillsAndClasses.playerStats["Endurance"] > 50)
        {
            int healthGain = (SkillsAndClasses.playerStats["Endurance"] - 50) * SkillsAndClasses.statChanges["Endurance"];
            BattleInfo.currentPlayerHealth = BattleValues.basePlayerHealth + healthGain;
        } 
    }

    /// <summary> method <c>ApplyWeaponBuffs</c> applies stat changes to weapon every reload, seperate from other stats. </summary>
    public void ApplyWeaponBuffs()
    {
        // Adjusts player's weapon range, based on Awareness stat. 
        if (SkillsAndClasses.playerStats["Perception"] > 50)
        {
            GetComponentInChildren<WeaponValues>().range = GetComponentInChildren<WeaponValues>().range +
                (SkillsAndClasses.playerStats["Perception"] * SkillsAndClasses.statChanges["Perception"]);
        }
    }

    // Called once when active & enabled.
    void OnEnable()
    {
        // Sets level ref to player obj.
        BattleInfo.player = this.gameObject;

        // Sets health for this battle.
        BattleInfo.currentPlayerHealth = BattleValues.basePlayerHealth;

        // Sets beginning ammo for encounter.
        BattleInfo.currentAmmo = GetComponentInChildren<WeaponValues>().magSize;

        // Resets turns.
        BattleInfo.playerTurn = true;
        BattleInfo.aiTurn = false;

        // Reset attack cooldowns.
        BattleInfo.steadyAimUsed = false;
        BattleInfo.quickDrawUsed = false;
        BattleInfo.steadyAimCooldown = 0;

        // Resets selected enemy.
        BattleInfo.currentSelectedEnemy = null;

        // Resets starting player AP.
        BattleInfo.currentActionPoints = 2;

        // Resets current grid.
        BattleInfo.currentPlayerGrid = 0;

        BattleInfo.begunMovement = false;

        BattleInfo.showRange = false;
        BattleInfo.showDetectionRange = false;
        BattleInfo.gridMouseAllowed = false;

        // Change base stats based on skill points.
        ApplyStatBuffs();

        // Not applying these until actually finalised. //

        // Applies weapon stats seperate, need to be set every reload.
        //ApplyWeaponBuffs();
    }
}