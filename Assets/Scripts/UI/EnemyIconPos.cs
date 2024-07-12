// Author - Ronnie Rawlings.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyIconPos : MonoBehaviour
{
    // Ability background objects.
    [SerializeField] private List<GameObject> backgroundObjs;

    private RectTransform objRect;

    // Both object positions.
    private Vector2 upPos, downPos;

    /// <summary> method <c>CheckEnabled</c> if a background obj is enabled, move this obj up. </summary>
    private void CheckEnabled()
    {
        // Loop through each background.
        foreach (GameObject obj in backgroundObjs)
        {
            if (obj.activeInHierarchy)
            {
                // Move to up pos, don't continue checking.
                objRect.anchoredPosition = upPos;
                return;
            }
        }

        // Not enabled, move to down pos.
        objRect.anchoredPosition = downPos;
    }

    void Awake()
    {
        // Ref the RectTransform component.
        objRect = GetComponent<RectTransform>();

        // Set object positions.
        downPos = objRect.anchoredPosition;
        upPos = new Vector2(downPos.x, downPos.y + 100);
    }

    void Update()
    {
        // Has a background obj been enabled.
        CheckEnabled();
    }
}
