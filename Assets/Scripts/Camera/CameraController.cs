// Author - Ronnie Rawlings.

using System.Collections;
using System.Collections.Generic;
using UnityEditor.XR.LegacyInputHelpers;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraController : MonoBehaviour
{
    // Decides currently active camera option.
    private bool cameraType;

    // Ref to zooming limits.
    [SerializeField] private float minZoom, maxZoom;

    // Determines rotation enabled state.
    [SerializeField] private bool preventedRotation = false, canRotate = true;

    [SerializeField] private Vector3 startPos, startRot;

    #region Properties

    public Vector3 StartPos
    {
        get { return startPos; }
    }

    public bool AlreadySet
    {
        set { alreadySet = value; }
    }

    #endregion

    // Prevents multiple settings.
    private bool alreadySet = false;

    /// <summary> method <c>ChangeCameraOption</c> switches between types of camera movement onKeyDown. </summary>
    public void ChangeCameraOption()
    {
        // Prevents rotation on AI turn begin.
        if (BattleInfo.aiTurn && !preventedRotation) { StartCoroutine(AllowRotation()); }

        // Allows rotation if not animating.
        if (!IsPlaying(GetComponent<Animator>()))
        {
            // Rotates playerCam around player, left/right.
            if (InputManager.playerControls.Camera.RotateLeft.WasPerformedThisFrame() && canRotate && !camAdjusting)
            {
                StartCoroutine(AdjustRotation("Q"));
            }
            else if (InputManager.playerControls.Camera.RotateRight.WasPerformedThisFrame() && canRotate && !camAdjusting)
            {
                StartCoroutine(AdjustRotation("E"));
            }
        }
        
        // Changes camera movement type on click.
        if (Input.GetKeyDown(KeyCode.F1) || BattleInfo.begunMovement || BattleInfo.aiTurn)
        {
            if (cameraType)
            {
                // Disables free-cam & locks camera to player.
                GetComponent<CameraPan>().enabled = false;
                GetComponent<CameraLock>().enabled = true;

                cameraType = false;

                // Prevents multiple changes.
                BattleInfo.begunMovement = false;
            }
            else
            {
                // Prevents cam from switching.
                if (BattleInfo.begunMovement || BattleInfo.aiTurn) { BattleInfo.begunMovement = false; return; }

                // Allows free-cam.
                GetComponent<CameraPan>().enabled = true;
                GetComponent<CameraLock>().enabled = false;

                cameraType = true;
            }
            
        }

        // Allows scripts to function when not animating.
        if ((!IsPlaying(GetComponent<Animator>()) || !GetComponent<Animator>().enabled) && !(GameObject.Find("SayDialog") || GameObject.Find("MenuDialog")))
        {
            // Allows player actions.
            BattleInfo.inAnimation = false;
            BattleInfo.fungusPlaying = false;

            // Enables root motion.
            GetComponent<Animator>().applyRootMotion = true;

            // Re-enables player movement.            
            BattleInfo.player.GetComponent<PlayerMovement>().enabled = true;                                  
        }
        else if (!IsPlaying(GetComponent<Animator>()) && (GameObject.Find("SayDialog") || GameObject.Find("MenuDialog")))
        {
            // Prevents player actions.
            BattleInfo.inAnimation = true;
            BattleInfo.fungusPlaying = true;

            // Enables root animation but prevents movement.
            GetComponent<Animator>().applyRootMotion = true;
            BattleInfo.player.GetComponent<PlayerMovement>().enabled = false;
        }
        else
        {
            // Prevents player actions.
            BattleInfo.inAnimation = true;
            BattleInfo.fungusPlaying = false;
            if (!alreadySet) { GetComponent<CameraLock>().shouldCallInstant = true; alreadySet = true; }

            // Enables root animation but prevents movement.
            GetComponent<Animator>().applyRootMotion = false;
            BattleInfo.player.GetComponent<PlayerMovement>().enabled = false;            
        }
    }

    /// <summary> coroutine <c>AllowRotation</c> prevents rotation from occuring when AIs turn, delays enabling by smoothTime (+10%). </summary>
    public IEnumerator AllowRotation()
    {
        // Prevents multiple calls & prevents rotation.
        preventedRotation = true;
        canRotate = false;

        Debug.Log("Here");

        // Prevents rotation until player turn again.
        while (BattleInfo.aiTurn)
        {
            yield return null;
        }

        // Waits out smoothTime.
        yield return new WaitForSeconds(1f);
        
        // Resets rotation vars.
        canRotate = true;
        preventedRotation = false;
    }

    private bool camAdjusting = false;

    /// <summary> method <c>AdjustRotation</c> allows cam perspective change, 90 degree rotation (Q & E). </summary>
    public IEnumerator AdjustRotation(string keyType)
    {
        camAdjusting = true;

        Quaternion targetRotation;
        if (keyType == "Q")
        {
            targetRotation = Quaternion.Euler(transform.parent.eulerAngles + new Vector3(0, 90, 0));
        }
        else // keyType == "E"
        {
            targetRotation = Quaternion.Euler(transform.parent.eulerAngles + new Vector3(0, -90, 0));
        }

        float yRotation = targetRotation.eulerAngles.y;
        yRotation = Mathf.Round(yRotation / 90) * 90;
        targetRotation = Quaternion.Euler(new Vector3(0, yRotation, 0));

        float t = 0;
        float rotationSpeed = 0.5f; // Adjust this value to control the speed of rotation
        while (Quaternion.Angle(transform.parent.rotation, targetRotation) > 0.001f)
        {
            t += Time.deltaTime * rotationSpeed;
            transform.parent.rotation = Quaternion.Lerp(transform.parent.rotation, targetRotation, t);
            yield return null;
        }

        camAdjusting = false;
    }


    /// <summary> method <c>CameraZoom</c> allows the player to zoom the camera in and out. Within set limits. </summary>
    public void CameraZoom(float zoomSpeed)
    {
        // Get the scroll wheel input.
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");

        // Calculate new pos, clamp to bounds.
        Vector3 newPosition = transform.position + transform.forward * scrollInput * (zoomSpeed * 8);
        float clampedY = Mathf.Clamp(newPosition.y, minZoom, maxZoom);

        // Prevents x/z movement when at bounds.
        if (clampedY == newPosition.y)
        {
            // Interpolates between current & final pos.
            Vector3 smoothPos = Vector3.Lerp(transform.position, newPosition, 1);

            // Actually updates pos.
            transform.position = smoothPos;
        }
    }

    /// <summary> method <c>IsPlaying</c> returns true if an animation is playing and false if not. </summary>
    public bool IsPlaying(Animator animator)
    {
        // Animator is not playing if disabled.
        if (!animator.enabled) { return false; }

        // Gets info on current anim state.
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        // Returns current animation status.
        return (stateInfo.normalizedTime < 1 && !animator.IsInTransition(0));
    }

    /// <summary> method <c>RotationReset</c> resets the camera's rotation to it's usual after opening anim has played. </summary>
    public IEnumerator RotationReset()
    {
        resetRotBegun = true;

        // When anim over, reset rot.
        while (IsPlaying(GetComponent<Animator>()))
        {
            yield return null;
        }

        transform.localPosition = startPos;
        transform.rotation = Quaternion.Euler(42, 44, 0);
    }

    private bool resetRotBegun = false;

    // Update is called once per frame
    void Update()
    {
        // Decides which cam type is active.
        ChangeCameraOption();

        // Only zoom if camera isn't animating.
        if (!IsPlaying(GetComponent<Animator>()))
        {
            CameraZoom(1);
        } 
        else if (!resetRotBegun)
        {
            StartCoroutine(RotationReset());
        }
    }
}
