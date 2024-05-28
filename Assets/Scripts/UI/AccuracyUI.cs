// Author - Ronnie Rawlings.

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AccuracyUI : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        GetComponent<TextMeshProUGUI>().text = BattleValues.playerAcc + "%";
    }
}
