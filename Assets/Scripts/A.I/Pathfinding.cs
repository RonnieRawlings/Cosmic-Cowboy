// Author - Ronnie Rawlings.

using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class Pathfinding : MonoBehaviour
{
    // Ref to the gridManager script.
    private GridManager nodeGrid;

    /// <summary> method <c>FindPath</c> determines the best path from the start position to target position. </summary>
    public List<Node> FindPath(Vector3 startPos, Vector3 targetPos, int currentGrid)
    {
        // Converts world positions to Nodes.
        Node startNode = nodeGrid.FindNodeFromWorldPoint(startPos, currentGrid);
        Node targetNode = nodeGrid.FindNodeFromWorldPoint(targetPos, currentGrid);

        // Lists containing unchecked & checked nodes.
        List<Node> openNodes = new List<Node>();
        List<Node> closedNodes = new List<Node>();

        // Adds start node to lists, loops while list isn't empty.
        openNodes.Add(startNode);
        while (openNodes.Count > 0)
        {
            // First node in list.
            Node currentNode = openNodes[0];

            // Loops through all remaining open nodes, finds cheapest.
            for (int i = 1; i < openNodes.Count; i++)
            {
                // If i is cheaper OR closer, assigns as current.
                if (openNodes[i].fCost < currentNode.fCost || openNodes[i].fCost == currentNode.fCost)
                {
                    if (openNodes[i].HCost < currentNode.HCost)
                    {
                        currentNode = openNodes[i];
                    }
                }
            }

            // Checked node removed from open, added to closed.
            openNodes.Remove(currentNode);
            closedNodes.Add(currentNode);

            // Target node has been found, retrace path then exit method.
            if (currentNode == targetNode)
            {
                // Retraces the node path taken, then returns.
                return RetracePath(startNode, targetNode);
            }

            // Loop through all neighbour nodes.
            foreach (Node neighbour in nodeGrid.FindNeighbourNodes(currentNode, 1, currentGrid))
            {
                // Skip node if unwalkable OR already checked OR not in BattleInfo.nodeObjects.
                if (!neighbour.Walkable || neighbour.Occupied != null || closedNodes.Contains(neighbour) || !BattleInfo.nodeObjects.ContainsKey(neighbour)) { continue; }

                // Find neighbour node cost.
                int newMovementCost = currentNode.GCost + FindDistanceBetweenNodes(currentNode, neighbour);

                // Is new cost less & is node open.
                if (newMovementCost < neighbour.GCost || !openNodes.Contains(neighbour))
                {
                    // Set g & h costs, set parent node.
                    neighbour.GCost = newMovementCost;
                    neighbour.HCost = FindDistanceBetweenNodes(neighbour, targetNode);
                    neighbour.Parent = currentNode;

                    // Add node to open list if not already in.
                    if (!openNodes.Contains(neighbour))
                    {
                        openNodes.Add(neighbour);
                    }
                }
            }
        }

        // If no path is found, return null or an empty list.
        return new List<Node>();
    }


    /// <summary> method <c>RetracePath</c> retraces the path taken from start node to target node. </summary>
    private List<Node> RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.Parent;
        }
        path.Reverse();

        return path;
    }

    /// <summary> method <c>FindDistanceBetweenNodes</c> calculates the distance between two given nodes, returns result. </summary>
    public int FindDistanceBetweenNodes(Node nodeA, Node nodeB)
    {
        // Finds x & y difference.
        int distanceX = Mathf.Abs(nodeA.IndexX - nodeB.IndexX);
        int distanceY = Mathf.Abs(nodeA.IndexY - nodeB.IndexY);

        // Lowest value is always taken away from highest value.
        if (distanceX > distanceY)
        {
            // Formula for calculating distance.
            return 14 * distanceY + 10 * (distanceX - distanceY);
        }
        else
        {
            // Formula for calculating distance.
            return 14 * distanceX + 10 * (distanceY - distanceX);
        }
    }

    // Called once when active & enabled.
    private void OnEnable()
    {
        // Sets ref to gridManager script.
        nodeGrid = BattleInfo.gridManager.GetComponent<GridManager>();
    }
}