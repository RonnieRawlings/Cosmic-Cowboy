// Author - Ronnie Rawlings.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AiInitlization : MonoBehaviour
{
    // Set to true once first script instance runs start.
    public static bool isSceneStarted = false;

    /// <summary> method <c>IncludeInBattle</c> adds this AI to relevant Dictionaries in BattleInfo. </summary>
    public void IncludeInBattle()
    {
        // Adds self to levelEnemies dict.
        if (BattleInfo.levelEnemies != null)
        {
            // Checks if this level is a reload.
            if (BattleInfo.levelEnemies.Count == 0)
            {
                // Sets up turn changing, adds self to dict.
                BattleInfo.levelEnemyTurns.Add(gameObject.name, false);
                BattleInfo.levelEnemies.Add(this.gameObject, GetComponentInChildren<EnemyStats>().Health);
                BattleInfo.levelEnemiesList.Add(gameObject.name);

                // Creates ref to enemy stats.
                BattleInfo.levelEnemyStats.Add(this.gameObject, GetComponentInChildren<EnemyStats>());
            }
            else
            {
                // Clears previous level load stuff.
                BattleInfo.levelEnemyTurns.Remove(gameObject.name);
                BattleInfo.levelEnemies.Remove(gameObject);
                BattleInfo.levelEnemyStats.Remove(gameObject);
                BattleInfo.levelEnemiesList.Remove(gameObject.name);

                // Sets up turn changing, adds self to dict.
                BattleInfo.levelEnemyTurns.Add(gameObject.name, false);
                BattleInfo.levelEnemies.Add(this.gameObject, GetComponentInChildren<EnemyStats>().Health);
                BattleInfo.levelEnemiesList.Add(gameObject.name);

                // Creates ref to enemy stats.
                BattleInfo.levelEnemyStats.Add(this.gameObject, GetComponentInChildren<EnemyStats>());
            }          
        }
    }

    /// <summary> method <c>DetachPath</c> removes the path points from the player, enableling world space. </summary>
    public void DetachPath()
    {
        // List of children to remove from obj.
        List<Transform> childrenToDetach = new List<Transform>();

        // Removes path points from their parent (obj).
        foreach (Transform child in transform)
        {
            // Checks for matching names.
            if (child.name == "PathStart" || child.name == "PathEnd")
            {
                childrenToDetach.Add(child);
            }
        }

        // Detaches all children from parent.
        foreach (Transform child in childrenToDetach)
        {
            child.parent = null;
        }
    }

    // Called once on script initlization.
    void OnEnable()
    {
        // Tracks scene loading.
        SceneManager.sceneLoaded += OnSceneLoaded;

        // Sets up enemy in BattleInfo.
        IncludeInBattle();

        // Detaches path from obj.
        DetachPath();

        // Change idle anim speed slighlty.
        Animator anim = GetComponent<Animator>();
        anim.SetFloat("idleSpeed", Random.Range(0.7f, 1f));


        // Allows current status to be checked.
        this.GetComponent<CheckAIStatus>().enabled = true;
    }

    // Called once before first update.
    private void Start()
    {
        // Prevents calls on subsequent scripts.
        if (isSceneStarted) return;

        BattleInfo.closestEnemy = null;
        BattleInfo.currentSelectedEnemy = null;
        BattleInfo.hoveredEnemy = null;

        // Prevents anymore calls.
        isSceneStarted = true;
    }

    // Called once when inactive & disabled.
    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // New scene loaded, reset var.
        isSceneStarted = false;
    }
}