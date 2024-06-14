// Author - Ronnie Rawlings.

using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class EnemiesInRange : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // Enemy assigned to this in range obj.
    private GameObject assignedEnemy;

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

    // Called once on script initlisation.
    private void Awake()
    {
        // Sets ref to cam lock script.
        camLock = Camera.main.GetComponent<CameraLock>();
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
            LockOnToEnemy();
            yield return null;
        }

        // Return to player lock on.
        BattleInfo.lockOnEnemy = false;
    }
}