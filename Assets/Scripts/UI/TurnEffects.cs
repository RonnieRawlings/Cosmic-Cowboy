// Author - Ronnie Rawlings.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnEffects : MonoBehaviour
{
    // Red glow obj, signifes enemy turn.
    [SerializeField] private GameObject enemyGlowEffect;

    // Update is called once per frame
    void Update()
    {
        if (BattleInfo.aiTurn && !BattleInfo.playerTurn)
        {
            enemyGlowEffect.SetActive(true);
        }
        else
        {
            enemyGlowEffect.SetActive(false);
        }
    }
}
