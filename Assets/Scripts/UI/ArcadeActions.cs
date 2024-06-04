// Author - Ronnie Rawlings.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ArcadeActions : MonoBehaviour
{
    // Alt button in playerActions.
    [SerializeField] private GameObject originalButton, altButton, background;

    // Should show range.
    [SerializeField] private bool shouldShowRange;

    /// <summary> method <c>SwitchToAlt</c> switches the current obj to alt. </summary>
    public void SwitchActive(GameObject switchButton, GameObject disableButton, bool shouldChangeSelected, bool backgroundActive)
    {               
        switchButton.SetActive(true);
        background.SetActive(backgroundActive);
        if (shouldChangeSelected) { EventSystem.current.SetSelectedGameObject(switchButton); }      
        disableButton.SetActive(false);
    }

    public void ShowRange()
    {
        if (!shouldShowRange) 
        { 
            BattleInfo.showRange = false; 
            GridVisuals gv = BattleInfo.gridManager.GetComponent<GridVisuals>();
            gv.VisualizeGridWhenCreated(true);
            return;
        }

        BattleInfo.showRange = true;

        GridManager gm = BattleInfo.gridManager.GetComponent<GridManager>();

        StartCoroutine(BattleInfo.gridManager.GetComponent<GridVisuals>().ShowRange(gm.FindNodeFromWorldPoint(BattleInfo.player.transform.position, BattleInfo.currentPlayerGrid),
            BattleInfo.playerWeapon.range, BattleInfo.currentPlayerGrid));
    }

    public void Reset()
    {
        // Reset range.
        BattleInfo.showRange = false;
        GridVisuals gv = BattleInfo.gridManager.GetComponent<GridVisuals>();
        gv.VisualizeGridWhenCreated(true);

        // Reset UI.
        altButton.SetActive(false); 
        background.SetActive(false);
        originalButton.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        // Prevents this script from running when normal build.
        if (!InputManager.actionBarMode) { return; }

        if (BattleInfo.inAnimation || BattleInfo.fungusPlaying || BattleInfo.fungusOverride) { Reset(); }

        // Prevents overlapping buttons.
        if (altButton.activeInHierarchy && originalButton.activeInHierarchy) { originalButton.SetActive(false); }

        // When selected, switch to alt.
        if (EventSystem.current.currentSelectedGameObject == originalButton && altButton.activeSelf == false) 
        {
            SwitchActive(altButton, originalButton, true, true);
            ShowRange();
        }
        else if (EventSystem.current.currentSelectedGameObject != altButton && originalButton.activeSelf == false)
        {
            SwitchActive(originalButton, altButton, false, false);
        }
    }
}