// Author - Ronnie Rawlings.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ActionHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        // Gets players hover data.
        PlayerHoverInfo hoverInfo = BattleInfo.player.GetComponent<PlayerHoverInfo>();

        // Enable hoverData, keep it active.
        hoverInfo.KeepCanvasActive = true;
        hoverInfo.EnableWithoutHover();
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        // Gets players hover data.
        PlayerHoverInfo hoverInfo = BattleInfo.player.GetComponent<PlayerHoverInfo>();

        // Enable hoverData, keep it active.
        hoverInfo.KeepCanvasActive = false;
        hoverInfo.EnableWithoutHover();
    }

    private void OnDisable()
    {
        // Gets players hover data.
        PlayerHoverInfo hoverInfo = BattleInfo.player.GetComponent<PlayerHoverInfo>();

        // Enable hoverData, keep it active.
        hoverInfo.KeepCanvasActive = false;
        hoverInfo.EnableWithoutHover();
    }
}
