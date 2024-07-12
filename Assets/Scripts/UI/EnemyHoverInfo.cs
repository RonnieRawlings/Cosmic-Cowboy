// Author - Ronnie Rawlings.

using Fungus;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

public class EnemyHoverInfo : MonoBehaviour
{
    // Ref to hoverData canvas.
    private GameObject hoverCanvas;

    // Outline/noOutline material shaders, renderer comp.
    [SerializeField] private Material outlineShader, noOutlineShader;
    private SkinnedMeshRenderer meshRend;

    // Material array refs.
    Material[] outlineMaterials;
    Material[] noOutlineMaterials;

    // Is the player in the detection range.
    private bool inRange = false;

    #region Properties

    public bool InRange
    {
        get { return inRange; }
    }

    #endregion

    /// <summary> method <c>ShowHoverData</c> enables the hoverData canvas if player is hovered over. </summary>
    public void ShowHoverData()
    {
        // Sends raycast from mouse to point.
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // Sets layer mask to Enemy.
        int layerMask = LayerMask.GetMask("Enemy");

        // Checks if an enemy has been hit by mouse.
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
        {
            // Only shows the current hovered enemy data.
            if (transform.name == hit.transform.name)
            {
                // Set materials as outline.
                meshRend.materials = outlineMaterials;
            }
        }
        else
        {
            // Sets materials as no outline.
            meshRend.materials = noOutlineMaterials;
        }
    }

    // Player is within detectionRange.
    private bool compromisedShown = false;

    /// <summary> method <c>CheckForCompromised</c> checks if the player is now within the current enemies detection range. </summary>
    public void CheckForCompromised()
    {
        // Prevents unneeded checkes.
        if (compromisedShowing) { return; }

        // Finds best path to target.        
        List<Node> nodePath = BattleInfo.gridManager.GetComponent<Pathfinding>().FindPath(transform.position, 
            BattleInfo.player.transform.position, GetComponent<AIMovement>().CurrentGrid);

        // Player has left the detection range.
        if (nodePath.Count > BattleInfo.levelEnemyStats[gameObject].DetectionRange) { compromisedShown = false; inRange = false; }
        else { inRange = true; }

        // Plays compromised if player is within detection range.
        if (nodePath.Count <= BattleInfo.levelEnemyStats[gameObject].DetectionRange && !compromisedShown)
        {
            StartCoroutine(PlayCompromised());
        }
    }

    /// <summary> coroutine <c>PlayCompromised</c> plays the compromised animation, waits until completed. </summary>
    public IEnumerator PlayCompromised()
    {
        // Prevents further routines.
        compromisedShowing = true;

        // Plays anim.
        hoverCanvas.SetActive(true);
        hoverCanvas.transform.GetChild(0).gameObject.SetActive(true);

        yield return new WaitForSeconds(1.8f);
            
        // Stops playing anim.
        hoverCanvas.transform.GetChild(0).gameObject.SetActive(true);
        hoverCanvas.SetActive(false);

        // Allows further routines & hoverData.
        compromisedShown = true;
        compromisedShowing = false;        
    }

    /// <summary> coroutine <c>StartingCompromised</c> makes sure gird is set-up before checking for first compromised. </summary>
    private IEnumerator StartingCompromised()
    {
        // Wait for grid set-up.
        yield return new WaitForSeconds(0.1f);
        CheckForCompromised();
    }

    // Called once when active & enabled.
    void OnEnable()
    {
        // Sets ref to hoverData child obj.
        hoverCanvas = transform.Find("HoverData").GetChild(0).gameObject;
    }

    // Called once before first update.
    void Start()
    {
        // Gets rend comp, sets up material arrays.
        meshRend = transform.Find("Extract5").GetComponent<SkinnedMeshRenderer>();
        outlineMaterials = new Material[meshRend.materials.Length];
        noOutlineMaterials = new Material[meshRend.materials.Length];

        // Sets each material array to correct shader.
        for (int i = 0; i < meshRend.materials.Length; i++)
        {
            outlineMaterials[i] = outlineShader;
            noOutlineMaterials[i] = noOutlineShader;
        }

        if (hoverCanvas == null)
        { 
            // Sets materials as no outline.
            meshRend.materials = noOutlineMaterials;

            Destroy(this);            
        }

        // Check starting position, if in range showcase.
        StartCoroutine(StartingCompromised());
    }

    // Store the last mouse position
    private Vector3 lastMousePosition;

    // Is the compromised anim playing.
    private bool compromisedShowing = false;

    // Keeps track of last amount of tiles moved by player.
    private int lastTilesMoved;

    // Update is called once per frame
    void Update()
    {
        // Only check for hover if the mouse has moved.
        if (lastMousePosition != Input.mousePosition && !compromisedShowing)
        {
            // Checks if mouse is hovering the player.
            ShowHoverData();

            // Update the last mouse position
            lastMousePosition = Input.mousePosition;
        }

        // If the player has moved, check for compromised once.
        if (BattleInfo.player.GetComponent<PlayerMovement>().TilesMoved != lastTilesMoved)
        {
            // Checks for compromised, updates move count.
            CheckForCompromised();
            lastTilesMoved = BattleInfo.player.GetComponent<PlayerMovement>().TilesMoved;
        }
    }
}