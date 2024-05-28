// Author - Ronnie Rawlings.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    // Stats specific to enemy.
    [SerializeField] private int health, detectionRange, movementRange;

    [SerializeField] public int endurance, firePower, perception, luck, lethality, toughness, fitness;

    #region Variable Properties

    public int Health
    {
        get { return health; }
    }

    public int DetectionRange
    {
        get { return detectionRange; }
    }

    public int MovementRange
    {
        get { return movementRange; }
    }

    #endregion
}
