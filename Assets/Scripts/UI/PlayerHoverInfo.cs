// Author - Ronnie Rawlings.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHoverInfo : MonoBehaviour
{
    // Ref to hoverData canvas.
    private GameObject hoverCanvas;

    // Prevents canvas from being disabled.
    private bool keepCanvasActive;

    // Player mesh renderer.
    private SkinnedMeshRenderer meshRend;

    // Lists to set materials in inspector, dicts to assign in script.
    [SerializeField] private List<Material> outlineMaterials, noOutlineMaterials;

    // Create a cache for the materials, prevents multiple searches.
    Material[] cachedOutlineMaterials;
    Material[] cachedNoOutlineMaterials;

    #region Variable Properties

    public bool KeepCanvasActive
    {
        set { keepCanvasActive = value; }
    }

    #endregion

    /// <summary> method <c>ShowHoverData</c> enables the hoverData canvas if player is hovered over. </summary>
    public void ShowHoverData()
    {
        // Sends raycast from mouse to point.
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // Sets layer mask to Enemy.
        int layerMask = LayerMask.GetMask("Player");

        // Checks if player has been hit by mouse.
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
        {
            // Switch to outline materials.
            meshRend.materials = cachedOutlineMaterials;

            // Finds range block parent, updates range UI.
            GameObject rangeParent = hoverCanvas.transform.Find("WeaponRange").Find("Images").gameObject;
            UpdateRange(rangeParent);

            // Finds AP block parent, updates AP UI.
            rangeParent = hoverCanvas.transform.Find("ActionPoints").Find("Images").gameObject;
            UpdateAPUI(rangeParent);

            // Enables canvas.
            hoverCanvas.SetActive(true);                    
        }
        else
        {
            // Switch to noOutline materials.
            meshRend.materials = cachedNoOutlineMaterials;

            if (keepCanvasActive) { return; }

            // Disables canvas.
            hoverCanvas.SetActive(false);

            // Disables all enabled range blocks.
            foreach (Transform child in hoverCanvas.transform.Find("WeaponRange").Find("Images"))
            {
                child.gameObject.SetActive(false);
            }

            // Disables enabled AP blocks.
            foreach (Transform child in hoverCanvas.transform.Find("ActionPoints").Find("Images"))
            {
                child.gameObject.SetActive(false);
            }
        }
    }

    /// <summary> method <c>EnableWithoutHover</c> allows hoverData canvas to be enabled without hovering over the player. </summary>
    public void EnableWithoutHover()
    {
        // Finds range block parent, updates range UI.
        GameObject rangeParent = hoverCanvas.transform.Find("WeaponRange").Find("Images").gameObject;
        UpdateRange(rangeParent);

        // Finds AP block parent, updates AP UI.
        rangeParent = hoverCanvas.transform.Find("ActionPoints").Find("Images").gameObject;
        UpdateAPUI(rangeParent);

        // Enables canvas.
        hoverCanvas.SetActive(true);
    }

    /// <summary> method <c>UpdateRange</c> sets the player's range in visual blocks. </summary>
    public void UpdateRange(GameObject blockParent)
    {
        // Player's range, amount of blocks needed.
        int rangeBlocks = BattleInfo.playerWeapon.GetComponent<WeaponValues>().range;

        // Offset increases after each instantiate.
        Vector3 offset = Vector3.zero;

        // Enables the amount of blocks nessecary for player range.
        for (int i = 0; i < rangeBlocks; i++)
        {
            blockParent.transform.GetChild(i).gameObject.SetActive(true);
        }
    }

    /// <summary> method <c>UpdateAPUI</c> enables rangeBlock prefabs for remaining player AP, each block represents 1 AP. </summary>
    public void UpdateAPUI(GameObject blockParent)
    {
        // Loops through each remaing ap, enables block.
        for (int i = 0; i < BattleInfo.currentActionPoints; i++)
        {
            blockParent.transform.GetChild(i).gameObject.SetActive(true);
        }
    }

    // Called once before first update.
    void Start()
    {
        // Get mesh renderer.
        meshRend = transform.Find("Merged_cowboy_pls").GetComponent<SkinnedMeshRenderer>();

        // Initialize dicts.
        Dictionary<string, Material> outlineMaterialsDict = new Dictionary<string, Material>();
        Dictionary<string, Material> noOutlineMaterialsDict = new Dictionary<string, Material>();

        // Assign outline materials to their names.
        foreach (Material mat in outlineMaterials)
        {
            outlineMaterialsDict.Add(mat.name, mat);
        }

        // Assign no outline materials to their names.
        foreach (Material mat in noOutlineMaterials)
        {
            noOutlineMaterialsDict.Add(mat.name, mat);
        }

        // Initialize cached materials arrays.
        cachedOutlineMaterials = new Material[meshRend.materials.Length];
        cachedNoOutlineMaterials = new Material[meshRend.materials.Length];

        // Fill the cached materials arrays.
        for (int i = 0; i < meshRend.materials.Length; i++)
        {
            // Assign material if a match is found, if not keep exisiting.
            string materialName = meshRend.materials[i].name.Replace(" (Instance)", "");
            if (outlineMaterialsDict.ContainsKey(materialName))
            {
                cachedOutlineMaterials[i] = outlineMaterialsDict[materialName];
            }
            else
            {
                cachedOutlineMaterials[i] = meshRend.materials[i];
            }

            if (noOutlineMaterialsDict.ContainsKey(materialName))
            {
                cachedNoOutlineMaterials[i] = noOutlineMaterialsDict[materialName];
            }
            else
            {
                cachedNoOutlineMaterials[i] = meshRend.materials[i];
            }
        }
    }

    // Called once when active & enabled.
    void OnEnable()
    {
        // Sets ref to hoverData child obj.
        hoverCanvas = transform.Find("HoverData").GetChild(0).gameObject;
    }

    // Store the last mouse position
    private Vector3 lastMousePosition;

    // Update is called once per frame
    void Update()
    {
        // Only check for hover if the mouse has moved.
        if (lastMousePosition != Input.mousePosition)
        {
            // Checks if mouse is hovering the player.
            ShowHoverData();

            // Update the last mouse position
            lastMousePosition = Input.mousePosition;
        }        
    }
}
