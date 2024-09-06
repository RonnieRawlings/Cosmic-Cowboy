// Author - Ronnie Rawlings.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CheckAIStatus : MonoBehaviour
{
    // Current health of AI Enemy.
    private int currentHealth;

    // EnemyType of Turret + When enemy health is 0.
    private bool isTurret, hasDied;

    // All possible death SFXs.
    [SerializeField] private List<AudioClip> deathSFX;

    #region Properties

    public bool IsTurret
    {
        set { isTurret = value; }
    }

    #endregion

    /// <summary> method <c>CheckStatus</c> checks if AI has been defeated, destroyes if so. </summary>
    public void CheckStatus()
    {
        // Destroys enemy when health is zero.
        if (BattleInfo.levelEnemies[this.gameObject] != currentHealth)
        {
            // Updates currentHealth, flashes colour.
            currentHealth = BattleInfo.levelEnemies[this.gameObject];
        }
        else if (BattleInfo.levelEnemies[this.gameObject] <= 0)
        {
            // No more calls. 
            hasDied = true;

            // Removes AI from lists.
            BattleInfo.levelEnemyStats.Remove(this.gameObject);
            BattleInfo.levelEnemies.Remove(this.gameObject);
            BattleInfo.levelEnemiesList.Remove(this.gameObject.name);
            BattleInfo.levelEnemyTurns.Remove(this.gameObject.name);

            // Unmarks current node as occupied.
            BaseAI aiScript = null;
            if (isTurret) { aiScript = GetComponent<TurretAI>(); }
            else
            {
                aiScript = GetComponent<AIMovement>();

                // Cast to AIMovement to access CurrentNode
                AIMovement aiMovementScript = aiScript as AIMovement;

                if (aiMovementScript != null && aiMovementScript.CurrentNode != null)
                {
                    aiMovementScript.CurrentNode.Occupied = null;
                }
            }        
                      
            // Disable components
            aiScript.enabled = false;
            GetComponent<EnemyHoverInfo>().enabled = false;

            // Remove hoverData entirly.
            Destroy(transform.Find("HoverData").gameObject);

            // Prevent enemy selection.
            BattleInfo.checkRange.CheckForEnemies();

            // Begins enemy death commands.
            StartCoroutine(EnemyDeath());
        }
    }

    /// <summary> coroutine <c>EnemyDeath</c> plays out commands before disabling script on enemy death. </summary>
    private IEnumerator EnemyDeath()
    {
        GetComponent<Animator>().SetBool("isDead", true);

        // Waits for hurt SFX to play out.
        yield return new WaitForSeconds(0.5f);

        // Play death SFX.        
        GetComponent<AudioSource>().PlayOneShot(deathSFX[Random.Range(0, deathSFX.Count)]);

        // Disable script.
        this.enabled = false;
    }

    // Called once on script initlization.
    void OnEnable()
    {
        // Sets AI's current health to base.
        currentHealth = BattleInfo.levelEnemies[gameObject];
    }

    // Update is called once per frame
    void Update()
    {
        // Checks if AI has been defeated.
        if (!hasDied) { CheckStatus(); }
    }
}
