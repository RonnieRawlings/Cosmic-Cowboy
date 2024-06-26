// Author - Ronnie Rawlings.

using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class UIEnemySelect : MonoBehaviour
{
    // Starting cam rot.
    private Quaternion initalRot;

    // X axis offset.
    private float xOffset = 1.5f, yOffset = 1, zOffset = 3f;

    /// <summary> method <c>CallPositionCamBehind</c> allows UI buttons to call the PositionCamBehind method. </summary>
    public void CallPositionCamBehind()
    {
        StartCoroutine(PositionCamBehind());
    }

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

            // Smoothly move the camera towards the desired position
            transform.position = Vector3.Lerp(transform.position, adjustedPos, Time.deltaTime * 2f);

            // Smoothly rotate the camera to face the same direction as the player
            float rotationSpeed = 2.0f;
            Quaternion targetRotation = Quaternion.LookRotation(BattleInfo.player.transform.forward, Vector3.up);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);

            // Satisfy return requirement
            yield return null;
        }

        // Set the duration.
        float duration = 2f;

        while (Vector3.Distance(transform.position, originalPos) >= 0.01f)
        {
            // Interpolate position and rotation
            transform.position = Vector3.Lerp(transform.position, originalPos, duration * Time.deltaTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, initalRot, duration * Time.deltaTime);

            // Yield to the next frame
            yield return null;
        }

        // Ensure final position and rotation match the target
        transform.position = originalPos;
        transform.rotation = initalRot;

        // Allow cam movement.
        BattleInfo.camBehind = false;
    }
}