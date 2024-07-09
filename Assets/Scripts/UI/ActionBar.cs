// Author - Ronnie Rawlings.

using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ActionBar : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // Should this show range on hover, is it a melee attack ability.
    [SerializeField] private bool showRangeOnHover, isMelee;

    // Both sprite varitents, background info obj.
    [SerializeField] private Sprite normalSprite, altSprite;
    [SerializeField] private GameObject background;

    // Has the cursor entered the rect.
    private bool hasEntered;

    // Range of this ability in nodes.
    private int abilityRange;

    public void OnPointerEnter(PointerEventData eventData)
    {
        hasEntered = true;

        // Only switch if action enabled.
        if (GetComponent<Image>().color.a == 1f)
        {
            GetComponent<Image>().sprite = altSprite;
            background.SetActive(true);
        }

        // Only showcase range if ability enabled & allowed.
        if (showRangeOnHover && GetComponent<Image>().color.a == 1f)
        {
            // Don't show range on enemySelect.
            if (BattleInfo.camBehind) { return; }

            // Set range value.
            int abilityRange;
            if (isMelee) { abilityRange = 1; }
            else { abilityRange = BattleInfo.playerWeapon.range; }

            // Range is showing.
            BattleInfo.showRange = true;

            // Visually showcases ability range.
            GridManager gm = BattleInfo.gridManager.GetComponent<GridManager>();
            StartCoroutine(BattleInfo.gridManager.GetComponent<GridVisuals>().ShowRange(gm.FindNodeFromWorldPoint(BattleInfo.player.transform.position,                 
                BattleInfo.currentPlayerGrid), abilityRange, BattleInfo.currentPlayerGrid));
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hasEntered = false;

        GetComponent<Image>().sprite = normalSprite;
        background.SetActive(false);

        if (showRangeOnHover && BattleInfo.showRange)
        {
            BattleInfo.showRange = false;

            GridVisuals gv = BattleInfo.gridManager.GetComponent<GridVisuals>();
            gv.VisualizeGridWhenCreated(true);
        }
    }

    private void OnEnable()
    {
        // Sets ability range, uses weapon value if using weapon.
        if (isMelee) { abilityRange = 1; }
        else { abilityRange = BattleInfo.playerWeapon.range; }
    }

    bool resetAfterDisable = false;

    private void Update()
    {
        // Disables info if disabled.
        if (GetComponent<Image>().color.a != 1f)
        {
            GetComponent<Image>().sprite = normalSprite;
            background.SetActive(false);

            if (hasEntered && !BattleInfo.camBehind)
            {
                BattleInfo.showRange = false;

                GridManager gm = BattleInfo.gridManager.GetComponent<GridManager>();
                GridVisuals gv = BattleInfo.gridManager.GetComponent<GridVisuals>();

                StartCoroutine(gv.ShowRange(gm.FindNodeFromWorldPoint(BattleInfo.player.transform.position, 
                    BattleInfo.currentPlayerGrid), abilityRange, BattleInfo.currentPlayerGrid));
            }

            resetAfterDisable = false;
        }
        
        if (hasEntered && GetComponent<Image>().color.a == 1f)
        {
            GetComponent<Image>().sprite = altSprite;
            background.SetActive(true);

            if (!resetAfterDisable && showRangeOnHover)
            {
                if (BattleInfo.camBehind) { return; }
                resetAfterDisable = true;               
                BattleInfo.showRange = true;

                GridManager gm = BattleInfo.gridManager.GetComponent<GridManager>();
                GridVisuals gv = BattleInfo.gridManager.GetComponent<GridVisuals>();

                StartCoroutine(gv.ShowRange(gm.FindNodeFromWorldPoint(BattleInfo.player.transform.position, 
                    BattleInfo.currentPlayerGrid), abilityRange, BattleInfo.currentPlayerGrid));
            }
            
        }
    }
}