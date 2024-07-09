// Author - Ronnie Rawlings.

using System.Collections;
using UnityEngine;

public class UIEnemySelect : MonoBehaviour
{
    // Starting cam rot.
    private Quaternion initalRot;

    // X axis offset.
    private float xOffset = 1.5f, yOffset = 1, zOffset = 3f;

    /// <summary> method <c>PositionCamBehind</c> position the camera behind the player on UI enemy click. </summary>
    public IEnumerator PositionCamBehind()
    {
        // Prevent multiple routines.
        if (BattleInfo.camBehind) { yield break; }

        // Prevent other cam movement.
        BattleInfo.camBehind = true;

        // Access start pos/rot.
        Vector3 originalPos = transform.position;
        initalRot = transform.rotation;

        while (BattleInfo.playerTurn && !InputManager.playerControls.Basic.Escape.WasPressedThisFrame())
        {
            // Calculate the desired camera position
            Vector3 desiredPosition = BattleInfo.player.transform.position - BattleInfo.player.transform.forward * zOffset;

            // Adjust Y height and apply X offset relative to the player's right vector
            Vector3 adjustedPos = desiredPosition + BattleInfo.player.transform.right * xOffset;
            adjustedPos.y += yOffset;

            if (Vector3.Distance(transform.position, adjustedPos) < 0.1f) { BattleInfo.camTransitioning = false; }
            else { BattleInfo.camTransitioning = true; }

            // Smoothly move the camera towards the desired position
            transform.position = Vector3.Lerp(transform.position, adjustedPos, 0.01f);

            // Smoothly rotate the camera to face the same direction as the player
            Quaternion targetRotation = Quaternion.LookRotation(BattleInfo.player.transform.forward, Vector3.up);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 0.01f);

            // Satisfy return requirement
            yield return null;
        }

        // Reset selected enemy.
        previousSelected = null;
        BattleInfo.currentSelectedEnemy = null;
        
        while (Vector3.Distance(transform.position, originalPos) >= 0.01f)
        {
            // Interpolate position and rotation
            transform.position = Vector3.Lerp(transform.position, originalPos, 0.025f);
            transform.rotation = Quaternion.Lerp(transform.rotation, initalRot, 0.025f);

            // Yield to the next frame
            yield return null;
        }

        // Ensure final position and rotation match the target
        transform.position = originalPos;
        transform.rotation = initalRot;

        // Allow cam movement & player actions.
        BattleInfo.camBehind = false;
        BattleInfo.camTransitioning = false;
    }

    // Last selected enemy.
    private GameObject previousSelected; 

    private void Update()
    {
        // If enemy selected, enter enemySelect view.
        if (BattleInfo.currentSelectedEnemy != previousSelected)
        {
            // Position cam behind enemy & face them.
            StartCoroutine(PositionCamBehind());
            StartCoroutine(BattleInfo.player.GetComponent<PlayerMovement>().RotateTowardsEnemy
                (BattleInfo.currentSelectedEnemy));

            // Prevent multiple selections.
            previousSelected = BattleInfo.currentSelectedEnemy;
        }
    }
}