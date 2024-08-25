// Author - Ronnie Rawlings.

using System;
using UnityEngine;

public class Node
{
    // Is this node walkable.
    private bool walkable;

    // Is this node a health station.
    private bool healthStation;

    private GameObject healthStationObj;

    // LadderNode data.
    private Tuple<bool, Tuple<int, GameObject>> ladderNode;

    // World pos of node.
    private Vector3 worldPos;

    // Inherint 'cost' to move between nodes. fCost is gCost + hCost.
    private int gCost, hCost;

    // Index position of Node in 2D Node Array (grid position).
    private int indexX, indexY;

    // Parent of current node.
    private Node parent;

    // AI currently occupying the Node.
    private GameObject occupied;   

    #region Variable Properties

    public bool Walkable
    {
        get { return walkable; }
        set { walkable = value; }
    }

    public Vector3 WorldPos 
    {
        get { return worldPos; }
        set { worldPos = value; }
    }

    public int GCost
    {
        get { return gCost; }
        set { gCost = value; }
    }

    public int HCost
    {
        get { return hCost; }
        set { hCost = value; }
    }

    public int fCost
    {
        get { return gCost + hCost; }
    }

    public int IndexX
    {
        get { return indexX; }
    }

    public int IndexY
    {
        get { return indexY; }
    }

    public Node Parent
    {
        get { return parent; }
        set { parent = value; }
    }

    public GameObject Occupied
    {
        get { return occupied; }
        set { occupied = value; }
    }

    public bool IsLadder
    {
        get { return ladderNode.Item1; }
    }

    public bool IsHealthStation
    {
        get { return healthStation; }
    }

    public GameObject HealthStationObj
    {
        get { return healthStationObj; }
    }

    public int HomeGrid
    {
        get { return ladderNode.Item2.Item1; }
    }

    public GameObject LinkedNode
    {
        get { return ladderNode.Item2.Item2; }
    }

    #endregion

    /// <summary> constructor <c>Node</c> assigns values on script initlization. </summary>
    public Node(bool _walkable, Vector3 _worldPos, int _indexX, int _indexY, Tuple<bool, Tuple<int, GameObject>> _ladderNode, bool _healthStation, GameObject _healthStationObj)
    {
        // Is node walkable & world position.
        walkable = _walkable;
        worldPos = _worldPos;

        // Refs to position in 2D Array.
        indexX = _indexX;
        indexY = _indexY;

        // Is this grid space a ladder node.
        ladderNode = _ladderNode;

        // Is this grid space a health station.
        healthStation = _healthStation;
        healthStationObj = _healthStationObj;
    }
}