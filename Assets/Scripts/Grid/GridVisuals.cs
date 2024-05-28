// Author - Ronnie Rawlings.

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.ProBuilder;

public class GridVisuals : MonoBehaviour
{
    // Ref to grid material.
    [SerializeField] private Material gridMat;

    // Ref to obj for grid nodes.
    [SerializeField] GameObject gridObj;

    // Ref to grid manager script.
    private GridManager gridM;

    // Best path from mouse to player.
    public List<Node> mouseToPlayer;

    private PlayerMovement playerMovement;

    // Has path been selected.
    private bool setPath = false;

    #region Variable Properties

    public bool SetPath 
    {
        get { return setPath; }
        set { setPath = value; }
    }

    #endregion

    /// <summary> method <c>DrawGridInEditor</c> only draws the grid when the application is running. </summary>
    public void DrawGridInEditor()
    {
        // Used to visualize grid size in editor.
        if (!Application.isPlaying)
        {
            gridM = GetComponent<GridManager>();

            for (int i = 0; i < gameObject.transform.childCount; i++)
            {
                CreateAGrid grid = gameObject.transform.GetChild(i).GetComponent<CreateAGrid>();
                if (grid != null)
                {
                    Gizmos.DrawWireCube(grid.transform.position, new Vector3(grid.GridWorldSize.x, 0.01f, grid.GridWorldSize.y));
                }
            }
        }
    }

    /// <summary> method <c>VisualizeGridWhenCreated</c> finds mouse/player nodes & calls grid drawing methods when grid is available. </summary>
    public void VisualizeGridWhenCreated(bool overrideValue)
    {
        // Visualize grid when created.
        if (gridM.NodeGrid != null)
        {
            // Converts mouse/player pos to node.
            Node mouseNode = FindNodeFromMousePos();
            Node playerNode = FindNodeFromPlayerPos();

            // Draws gizmos for area around mouse/player.
            DrawSurroundingNodes(mouseNode, playerNode, overrideValue);

            // Draws gizmos for selected path.
            DrawPath();
        }
    }

    /// <summary> method <c>FindNodeFromMousePos</c> returns the closest node to current mouse position. </summary>
    public Node FindNodeFromMousePos()
    {
        // Calculates cursor position.
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // Defines grid mask.
        int layerMask = LayerMask.GetMask("Grid");
        Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask);

        // Actual mouse node from point.
        Node closestNode = null;

        // Tries each grid, closest distance is the correct node.
        float closestDistance = Mathf.Infinity;
        for (int i = 0; i < gridM.NodeGrid.Count; i++)
        {
            Node node = gridM.FindNodeFromWorldPoint(hit.point, i);
            float distance = Vector3.Distance(hit.point, node.WorldPos);
            if (distance < closestDistance)
            {
                closestNode = node;
                closestDistance = distance;
            }
        }

        // Returns mouseNode ref.
        return closestNode;
    }

    /// <summary> method <c>FindNodeFromPlayerPos</c> returns the closest node to current player position. </summary>
    public Node FindNodeFromPlayerPos()
    {
        // Get node from player pos.
        return gridM.FindNodeFromWorldPoint(BattleInfo.player.transform.position, BattleInfo.currentPlayerGrid);
    }

    /// <summary> method <c>DrawGrid</c> draws a visible node for each node in the grid, near invisible. </summary>
    public void DrawGrid()
    {
        // Loops through all grid nodes, creates visuals.
        foreach (Node n in gridM.NodeGrid[0])
        {
            // World position.
            Vector3 nodePos = new Vector3(n.WorldPos.x, 0, n.WorldPos.z);

            // Check if the node is within the camera's view.
            if (IsNodeVisible(nodePos))
            {
                // Set alpha value, depends on bool.
                float alpha = n.Walkable ? 0.05f : 0.13f;

                // Check if the node is on the boundary of an unwalkable object.
                if (n.Walkable)
                {
                    
                }
            }
        }
    }


    /// <summary> method <c>IsNodeVisible</c> checks if the current node is visible by the camera. </summary>
    private bool IsNodeVisible(Vector3 nodePos)
    {
        // Convert the node pos to viewport pos.
        Vector3 viewportPos = Camera.main.WorldToViewportPoint(nodePos);

        // Is within horizontal bounds.
        bool inHBounds = viewportPos.x >= 0 && viewportPos.x <= 1;

        // Is within vertical bounds.
        bool inVBounds = viewportPos.y >= 0 && viewportPos.y <= 1;

        // Is in front of camera.
        bool isInFrontOfCamera = viewportPos.z > 0;

        // Is the node compeletly within camera's view.
        return inHBounds && inVBounds && isInFrontOfCamera;
    }

    // Surrounding node raidus shown.
    private int cursorRadius = 10;

    List<Node> allNodes;

    // Nodes in range of reference node.
    public HashSet<Node> nodesInRange;

    // Previous nodes on last check.
    private Node previousRefNode;

    /// <summary> method <c>DrawSurroundingNodes</c> draws wireframe cubes for all nodes surrounding mouse/player (in set radius). </summary>
    public void DrawSurroundingNodes(Node mouseNode, Node playerNode, bool overrideSameNode)
    {
        if (overrideSameNode) { changedNodes.Clear(); }

        // Player if list set, else mouse.
        Node referenceNode = mouseToPlayer != null ? playerNode : mouseNode;

        // Changes ref to player if action is hovered.
        if (BattleInfo.showRange && mouseToPlayer == null || !BattleInfo.gridMouseAllowed)
        {
            referenceNode = playerNode;
        }
        else if (BattleInfo.showDetectionRange && mouseToPlayer == null)
        {
            // Ref node is enemy node, if enemy hovered.
            referenceNode = gridM.FindNodeFromWorldPoint(BattleInfo.hoveredEnemy.transform.position,
                BattleInfo.hoveredEnemy.GetComponent<AIMovement>().CurrentGrid);
        }

        // If the referenceNode is the same as the previousRefNode, return from the method
        if (referenceNode == previousRefNode && !overrideSameNode)
        {
            return;
        }

        // Update the previousRefNode to the current referenceNode
        previousRefNode = referenceNode;

        // Set of nodes currently in range.
        nodesInRange.Clear();

        // Decrease cursor radius if showRange isn't true.
        if (!BattleInfo.showRange)
        {
            cursorRadius = (BattleValues.playerTileAmount - playerMovement.TilesMoved);
        }      
        else
        {
            cursorRadius = BattleInfo.playerWeapon.range;
        }

        // Enables objs for nodes around mouse/player.
        foreach (Node n in allNodes)
        {
            // Calculate grid distance between nodes, chebyshev distance.
            float dx = Mathf.Abs(n.WorldPos.x - referenceNode.WorldPos.x);
            float dy = Mathf.Abs(n.WorldPos.y - referenceNode.WorldPos.y);
            float dz = Mathf.Abs(n.WorldPos.z - referenceNode.WorldPos.z);
            float distance = Mathf.Max(dx, dy, dz);

            // Enables in range nodes.
            if (distance <= cursorRadius * 2 * gridM.GridData[0].NodeRadius && BattleInfo.nodeObjects.ContainsKey(n))
            {
                if (playerMovement.startDragOnPlayer)
                {
                    if (!IsNodeReachableWithinMoveLimit(n, playerNode, (BattleValues.playerTileAmount - playerMovement.TilesMoved)))
                    {
                        nodesInRange.Remove(n);
                        BattleInfo.nodeObjects[n].SetActive(false);
                    }
                }
                else
                {
                    nodesInRange.Add(n);
                }

                // Update the color of the existing GameObject
                GameObject existingObject = BattleInfo.nodeObjects[n];
                MeshRenderer existingRenderer = existingObject.GetComponent<MeshRenderer>();

                if (!BattleInfo.showRange)
                {
                    // Calculate alpha based on distance, closer nodes have higher alpha
                    float maxDistance = cursorRadius * 2 * gridM.GridData[0].NodeRadius;

                    // Determine node alpha value.
                    float alpha;
                    if (playerMovement.startDragOnPlayer)
                    {
                        alpha = 1f;
                    }
                    else
                    {
                        alpha = Mathf.Clamp(1f - (distance / maxDistance), 0.1f, 1f);
                    }

                    // Ladder tiles set to original, outranks normal colour.
                    if (n.IsLadder)
                    {
                        existingRenderer.material.color = new Color(1, 0.5f, 0, alpha);
                    }
                    else
                    {
                        // Sets walkable colour, yellow if 1 AP remaining.
                        Color walkableColour;
                        if (BattleInfo.currentActionPoints == 1)
                        {
                            walkableColour = new Color(0.6f, 0.6f, 0.3f, alpha);
                        }
                        else
                        {
                            walkableColour = new Color(0.5f, 0.5f, 1f, alpha);
                        }

                        // Not a ladder, red for unwalkable/white for walkable.
                        existingRenderer.material.color = n.Walkable ? walkableColour : new Color(0.15f, 0f, 0f, alpha);
                    }

                    // Enable the GameObject
                    if (nodesInRange.Contains(n))
                    {
                        existingObject.SetActive(true);
                    }
                }
            }
            else
            {
                // If the node is not in range, disable the GameObject
                if (BattleInfo.nodeObjects.ContainsKey(n))
                {
                    BattleInfo.nodeObjects[n].SetActive(false);
                }
            }
        }
    }

    /// <summary> method <c>IsNodeReachableWithinMoveLimit</c> checks wether given node can be reached within player's moves. </summary>
    public bool IsNodeReachableWithinMoveLimit(Node targetNode, Node startNode, int moveLimit)
    {
        if (!targetNode.Walkable) { return false; }

        List<Node> path = BattleInfo.gridManager.GetComponent<Pathfinding>().FindPath(startNode.WorldPos, 
            targetNode.WorldPos, BattleInfo.currentPlayerGrid);
        return path.Count <= moveLimit;
    }

    // Trakcs altered nodes, colour & node.
    private List<KeyValuePair<Node, Color>> changedNodes = new List<KeyValuePair<Node, Color>>();

    /// <summary> method <c>DrawPath</c> draws a wireframe cube for each node in mouseToPlayer, colour depends on setPath. </summary>
    public void DrawPath()
    {
        if (playerMovement.startDragOnPlayer) 
        { 
            UpdateNodesInRange(); 
        }
        else if (shownInRange)
        {
            shownInRange = false;
        }

        // Reset the color of the nodes in changedNodes.
        foreach (KeyValuePair<Node, Color> pair in changedNodes)
        {
            Node node = pair.Key;
            Color originalColor = pair.Value;

            if (BattleInfo.nodeObjects.ContainsKey(node))
            {
                // Reset the color to its original color.
                BattleInfo.nodeObjects[node].GetComponent<MeshRenderer>().material.color = originalColor;
            }
        }

        // Clears list after reseting.
        changedNodes.Clear();

        if (mouseToPlayer != null)
        {
            // Choose the color based on the value of setPath.
            Color colorToSet = !setPath ? Color.green : Color.blue;

            foreach (Node node in mouseToPlayer)
            {
                if (BattleInfo.nodeObjects.ContainsKey(node))
                {
                    // Store the original color
                    Color originalColor = BattleInfo.nodeObjects[node].GetComponent<MeshRenderer>().material.color;

                    // Change the color
                    BattleInfo.nodeObjects[node].GetComponent<MeshRenderer>().material.color = colorToSet;

                    // Add the node and its original color to changedNodes
                    changedNodes.Add(new KeyValuePair<Node, Color>(node, originalColor));
                }
            }
        }
    }

    private bool shownInRange = false;

    public void UpdateNodesInRange()
    {
        if (shownInRange) { return; }
        shownInRange = true;

        Node playerNode = FindNodeFromPlayerPos();

        // Create a temporary list to store nodes to be removed
        List<Node> nodesToRemove = new List<Node>();

        foreach (Node n in nodesInRange)
        {
            if (playerMovement.startDragOnPlayer)
            {
                if (!IsNodeReachableWithinMoveLimit(n, playerNode, (BattleValues.playerTileAmount - playerMovement.TilesMoved)))
                {
                    // Add the node to the temporary list instead of removing it directly
                    nodesToRemove.Add(n);
                    BattleInfo.nodeObjects[n].SetActive(false);
                }
                else
                {
                    Color currentColour = BattleInfo.nodeObjects[n].GetComponent<MeshRenderer>().material.color;
                    currentColour.a = 1f;
                    BattleInfo.nodeObjects[n].GetComponent<MeshRenderer>().material.color = currentColour;
                }
            }
        }

        // Remove the nodes in the temporary list from nodesInRange
        foreach (Node n in nodesToRemove)
        {
            nodesInRange.Remove(n);
        }
    }


    /// <summary> method <c>ShowPlayerRange</c> visually shows the player's current range when hovering over a button, shown in yellow. </summary>
    public IEnumerator ShowRange(Node givenNode, int range, int gridIndex)
    {
        cursorRadius = range;

        // Get all nodes in player's range
        List<Node> localNodesInRange = GetComponent<GridManager>().FindNeighbourNodes(givenNode, range, gridIndex);

        // Group nodes by distance to the givenNode and sort in ascending order
        var groupedNodes = localNodesInRange.GroupBy(n => Vector3.Distance(n.WorldPos, givenNode.WorldPos)).OrderBy(g => g.Key);

        foreach (var group in groupedNodes)
        {
            if (!BattleInfo.showRange)
            {
                yield break; // Stop the coroutine if showRange is set to false
            }

            foreach (Node n in group)
            {
                if (BattleInfo.nodeObjects.ContainsKey(n))
                {
                    GameObject existingObject = BattleInfo.nodeObjects[n];
                    MeshRenderer existingRenderer = existingObject.GetComponent<MeshRenderer>();
                    existingRenderer.material.color = new Color(0.5f, 0, 0.5f, 1);

                    existingObject.SetActive(true);
                }
            }

            // Wait for a short period of time before changing the color of the next group of nodes
            yield return new WaitForSeconds(0.02f);
        }        
    }

    /// <summary> method <c>InstantiateGridObjects</c> instanties an obj for each valid grid node. </summary>
    public void InstantiateGridObjects()
    {
        // If nodeObjects isn't empty, destroy all GameObjects.
        if (BattleInfo.nodeObjects != null && BattleInfo.nodeObjects.Count > 0)
        {
            foreach (Node n in BattleInfo.nodeObjects.Keys)
            {
                Destroy(BattleInfo.nodeObjects[n]);
            }
            BattleInfo.nodeObjects.Clear();
        }

        // Create a list to hold all nodes from all grids.
        allNodes = new List<Node>();

        // Add all nodes from all grids to the list.
        foreach (Node[,] grid in gridM.NodeGrid)
        {
            foreach (Node n in grid)
            {
                allNodes.Add(n);
            }
        }

        float radius = gridM.GridData[0].NodeRadius;

        // Instantiate all nodes at the start.
        foreach (Node n in allNodes)
        {
            bool allEdgesOnGrid = true;
            if (!n.IsLadder)
            {
                // Calculate the positions of the four edges of the GameObject.
                Vector3[] edges = new Vector3[4];
                edges[0] = n.WorldPos + new Vector3(radius - 0.1f, 0, 0);  // Right edge
                edges[1] = n.WorldPos - new Vector3(radius - 0.1f, 0, 0);  // Left edge
                edges[2] = n.WorldPos + new Vector3(0, 0, radius - 0.1f);  // Forward edge
                edges[3] = n.WorldPos - new Vector3(0, 0, radius - 0.1f);  // Backward edge

                foreach (Vector3 edge in edges)
                {
                    // Raycast downwards from each edge to check if it hits a grid layer.
                    RaycastHit hit;
                    if (!Physics.Raycast(edge, Vector3.down, out hit, 2, LayerMask.GetMask("Grid")))
                    {
                        allEdgesOnGrid = false;
                        break;
                    }
                }
            } 

            // If all edges hit the grid layer, the entire GameObject is within the grid layer.
            if (allEdgesOnGrid)
            {
                // If the node doesn't have a GameObject yet, create one.
                if (!BattleInfo.nodeObjects.ContainsKey(n))
                {
                    // Instantiate gridObj prefab.
                    GameObject nodeObject = Instantiate(gridObj, n.WorldPos, Quaternion.identity);
                    nodeObject.transform.Rotate(0, 0, 90);

                    // Adjust the size of the GameObject.
                    nodeObject.transform.localScale = new Vector3(nodeObject.transform.localScale.x * 0.9f, nodeObject.transform.localScale.y
                        * 0.9f, nodeObject.transform.localScale.z * 0.9f);

                    // Add the GameObject to the dictionary.
                    BattleInfo.nodeObjects[n] = nodeObject;
                }
            }
        }
    }

    // Called when script instance is loaded.
    void Awake()
    {
        nodesInRange = new HashSet<Node>();
    }

    private bool firstCall = true;

    // Called once before first update.
    void OnEnable()
    {
        // Sets script ref.
        gridM = GetComponent<GridManager>();

        if (!firstCall) { VisualizeGridWhenCreated(true); }
        firstCall = false;
    }

    void Start()
    {
        playerMovement = BattleInfo.player.GetComponent<PlayerMovement>();
    }

    bool instatitatedObjs = false;

    private void Update()
    {
        // If objs haven't been created, create them.
        if (!instatitatedObjs)
        {
            InstantiateGridObjects();
            instatitatedObjs = true;

            // Draws grid around mouse/player when grid available.
            VisualizeGridWhenCreated(true);
        }

        // Draws grid around mouse/player when grid available.
        VisualizeGridWhenCreated(false);
    }

    void OnDrawGizmos()
    {
        // Only draws grid when editor is running.
        DrawGridInEditor();    
    }
}