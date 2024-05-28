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

    /// <summary> method <c>WASDTileMovement</c> allows player to move to surrounding tiles using WASD or Arrow Keys. </summary>
    public void WASDTileMovement()
    {
        // Camera is in comic anim.
        if (Camera.main.transform.parent == null) { return; }

        // Player can only move on their turn.
        if (BattleInfo.playerTurn && !BattleInfo.aiTurn)
        {
            if (InputManager.playerControls.Movement.SubmitPath.WasPressedThisFrame())
            {
                StartCoroutine(MovePlayer(gridManager.GetComponent<GridVisuals>().mouseToPlayer));
            }

            // If no input received, no movement will occur.
            int directionIndex = -1;

            // Get the camera's forward and right vectors, ignore Y-axis.
            Vector3 cameraForward = Vector3.Scale(Camera.main.transform.parent.forward, new Vector3(1, 0, 1)).normalized;
            Vector3 cameraRight = Vector3.Scale(Camera.main.transform.parent.right, new Vector3(1, 0, 1)).normalized;

            if (InputManager.isArcade)
            {
                // Checks for WASD, Arrow Keys, and D-Pad key presses.
                if (InputManager.playerControls.ArcadeMovement.MoveForward.WasPressedThisFrame())
                {
                    directionIndex = GetDirectionIndex(cameraForward);
                }
                else if (InputManager.playerControls.ArcadeMovement.MoveLeft.WasPressedThisFrame())
                {
                    directionIndex = GetDirectionIndex(-cameraRight);
                }
                else if (InputManager.playerControls.ArcadeMovement.MoveRight.WasPressedThisFrame())
                {
                    directionIndex = GetDirectionIndex(cameraRight);
                }
                else if (InputManager.playerControls.ArcadeMovement.MoveBack.WasPressedThisFrame())
                {
                    directionIndex = GetDirectionIndex(-cameraForward);
                }
            }
            else
            {
                // Checks for WASD, Arrow Keys, and D-Pad key presses.
                if (InputManager.playerControls.Movement.MoveForward.WasPressedThisFrame())
                {
                    directionIndex = GetDirectionIndex(cameraForward);
                }
                else if (InputManager.playerControls.Movement.MoveLeft.WasPressedThisFrame())
                {
                    directionIndex = GetDirectionIndex(-cameraRight);
                }
                else if (InputManager.playerControls.Movement.MoveRight.WasPressedThisFrame())
                {
                    directionIndex = GetDirectionIndex(cameraRight);
                }
                else if (InputManager.playerControls.Movement.MoveBack.WasPressedThisFrame())
                {
                    directionIndex = GetDirectionIndex(-cameraForward);
                }
            }

            // Moves the player if an input has been received.
            if (directionIndex != -1)
            {
                // Actually moves the player.
                SetPathWASD(directionIndex);
            }
        }
    }

    /// <summary> method <c>GetDirectionIndex</c> returns the direction index based on the camera's orientation. </summary>
    public int GetDirectionIndex(Vector3 direction)
    {
        // Calculate angle between direction and world's forward vector.
        float angle = Vector3.SignedAngle(direction, Vector3.forward, Vector3.up);

        // Convert angle range from [-180, 180] to [0, 360].
        if (angle < 0) angle += 360;

        // Return direction index based on angle.
        if (angle >= 45 && angle < 135) return 1; // Left
        else if (angle >= 135 && angle < 225) return 3; // Down
        else if (angle >= 225 && angle < 315) return 7; // Right
        else return 5; // Up
    }

    /// <summary> method <c>MovePlayer</c> moves the player to the given node index using neighbouring nodes. </summary>
    public void SetPathWASD(int directionIndex)
    {
        // Creates neighobur list & resets setPath.
        List<Node> neighbouringNodes;
        gridManager.GetComponent<GridVisuals>().SetPath = false;

        // Creates list if empty.
        if (WASDNodeList == null && tilesMoved < BattleValues.playerTileAmount) 
        { 
            WASDNodeList = new List<Node>();

            // Finds all neighbouring nodes (3x3 radius).
            neighbouringNodes = gridManager.FindNeighbourNodesFullList(gridManager.FindNodeFromWorldPoint(transform.position, BattleInfo.currentPlayerGrid), BattleInfo.currentPlayerGrid);
        }
        else if (WASDNodeList.Count < BattleValues.playerTileAmount && (tilesMoved + WASDNodeList.Count) < BattleValues.playerTileAmount)
        {
            neighbouringNodes = gridManager.FindNeighbourNodesFullList(gridManager.FindNodeFromWorldPoint(
                gridManager.GetComponent<GridVisuals>().mouseToPlayer[gridManager.GetComponent<GridVisuals>().mouseToPlayer.Count - 1].
                    WorldPos, BattleInfo.currentPlayerGrid), BattleInfo.currentPlayerGrid);
        }
        else
        {
            return;
        }

        // Only adds node if it's walkable, not occupied, & present in the level.
        if (neighbouringNodes[directionIndex].Walkable && neighbouringNodes[directionIndex].Occupied == null && 
            BattleInfo.nodeObjects.ContainsKey(neighbouringNodes[directionIndex]))
        {
            WASDNodeList.Add(neighbouringNodes[directionIndex]);
            gridManager.GetComponent<GridVisuals>().mouseToPlayer = WASDNodeList;
        }
        else if (WASDNodeList.Count == 0) { WASDNodeList = null; }
    }

    public bool startDragOnPlayer = false;

    /// <summary> method <c>MouseTileMovement</c> allows the player to move to surrounding tiles using mouse click/move. </summary>
    public void MouseTileMovement()
    {
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

        // Waits before changing turns.
        yield return new WaitForSeconds(0.75f);

        // Allows gridVisuals to be disabled again.
        gridManager.PlayerIsSettingPath = false;

        // Resets path variable.
        gridManager.GetComponent<GridVisuals>().mouseToPlayer = null;

        // Changes turn.
        UpdateTurn();
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

            // Changes turn.        
            BattleInfo.aiTurn = true;
            tilesMoved = 0;

            // Resets quickDraw value.
            GameObject.Find("UICanvas").GetComponentInChildren<PlayerActions>().HasQuickDrawn = false;

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

        // Prevents checking if in ArcadeMode & not in move mode.
        if (InputManager.isArcade && InputManager.actionBarMode) { return; }

        // Allows player to move on key press.
        WASDTileMovement();

        // Finds path from player to mouse node.
        if (!InputManager.isArcade) { MouseTileMovement(); }
    }

    private void OnDisable()
    {
        InputManager.playerControls.Movement.Disable();
        InputManager.playerControls.ArcadeMovement.Disable();
    }
}