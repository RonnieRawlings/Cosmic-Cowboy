// Author - Ronnie Rawlings.

using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerAmmo : MonoBehaviour
{
    /// <summary> method <c>CheckAmmo</c> Updates UI with current ammo. </summary>
    public void CheckAmmo()
    {
        // Keeps track of current ammo.
        int currentAmmo = 0;
        if (BattleInfo.currentAmmo >= 0) { currentAmmo = BattleInfo.currentAmmo; };
        GetComponent<TextMeshProUGUI>().text = currentAmmo.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        // Decides when Reload action is available.
        CheckAmmo();
    }
}
