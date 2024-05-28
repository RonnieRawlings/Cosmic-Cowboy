// Author - Ronnie Rawlings.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UpdateStatPoints : MonoBehaviour
{
    void Awake()
    {
        // Adds extra points for new skill scene.
        if (SceneManager.GetActiveScene().name == "SkillSelect 2")
        {
            SkillsAndClasses.remainingPoints += 14;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Keeps remaining points up to date.
        GetComponent<Text>().text = SkillsAndClasses.remainingPoints.ToString();
    }
}
