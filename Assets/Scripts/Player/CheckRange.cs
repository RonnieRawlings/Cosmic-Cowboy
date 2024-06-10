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
    /// <param name="enemy">Enemy accosiated with the icon.</param>
    public void RemoveInRangeIcon(GameObject enemy) 
    {
        Destroy(inRangeIcons[enemy]);
    }

    void Awake()
    {
        // Initlisaes dictionary on script load.
        inRangeIcons = new Dictionary<GameObject, GameObject>();
    }
}