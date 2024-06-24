// Author - Ronnie Rawlings.

using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class BattleInfo
{
    #region Combat

    // Current level enemies, with health ref + has taken turn vars.
    public static Dictionary<GameObject, int> levelEnemies;
    public static SortedDictionary<string, bool> levelEnemyTurns;

    // All enemies in the level.
    public static List<string> levelEnemiesList;

    // Ref to all levelEnemies stats.
    public static Dictionary<GameObject, EnemyStats> levelEnemyStats;

    // Enemy in range check.
    public static CheckRange checkRange;

    // Should lock to enemy/is cam behind player.
    public static bool lockOnEnemy = false, camBehind = false;

    // Keeps track of who's turn it is.
    public static bool playerTurn = true, aiTurn = false;

    // Player turns AP.
    public static int currentActionPoints = 2;

    // Player health.
    public static int currentPlayerHealth;

    // Is the player currently in cover.
    public static bool playerInCover = false;

    // Current player weapon ammo.
    public static int currentAmmo;

    // Damage reduction when out of cover. 
    public static int damageReduction = 0;

    // Canvas housing the crit effect.
    public static GameObject critCanvas, hitCanvas, missCanvas;

    // Should turns be paused.
    public static bool pauseTurns = false;

    #endregion

    #region Cooldown Logic

    // Prevents SteadyAim/QuickDraw from being used.
    public static bool steadyAimUsed = false, quickDrawUsed = false;
    public static int steadyAimCooldown = 0, quickDrawCooldown;

    #endregion

    #region Enemy Selection

    // Ref to closest enemy to the player.
    public static GameObject closestEnemy;

    // Ref to enemy currently selected by the player.
    public static GameObject currentSelectedEnemy;

    public static GameObject hoveredEnemy;

    #endregion

    #region Bug Prevention

    // Is in animation
    public static bool inAnimation = false, fungusPlaying = false, fungusOverride = false;

    // True when the player first begins movement.
    public static bool begunMovement;

    // Should range be shown visually.
    public static bool showRange = false, showDetectionRange;

    // Ref to the player's current grid.
    public static int currentPlayerGrid = 0;

    // When true allows gridVisuals node ref to be mouse.
    public static bool gridMouseAllowed = false;

    // All nodes actually in the level.
    public static Dictionary<Node, GameObject> nodeObjects;

    // True when the game is paused.
    public static bool gamePaused = false;

    // Has the camera locked to enemy pos.
    public static bool hasLockedEnemy = false;

    // Did the player die this level load.
    public static bool playerDiedThisLoad = false;

    #endregion

    #region Scene Obj Refs

    // Ref to gridManager gameObject.
    public static GameObject gridManager;

    // Ref to the player obj.
    public static GameObject player;

    // Ref to player weapon values.
    public static WeaponValues playerWeapon;

    #endregion

    /// <summary> constructor <c>BattleInfo</c> sets up level values before first level start. </summary>
    static BattleInfo()
    {       
        levelEnemies = new Dictionary<GameObject, int>();
        levelEnemiesList = new List<string>();
        levelEnemyTurns = new SortedDictionary<string, bool>();
        levelEnemyStats = new Dictionary<GameObject, EnemyStats>();

        nodeObjects = new Dictionary<Node, GameObject>();
    }

    /// <summary> static method <c>CalculateAttackChance</c> uses percentage to determine if attack has landed. </summary>
    public static bool CalculateAttackChance(int percentage)
    {
        // Generates num between 0, 99. 
        int randNum = Random.Range(0, 100);

        // If randNum is less than percentage attack is successful.
        return randNum < percentage;
    }

    /// <summary> static method <c>ApplyCritMultiplyer</c> increases the given value by the specified percentage. </summary>
    public static int ApplyCritMultiplyer(int currentDamage, int multiplyerPercentage)
    {
        // Calculates the increased damage.
        int increasedDamage = Mathf.RoundToInt(currentDamage + (currentDamage * 
            (multiplyerPercentage / 100.0f)));

        // Returns the new damage value.
        return increasedDamage;
    }

    /// <summary> static method <c>CalculateDamage</c> uses given values to calculate base damage done to enemy/player. </summary>
    public static int CalculateDamage(int weaponDamage, int yourDefense, int enemyDefense)
    {
        return Mathf.RoundToInt((float)yourDefense / enemyDefense * weaponDamage);
    }

    /// <summary> static method <c>FindClosestEnemy</c> determines the closest enemy to the player, sets to closestEnemy var. </summary>
    public static void FindClosestEnemy(Transform player)
    {
        // Tracks current closest distance.
        float closestDistance = Mathf.Infinity;
         
        // Loop through all levelEnemies, find closest. Set it.
        foreach (GameObject enemy in levelEnemies.Keys)
        {
            // Calculate distance from player to enemy.
            float distanceToEnemy = Vector3.Distance(player.position, enemy.transform.position);

            // Update closestEnemy if distance is smaller.
            if (distanceToEnemy < closestDistance)
            {            
                closestEnemy = enemy;
                closestDistance = distanceToEnemy;
            }
        }
    }

    /// <summary> method <c>ResetLevelInfo</c> resets the levelEnemies data when new level loaded or level reset. </summary> 
    public static void ResetLevelInfo()
    {
        gamePaused = false;
        lockOnEnemy = false;
        camBehind = false;

        levelEnemiesList.Clear();
        levelEnemies.Clear();        
        levelEnemyTurns.Clear();
        levelEnemyStats.Clear();

        nodeObjects.Clear();

        checkRange = null;
    }
}