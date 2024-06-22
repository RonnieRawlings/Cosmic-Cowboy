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
    private float xOffset = -0.7f, yOffset = 1, zOffset = 2;

    /// <summary> method <c>PositionCamBehind</c> position the camera behind the player on UI enemy click. </summary>
    public void PositionCamBehind()
    {
        // Calculate the desired camera position
        Vector3 desiredPosition = BattleInfo.player.transform.position - BattleInfo.player.transform.forward;

        // Adjust Y height.
        Vector3 adjustedPos = new Vector3(desiredPosition.x + xOffset, desiredPosition.y + yOffset, desiredPosition.z + zOffset);

        // Smoothly move the camera towards the desired position
        transform.position = Vector3.Lerp(transform.position, adjustedPos, Time.deltaTime * 5f);

        // Make the camera look at the player
        transform.rotation = Quaternion.LookRotation(BattleInfo.player.transform.forward, Vector3.up);
    }

    void Awake()
    {
        // Save inital cam rot.
        initalRot = transform.rotation;
    }

    private void Update()
    {
        PositionCamBehind();
    }

    private void OnDisable()
    {
        // Reset rotation.
        transform.rotation = initalRot;
    }
}