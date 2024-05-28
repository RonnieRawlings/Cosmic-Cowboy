// Author - Ronnie Rawlings.

using Fungus;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkipOpening : MonoBehaviour
{
    // All to do lists.
    [SerializeField] private List<GameObject> toDestroy, toEnable, toDisable;
    
    // Fungus Flowchart & block name.
    [SerializeField] private Flowchart fungusToCall;
    [SerializeField] private string blockName;

    // Opening anim name.
    [SerializeField] private string animationName;

    /// <summary> coroutine <c>SkipOpeningAnim</c> skips the opening anim, disables/enables/destroies given gameObjects. </summary>
    public IEnumerator SkipOpeningAnim()
    {
        // Prevents more calls.
        alreadyCalled = true;
        BattleInfo.playerDiedThisLoad = false;

        // Destroies, disables, & enables given objs.
        foreach (GameObject n in toDestroy)
        {
            Destroy(n);
        }
        foreach (GameObject n in toDisable)
        {
            n.SetActive(false);
        }
        foreach (GameObject n in toEnable)
        {
            n.SetActive(true);
        }

        // Fades in the level & locks to player.
        fungusToCall.SendFungusMessage(blockName);
        yield return new WaitForSeconds(1f);
        Camera.main.GetComponent<CameraLock>().InstantPlayerLockOn();

        // Waits for fade in.
        yield return new WaitForSeconds(1f);
        
        // Removes this script.
        Destroy(this);
    }

    // Don't call anim twice.
    private bool alreadyCalled = false;

    /// <summary> method <c>PlayAnim</c> player didn't die this load, so play opening level anim. </summary>
    public void PlayAnim()
    {
        // Prevents multiple calls.
        alreadyCalled = true;

        // Plays the opening level anim.
        Camera.main.GetComponent<Animator>().Play(animationName);
        Destroy(this);
    }

    private void Awake()
    {
        // Skips anim if player died last load, plays anim otherwise.
        if (!alreadyCalled && BattleInfo.playerDiedThisLoad)
        {
            StartCoroutine(SkipOpeningAnim());
        }
        else if (!alreadyCalled && !BattleInfo.playerDiedThisLoad) { PlayAnim(); }
    }
}