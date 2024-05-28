// Author - Ronnie Rawlings.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Fungus;

public class CheckPlayerStatus : MonoBehaviour
{
    // Current health of player.
    [SerializeField] int currentHealth;

    // Prevents multiple coroutine calls.
    private bool isDead = false;

    /// <summary> method <c>CheckStatus</c> used to check if the player still has health remaning. </summary>
    public void CheckStatus()
    {
        // Checks if the player has been defeated.
        if (BattleInfo.currentPlayerHealth != currentHealth)
        {
            // Updates currentHealth, flashes colour.
            currentHealth = BattleInfo.currentPlayerHealth;
        }
        else if (BattleInfo.currentPlayerHealth <= 0 && !isDead)
        {
            // Plays death anim.
            GetComponent<Animator>().SetBool("isDead", true);
            
            // Restarts the level.
            StartCoroutine(RestartLevel());

            // Prevents more calls.
            isDead = true;
            BattleInfo.playerDiedThisLoad = true;
        }
    }

    /// <summary> interface <c>RestartLevel</c> waits a specified amount of time then restarts the level. </summary>
    public IEnumerator RestartLevel()
    {
        // Gets death audio, prevents movement.
        AudioClip deathSFX = GetComponent<PlayerMovement>().playerAnimationSFX[1];
        GetComponent<PlayerMovement>().enabled = false;

        // Waits, plays death SFX.
        yield return new WaitForSeconds(2.0f);
        GetComponent<AudioSource>().PlayOneShot(deathSFX);

        // Fades the screen to black.
        yield return new WaitForSeconds(3.0f);
        GameObject.Find("Flowchart").GetComponent<Flowchart>().SendFungusMessage("FadeOut");

        // Waits, restarts current level.
        yield return new WaitForSeconds(2.0f);
        BattleInfo.ResetLevelInfo();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Called once on script initlization.
    void OnEnable()
    {
        // Sets Player's current health to base.
        currentHealth = BattleValues.basePlayerHealth;
    }

    // Update is called once per frame
    void Update()
    {
        CheckStatus();
    }
}
