// Author - Ronnie Rawlings.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class EnemiesInRange : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // Different enemy state sprites.
    [SerializeField] private Sprite unalerted, alerted;

    // Enemy assigned to this in range obj.
    private GameObject assignedEnemy;

    // Is the enemy in range of player.
    private bool inRange;

    // CameraLock script reference.
    private CameraLock camLock;

    // Flag to check if the mouse is over the UI obj.
    private bool isMouseOver = false;

    #region Properties

    public GameObject AssignedEnemy
    {
        set { assignedEnemy = value; }
    }

    #endregion

    /// <summary> method <c>LockOnToEnemy</c>While this is hovered, lock camera to enemy postion. </summary>
    public void LockOnToEnemy()
    {
        // Prevent player lock, lock to enemy.
        BattleInfo.lockOnEnemy = true;
        camLock.EnemyLockOn(assignedEnemy.transform);
    }

    /// <summary> method <c>UpdateSelectedEnemy</c> prevents selectedEnemy from becoming incorrect due to hover select. </summary>
    public void UpdateSelectedEnemy()
    {
        // Only assign enemy if not still transitioning.
        if (BattleInfo.camTransitioning) { return; }
        BattleInfo.currentSelectedEnemy = assignedEnemy;
    }
    
    /// <summary> method <c>SwitchSprite</c> switch the icons sprite depending on inRange value. </summary>
    public void SwitchSprite()
    {
        // Get image comp.
        Image iconImage = GetComponent<Image>();

        // Switch sprite accordingly.
        if (inRange) { iconImage.sprite = alerted; }
        else { iconImage.sprite = unalerted; }
    }   

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Mouse has entered obj, begin lockOn.
        isMouseOver = true;
        StartCoroutine(LockOnWhileMouseOver());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Finish lock on, mouse has exited.
        isMouseOver = false;
    }

    /// <summary> coroutine <c>LockOnWhileMouseOver</c> continually calls LockOnToEnemy() while hovering. </summary>
    private IEnumerator LockOnWhileMouseOver()
    {
        // Is mouse over the UI obj.
        while (isMouseOver)
        {
            if (BattleInfo.camBehind) { break; }

            LockOnToEnemy();
            yield return null;
        }

        // Return to player lock on.
        BattleInfo.lockOnEnemy = false;
    }

    // Icon displayed when enemy selected.
    private Image selectedIcon;

    // Called once on script initlisation.
    private void Awake()
    {
        // Sets ref to cam lock script.
        camLock = Camera.main.GetComponent<CameraLock>();

        // Gets ref to button.
        Button buttonRef = GetComponent<Button>();

        // Selected icon is last child image.
        selectedIcon = transform.GetChild(transform.childCount - 1).GetComponent<Image>();

        // Call camera behind method on UI icon click, rotate player, & set selected.
        buttonRef.onClick.AddListener(UpdateSelectedEnemy);
        buttonRef.onClick.AddListener(() => StartCoroutine(Camera.main.GetComponent
            <UIEnemySelect>().PositionCamBehind()));
        buttonRef.onClick.AddListener(() => StartCoroutine(BattleInfo.player.GetComponent
            <PlayerMovement>().RotateTowardsEnemy(assignedEnemy)));        
    }

    // Tracks last value.
    private bool lastValue = false;

    private void Update()
    {
        // Can't select enemy when cam is transitioning.
        if (BattleInfo.camTransitioning || !BattleInfo.playerTurn) { GetComponent<Button>().interactable = false; }
        else { GetComponent<Button>().interactable = true; }

        // Check if in detection range.
        inRange = assignedEnemy.GetComponent<EnemyHoverInfo>().InRange;

        // Switch sprite if inRange has changed.
        if (lastValue != inRange) { SwitchSprite(); }

        // Switch status of selected icon.
        if (BattleInfo.currentSelectedEnemy == assignedEnemy) { selectedIcon.enabled = true; }
        else { selectedIcon.enabled = false; }
    }
}