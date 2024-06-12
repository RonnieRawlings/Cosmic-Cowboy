// Author - Ronnie Rawlings.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckRange : MonoBehaviour
{
    // Icon to show enemy in range.
    [SerializeField] private GameObject inRangeIcon;

    public Dictionary<GameObject, GameObject> inRangeIcons;

    /// <summary> method <c>DisplayUponInRange</c> instantiates a new icon as a child of this obj upon enemy entering player range. </summary>
    /// <param name="enemy">Enemy obj to accosiate with icon.</param>
    public void DisplayUponInRange(GameObject enemy)
    {
        // Instantiates icon as child, adds to dict.
        GameObject newIcon = Instantiate(inRangeIcon, this.transform);
        inRangeIcons.Add(enemy, newIcon);
    }

    /// <summary> method <c>RemoveInRangeIcon</c> removes the icon from the scene if enemy no longer in range of player. </summary>
    /// <param name="enemies">List of enemies to remove icons for.</param>
    public void RemoveInRangeIcons(List<GameObject> enemies) 
    {
        for (int i = 0; i < enemies.Count; i++)
        {
            Destroy(inRangeIcons[enemies[i]]);
            inRangeIcons.Remove(enemies[i]);
        }       
    }

    /// <summary> method <c>RemoveInRangeIcon</c> removes the icon from the scene if enemy no longer in range of player. </summary>
    /// <param name="enemy">Enemy accosiated with the icon.</param>
    public void RemoveInRangeIcon(GameObject enemy)
    {
        Destroy(inRangeIcons[enemy]);
        inRangeIcons.Remove(enemy);
    }

    public void CheckForEnemies()
    {
        // Finds neighbouring nodes to player, range based on attachted weapon        
        List<Node> neighbourNodes = BattleInfo.gridManager.GetComponent<GridManager>().FindNeighbourNodes(BattleInfo.gridManager.GetComponent<GridManager>().
            FindNodeFromWorldPoint(BattleInfo.player.transform.position, BattleInfo.currentPlayerGrid), BattleInfo.playerWeapon.range, BattleInfo.currentPlayerGrid);

        // Create icons for all enemies in range.
        List<GameObject> enemiesInRange = new List<GameObject>();
        foreach (Node node in neighbourNodes)
        {
            if (node.Occupied != null)
            {
                enemiesInRange.Add(node.Occupied);
                if (!inRangeIcons.ContainsKey(node.Occupied))
                {
                    DisplayUponInRange(node.Occupied);
                }               
            }
        }

        // Find enemies that are now out of range, remove their icons.
        List<GameObject> enemiesToRemove = new List<GameObject>();
        foreach (KeyValuePair<GameObject, GameObject> kvp in inRangeIcons)
        {
            if (!enemiesInRange.Contains(kvp.Key))
            {
                enemiesToRemove.Add(kvp.Key);
            }
        }
        RemoveInRangeIcons(enemiesToRemove);
    }

    public IEnumerator FirstCheck()
    {
        // Waits for first occupied.
        yield return new WaitForSeconds(0.05f);
        CheckForEnemies();

        Debug.Log(inRangeIcons.Count);
    }

    void Awake()
    {
        // Initlisaes dictionary on script load.
        inRangeIcons = new Dictionary<GameObject, GameObject>();

        // Sets ref to script.
        BattleInfo.checkRange = this;
    }

    private void Start()
    {
        // Check if enemies are already in player's range.
        StartCoroutine(FirstCheck());
    }
}