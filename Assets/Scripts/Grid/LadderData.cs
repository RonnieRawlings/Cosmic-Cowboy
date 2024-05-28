// Author - Ronnie Rawlings.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LadderData : MonoBehaviour
{
    // Ref to the grid ladder goes to.
    [SerializeField] private GameObject linkedNode, grid;

    // Nodes gridIndex, grid the node is on. 
    private int gridIndex;

    #region Variable Properties

    public GameObject LinkedNode
    {
        get { return linkedNode; }
    }

    public int GridIndex
    {
        get { return gridIndex; }
    }

    #endregion

    // Called once when script instance is loaded.
    private void Awake()
    {
        // Sets gridIndex for player use.
        gridIndex = grid.transform.GetSiblingIndex();
    }
}
