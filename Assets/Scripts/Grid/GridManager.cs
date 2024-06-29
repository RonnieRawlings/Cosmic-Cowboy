// Author - Ronnie Rawlings.

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;
using UnityEngine.UIElements;
using Unity.Burst.CompilerServices;
using UnityEngine.EventSystems;
using Fungus;

public class GridManager : MonoBehaviour
{
    // 2D Node Array, all grid nodes.
    private List<Node[,]> nodeGrids;

    // Obstacle layer, unwalkable.
    [SerializeField] private LayerMask unwalkableMask;

    // Grants access to all grids sizes.
    private List<CreateAGrid> gridData;

    // Is the player currently setting a path.
    private bool playerIsSettingPath;

    #region Variable Properties

    public List<Node[,]> NodeGrid
    {
        get { return nodeGrids; }
    }

    public List<CreateAGrid> GridData
    {
        get { return gridData; }
    }

    public bool PlayerIsSettingPath
    {
        get { return playerIsSettingPath; }
        set { playerIsSettingPath = value; }
    }

    #endregion

    /// <summary> constructor <c>GridManager</c> initilizes variables before onEnable, start, etc. </summary>
    GridManager()
    {
        // Initlizes grid lists, nodes & data.
        nodeGrids = new List<Node[,]>();
        gridData = new List<CreateAGrid>();        
    }

    /// <summary> method <c>CreateGrid</c> creates a grid using a node array, size defined by assigned vars. </summary>
    public void CreateGrid()
    {
        // Reset nodeGrids on re-create.
        nodeGrids.Clear();

        for (int i = 0; i < gridData.Count; i++)
        {
            // Initlizes node array.
            nodeGrids.Add(new Node[gridData[i].GridSizeX, gridData[i].GridSizeY]);

            // Finds bottom left of grid.
            Vector3 worldBottomLeft = gridData[i].transform.position - Vector3.right * gridData[i].GridWorldSize.x / 2 -
                Vector3.forward * gridData[i].GridWorldSize.y / 2;

            // Loops through each grid space (node).
            for (int x = 0; x < gridData[i].GridSizeX; x++)
            {
                for (int y = 0; y < gridData[i].GridSizeY; y++)
                {
                    // Finds middle of node (world point).
                    Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * (gridData[i].NodeRadius * 2) + gridData[i].NodeRadius) +
                        Vector3.forward * (y * (gridData[i].NodeRadius * 2) + gridData[i].NodeRadius);

                    // If sphere hits unwalkable layer, returns true.
                    bool isWalkable = !(Physics.CheckSphere(worldPoint, gridData[i].NodeRadius / 3, unwalkableMask));

                    // True if hit obj is on ladder layer.
                    bool isLadder = Physics.CheckSphere(worldPoint, gridData[i].NodeRadius / 3, LayerMask.GetMask("Ladder"));

                    // Finds ladderData script, assigns linked ladder to linkedNode.
                    GameObject linkedNode = null;
                    if (isLadder)
                    {
                        // Sends SphereCast down to find ladder obj.
                        RaycastHit hit;
                        Vector3 startPoint = worldPoint + new Vector3(0, 100f, 0);
                        if (Physics.SphereCast(startPoint, gridData[i].NodeRadius / 3, Vector3.down, out hit, Mathf.Infinity, LayerMask.GetMask("Ladder")))
                        {
                            // Gets ladderData comp, assigns linkedGrid to value.
                            LadderData ladderComp = hit.transform.GetComponent<LadderData>();
                            linkedNode = ladderComp.LinkedNode;
                        }

                        // Ladder node walkable override.
                        isWalkable = true;
                    }

                    // Create node at position, uses constructor to assign values.
                    nodeGrids[nodeGrids.Count - 1][x, y] = new Node(isWalkable, worldPoint, x, y, new Tuple<bool, Tuple<int, GameObject>>(isLadder, new Tuple<int, GameObject>(i, linkedNode)));
                }
            }
        }       
    }

    /// <summary> method <c>SetUpCollisionBox</c> sets the size of the attatched collision box relative to the grids size. </summary>
    public void SetUpCollisionBox()
    {
        // Gets ref to box collider.
        BoxCollider boxCollider = this.GetComponent<BoxCollider>();

        // Updates the box collider size relevant to grid size.
        Vector3 boxColliderSize = boxCollider.size;
        boxColliderSize.x = gridData[0].GridSizeX + 2;
        boxColliderSize.z = gridData[0].GridSizeY + 2;

        // Assigns the updated values.
        boxCollider.size = boxColliderSize;
    }

    /// <summary> method <c>FindNodeFromWorldPoint</c> finds the corrosponding node to the given world point, returns it. </summary>
    public Node FindNodeFromWorldPoint(Vector3 worldPoint, int gridIndex)
    {
        // Calculate the size of each node
        float nodeDiameter = gridData[gridIndex].NodeRadius * 2;
        float gridSizeX = gridData[gridIndex].GridSizeX * nodeDiameter;
        float gridSizeY = gridData[gridIndex].GridSizeY * nodeDiameter;

        foreach (Node node in nodeGrids[gridIndex])
        {
            if (node.WorldPos == worldPoint)
            {
                return node;
            }
        }

        // Calculate the position of the worldPoint relative to the bottom-left corner of the grid
        Vector3 relativePosition = worldPoint - gridData[gridIndex].transform.position + new Vector3(gridSizeX / 2, worldPoint.y, gridSizeY / 2);

        // Calculate the position of the node in the grid.
        int x = Mathf.Clamp(Mathf.RoundToInt(relativePosition.x / nodeDiameter), 0, gridData[gridIndex].GridSizeX - 1);
        int y = Mathf.Clamp(Mathf.RoundToInt(relativePosition.z / nodeDiameter), 0, gridData[gridIndex].GridSizeY - 1);

        // Returns corresponding node
        return nodeGrids[gridIndex][x, y];
    }

    /// <summary> method <c>FindNeighbourNodes</c> returns a list of all neighbouring nodes to given node. </summary>
    public List<Node> FindNeighbourNodes(Node node, int radius, int currentGrid)
    {
        // Finds neighbouring nodes, searches (2*radius + 1)x(2*radius + 1) around the node.
        List<Node> neighbouringNodes = new List<Node>();
        for (int x = -radius; x <= radius; x++)
        {
            for (int y = -radius; y <= radius; y++)
            {
                // Skip the center node.
                if (x == 0 && y == 0) { continue; }

                // Updates values to actual node values.
                int neighbourX = node.IndexX + x;
                int neighbourY = node.IndexY + y;

                // Checks if neighbour is within grid bounds.
                if (neighbourX >= 0 && neighbourX < gridData[currentGrid].GridSizeX && neighbourY >= 0 && neighbourY < gridData[currentGrid].GridSizeY)
                {
                    // Node is a neighbour, add to list.
                    neighbouringNodes.Add(nodeGrids[currentGrid][neighbourX, neighbourY]);
                }
            }
        }

        // Return list of all neighbouring nodes.
        return neighbouringNodes;
    }

    /// <summary> method <c>FindNeighbourNodesFullList</c> returns a list of all neighbouring nodes to given node but at specific indexs. </summary>
    public List<Node> FindNeighbourNodesFullList(Node node, int currentGrid)
    {
        // Initialize a list with 9 elements (null by default)
        List<Node> neigbouringNodes = new List<Node>(new Node[9]);

        // Finds neighbouring nodes, searches 3x3 around the node.
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                // 0,0 is the center node, so skip.
                if (x == 0 && y == 0) { continue; }

                // Updates values to actual node values.
                int neighbourX = node.IndexX + x;
                int neighbourY = node.IndexY + y;

                // Checks if neighbour is within grid bounds.
                if (neighbourX >= 0 && neighbourX < gridData[currentGrid].GridSizeX && neighbourY >= 0 && neighbourY < gridData[currentGrid].GridSizeY)
                {
                    // Node is a neighbour, add to list at specific index.
                    neigbouringNodes[(x + 1) * 3 + (y + 1)] = nodeGrids[currentGrid][neighbourX, neighbourY];
                }
            }
        }

        // Return list of all neighbouring nodes (including nulls).
        return neigbouringNodes;
    }

    // Was cam set behind.
    private bool camWasBehind = false;

    /// <summary> method <c>ChangeGridVisualsEnabledStatus</c> changes the active status of the gridVisuals script on key press. </summary>
    public void ChangeGridVisualsEnabledStatus()
    {
        // Assigns comp to var.
        GridVisuals gridVisuals = GetComponent<GridVisuals>();

        // Grid visual to be disabled upon enemy select.
        if (BattleInfo.camBehind) 
        {
            // Prevents multiple foreach loops.
            if (camWasBehind) { return; }
            camWasBehind = true;

            // Disables active nodes.
            foreach (Node node in gridVisuals.nodesInRange)
            {
                BattleInfo.nodeObjects[node].SetActive(false);
            }

            gridVisuals.enabled = false; 
            return; 
        }
        else { camWasBehind = false; gridVisuals.enabled = true; }

        // If a path is being set gridVisuals can't be disabled.
        if (playerIsSettingPath)
        {
            gridVisuals.enabled = true;
            return;
        }

        // Changes active status on key press down.
        if (Input.GetKeyDown(KeyCode.F2))
        {
            // Disables active nodes.
            foreach (Node node in gridVisuals.nodesInRange)
            {
                BattleInfo.nodeObjects[node].SetActive(false);
            }

            // Changes active status.
            gridVisuals.enabled = !gridVisuals.enabled;           
        }
        

        // Switches if grid can be attachted to the mouse.
        if (Input.GetKeyDown(KeyCode.F3) && gridVisuals.isActiveAndEnabled)
        {
            BattleInfo.gridMouseAllowed = !BattleInfo.gridMouseAllowed;
            gridVisuals.VisualizeGridWhenCreated(true);
        }
    }

    // Called once when active & enabled.
    void OnEnable()
    {
        // Sets ref to itself.
        BattleInfo.gridManager = this.gameObject;

        // Enables other scripts, prevents enable order.
        this.GetComponent<Pathfinding>().enabled = true;

        // Adds all grids data to gridData.
        foreach (Transform child in transform)
        {
            if (child.GetComponent<CreateAGrid>() != null)
            {
                gridData.Add(child.GetComponent<CreateAGrid>());
                Debug.Log("True");
            }
            
        }
    }

    // All grid meshRends in scene.
    [SerializeField] private List<MeshCollider> gridMeshes;

    /// <summary> method <c>GridMeshes</c> assigns all mesh renderers on grid layer to relvant grids, allows easy enable switching. </summary>
    public void GridMeshes()
    {
        // Fills list with meshRends if first call.
        if (gridMeshes.Count == 0) 
        {
            // Get all MeshRenderers in the scene.
            MeshCollider[] allMeshRenderers = FindObjectsOfType<MeshCollider>();

            // Grid layer index.
            int gridLayer = LayerMask.NameToLayer("Grid");

            // Only add meshRenderers on the grid layer.
            foreach (MeshCollider renderer in allMeshRenderers)
            {
                if (renderer.gameObject.layer == gridLayer)
                {
                    gridMeshes.Add(renderer);
                }
            }
        }
    }

    /// <summary> method <c>ChangeGridMeshActiveStatus</c> switches all meshRends off if above the player for that grid. </summary>
    public void ChangeGridMeshActiveStatus()
    {
        // Prevents higher grids being shown, obscuring camera view.
        foreach (CreateAGrid grid in gridData)
        {
            // Disables collider on higher grids, enables if not higher.
            float offset = 0.5f, variance = 0.2f;
            if (grid.transform.position.y > (BattleInfo.player.transform.position.y + offset))
            {
                // Disable the MeshRenderers that belong to this grid.
                foreach (MeshCollider meshRend in gridMeshes)
                {
                    if (Math.Abs(meshRend.transform.position.y - grid.transform.position.y) <= variance)
                    {
                        meshRend.enabled = false;
                    }
                }
            }
            else
            {
                // Enable the MeshRenderers that belong to this grid.
                foreach (MeshCollider meshRend in gridMeshes)
                {
                    if (Math.Abs(meshRend.transform.position.y - grid.transform.position.y) <= variance)
                    {
                        meshRend.enabled = true;
                    }
                }
            }
        }
    }

    // Called once before first update.
    void Start()
    {
        // Calls method to create the node array.
        Debug.Log("Grid Create");
        CreateGrid();

        // Fills gridMeshes list.
        GridMeshes();
    }

    // Called once every frame.
    void Update()
    {
        // Changes gridVisuals active status.
        ChangeGridVisualsEnabledStatus();

        // Prevents grid from obscuring camera view.
        ChangeGridMeshActiveStatus();
    }
}