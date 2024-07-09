// Author - Ronnie Rawlings.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.VFX;
using Random = UnityEngine.Random;

public class PlayerActions : MonoBehaviour
{
    [SerializeField] private GameObject gunshotVFX;

    // All player action UI objs.
    [SerializeField] private List<GameObject> playerActionObjs;

    // Ref to gird manager script.
    private GridManager gridManager;

    // Ref to player obj.
    private GameObject player;

    // Has used attack.
    private bool hasQuickDrawn;

    #region Sound FX

    // Player's attacking audio clips.
    [SerializeField] private AudioClip playerFire, playerReload, playerQuickDraw, endTurn;

    // Enemies hit audio clips.
    [SerializeField] private List<AudioClip> enemyHitSFX;

    public AudioClip EndTurnAudio
    {
        get { return endTurn; }
    }

    #endregion

    #region Variable Properties

    public bool HasQuickDrawn
    {
        set { hasQuickDrawn = value; }
    }

    #endregion

    /// <summary> method <c>UpdateButtonInteraction</c> changes the interaction status of the player actions. </summary>
    public void UpdateButtonInteraction()
    {
        // Define the conditions for enabling/disabling attack options.
        bool canAttack = BattleInfo.currentAmmo > 0 && !hasQuickDrawn && !BattleInfo.inAnimation && BattleInfo.playerTurn;

        // Define the conditions for enabling/disabling take cover.
        bool nearCover = gridManager.GetComponent<CoverSystem>().CheckForCover(player.transform);
        if (!nearCover) { BattleInfo.playerInCover = false; }

        // Define the conditions for enabling/disabling end turn.
        bool canEndTurn = BattleInfo.playerTurn && !BattleInfo.aiTurn && !BattleInfo.inAnimation;

        // Define the conditions for enabling/disabling reload.
        bool canReload = BattleInfo.currentAmmo < player.GetComponentInChildren<WeaponValues>().magSize && BattleInfo.playerTurn && 
            !BattleInfo.inAnimation;

        // Define the conditions for enabling/disabling Follow-Up-Shot.
        bool canFollowUpShot = BattleInfo.currentAmmo <= player.GetComponentInChildren<WeaponValues>().magSize && 
            BattleInfo.playerTurn && !BattleInfo.inAnimation && hasQuickDrawn;

        // No actions while in cam transition.
        if (BattleInfo.camTransitioning) { canAttack = false; canEndTurn = false; canReload = false; canFollowUpShot = false; }

        // Update attack options
        UpdateActionOptions(0, 3, canAttack);

        // Update Take Cover action
        UpdateActionOption(3, nearCover && !BattleInfo.playerInCover && BattleInfo.playerTurn && !BattleInfo.inAnimation);

        // Update End Turn action
        UpdateActionOption(4, canEndTurn);

        // Update Reload action
        UpdateActionOption(5, canReload);

        // Update Follow-Up-Shot action
        UpdateActionOption(6, canFollowUpShot);
    }

    /// <summary> method <c>UpdateActionOptions</c> calls UpdateActionOption for all given actions. </summary>
    private void UpdateActionOptions(int start, int end, bool condition)
    {
        // Calls an update action for each index given.
        for (int index = start; index < end; index++)
        {
            // Prevents steadyAim use for a turn.
            if (index == 1 && BattleInfo.steadyAimUsed)
            {
                UpdateActionOption(index, false); 
            }
            else
            {
                // Updates active status of aciton.
                UpdateActionOption(index, condition);
            }          
        }
    }

    /// <summary> method <c>UpdateActionOption</c> changes the active status of the given player action. </summary>
    private void UpdateActionOption(int index, bool condition)
    {
        // Changes action image transparency.
        Color originalA = playerActionObjs[index].GetComponent<Image>().color;
        originalA.a = condition ? 1.0f : 0.5f;
        playerActionObjs[index].GetComponent<Image>().color = originalA;

        playerActionObjs[index].GetComponent<Button>().enabled = condition;

        // Prevents multiple clicks on alt button.
        if (!condition)
        {
            // Prevent enabled issues.
            playerActionObjs[index].SetActive(true);
        }
    }

    /// <summary> method <c>BasicPlayerAttack</c> checks if an enemy is within range, if so returns found enemy Node. </summary>
    public GameObject BasicPlayerAttackLogic(int abilityRange)
    {
        // Finds neighbouring nodes to player, range based on attachted weapon        
        List<Node> neighbourNodes = gridManager.FindNeighbourNodes(gridManager.FindNodeFromWorldPoint(player.
            transform.position, BattleInfo.currentPlayerGrid), abilityRange, BattleInfo.currentPlayerGrid);

        // Finds either enemy in range or selected enemy.
        GameObject foundEnemy = null;
        if (BattleInfo.currentSelectedEnemy != null)
        {
            // Finds selected enemy, retrieves if in range.            
            foreach (Node node in neighbourNodes)
            {
                if (node.Occupied && node.Occupied.name == BattleInfo.currentSelectedEnemy.name && 
                    BattleInfo.levelEnemies.ContainsKey(node.Occupied))
                {
                    foundEnemy = node.Occupied;
                    break;
                }
            }
        }
        else
        {
            // Finds first occupied node, retrieves it.
            List<Node> distanceToPlayer = new List<Node>();
            foreach (Node node in neighbourNodes)
            {
                if (node.Occupied && BattleInfo.levelEnemies.ContainsKey(node.Occupied))
                {
                    // Finds path from node to player.
                    List<Node> thisNodeDistance = gridManager.GetComponent<Pathfinding>().FindPath(node.WorldPos, 
                        player.transform.position, BattleInfo.currentPlayerGrid);

                    // Compares current selected to node, if shorter change setting.
                    if (thisNodeDistance.Count < distanceToPlayer.Count || distanceToPlayer.Count == 0) 
                    { 
                        distanceToPlayer = thisNodeDistance;
                        foundEnemy = node.Occupied;
                    }
                }
            }
        }

        // Checks once more if no enemy was found.
        if (foundEnemy == null)
        {
            // Finds first occupied node, retrieves it.
            List<Node> distanceToPlayer = new List<Node>();
            foreach (Node node in neighbourNodes)
            {
                if (node.Occupied && BattleInfo.levelEnemies.ContainsKey(node.Occupied))
                {
                    // Finds path from node to player.
                    List<Node> thisNodeDistance = gridManager.GetComponent<Pathfinding>().FindPath(node.WorldPos,
                        player.transform.position, BattleInfo.currentPlayerGrid);

                    // Compares current selected to node, if shorter change setting.
                    if (thisNodeDistance.Count < distanceToPlayer.Count || distanceToPlayer.Count == 0)
                    {
                        distanceToPlayer = thisNodeDistance;
                        foundEnemy = node.Occupied;
                    }
                }
            }
        }

        // Removes player from cover if enemy found.
        if (foundEnemy != null)
        {
            GameObject.Find("Player").GetComponent<Animator>().SetBool("inCover", false);
            BattleInfo.playerInCover = false;
        }
           
        // Returns enemy found, null or not.
        return foundEnemy;
    }

    /// <summary> method <c>CallFire</c> allows UI buttons to call the Fire coroutine. </summary>
    public void CallFire()
    {
        StartCoroutine(Fire());
    }

    /// <summary> method <c>Fire</c> executes the player's most basic attack with no modifiyed stats. </summary>
    public IEnumerator Fire()
    {
        // Locates enemy, returns if none found.
        GameObject foundEnemy = BasicPlayerAttackLogic(BattleInfo.playerWeapon.range);
        if (foundEnemy == null) 
        {
            GameObject.Find("UICanvas").transform.Find("OutOfRange").gameObject.SetActive(true);
            yield break; 
        }

        // Ends playerTurn.
        BattleInfo.playerTurn = false;

        // Rotates player to face enemy, with adjust.
        yield return RotateToFaceEnemy(foundEnemy, true);

        // Plays PistolIdle anim.
        player.GetComponent<Animator>().SetBool("isFireing", true);
        yield return new WaitForSeconds(1.6f);

        // Player gunshot VFX.
        gunshotVFX.GetComponent<VisualEffect>().Play();

        // Plays shoot SFX.
        player.GetComponent<AudioSource>().PlayOneShot(playerFire); 

        // Calc dmg, apply dmg, play effects.
        yield return StartCoroutine(DamageCalcs(0, 0, foundEnemy));

        // Reset closestEnemy.
        BattleInfo.closestEnemy = null;

        // Clears action points.        
        BattleInfo.currentActionPoints = 0;

        // Waits, ends anim.
        yield return new WaitForSeconds(0.75f);
        foundEnemy.GetComponent<Animator>().SetBool("isHit", false);

        // Update enemies in range.
        BattleInfo.checkRange.CheckForEnemies();

        // If enemy died, wait extra time.
        if (!BattleInfo.levelEnemies.ContainsKey(foundEnemy))
        {
            yield return new WaitForSeconds(1.25f);
        }

        // Ends the players turn.
        StartCoroutine(EndTurn(0.4f));
    }

    /// <summary> method <c>CallSteadyAim</c> allows UI buttons to call the SteadyAim coroutine. </summary>
    public void CallSteadyAim()
    {
        StartCoroutine(SteadyAim());
    }

    /// <summary> method <c>SteadyAim</c> executes a player attack with an increased hit & crit chance, at the cost of aux action for the turn. </summary>
    public IEnumerator SteadyAim()
    {       
        // Locates enemy, returns if none found.
        GameObject foundEnemy = BasicPlayerAttackLogic(BattleInfo.playerWeapon.range);
        if (foundEnemy == null) 
        {
            GameObject.Find("UICanvas").transform.Find("OutOfRange").gameObject.SetActive(true);
            yield break; 
        }

        // Prevents use of SteadyAim, begins cooldown.
        BattleInfo.steadyAimUsed = true;
        StartCoroutine(AttackCooldown("SteadyAim"));

        // Ends playerTurn.
        BattleInfo.playerTurn = false;

        // Rotates player to face enemy, with adjust.
        yield return RotateToFaceEnemy(foundEnemy, true);

        // Plays PistolIdle anim.
        player.GetComponent<Animator>().SetBool("isFireing", true);
        yield return new WaitForSeconds(1.6f);

        // Player gunshot VFX.
        gunshotVFX.GetComponent<VisualEffect>().Play();

        // Plays shoot SFX.
        player.GetComponent<AudioSource>().PlayOneShot(playerFire);

        // Calc dmg, apply dmg, play effects. 
        yield return StartCoroutine(DamageCalcs(BattleValues.steadyAimHitGain, 
            BattleValues.steadyAimCritGain, foundEnemy));

        // Reset closestEnemy.
        BattleInfo.closestEnemy = null;

        // Clears action points.
        BattleInfo.currentActionPoints = 0;

        // Waits, ends anim.
        yield return new WaitForSeconds(0.75f);
        foundEnemy.GetComponent<Animator>().SetBool("isHit", false);

        // Update enemies in range.
        BattleInfo.checkRange.CheckForEnemies();

        // If enemy died, wait extra time.
        if (!BattleInfo.levelEnemies.ContainsKey(foundEnemy))
        {
            yield return new WaitForSeconds(1.25f);
        }

        // Ends the players turn.
        StartCoroutine(EndTurn(0.4f));
    }

    /// <summary> method <c>CallQuickDraw</c> allows UI buttons to call the QuickDraw coroutine. </summary>
    public void CallQuickDraw()
    {
        StartCoroutine(QuickDraw());
    }

    /// <summary> method <c>QuickDraw</c> executes a player attack with a decreased hit chance but enables access to 'Follow-Up Shot' aux action. </summary>
    public IEnumerator QuickDraw()
    {        
        // Locates enemy, returns if none found.
        GameObject foundEnemy = BasicPlayerAttackLogic(BattleInfo.playerWeapon.range);
        if (foundEnemy == null) 
        {
            GameObject.Find("UICanvas").transform.Find("OutOfRange").gameObject.SetActive(true);
            yield break; 
        }

        // Ends playerTurn.
        BattleInfo.playerTurn = false;

        // Rotates player to face enemy, with adjust.
        yield return RotateToFaceEnemy(foundEnemy, true);

        // Plays PistolIdle anim.
        player.GetComponent<Animator>().SetBool("isFireing", true);
        yield return new WaitForSeconds(1.6f);

        // Player gunshot VFX.
        gunshotVFX.GetComponent<VisualEffect>().Play();

        // Plays quick draw SFX.
        player.GetComponent<AudioSource>().PlayOneShot(playerQuickDraw);

        // Calc dmg, apply dmg, play effects.
        yield return StartCoroutine(DamageCalcs(-BattleValues.quickDrawHitDecrease, 0, foundEnemy));

        // Deincrements AP.
        BattleInfo.currentActionPoints--;

        // Waits, ends animations.
        yield return new WaitForSeconds(0.75f);
        foundEnemy.GetComponent<Animator>().SetBool("isHit", false);
        player.GetComponent<Animator>().SetBool("isFireing", false);

        // Update enemies in range.
        BattleInfo.checkRange.CheckForEnemies();

        // Reset grid spaces if AP remaining.
        if (BattleInfo.currentActionPoints == 1)
        {
            BattleInfo.playerTurn = true;
            player.GetComponent<PlayerMovement>().TilesMoved = 0;

            gridManager.GetComponent<GridVisuals>().VisualizeGridWhenCreated(true);

            // Allows enabling of Follow-Up-Shot.
            if (BattleInfo.currentAmmo > 0) { hasQuickDrawn = true; }
        }
        else
        {
            // Ends the players turn.
            StartCoroutine(EndTurn(0.4f));
        }       
    }

    /// <summary> method <c>FollowUpShot</c> is an aux action open to the player upon QuickDraw usage, simple extra attack. </summary>
    public void FollowUpShot()
    {
        // Executes follow up shot attack.
        StartCoroutine(Fire());
    }

    /// <summary> method <c>CallMelee</c> allows UI buttons to begin the Melee coroutine. </summary>
    public void CallMelee()
    {
        StartCoroutine(Melee());
    }
    
    /// <summary> coroutine <c>Melee</c> allows the player to attack using a kick or fist/knife. Consumes 1 AP. </summary>
    public IEnumerator Melee()
    {
        // Locates enemy, returns if none found.
        GameObject foundEnemy = BasicPlayerAttackLogic(1);
        if (foundEnemy == null)
        {
            GameObject.Find("UICanvas").transform.Find("OutOfRange").gameObject.SetActive(true);
            yield break;
        }

        // Ends playerTurn.
        BattleInfo.playerTurn = false;

        // Rotates player to face enemy, no adjust.
        yield return RotateToFaceEnemy(foundEnemy, false);

        // Plays PistolIdle anim.
        player.GetComponent<Animator>().SetBool("isMelee", true);
        yield return new WaitForSeconds(1f);

        // Calc dmg, apply dmg, play effects. 
        yield return StartCoroutine(DamageCalcs(100, 0, foundEnemy));

        // Reset closestEnemy.
        BattleInfo.closestEnemy = null;

        // Clears action points.
        BattleInfo.currentActionPoints = 0;

        // Waits, ends anim.
        yield return new WaitForSeconds(0.75f);
        foundEnemy.GetComponent<Animator>().SetBool("isHit", false);

        // Update enemies in range.
        BattleInfo.checkRange.CheckForEnemies();

        // If enemy died, wait extra time.
        if (!BattleInfo.levelEnemies.ContainsKey(foundEnemy))
        {
            yield return new WaitForSeconds(1.25f);
        }

        // Ends the players turn.
        StartCoroutine(EndTurn(0.4f));
    }

    /// <summary> coroutine <c>DamageCalcs</c> calculates dmg, begins effect, & applies dmg to enemy. </summary>
    /// <param name="hitChange">offset from base hit chance.</param>
    /// <param name="critChange">offset from base crit chance.</param>
    /// <param name="enemy">enemy to attack.</param>
    private IEnumerator DamageCalcs(int hitChange, int critChange, GameObject enemy)
    {
        // Players attachted attack values.
        WeaponValues weaponValues = BattleInfo.playerWeapon;

        int baseAccuracy = Mathf.RoundToInt((float)SkillsAndClasses.playerStats["Perception"] / enemy.GetComponentInChildren            
            <EnemyStats>().fitness * weaponValues.baseAccuracy);

        if (BattleInfo.CalculateAttackChance(baseAccuracy + hitChange))
        {
            // Play enemy hit anim + sfx.
            enemy.GetComponent<Animator>().SetBool("isHit", true);
            yield return new WaitForSeconds(0.1f);
            enemy.GetComponent<AudioSource>().PlayOneShot(enemyHitSFX[Random.Range(0, enemyHitSFX.Count)]);

            // Plays enemy hit VFX.
            enemy.GetComponentInChildren<VisualEffect>().Play();

            // Sets attackDamage, applies crit multiplyer if successful.
            int attackDamage = BattleInfo.CalculateDamage(weaponValues.baseDamage, SkillsAndClasses.playerStats["Toughness"],
                enemy.GetComponentInChildren<EnemyStats>().toughness);

            // Players chance of landing a crit, based on luck stats.
            int critChance = Mathf.RoundToInt((float)SkillsAndClasses.playerStats["Luck"] /
                enemy.GetComponentInChildren<EnemyStats>().luck * weaponValues.critChance);

            if (BattleInfo.CalculateAttackChance(critChance + critChange))
            {
                attackDamage = BattleInfo.ApplyCritMultiplyer(attackDamage, weaponValues.critMultiplyer);
                StartCoroutine(CritEffect(enemy));
            }
            else
            {
                // Only normal hit, play hit effect.
                StartCoroutine(HitEffect(enemy));
            }

            // Attacks the enemy & deincrements ammo.
            BattleInfo.levelEnemies[enemy] -= attackDamage;
            --BattleInfo.currentAmmo;
        }
        else
        {
            // Missed but still remove ammo.
            --BattleInfo.currentAmmo;

            // Plays miss effect.
            StartCoroutine(MissEffect(enemy));
        }
    }

    /// <summary> coroutine <c>CritEffect</c> allows crit effect to play, disables canvas again once finished. </summary>
    public IEnumerator CritEffect(GameObject foundEnemy) 
    {
        yield return new WaitForSeconds(0.2f);

        // Enables, sets hitEnemy.
        BattleInfo.critCanvas.GetComponent<ShotEffects>().HitEnemy = foundEnemy;
        BattleInfo.critCanvas.GetComponent<ShotEffects>().PlayerHit = false;
        BattleInfo.critCanvas.SetActive(true);
        
        // Waits for anim to complete, disables again.
        yield return new WaitForSeconds(0.8f);
        BattleInfo.critCanvas.SetActive(false);
    }

    /// <summary> coroutine <c>HitEffect</c> allows hit effect to play, disables canvas again once finished. </summary>
    public IEnumerator HitEffect(GameObject foundEnemy)
    {
        // Enables, sets hitEnemy.
        BattleInfo.hitCanvas.GetComponent<ShotEffects>().HitEnemy = foundEnemy;
        BattleInfo.hitCanvas.GetComponent<ShotEffects>().PlayerHit = false;
        BattleInfo.hitCanvas.SetActive(true);

        // Waits for anim to complete, disables again.
        yield return new WaitForSeconds(0.6f);
        BattleInfo.hitCanvas.SetActive(false);
    }

    /// <summary> coroutine <c>MissEffect</c> allows miss effect to play, disables canvas again once finished. </summary>
    public IEnumerator MissEffect(GameObject foundEnemy)
    {
        // Enables, sets hitEnemy.
        BattleInfo.missCanvas.GetComponent<ShotEffects>().HitEnemy = foundEnemy;
        BattleInfo.missCanvas.GetComponent<ShotEffects>().PlayerHit = false;
        BattleInfo.missCanvas.SetActive(true);

        // Waits for anim to complete, disables again.
        yield return new WaitForSeconds(0.6f);
        BattleInfo.missCanvas.SetActive(false);
    }

    /// <summary> interface <c>RotateToFaceEnemy</c> rotates the player to face the specified enemy. </summary>
    public IEnumerator RotateToFaceEnemy(GameObject foundEnemy, bool accountForShooting)
    {
        // Calculate direction & target rot.
        Vector3 direction = (foundEnemy.transform.position - player.transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        
        // Account for the animation direction.
        if (accountForShooting) { targetRotation *= Quaternion.Euler(0, 90, 0); }

        // Calculate total angle & time for rotation
        float totalAngle = Quaternion.Angle(player.transform.rotation, targetRotation);
        float totalTime = totalAngle / 20;

        // Time since move/rotation began.
        float elapsedTime = 0;

        // Rotate player to face the enemy.
        while (Quaternion.Angle(player.transform.rotation, targetRotation) > 0.01f)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / totalTime;
            player.transform.rotation = Quaternion.Slerp(player.transform.rotation, targetRotation, t);
            yield return null;
        }
    }

    /// <summary> method <c>TakeCover</c> changes the player status to 'inCover', gives increased evasion. </summary>
    public void TakeCover()
    {
        // Player is now in cover.
        BattleInfo.playerInCover = true;
        player.GetComponent<Animator>().SetBool("inCover", true);
        player.GetComponent<AudioSource>().PlayOneShot(player.GetComponent<PlayerMovement>().playerAnimationSFX[2]);

        // Ends the players turn.
        StartCoroutine(EndTurn(0.4f));
    }

    /// <summary> method <c>CallReloadWeapon</c> allows UI buttons to start the reloadWeapon routine. </summary>
    public void CallReloadWeapon()
    {
        StartCoroutine(ReloadWeapon());
    }

    /// <summary> coroutine <c>ReloadWeapon</c> re-fills the magazine, allows player to attack again. </summary>
    public IEnumerator ReloadWeapon()
    {
        // Plays reload animation.
        player.GetComponent<Animator>().SetBool("isReloading", true);
        BattleInfo.playerTurn = false;

        // Wait for anim to complete.
        yield return new WaitForSeconds(3f);

        // Gets weapon values, resets currentAmmo.
        WeaponValues weaponValues = BattleInfo.playerWeapon;
        BattleInfo.currentAmmo = weaponValues.magSize;

        player.GetComponent<AudioSource>().PlayOneShot(playerReload);

        // Ends the players turn.
        StartCoroutine(EndTurn(0.4f));
    }

    /// <summary> method <c>CallEndTurn</c> allows UI buttons to call the EndTurn coroutine. </summary>
    public void CallEndTurn()
    {
        StartCoroutine(EndTurn(0.0f));
    }

    /// <summary> method <c>EndTurn</c> ends the player's turn, begins the AIs. </summary>
    public IEnumerator EndTurn(float delay)
    {
        // Reset closestEnemy.
        BattleInfo.closestEnemy = null;

        // Ends players turn.
        BattleInfo.playerTurn = false;
        BattleInfo.currentActionPoints = 0;

        // Delay between turns.
        yield return new WaitForSeconds(delay);

        // Begins enemy turn.
        BattleInfo.aiTurn = true;        

        // Resets quick drawn.
        hasQuickDrawn = false;

        // Reset moved tiles.
        GameObject.Find("Player").GetComponent<PlayerMovement>().TilesMoved = 0;

        // Resets player turn if no enemies are left.
        if (BattleInfo.levelEnemies.Count == 0)
        {
            // Resets values.
            BattleInfo.playerTurn = true;
            BattleInfo.aiTurn = false;

            // Returns player to idle state.
            player.GetComponent<Animator>().SetBool("isMelee", false);
            player.GetComponent<Animator>().SetBool("isReloading", false);
            player.GetComponent<Animator>().SetBool("isFireing", false);            

            // Ends the coroutine early.
            yield break;
        }

        // Returns all AI turns to true.
        foreach (var key in BattleInfo.levelEnemyTurns.Keys.ToList())
        {
            BattleInfo.levelEnemyTurns[key] = true;
        }

        // Delay, play SFX.       
        player.GetComponent<AudioSource>().PlayOneShot(endTurn);

        // Returns player to idle state.
        player.GetComponent<Animator>().SetBool("isMelee", false);
        player.GetComponent<Animator>().SetBool("isReloading", false);
        player.GetComponent<Animator>().SetBool("isFireing", false);

        // Resets grid. 
        gridManager.GetComponent<GridVisuals>().VisualizeGridWhenCreated(true);
    }

    /// <summary> method <c>CheckForRemainingEnemies</c> continues player turn if no enemies are left in the level. </summary>
    public void CheckForRemainingEnemies()
    {
        // Resets player turn if no enemies are left.
        if (BattleInfo.levelEnemies.Count == 0)
        {
            // Resets values.
            BattleInfo.playerTurn = true;
            BattleInfo.aiTurn = false;
            player.GetComponent<PlayerMovement>().MoveUsedUp = false;

            // Prevents repeated checks.
            noEnemiesRemaning = true;
        }
    }

    /// <summary> coroutine <c>AttackCooldown</c> prevents the player from using the given attack for specified turns. </summary>
    public IEnumerator AttackCooldown(string attack)
    {
        // Sets cooldown for steadyAim.
        if (attack == "SteadyAim")
        {
            // Sets cooldown, waits until it's over.
            BattleInfo.steadyAimCooldown = 2;
            bool turnChanged = true;
            while (BattleInfo.steadyAimCooldown != 0)
            {
                // Only removes 1 when back to player turn occurs.
                if (BattleInfo.playerTurn && !BattleInfo.aiTurn && !turnChanged)
                {
                    turnChanged = true;
                    --BattleInfo.steadyAimCooldown;

                    Debug.Log("Here");
                }
                else if (BattleInfo.aiTurn)
                {
                    turnChanged = false;
                }

                // Fulfills yield req.
                yield return new WaitForEndOfFrame();
            }

            // Re-allows SteayAim.
            BattleInfo.steadyAimUsed = false;
        }
    }

    // Called once before first update.
    void Start()
    {
        // Sets ref to grid manager script.
        gridManager = BattleInfo.gridManager.GetComponent<GridManager>();

        // Sets ref to player obj.
        player = GameObject.Find("Player");
    }

    private bool noEnemiesRemaning;

    // Called once each frame.
    void Update()
    {
        // Makes sure buttons are set to correct state.
        UpdateButtonInteraction();

        // Prevents battling getting stuck if no enemies.
        if (!noEnemiesRemaning) { CheckForRemainingEnemies(); }       
    }
}