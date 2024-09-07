// Author - Ronnie Rawlings.

using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.VFX;

public class TurretAI : BaseAI
{
    // Current turns decison is made.
    private bool decisonMade = false;

    // Grid the AI is currently on.
    [SerializeField] private int currentGrid = 0;

    // Gunshot (MuzzleFlash) VFX.
    [SerializeField] private List<VisualEffect> muzzelFlashes;

    // Shot SFX.
    [SerializeField] private AudioClip shotSFX;

    /// <summary> method <c>AttackPlayer</c> tries to damage the player when in range. </summary>
    public IEnumerator AttackPlayer()
    {
        // Weapon attached to the AI obj.
        WeaponValues attachtedWeapon = GetComponentInChildren<WeaponValues>();

        // Rotate turret to face player.
        yield return RotateToFacePlayer(BattleInfo.player);

        // Chance of AIs attack landing.
        int attackChance = attachtedWeapon.baseAccuracy;
        if (BattleInfo.playerInCover)
        {
            // Decreases hit chance by player evasion.
            attackChance -= BattleValues.coverEvasionChance;
        }

        int baseAccuracy = Mathf.RoundToInt((float)GetComponentInChildren<EnemyStats>().perception /
            SkillsAndClasses.playerStats["Fitness"] * attackChance);

        // Waits before attacking.
        yield return new WaitForSeconds(0.75f);

        // Plays muzzle flash VFX.
        if (muzzelFlashes != null)
        {
            for (int i = 0; i < muzzelFlashes.Count; i++)
            {
                muzzelFlashes[i].Play();
            }
        }
        GetComponent<AudioSource>().PlayOneShot(shotSFX);

        // Only remove health if hit attack chance.
        if (BattleInfo.CalculateAttackChance(baseAccuracy))
        {
            // Player's player hit anim + VFX.
            BattleInfo.player.GetComponent<Animator>().SetBool("isHit", true);
            SetHitEffect();

            BattleInfo.player.GetComponent<AudioSource>().PlayOneShot(BattleInfo.player.GetComponent<PlayerMovement>().
                playerAnimationSFX[0]);

            // Checks if attack is a crit.
            int attackDamage = BattleInfo.CalculateDamage(attachtedWeapon.baseDamage, GetComponentInChildren<EnemyStats>().toughness,
                SkillsAndClasses.playerStats["Toughness"]);

            int critChance = Mathf.RoundToInt((float)GetComponentInChildren<EnemyStats>().luck /
                SkillsAndClasses.playerStats["Luck"] * attachtedWeapon.critChance);

            if (BattleInfo.CalculateAttackChance(critChance))
            {
                // Increases damage by multiplyer.
                attackDamage = BattleInfo.ApplyCritMultiplyer(attackDamage, attachtedWeapon.critMultiplyer);

                Debug.Log("AI Attack Crit!");
                StartCoroutine(CritEffect());
            }
            else
            {
                StartCoroutine(HitEffect());
            }

            // Applyies damage to player.
            BattleInfo.currentPlayerHealth -= attackDamage;
            Debug.Log("AI Attack Success!");

        }
        else
        {
            Debug.Log("AI Attack Missed!");
            StartCoroutine(MissEffect());
        }      

        // Waits, ends turn.
        yield return new WaitForSeconds(1f);

        // Ends player anim.
        BattleInfo.player.GetComponent<Animator>().SetBool("isHit", false);

        // Update enemies in range.
        BattleInfo.checkRange.CheckForEnemies();

        // Ends this AIs turn.       
        BattleInfo.levelEnemyTurns[gameObject.name] = false;
        BattleInfo.hasLockedEnemy = false;
    }

    /// <summary> coroutine <c>RotateToFacePlayer</c> has the enemy rotate to face the player before attacking. </summary>
    public IEnumerator RotateToFacePlayer(GameObject player)
    {
        // Calculate direction & target rot.
        Vector3 direction = (player.transform.position - transform.position).normalized;
        Vector3 targetDirection = new Vector3(direction.x, 0, direction.z);

        // Calculate total angle & time for rotation
        float totalAngle = Vector3.Angle(transform.forward, targetDirection);
        float totalTime = totalAngle / 5;

        // Time since move/rotation began.
        float elapsedTime = 0;

        // Rotate enemy to face the player.
        while (Vector3.Angle(transform.forward, targetDirection) > 3f)
        {
            elapsedTime += Time.deltaTime;
            float maxAngleChange = elapsedTime / totalTime * totalAngle;
            transform.forward = Vector3.RotateTowards(transform.forward, targetDirection, maxAngleChange * Mathf.Deg2Rad, 0.0f);
            yield return null;
        }
    }

    /// <summary> method <c>SetHitEffect</c> moves the player's hit VFX to pos relevant to enemies hit. </summary>
    public void SetHitEffect()
    {
        // Determines hit direction/position, uses raycast.
        RaycastHit hit;
        if (Physics.Raycast(transform.position, (BattleInfo.player.transform.position - transform.position).normalized, out hit,
            Mathf.Infinity, LayerMask.GetMask("Player")))
        {
            // Moves hitVFX to player hit point.
            if (hit.collider.gameObject == BattleInfo.player)
            {
                // Set new VFX position to hit point.
                BattleInfo.player.transform.Find("hit").transform.position = new Vector3(hit.point.x, BattleInfo.player.transform.Find("hit").
                    transform.position.y, hit.point.z);
            }
        }

        // Plays VFX.
        BattleInfo.player.transform.Find("hit").GetComponent<VisualEffect>().Play();
    }

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
            // In range so attack.
            StartCoroutine(AttackPlayer());
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
