// Author - Ronnie Rawlings.

using Fungus;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    // Ref to UICanvas obj.
    [SerializeField] GameObject uiCanvas;

    // Player move speed.
    [SerializeField] private float moveSpeed;

    // Ref to grid manager script.
    private GridManager gridManager;

    // Ref to pathfinding script.
    private Pathfinding pathfinding;

    // Current WASD tiles.
    private List<Node> WASDNodeList;

    // Ref to EndTurn AudioClip
    private AudioClip endTurn;

    // List of all animation SFXs.
    [SerializeField] public List<AudioClip> playerAnimationSFX;

    // Tiles player has moved this turn.
    private int tilesMoved;

    // Has movement been used up.
    private bool moveUsedUp = false;

    #region Variable Properties

    public int TilesMoved
    {
        get { return tilesMoved; }
        set { tilesMoved = value; }
    }

    public bool MoveUsedUp
    {
        set { moveUsedUp = value; }
    }

    #endregion

    public bool startDragOnPlayer = false;

    /// <summary> method <c>MouseTileMovement</c> allows the player to move to surrounding tiles using mouse click/move. </summary>
    public void MouseTileMovement()
    {
        // Prevents movement in enemySelect.
        if (BattleInfo.camBehind) { return; }

        // If not the player's turn, end method.
        if (!(BattleInfo.playerTurn && !BattleInfo.aiTurn)) 
        {
            return; 
        }

        // Resets movement when game is paused.
        if (BattleInfo.gamePaused)
        {
            startDragOnPlayer = false;

            // Resets nodes.
            if (gridManager.GetComponent<GridVisuals>().mouseToPlayer != null)
            {
                foreach (Node n in gridManager.GetComponent<GridVisuals>().mouseToPlayer)
                {
                    BattleInfo.nodeObjects[n].GetComponent<MeshRenderer>().material.color = new Color(0.5f, 0.5f, 1f, 1f);
                }
                gridManager.GetComponent<GridVisuals>().mouseToPlayer = null;

                // Updates grid visuals.
                gridManager.GetComponent<GridVisuals>().VisualizeGridWhenCreated(true);
            }           
        }

        // Prevent movement when over UI.
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        // Only allow path to be set if starting from player.
        if (Input.GetMouseButtonDown(0))
        {
            // Calc cursor pos.
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // Sets layer mask to player.
            int layerMask = LayerMask.GetMask("Player");

            // If raycast hits player collision box, allow path setting.
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
            {
                if (hit.collider.gameObject == this.gameObject)
                {
                    startDragOnPlayer = true;
                }
            }
        }

        if (startDragOnPlayer && Input.GetKey(KeyCode.Mouse0))
        {
            // Re-enables visual if disabled, prevents it from being turned off.
            gridManager.PlayerIsSettingPath = true;

            // Resets set path.
            gridManager.GetComponent<GridVisuals>().SetPath = false;

            // Calculates cursor position.
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // Sets layer mask to grid.
            int layerMask = LayerMask.GetMask("Node");

            // Closest node to mouse pos.
            Node mouseNode;

            // Finds best path from player to mouse node.
            List<Node> bestPath = new List<Node>();
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
            {
                // Finds node from hit, finds best path to node.
                mouseNode = gridManager.FindNodeFromWorldPoint(hit.collider.gameObject.transform.position, BattleInfo.currentPlayerGrid);

                if (mouseNode != null && mouseNode.Walkable)
                {
                    bestPath = pathfinding.FindPath(transform.position, mouseNode.WorldPos, BattleInfo.currentPlayerGrid);

                    // Removes range above move tile limit.
                    if (bestPath.Count > BattleValues.playerTileAmount)
                    {
                        bestPath.RemoveRange(BattleValues.playerTileAmount, bestPath.Count -
                            BattleValues.playerTileAmount);
                    }

                    if (bestPath.Count > (BattleValues.playerTileAmount - tilesMoved))
                    {
                        int removeCount = bestPath.Count - (BattleValues.playerTileAmount - tilesMoved);
                        bestPath.RemoveRange(BattleValues.playerTileAmount - tilesMoved, removeCount);
                    }

                    gridManager.GetComponent<GridVisuals>().mouseToPlayer = bestPath;
                }
            }                       
        }

        // Checks for set path.
        if (Input.GetMouseButtonUp(0))
        {
            Debug.Log("Called here");

            if (gridManager.GetComponent<GridVisuals>().mouseToPlayer != null)
            {
                StartCoroutine(MovePlayer(gridManager.GetComponent<GridVisuals>().mouseToPlayer));
            }

            // Resets drag bool.
            startDragOnPlayer = false;

            // Updates grid visuals.
            gridManager.GetComponent<GridVisuals>().VisualizeGridWhenCreated(true);
        }
    }

    /// <summary> interface <c>MovePlayerMouse</c> moves the player along the given path. </summary>
    public IEnumerator MovePlayer(List<Node> bestPath)
    {
        // Prevents issues with further movement.
        if (bestPath.Count > 5) 
        {
            // Resets path, exits routine.
            gridManager.GetComponent<GridVisuals>().mouseToPlayer = null;
            yield break; 
        }

        // Pans cam back to player.
        BattleInfo.begunMovement = true;

        // Prevents more coroutine runs.
        BattleInfo.playerTurn = false;

        // Begins walking animation.
        GetComponent<Animator>().SetBool("isWalking", true);

        // Sets path in visuals & resets WASD path.
        gridManager.GetComponent<GridVisuals>().SetPath = true;
        WASDNodeList = null;

        // Loops through all nodes, moves player to each.
        foreach (Node n in bestPath)
        {
            // Switches to normal walking anim.
            if (!BattleInfo.playerInCover) { GetComponent<Animator>().SetBool("inCover", false); }

            // Next node pos.
            Vector3 targetPosition = new Vector3(n.WorldPos.x, n.WorldPos.y, n.WorldPos.z - 0.75f);

            // Calculate direction & target rot.
            Vector3 direction = (targetPosition - transform.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(direction);

            // Calculate total angle & time for rotation
            float totalAngle = Quaternion.Angle(transform.rotation, targetRotation);
            float totalTime = totalAngle / 20;

            // Time since move/rotation began.
            float elapsedTime = 0;

            // Move player towards next node.
            while (Vector3.Distance(transform.position, targetPosition) > 0.001f)
            {
                // Time since last pass.
                elapsedTime += Time.deltaTime;

                // Prevents rotation issues when 1 node.
                float timeDecrease = 4f;

                // Rotate player to face the target position
                float t = (elapsedTime / totalTime) * timeDecrease;
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, t);

                // Move towards target pos.
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

                yield return null;
            }

            // Increases tiles moved.
            tilesMoved++;

            // Updates grid visuals.
            gridManager.GetComponent<GridVisuals>().VisualizeGridWhenCreated(true);

            // Player moves to ladder linked grid, exits routine.
            if (n.IsLadder)
            {
                StartCoroutine(MoveToNewGrid(n));
                yield break;
            }
        }

        // Ends walking animation.
        GetComponent<Animator>().SetFloat("differentIdles", Random.Range(0, 2));
        GetComponent<Animator>().SetBool("isWalking", false);

        // Update enemies in range.
        BattleInfo.checkRange.CheckForEnemies();

        // Enables click text for health station node.
        TextMeshProUGUI interactText = uiCanvas.transform.Find("HealthInteract").GetComponent<TextMeshProUGUI>();
        if (bestPath[bestPath.Count - 1].IsHealthStation) { interactText.enabled = true; }
        else { interactText.enabled = false; }

        // Waits before changing turns.
        yield return new WaitForSeconds(0.75f);

        // Allows gridVisuals to be disabled again.
        gridManager.PlayerIsSettingPath = false;

        // Resets path variable.
        gridManager.GetComponent<GridVisuals>().mouseToPlayer = null;

        // Changes turn.
        UpdateTurn();        
    }

    /// <summary> coroutine <c>MoveToNewGrid</c> moves the player to the linked grid assigned to given ladderNode. Updates turn after. </summary>
    public IEnumerator MoveToNewGrid(Node ladderNode)
    {
        Debug.Log("Player is positioned on a ladder node!");

        // Finds linkedNode grid index.
        int gridIndex = ladderNode.LinkedNode.GetComponent<LadderData>().GridIndex;

        // Updates player's currentGrid var.
        BattleInfo.currentPlayerGrid = gridIndex;

        // Finds targetNode using found gridIndex.
        Node targetNode = gridManager.FindNodeFromWorldPoint(ladderNode.LinkedNode.transform.position, gridIndex);

        // Next node pos.
        Vector3 targetPosition = new Vector3(targetNode.WorldPos.x, targetNode.WorldPos.y, targetNode.WorldPos.z - 0.75f);
        Debug.Log(targetPosition);

        // Calculate direction & target rot.
        Vector3 direction = (targetPosition - transform.position).normalized;
        float yRotation = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(0, yRotation, 0);

        // Calculate total angle & time for rotation
        float totalAngle = Quaternion.Angle(transform.rotation, targetRotation);
        float totalTime = totalAngle / 20;

        GetComponent<Animator>().SetBool("isWalking", false);       

        // Time since move/rotation began.
        float elapsedTime = 0;

        // First, rotate player to face targetNode.
        while (Quaternion.Angle(transform.rotation, targetRotation) > 0.001f)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / totalTime;
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, t);
            yield return null;
        }

        if (transform.position.y < targetPosition.y) 
        {
            GetComponent<Animator>().SetBool("climbingUp", true);
        }
        
        // Then, move player up the Y axis.
        while (transform.position.y < targetPosition.y)
        {
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x, targetPosition.y,
                transform.position.z), (moveSpeed / 2.5f) * Time.deltaTime);
            yield return null;
        }

        if (GetComponent<Animator>().GetBool("climbingUp"))
        {
            GetComponent<Animator>().SetBool("climbingUp", false);
            GetComponent<Animator>().SetBool("isWalking", true);
        }
        else
        {
            GetComponent<Animator>().SetBool("jumpingDown", true);
            yield return new WaitForSeconds(0.2f);
        }

        // Then, move player towards targetNode.
        while (Vector3.Distance(transform.position, targetPosition) > 0.001f)
        {
            // Move towards target pos.
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, (moveSpeed * 1.3f) * Time.deltaTime);

            yield return null;
        }

        GetComponent<Animator>().SetBool("jumpingDown", false);

        // Updates grid visuals.
        gridManager.GetComponent<GridVisuals>().VisualizeGridWhenCreated(true);

        // Ends walking animation.
        GetComponent<Animator>().SetFloat("differentIdles", Random.Range(0, 2));
        GetComponent<Animator>().SetBool("isWalking", false);

        // Update enemies in range.
        BattleInfo.checkRange.CheckForEnemies();

        // Waits before changing turns.
        yield return new WaitForSeconds(0.75f);

        // Allows gridVisuals to be disabled again.
        gridManager.PlayerIsSettingPath = false;

        // Resets path variable.
        gridManager.GetComponent<GridVisuals>().mouseToPlayer = null;

        // Changes turn.
        UpdateTurn();
    }

    private bool currentlyRotating = false;

    /// <summary> coroutine <c>RotateTowardsEnemy</c> rotate the player to face the enemy. Usually on enemy select. </summary>
    /// <param name="enemy">Enemy to face.</param>
    public IEnumerator RotateTowardsEnemy(GameObject enemy)
    {
        // Deals with multiple routines.
        if (currentlyRotating) { yield break; }
        currentlyRotating = true;
        BattleInfo.isPlayerRotating = true;

        // Calculate the direction to the enemy.
        Vector3 direction = enemy.transform.position - transform.position;
        direction.y = 0;

        // Calculate the target rotation.
        Quaternion targetRotation = Quaternion.LookRotation(direction);

        // Rotate the player until it faces the enemy.
        while (Quaternion.Angle(transform.rotation, targetRotation) > 0.1f)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 4f);
            yield return null;
        }

        // Ensure the final rotation is met.
        transform.rotation = targetRotation;

        currentlyRotating = false;
        BattleInfo.isPlayerRotating = false;
    }

    /// <summary> method <c>UpdateTurn</c> checks tiles moved, ends player turn if total move exceeds limit. Continue otherwise. </summary>
    public void UpdateTurn()
    {
        // Ends player turn if total tiles have been moved.
        if (tilesMoved >= BattleValues.playerTileAmount && BattleInfo.currentActionPoints == 1)
        {
            // Removes remaining AP.
            BattleInfo.currentActionPoints--;

            BattleInfo.closestEnemy = null;

            // Disables health station interact text.
            uiCanvas.transform.Find("HealthInteract").GetComponent<TextMeshProUGUI>().enabled = false;

            // Changes turn.        
            BattleInfo.aiTurn = true;
            tilesMoved = 0;

            // Resets quickDraw value.
            GameObject.Find("UICanvas").GetComponentInChildren<PlayerActions>().HasQuickDrawn = false;

            // Disables follow up shot.
            uiCanvas.GetComponentInChildren<PlayerActions>().playerActionObjs[uiCanvas.GetComponentInChildren
                <PlayerActions>().playerActionObjs.Count - 1].SetActive(false);

            if (BattleInfo.levelEnemies.Count == 0)
            {
                BattleInfo.aiTurn = false;
                BattleInfo.playerTurn = true;
            }
            else
            {
                // Returns all AI turns to true.
                foreach (var key in BattleInfo.levelEnemyTurns.Keys.ToList())
                {
                    BattleInfo.levelEnemyTurns[key] = true;
                    Debug.Log(key);
                }

                // Plays endTurn SFX.
                GameObject.Find("Player").GetComponent<AudioSource>().PlayOneShot(endTurn);
            }

            // Re-enables movement if disabled.
            moveUsedUp = false;

            gridManager.GetComponent<GridVisuals>().VisualizeGridWhenCreated(true);
        }
        else if (BattleInfo.currentActionPoints > 1 && tilesMoved >= BattleValues.playerTileAmount)
        {
            // Removes AP & resets moved.
            BattleInfo.currentActionPoints--;
            tilesMoved = 0;

            // Continues turn & prevents more movement.
            BattleInfo.playerTurn = true;
            gridManager.GetComponent<GridVisuals>().VisualizeGridWhenCreated(true);
        }
        else
        {
            // Continues turn.
            BattleInfo.playerTurn = true;

            // Resets quickDraw value.
            GameObject.Find("UICanvas").GetComponentInChildren<PlayerActions>().HasQuickDrawn = false;

            // Disables follow up shot.
            uiCanvas.GetComponentInChildren<PlayerActions>().playerActionObjs[uiCanvas.GetComponentInChildren
                <PlayerActions>().playerActionObjs.Count - 1].SetActive(false);

            // Keeps movement available.
            if (BattleInfo.levelEnemies.Count == 0)
            {
                BattleInfo.currentActionPoints = 2;
            }

            gridManager.GetComponent<GridVisuals>().VisualizeGridWhenCreated(true);
        }
    }

    /// <summary> method <c>MoveToStartNode</c> moves the player onto it's actual start node. </summary>
    public void MoveToStartNode()
    {
        Node closestNode = gridManager.FindNodeFromWorldPoint(transform.position, BattleInfo.currentPlayerGrid);
        Vector3 targetPosition = new Vector3(closestNode.WorldPos.x, closestNode.WorldPos.y, closestNode.WorldPos.z - 0.75f);

        transform.position = targetPosition;
    }

    private void OnEnable()
    {
        if (InputManager.playerControls == null) { return; }
        InputManager.playerControls.Movement.Enable();
        InputManager.playerControls.ArcadeMovement.Enable();
    }

    // Called once before first update.
    void Start()
    {
        // Sets ref to AudioClip.
        endTurn = uiCanvas.GetComponentInChildren<PlayerActions>().EndTurnAudio;

        // Sets refs to pathfinding & gridManager scripts.
        GameObject gm = BattleInfo.gridManager;
        pathfinding = gm.GetComponent<Pathfinding>();
        gridManager = gm.GetComponent<GridManager>();       
    }

    bool movedToStartNode = false;

    // Called once each frame.
    void Update()
    {
        // Pushes player to start node.
        if (!movedToStartNode) 
        {
            movedToStartNode = true;
            MoveToStartNode();
        }

        // Checks for mouse movement.
        MouseTileMovement();
    }

    private void OnDisable()
    {
        InputManager.playerControls.Movement.Disable();
        InputManager.playerControls.ArcadeMovement.Disable();
    }
}