// Author - Ronnie Rawlings.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseTurns : MonoBehaviour
{
    public void HaltTurns()
    {
        Debug.Log("TurnHalted");
        BattleInfo.pauseTurns = true;
    }

    public void UnHaltTurns()
    {
        Debug.Log("Unhalted Turn");
        BattleInfo.pauseTurns = false;
    }
}
