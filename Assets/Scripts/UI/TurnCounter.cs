// Author - Ronnie Rawlings.

using TMPro;
using UnityEngine;

public class TurnCounter : MonoBehaviour
{
    // Max turns & current turns.
    private int totalTurnsForLevel = 30, currentTurns = 30;

    // Turn counter text.
    private TextMeshProUGUI textComp;

    // For turn updates.
    private bool hasChanged = false;

    /// <summary> method <c>UpdateTurns</c> update counter each time a turn has passed. </summary>
    public void UpdateTurns()
    {
        // Player turn ended, AIs begun, update turns.
        if (!BattleInfo.playerTurn && BattleInfo.aiTurn && !hasChanged)
        {
            // Decrease turns, update text.
            currentTurns--;
            textComp.text = currentTurns + " TURNS LEFT";

            // Prevent multiple decreases.
            hasChanged = true;

            // Level is over if turn limit exceeded.
            if (currentTurns <= 0) { EndLevel(); }
        }
        else if (BattleInfo.playerTurn && !BattleInfo.aiTurn) { hasChanged = false; }        
    }

    /// <summary> method <c>EndLevel</c> when turn limit exceeded, end level via death. </summary>
    public void EndLevel()
    {
        // Kills player lmao.
        BattleInfo.currentPlayerHealth = 0;
    }

    // Called once before first update.
    void OnEnable()
    {
        // Get text comp from child.
        textComp = GetComponentInChildren<TextMeshProUGUI>();

        // Set starting turns.
        textComp.text = totalTurnsForLevel + " TURNS LEFT";
    }

    void Update()
    {
        // Keep turns updated.
        UpdateTurns();
    }
}
