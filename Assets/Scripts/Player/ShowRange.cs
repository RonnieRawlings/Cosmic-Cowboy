// Author - Ronnie Rawlings.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowRange : MonoBehaviour
{
    /// <summary> method <c>ShowRangeOnHover</c> allows ShowPlayerRange to be called in gridViusals. </summary>
    public void ShowRangeOnHover()
    {
        BattleInfo.showRange = true;
    }

    public void DisableRangeOnExit()
    {
        BattleInfo.showRange = false;
    }
}
