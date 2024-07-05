// Author - Ronnie Rawlings.

using System.Collections.Generic;
using TMPro;
using UnityEngine;

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
                // Update enemy selection.
                BattleInfo.currentSelectedEnemy = hit.transform.gameObject;
            }
        }
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

    void Update()
    {
        // Checks each frame for enemy select.
        SelectEnemyOnClick();
    }
}
