// Author - Ronnie Rawlings.

using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TurretAI : BaseAI
{
    // Current turns decison is made.
    private bool decisonMade = false;

    // Grid the AI is currently on.
    [SerializeField] private int currentGrid = 0;

    /// <summary> method <c>DecideAction</c> uses nodePath (distance to target) to decide relevant action. </summary>
    public void DecideAction()
    {
        // Exits if already run.
        if (decisonMade) { return; }

        // Finds best path to target.        
        float distanceToPlayer = Vector3.Distance(BattleInfo.player.transform.position, transform.position);
        Debug.Log(distanceToPlayer);

        // Decide which action to take.
        if (distanceToPlayer <= 10 && BattleInfo.currentPlayerGrid == currentGrid)
        {
            Debug.Log("WILL ATTACK PLAYER WHEN IMPLEMENTED.");

            // Update enemies in range.
            BattleInfo.checkRange.CheckForEnemies();

            // Ends this AIs turn.        
            BattleInfo.levelEnemyTurns[gameObject.name] = false;
            BattleInfo.hasLockedEnemy = false;
        }
        else
        {
            // Ends this AIs turn.        
            BattleInfo.levelEnemyTurns[gameObject.name] = false;
            BattleInfo.hasLockedEnemy = false;
        }
    }

    /// <summary> method <c>CheckIfTurnOver</c> checks weather all turns have been taken, if switches turn to player. </summary>
    public void CheckIfTurnOver()
    {
        // Loops over all AI turn values.
        foreach (KeyValuePair<string, bool> turn in BattleInfo.levelEnemyTurns)
        {
            // Exits if turn still ongoing.
            if (turn.Value)
            {
                return;
            }
        }

        // Switches to player turn.
        BattleInfo.playerTurn = true;
        BattleInfo.aiTurn = false;

        // Resets player's AP.
        BattleInfo.currentActionPoints = 2;

        // Resets decison making var.
        decisonMade = false;
    }

    /// <summary> coroutine <c>CritEffect</c> allows crit effect to play, disables canvas again once finished. </summary>
    public IEnumerator CritEffect()
    {
        yield return new WaitForSeconds(0.3f);

        // Enables, sets hitEnemy.
        BattleInfo.critCanvas.GetComponent<ShotEffects>().PlayerHit = true;
        BattleInfo.critCanvas.SetActive(true);

        // Waits for anim to complete, disables again.
        yield return new WaitForSeconds(0.8f);
        BattleInfo.critCanvas.SetActive(false);
        BattleInfo.critCanvas.GetComponent<ShotEffects>().PlayerHit = false;
    }

    /// <summary> coroutine <c>HitEffect</c> allows hit effect to play, disables canvas again once finished. </summary>
    public IEnumerator HitEffect()
    {
        yield return new WaitForSeconds(0.3f);

        // Enables, sets hitEnemy.
        BattleInfo.hitCanvas.GetComponent<ShotEffects>().PlayerHit = true;
        BattleInfo.hitCanvas.SetActive(true);

        // Waits for anim to complete, disables again.
        yield return new WaitForSeconds(0.6f);
        BattleInfo.hitCanvas.SetActive(false);
        BattleInfo.hitCanvas.GetComponent<ShotEffects>().PlayerHit = false;
    }

    /// <summary> coroutine <c>MissEffect</c> allows miss effect to play, disables canvas again once finished. </summary>
    public IEnumerator MissEffect()
    {
        yield return new WaitForSeconds(0.3f);

        // Enables, sets hitEnemy.
        BattleInfo.missCanvas.GetComponent<ShotEffects>().PlayerHit = true;
        BattleInfo.missCanvas.SetActive(true);

        // Waits for anim to complete, disables again.
        yield return new WaitForSeconds(0.6f);
        BattleInfo.missCanvas.SetActive(false);
        BattleInfo.missCanvas.GetComponent<ShotEffects>().PlayerHit = false;
    }

    // Called once every frame.
    private void Update()
    {
        // If the enemy is far away, turn is ended automatially.
        if (Vector3.Distance(transform.position, BattleInfo.player.transform.position) > 35 && BattleInfo.levelEnemyTurns[gameObject.name])
        {
            Debug.Log("TOO GAR");

            BattleInfo.levelEnemyTurns[gameObject.name] = false;
            BattleInfo.hasLockedEnemy = false;

            CheckIfTurnOver();
            return;
        }

        if (BattleInfo.currentPlayerGrid != currentGrid && BattleInfo.levelEnemyTurns[gameObject.name])
        {
            Debug.Log("Not on same grid");

            BattleInfo.levelEnemyTurns[gameObject.name] = false;
            BattleInfo.hasLockedEnemy = false;

            CheckIfTurnOver();
            return;
        }

        // Finds enemy index.
        int currentIndex = BattleInfo.levelEnemiesList.IndexOf(gameObject.name);

        // Have previous enemies finished their turns.
        bool allPreviousFinished = true;
        for (int i = 0; i < currentIndex; i++)
        {
            if (BattleInfo.levelEnemyTurns[BattleInfo.levelEnemiesList[i]])
            {
                allPreviousFinished = false;
                break;
            }
        }

        // Begins AI turn if aiTurn & all enemies before finished.
        if (allPreviousFinished)
        {
            if (BattleInfo.levelEnemyTurns[gameObject.name] && !BattleInfo.playerTurn && BattleInfo.hasLockedEnemy)
            {
                // Decides which action to take.
                DecideAction();
                decisonMade = true;
            }
            else if (!BattleInfo.levelEnemyTurns[gameObject.name] && !BattleInfo.playerTurn && BattleInfo.aiTurn)
            {
                // Check for turn over.
                CheckIfTurnOver();
            }
            else if (BattleInfo.playerTurn && !BattleInfo.aiTurn)
            {
                // Prevents AI becoming stuck.
                decisonMade = false;
            }
        }
    }
}
