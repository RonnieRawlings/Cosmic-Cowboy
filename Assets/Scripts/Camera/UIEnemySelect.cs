// Author - Ronnie Rawlings.

using System.Collections;
using UnityEngine;

public class UIEnemySelect : MonoBehaviour
{
    // Starting cam rot.
    private Quaternion initalRot;

    // X axis offset.
    private float xOffset = 1.5f, yOffset = 1, zOffset = 3f;
    private float transitionDuration = 2f;

    /// <summary> method <c>PositionCamBehind</c> position the camera behind the player on UI enemy click. </summary>
    public IEnumerator PositionCamBehind()
    {
        // Prevent multiple routines.
        if (BattleInfo.camBehind) { yield break; }

        // Prevent other cam movement.
        BattleInfo.camBehind = true;

        // Access start pos/rot.
        Vector3 originalPos = transform.position;
        Quaternion initialRot = transform.rotation;

        float elapsedTime = 0f;
        Vector3 startPos = transform.position;
        Quaternion startRot = transform.rotation;

        while (BattleInfo.playerTurn && !InputManager.playerControls.Basic.Escape.WasPressedThisFrame())
        {
            BattleInfo.camTransitioning = true;
            elapsedTime += Time.deltaTime;

            // Calculate the desired camera position
            Vector3 desiredPosition = BattleInfo.player.transform.position - BattleInfo.player.transform.forward * zOffset;

            // Adjust Y height and apply X offset relative to the player's right vector
            Vector3 adjustedPos = desiredPosition + BattleInfo.player.transform.right * xOffset;
            adjustedPos.y += yOffset;

            if (elapsedTime < transitionDuration)
            {
                // Smoothly move the camera towards the desired position
                transform.position = Vector3.Lerp(startPos, adjustedPos, elapsedTime / transitionDuration);

                // Smoothly rotate the camera to face the same direction as the player
                Quaternion targetRotation = Quaternion.LookRotation(BattleInfo.player.transform.forward, Vector3.up);
                transform.rotation = Quaternion.Lerp(startRot, targetRotation, elapsedTime / transitionDuration);              
            }
            else
            {
                // Ensure final position and rotation match the target
                transform.position = adjustedPos;
                transform.rotation = Quaternion.LookRotation(BattleInfo.player.transform.forward, Vector3.up);

                BattleInfo.camTransitioning = false;
            }

            // Satisfy return requirement
            yield return null;
        }

        // Reset selected enemy.
        previousSelected = null;
        BattleInfo.currentSelectedEnemy = null;

        elapsedTime = 0f;
        startPos = transform.position;
        startRot = transform.rotation;

        while (elapsedTime < transitionDuration)
        {
            elapsedTime += Time.deltaTime;

            // Interpolate position and rotation back to the original
            transform.position = Vector3.Lerp(startPos, originalPos, elapsedTime / transitionDuration);
            transform.rotation = Quaternion.Lerp(startRot, initialRot, elapsedTime / transitionDuration);

            // Yield to the next frame
            yield return null;
        }

        // Ensure final position and rotation match the target
        transform.position = originalPos;
        transform.rotation = initialRot;

        // Allow cam movement & player actions.
        BattleInfo.camBehind = false;
        BattleInfo.camTransitioning = false;
    }

    // Last selected enemy.
    private GameObject previousSelected; 

    private void FixedUpdate()
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