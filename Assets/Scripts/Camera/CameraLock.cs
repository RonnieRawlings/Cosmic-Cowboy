// Author - Ronnie Rawlings.

using DG.Tweening.Core.Easing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLock : MonoBehaviour
{   
    // Camera smoothing speed.
    private float smoothSpeed = 0.05f;

    // Player transform ref.
    private Transform player;

    // Locks onto drop ship if true.
    private bool dropshipAnim = false;

    #region Variable Properties

    public float SmoothSpeed
    {
        get { return smoothSpeed; }
    }

    public bool DropshipAnim
    {
        set { dropshipAnim = value; }
    }

    #endregion

    /// <summary> method <c>PlayerLockOn</c> locks the camera to the player, applies offset, and follows player smoothly. </summary>
    public void PlayerLockOn()
    {
        if (shouldCallInstant) { InstantPlayerLockOn(); return; }

        // Player pos + offset, for final.
        Vector3 finalPos = player.position;

        // Interpolates between current & final pos.
        Vector3 smoothPos = Vector3.Lerp(transform.parent.position, finalPos, Time.deltaTime * 2f);

        // Actually updates pos.
        transform.parent.position = smoothPos;
    }

    public void InstantPlayerLockOn()
    {
        transform.parent.position = player.position;
        shouldCallInstant = false;
        GetComponent<CameraController>().AlreadySet = false;
    }

    /// <summary> method <c>EnemyLockOn</c> locks the camera to the given enemy, applies offset, and follows enemy. </summary>
    public void EnemyLockOn(Transform enemy)
    {
        // Player pos + offset, for final.
        Vector3 finalPos = enemy.position;

        // Interpolates between current & final pos.
        Vector3 smoothPos = Vector3.Lerp(transform.parent.position, finalPos, 0.025f);

        // Actually updates pos.
        transform.parent.position = smoothPos;

        // Rotate cam behind enemy, prevents player being out of shot.
        Quaternion finalRot = Quaternion.LookRotation(enemy.position - transform.parent.position);
        transform.parent.rotation = Quaternion.Lerp(transform.parent.rotation, finalRot, 0.025f);

        // Check if the camera has reached the enemy.
        if (Vector3.Distance(transform.parent.position, finalPos) < 0.1f && !BattleInfo.hasLockedEnemy)
        {
            BattleInfo.hasLockedEnemy = true;
        }
    }

    // Called once before first update.
    void Start()
    {
        // Sets player ref.
        player = BattleInfo.player.transform;
    }

    int currentIndex = 0;
    public bool shouldCallInstant = false;

    // Called once after other updates.
    void LateUpdate()
    {
        if (BattleInfo.playerDiedThisLoad) { BattleInfo.aiTurn = false; }

        if (BattleInfo.fungusOverride) { return; }

        // Followed obj is decided by current turn.
        if (!BattleInfo.aiTurn && (!BattleInfo.inAnimation || BattleInfo.fungusPlaying))
        {
            // Resets enemy index.
            currentIndex = 0;

            // Forces camera to follow player.
            if (!BattleInfo.lockOnEnemy) { PlayerLockOn(); }                    
        }
        else if (BattleInfo.aiTurn)
        {
            if (currentIndex >= BattleInfo.levelEnemiesList.Count) { currentIndex--; }

            if (!GameObject.Find("SayDialog") && !GameObject.Find("MenuDialog") && !BattleInfo.pauseTurns)
            {
                // Locks to each enemy doing an action.            
                if (BattleInfo.levelEnemyTurns[BattleInfo.levelEnemiesList[currentIndex]])
                {
                    EnemyLockOn(GameObject.Find("Enemies").transform.Find(BattleInfo.levelEnemiesList[currentIndex]));
                }
                else
                {
                    currentIndex++;
                }
            }                                         
        }       
    }   
}