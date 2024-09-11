// Author - Ronnie Rawlings.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseAI : MonoBehaviour
{
    // Node the AI currently occupies.
    protected Node currentNode;

    // Grid the AI is currently on.
    [SerializeField] protected int currentGrid = 0;

    #region Properties

    public Node CurrentNode
    {
        get { return currentNode; }
        set { currentNode = value; }
    }

    public int CurrentGrid
    {
        get { return currentGrid; }
    }

    #endregion

    /// <summary> method <c>SetStartNode</c> sets currentNode to first node. </summary>
    public void SetStartNode(int currentGrid)
    {
        // Sets AIs starting node.
        currentNode ??= BattleInfo.gridManager.
            GetComponent<GridManager>().FindNodeFromWorldPoint(transform.position, currentGrid);
    }

    /// <summary> coroutine <c>SetFirstOccupied</c> waits until first frame end, finds starting node & sets. </summary>
    public IEnumerator SetFirstOccupied(int currentGrid)
    {
        yield return new WaitForEndOfFrame();

        // Sets occupied status of start node.
        BattleInfo.gridManager.GetComponent<GridManager>().FindNodeFromWorldPoint(transform.position, currentGrid).
            Occupied = this.gameObject;

        // Push AI unit to start node middle.
        Node startNode = BattleInfo.gridManager.GetComponent<GridManager>().FindNodeFromWorldPoint(transform.position, currentGrid);
        transform.position = new Vector3(startNode.WorldPos.x, transform.position.y, startNode.WorldPos.z - 0.75f);
    }
}