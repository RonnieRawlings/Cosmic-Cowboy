// Author - Ronnie Rawlings.

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Management : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI victoryText;

    // Update is called once per frame
    void Update()
    {
        if (BattleInfo.levelEnemies.Count == 0)
        {
            victoryText.gameObject.SetActive(true);
        }
    }
}
