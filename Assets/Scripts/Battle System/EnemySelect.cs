// Author - Ronnie Rawlings.

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.UI;

public class EnemySelect : MonoBehaviour
{
    // All enemy types sprites.
    [SerializeField] private List<Sprite> enemySprites;

    // Ref to EnemySelect UI.
    private GameObject enemySelectUI;

    // Ref to the gridManager script.
    private GridManager gridManager;

    // Ref to the gridVisuals script.
    private GridVisuals gridVisuals;

    // Selected enemies current life status.
    private bool selectedEnemyStatus;

    /// <summary> method <c>SelectEnemyOnClick</c> enables the 'EnemySelect' UI on click of an enemy AI obj. </summary>
    public void SelectEnemyOnClick()
    {
        // Only functions when LeftClick received.
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            // Sends raycast from mouse to point.
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // Sets layer mask to Enemy.
            int layerMask = LayerMask.GetMask("Enemy");

            // Performs raycast, true if enemy is hit.
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
            {
                // Gets access to enemy obj.
                GameObject selectedEnemy = hit.transform.gameObject;

                // Toggle UI if enemy is the same.
                if (selectedEnemy == BattleInfo.currentSelectedEnemy)
                {
                    // Change UI active status.
                    enemySelectUI.SetActive(!enemySelectUI.activeInHierarchy);                    
                }
                else
                {
                    // Enable UI & change UI data to match enemy.
                    enemySelectUI.SetActive(true);                 
                }

                // Updates UI info.
                SetEnemySelectInfo(selectedEnemy);

                // Update enemy selection.
                BattleInfo.currentSelectedEnemy = selectedEnemy;

                // Is the enemy dead or alive.
                if (selectedEnemy.GetComponent<CheckAIStatus>().enabled)
                {
                    selectedEnemyStatus = true;
                }
                else
                {
                    selectedEnemyStatus = false;
                }
            }
        }
        else
        {
            // Updates UI in case of value changes while active.
            if (enemySelectUI.activeInHierarchy)
            {                
                SetEnemySelectInfo(BattleInfo.currentSelectedEnemy);
            }
        }
    }

    /// <summary> method <c>DeSelectEnemy</c> deselects an enemy if they were alive and their status has changed, dosen't deselect already dead enemies. </summary>
    public void DeSelectEnemy()
    {
        // Returns if no enemy selected.
        if (BattleInfo.currentSelectedEnemy == null) { return; }

        // Disables enemySelect if status changed.
        if (BattleInfo.currentSelectedEnemy.GetComponent<CheckAIStatus>().enabled != selectedEnemyStatus)
        {
            enemySelectUI.SetActive(false);
        }
    }

    /// <summary> method <c>SetEnemySelectedInfo</c> sets the relevant info on the enemy selected UI element. </summary>
    public void SetEnemySelectInfo(GameObject selectedEnemy)
    {
        // Updates selected enemy name.
        enemySelectUI.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = selectedEnemy.name;

        // Set image, depends on enemy type.
        if (selectedEnemy.name.Contains("Tank"))
        {
            enemySelectUI.transform.GetChild(1).GetComponent<Image>().sprite = enemySprites[1];
        }
        else
        {
            enemySelectUI.transform.GetChild(1).GetComponent<Image>().sprite = enemySprites[0];
        }

        if (!BattleInfo.levelEnemies.ContainsKey(selectedEnemy))
        {
            // Updates selected enemy health.
            enemySelectUI.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = "0";
            SetUIColour(Color.grey);
        }
        else
        {
            // Get the enemy health.
            int enemyHealth = BattleInfo.levelEnemies[selectedEnemy];

            // Prevent health from displaying less than 0.
            enemyHealth = Mathf.Max(0, enemyHealth);

            // Updates selected enemy health.
            enemySelectUI.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = enemyHealth.ToString();
            SetUIColour(enemyHealth > 0 ? Color.red : Color.grey);
        }

        // Get enemy weapon & stat scripts.
        WeaponValues enemyWeapon = selectedEnemy.GetComponentInChildren<WeaponValues>();
        EnemyStats enemyStats = selectedEnemy.GetComponentInChildren<EnemyStats>();

        // Uses dmg/crit calcs to work out damage range.
        int lowerDamage = BattleInfo.CalculateDamage(enemyWeapon.baseDamage, enemyStats.toughness, SkillsAndClasses.playerStats["Toughness"]);
        int higherDamage = BattleInfo.ApplyCritMultiplyer(lowerDamage, enemyWeapon.critMultiplyer);

        // Updates enemy damage potential.
        enemySelectUI.transform.GetChild(4).GetComponent<TextMeshProUGUI>().text = lowerDamage + " - " + higherDamage;
    }

    /// <summary> method <c>SetUIColour</c> sets the EnemySelect UIs colour to given. </summary>
    public void SetUIColour(Color colour)
    {
        enemySelectUI.transform.GetChild(2).GetComponent<TextMeshProUGUI>().color = colour;
        enemySelectUI.transform.GetChild(3).GetComponent<TextMeshProUGUI>().color = colour;
        enemySelectUI.transform.GetChild(4).GetComponent<TextMeshProUGUI>().color = colour;
    }

    // Called once on script initlization.
    void OnEnable()
    {
        // Sets ref to EnemySelect UI element.
        enemySelectUI = GameObject.Find("UICanvas").transform.Find("Enemy Select UI").gameObject;
    }

    private void Start()
    {
        // Get the GridManager and GridVisuals scripts from the gridManager object
        gridManager = BattleInfo.gridManager.GetComponent<GridManager>();
        gridVisuals = BattleInfo.gridManager.GetComponent<GridVisuals>();
    }

    // Store the last mouse position
    private Vector3 lastMousePosition;

    void Update()
    {
        // Checks each frame for enemy select.
        SelectEnemyOnClick();

        // Checks if selected enemy has died, deselects.
        DeSelectEnemy();
    }
}
