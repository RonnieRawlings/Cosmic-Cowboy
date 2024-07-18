// Author - Ronnie Rawlings.

using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class OverlappingCam : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Fade out object if not player.
        if (other.gameObject != BattleInfo.player) 
        {
            other.gameObject.GetComponent<ObjectFade>().DoFade = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Fade object back in if not player.
        if (other.gameObject != BattleInfo.player)
        {
            other.gameObject.GetComponent<ObjectFade>().DoFade = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
