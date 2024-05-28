// Author - Ronnie Rawlings.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateActionBar : MonoBehaviour
{
    private bool firstTurn = true;

    private GameObject screenCamera;

    // Update is called once per frame
    void Update()
    {
        if ((!BattleInfo.playerTurn && BattleInfo.aiTurn) || BattleInfo.inAnimation)
        {
            GetComponent<Animator>().SetBool("moveUp", false);
            GetComponent<Animator>().SetBool("moveDown", true);

            firstTurn = false;
        }
        else if (!BattleInfo.aiTurn && BattleInfo.playerTurn && !firstTurn)
        {
            GetComponent<Animator>().SetBool("moveUp", true);
            GetComponent<Animator>().SetBool("moveDown", false);           
        }
    }
}
