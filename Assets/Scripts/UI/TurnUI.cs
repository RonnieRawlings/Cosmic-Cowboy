// Author - Ronnie Rawlings.

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TurnUI : MonoBehaviour
{
    /// <summary> method <c>ChangeTurnVisual</c> updates the current turn text visual to whatever the current turn is. </summary>
    public void ChangeTurnVisual()
    {
        if (BattleInfo.inAnimation)
        {
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(false);
            }

            return;
        }
        else
        {
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(true);
            }
        }

        if (BattleInfo.aiTurn && !BattleInfo.playerTurn)
        {
            GetComponentInChildren<TextMeshProUGUI>().text = "Enemy Turn";
            GetComponentInChildren<TextMeshProUGUI>().color = Color.red;
        }
        else if (!BattleInfo.aiTurn && BattleInfo.playerTurn)
        {
            GetComponentInChildren<TextMeshProUGUI>().text = "Player Turn";
            GetComponentInChildren<TextMeshProUGUI>().color = Color.green;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Updates the turn visual based on current turn.
        ChangeTurnVisual();
    }
}
