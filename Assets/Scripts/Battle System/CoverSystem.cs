// Author - Ronnie Rawlings.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoverSystem : MonoBehaviour
{
    /// <summary> method <c>CheckForCover</c> returns true if position is next to an unwalkable node. </summary>
    public bool CheckForCover(Transform position)
    {
        // Player's current node
        Node playerNode = GetComponent<GridManager>().FindNodeFromWorldPoint(position.position, 0);

        // Neighboring nodes within a 1 node radius.
        List<Node> neighboringNodes = GetComponent<GridManager>().FindNeighbourNodesFullList(playerNode, BattleInfo.currentPlayerGrid);

        // Directly adjacent nodes (Up, Down, Left, Right).
        int[] indices = { 1, 3, 5, 7 };

        // Check each directly adjacent node
        foreach (int i in indices)
        {
            // Check if unwalkable node is found.
            if (neighboringNodes[i] != null && !neighboringNodes[i].Walkable)
            {
                return true;
            }                
        }

        // No cover, return false
        return false;
    }
}
