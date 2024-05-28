// Author - Ronnie Rawlings.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ActionBar : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private bool showRangeOnHover;

    [SerializeField] private Sprite normalSprite, altSprite;
    [SerializeField] private GameObject background;

    private bool hasEntered;

    public void OnPointerEnter(PointerEventData eventData)
    {
        hasEntered = true;

        // Only switch if action enabled.
        if (GetComponent<Image>().color.a == 1f)
        {
            GetComponent<Image>().sprite = altSprite;
            background.SetActive(true);
        }

        if (showRangeOnHover && GetComponent<Image>().color.a == 1f)
        {
            BattleInfo.showRange = true;

            GridManager gm = BattleInfo.gridManager.GetComponent<GridManager>();

            StartCoroutine(BattleInfo.gridManager.GetComponent<GridVisuals>().ShowRange(gm.FindNodeFromWorldPoint(BattleInfo.player.transform.position, BattleInfo.currentPlayerGrid), 
                BattleInfo.playerWeapon.range, BattleInfo.currentPlayerGrid));
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hasEntered = false;

        GetComponent<Image>().sprite = normalSprite;
        background.SetActive(false);

        if (showRangeOnHover)
        {
            BattleInfo.showRange = false;

            GridVisuals gv = BattleInfo.gridManager.GetComponent<GridVisuals>();
            gv.VisualizeGridWhenCreated(true);
        }
    }

    bool resetAfterDisable = false;

    private void Update()
    {
        // Disables info if disabled.
        if (GetComponent<Image>().color.a != 1f)
        {
            GetComponent<Image>().sprite = normalSprite;
            background.SetActive(false);

            if (hasEntered)
            {
                BattleInfo.showRange = false;

                GridManager gm = BattleInfo.gridManager.GetComponent<GridManager>();
                GridVisuals gv = BattleInfo.gridManager.GetComponent<GridVisuals>();

                StartCoroutine(gv.ShowRange(gm.FindNodeFromWorldPoint(BattleInfo.player.transform.position, BattleInfo.currentPlayerGrid),
                    BattleInfo.playerWeapon.range, BattleInfo.currentPlayerGrid));
            }

            resetAfterDisable = false;
        }
        
        if (hasEntered && GetComponent<Image>().color.a == 1f)
        {
            GetComponent<Image>().sprite = altSprite;
            background.SetActive(true);

            if (!resetAfterDisable && showRangeOnHover)
            {
                resetAfterDisable = true;
                BattleInfo.showRange = true;

                GridManager gm = BattleInfo.gridManager.GetComponent<GridManager>();
                GridVisuals gv = BattleInfo.gridManager.GetComponent<GridVisuals>();

                StartCoroutine(gv.ShowRange(gm.FindNodeFromWorldPoint(BattleInfo.player.transform.position, BattleInfo.currentPlayerGrid),
                    BattleInfo.playerWeapon.range, BattleInfo.currentPlayerGrid));
            }
            
        }
    }
}
