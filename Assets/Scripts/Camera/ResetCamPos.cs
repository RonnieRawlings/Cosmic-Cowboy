// Author - Ronnie Rawlings.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetCamPos : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        // When in an animation, reset parent cam pos.
        if ((BattleInfo.inAnimation && !BattleInfo.fungusPlaying) || BattleInfo.fungusOverride)
        {
            // Instant move if fungusOverride.
            if (BattleInfo.fungusOverride) 
            {  
                transform.rotation = Quaternion.Euler(0, 0, 0);
                GetComponentInChildren<Animator>().applyRootMotion = false;
            }
            transform.position = Vector3.zero;
        }
    }
}
