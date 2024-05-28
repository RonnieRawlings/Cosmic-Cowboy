// Author - Ronnie Rawlings.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class VisualCooldown : MonoBehaviour
{
    /// <summary> method <c>UpdateCooldown</c> updates the visual attack cooldown, by given. </summary>
    public void UpdateCooldown()
    {
        // Get attack name, if use, & cooldown turns.
        string attackName = gameObject.name.Split(" ")[0];
        bool attackUsed;
        int attackCooldown;

        // Determine which attack is used, return if none.
        switch (attackName)
        {
            case "SteadyAim":
                attackUsed = BattleInfo.steadyAimUsed;
                attackCooldown = BattleInfo.steadyAimCooldown;
                break;
            case "QuickDraw":
                attackUsed = BattleInfo.quickDrawUsed;
                attackCooldown = BattleInfo.quickDrawCooldown;
                break;
            default:
                return;
        }

        // When attack used, set visual cooldown text.
        if (attackUsed)
        {
            if (attackCooldown > 0)
            {
                // Sets cooldown to text.
                GetComponent<TextMeshProUGUI>().text = attackCooldown.ToString();
            }
            else
            {
                // Removes cooldown.
                GetComponent<TextMeshProUGUI>().text = "";
            }
            
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdateCooldown();
    }
}
