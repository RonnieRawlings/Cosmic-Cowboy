// Author - Ronnie Rawlings.

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    /// <summary> method <c>UpdateHealthUI</c> updates the current health UI number with current HP. </summary>
    public void UpdateHealthUI()
    {
        // Prevent player health from displaying below 0.
        int currentHealth = 0;
        if (BattleInfo.currentPlayerHealth >= 0) { currentHealth = BattleInfo.currentPlayerHealth; }

        // Keeps track of current player health.
        GetComponent<TextMeshProUGUI>().text = currentHealth.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        // Changes text UI of Player Health.
        UpdateHealthUI();
    }
}
