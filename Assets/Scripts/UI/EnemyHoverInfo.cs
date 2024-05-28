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
            // Gets the EnemyStats and WeaponValues from the hit enemy.
            EnemyStats enemyStats = hit.transform.GetComponentInChildren<EnemyStats>();
            WeaponValues weaponValues = hit.transform.GetComponentInChildren<WeaponValues>();

            // Only shows the current hovered enemy data.
            if (transform.name == hit.transform.name)
            {
                // Set materials as outline.
                meshRend.materials = outlineMaterials;

                // Finds range block parent.
                GameObject rangeParent = hoverCanvas.transform.Find("DetectionRange").Find("Images").gameObject;
                int blockRange = enemyStats.DetectionRange;

                // Updates correct detection range.
                UpdateRange(rangeParent, blockRange);

                // Finds weaponRange parent.
                rangeParent = hoverCanvas.transform.Find("WeaponRange").Find("Images").gameObject;
                blockRange = weaponValues.range;

                // Updates correct weapon range.
                UpdateRange(rangeParent, blockRange);

                // Enables canvas.
                hoverCanvas.SetActive(true);
            }
        }
        else
        {
            // Sets materials as no outline.
            meshRend.materials = noOutlineMaterials;

            // Disables canvas.
            hoverCanvas.SetActive(false);

            // Loops through all range UIs.
            for (int i = 0; i < hoverCanvas.transform.childCount - 1; i++)
            {
                // Disables all enabled range blocks.
                foreach (Transform child in hoverCanvas.transform.GetChild(i).Find("Images"))
                {
                    child.gameObject.SetActive(false);
                }
            }
        }
    }

    /// <summary> method <c>UpdateRange</c> sets the enemies given range in visual blocks. </summary>
    public void UpdateRange(GameObject blockParent, int blockAmount)
    {
        // Enables the amount of blocks nessecary for player range.
        for (int i = 0; i < blockAmount; i++)
        {
            blockParent.transform.GetChild(i).gameObject.SetActive(true);
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
        if (nodePath.Count > BattleInfo.levelEnemyStats[gameObject].DetectionRange) { compromisedShown = false; }

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

        // Plays animation.
        hoverCanvas.SetActive(true);
        hoverCanvas.transform.GetChild(0).gameObject.SetActive(false);
        hoverCanvas.transform.GetChild(1).gameObject.SetActive(false);
        hoverCanvas.transform.GetChild(2).gameObject.SetActive(true);

        yield return new WaitForSeconds(1.8f);

        hoverCanvas.transform.GetChild(0).gameObject.SetActive(true);
        hoverCanvas.transform.GetChild(1).gameObject.SetActive(true);               
        hoverCanvas.transform.GetChild(2).gameObject.SetActive(false);
        hoverCanvas.SetActive(false);

        // Allows further routines & hoverData.
        compromisedShown = true;
        compromisedShowing = false;        
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
    }

    // Store the last mouse position
    private Vector3 lastMousePosition;

    // Is the compromised anim playing.
    private bool compromisedShowing = false;

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