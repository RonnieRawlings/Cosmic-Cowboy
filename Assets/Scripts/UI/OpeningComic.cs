// Author - Ronnie Rawlings.

using Fungus;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class OpeningComic : MonoBehaviour
{
    [SerializeField] private Slider holdSlider;

    // Refs to objs that need enabling after comic.
    [SerializeField] List<GameObject> objsToEnable;

    [SerializeField] GameObject fungusFlow;

    public void CheckForAnimationEnd()
    {
        if (!IsAnimating(GetComponent<Animator>()))
        {
            StartCoroutine(EnableLevelStuff());
        }
    }

    private bool isSkipping = false, levelStuffEnabled = false;

    public void SkipComic()
    {
        var action = InputManager.playerControls.Basic.Skip;
        if (InputManager.isArcade) { action = InputManager.playerControls.Basic.ArcadeSkip; }

        if (action.IsPressed() && !isSkipping)
        {
            StartCoroutine(WaitForSkip(action));
        }
    }

    /// <summary> coroutine <c>WaitForSkip</c> tracks how long key has been held, skips comic when value met. </summary>
    private IEnumerator WaitForSkip(InputAction action)
    {
        // Prevents held from happening once finished.
        if (levelStuffEnabled) { yield break; }

        // Prevents multiple skips, sets current held.
        isSkipping = true;
        float holdTime = holdSlider.value * 2f;

        // Updates held time while key is held.
        while (action.IsPressed() && holdTime < 2f)
        {
            holdTime += Time.deltaTime;
            holdSlider.value = holdTime / 2f;
            yield return null;
        }

        // Skips comic if held complete.
        if (holdTime >= 2f)
        {
            StartCoroutine(EnableLevelStuff());
            levelStuffEnabled = true;
        }
        else
        {
            // Decrease the hold time along with the slider.
            while (!action.IsPressed() && holdSlider.value > 0)
            {
                holdSlider.value -= Time.deltaTime / 2f;
                yield return null;
            }
        }

        // Allows another routine to begin.
        isSkipping = false;
    }

    /// <summary> coroutine <c>EnableLevelStuff</c> disables comicUI & enables objs in objsToEnable list. </summary>
    public IEnumerator EnableLevelStuff()
    {
        // Fades screen.
        fungusFlow.SetActive(true);
        fungusFlow.GetComponent<Flowchart>().SetBooleanVariable("doFade", true);
        yield return new WaitForSeconds(1f);

        // Changes active status of all list objs.
        foreach (GameObject obj in objsToEnable)
        {
            if (obj.name == "ComicUI") { obj.SetActive(false); continue; }
            obj.SetActive(true);
        }

        // Enables grid visuals, destroies current obj.
        BattleInfo.gridManager.GetComponent<GridVisuals>().enabled = true;
        objsToEnable[3].GetComponentInChildren<Animator>().Play("Camera1");
        Destroy(this.gameObject);
    }

    public IEnumerator SkipImmediatly()
    {
        // Prevents multiple calls, disables comic audio
        BattleInfo.playerDiedThisLoad = false;
        GetComponent<AudioSource>().enabled = false;

        // Removes tutorial call.
        Destroy(objsToEnable[1].transform.GetChild(0).gameObject);

        // Enables flowchart, calls block to remove tutorial remnants.
        fungusFlow.SetActive(true);
        fungusFlow.GetComponent<Flowchart>().SendFungusMessage("RemoveTutBlocks");

        // Changes active status of all list objs.
        foreach (GameObject obj in objsToEnable)
        {
            if (obj.name == "ComicUI") { obj.SetActive(false); continue; }
            obj.SetActive(true);
        }

        // Waits for fade to complete & grid to create.
        yield return new WaitForSeconds(1f);

        // Enables grid visuals, destroies current obj.
        objsToEnable[3].GetComponentInChildren<CameraLock>().InstantPlayerLockOn();
        Destroy(this.gameObject);
    }

    public bool IsAnimating(Animator animator)
    {
        // Gets info on current anim state.
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        // Returns current animation status.
        return (stateInfo.normalizedTime < 1 && !animator.IsInTransition(0));
    }

    private void Awake()
    {
        if (BattleInfo.playerDiedThisLoad) { StartCoroutine(SkipImmediatly()); }
    }

    private void Update()
    {
        CheckForAnimationEnd();
        SkipComic();       
    }
}
