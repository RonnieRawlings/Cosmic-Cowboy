// Author - Ronnie Rawlings.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateAGrid : MonoBehaviour
{
    // Size of grid (x & y coords) & radius of each node.
    [SerializeField] private Vector2 gridWorldSize;
    [SerializeField] private float nodeRadius;

    // Size of 2D Node Array.
    private int gridSizeX, gridSizeY;

    #region Variable Properties

    public Vector2 GridWorldSize
    {
        get { return gridWorldSize; }
    }

    public float NodeRadius
    {
        get { return nodeRadius; }
    }

    public int GridSizeX
    {
        get { return gridSizeX; }
    }

    public int GridSizeY
    {
        get { return gridSizeY; }
    }

    #endregion

    // Called once on script load.
    void Awake()
    {
        gridSizeX = Mathf.FloorToInt(gridWorldSize.x / (nodeRadius * 2));
        gridSizeY = Mathf.FloorToInt(gridWorldSize.y / (nodeRadius * 2));
    }
}
