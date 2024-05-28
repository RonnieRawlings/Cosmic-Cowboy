// Author - Ronnie Rawlings.

using Fungus;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotEffects : MonoBehaviour
{
    // Crit enemy obj.
    private GameObject hitEnemy;

    // Y offset for Y miss pos.
    [SerializeField] private float yOffset;

    private bool playerHit = false;

    #region Variable Properties

    public GameObject HitEnemy
    {
        set { hitEnemy = value; }
    }

    public bool PlayerHit
    {
        set { playerHit = value; }
    }

    #endregion

    /// <summary> method <c>MoveCanvasPositon</c> moves the canvas to be over where the hit enemy is in world space. </summary>
    public void MoveCanvasPosition()
    {
        // Get the Image child object
        RectTransform imageRectTransform = transform.Find("Image").GetComponent<RectTransform>();

        // Find screen pos of player or enemy, depending on turn.
        Vector2 screenPosition; 
        if (!playerHit)
        {
            screenPosition = Camera.main.WorldToScreenPoint(hitEnemy.transform.position);
        }
        else
        {
            screenPosition = Camera.main.WorldToScreenPoint(BattleInfo.player.transform.position);
        }
        
        // Move the image a bit to the left
        float offset = 50; // Change this value as needed
        screenPosition.x -= offset;
        screenPosition.y += yOffset;

        // Assign the converted position to the Image object
        imageRectTransform.position = screenPosition;
    }

    // Called once before start & onEnable.
    private void Awake()
    {
        // Sets canvas, disables itself.
        if (gameObject.name == "CritPrefab")
        {
            BattleInfo.critCanvas = this.gameObject;
        }
        else if (gameObject.name == "HitPrefab")
        {
            BattleInfo.hitCanvas = this.gameObject;
        }
        else if (gameObject.name == "MissPrefab")
        {
            BattleInfo.missCanvas = this.gameObject;
        }
        
        gameObject.SetActive(false);
    }

    // Called once when active & enabled.
    private void OnEnable()
    {
        // Moves image over hit enemy.
        MoveCanvasPosition();
    }
}
