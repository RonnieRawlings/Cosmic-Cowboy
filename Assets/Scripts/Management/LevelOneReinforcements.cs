// Author - Ronnie Rawlings.

using Fungus;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

public class LevelOneReinforcements : MonoBehaviour
{
    public void SetOveride()
    {
        BattleInfo.fungusOverride = true;
    }

    /// <summary> coroutine <c>SpawnReinforcements</c> enables enemey reincforcements after animation has passed. </summary>
    public IEnumerator SpawnReinforcements(float waitTime)
    {       
        yield return new WaitForSeconds(waitTime);

        // Enables each reinforcement enemy.
        foreach (Transform child in transform)
        {
            // Remove numbers from enemy name.
            string childName = Regex.Replace(child.name, @"\d", "");

            if (childName == "T.W.I Reinforcement ")
            {
                child.gameObject.SetActive(true);
            }
        }

        yield return new WaitForSeconds(1.2f);

        // Reset camera position to parent.
        Camera.main.GetComponent<Animator>().applyRootMotion = true;
        Camera.main.transform.position = Camera.main.GetComponent<CameraController>().StartPos;

        BattleInfo.inAnimation = false;
        BattleInfo.fungusOverride = false;      
    }
}
