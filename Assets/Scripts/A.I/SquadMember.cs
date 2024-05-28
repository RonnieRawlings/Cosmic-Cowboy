// Author - Ronnie Rawlings.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquadMember : MonoBehaviour
{
    private bool takenTurn = false;

    [SerializeField] private int squadNumber;

    #region Properties

    public bool TakenTurn
    {
        get { return takenTurn; }
        set { takenTurn = value; }
    }

    public int SquadNumber
    {
        get { return squadNumber; }
    }

    #endregion

    void Awake()
    {
        if (SquadSystem.levelLoadedBefore)
        {
            SquadSystem.levelSquads.Remove(squadNumber);
        }
    }

    void Start()
    {
        // Allows clearing on next level restart.
        SquadSystem.levelLoadedBefore = true;

        if (SquadSystem.levelSquads != null)
        {
            if (!SquadSystem.levelSquads.ContainsKey(squadNumber))
            {
                SquadSystem.levelSquads.Add(squadNumber, new List<SquadMember>());
                SquadSystem.levelSquads[squadNumber].Add(this);
            }
            else
            {
                SquadSystem.levelSquads[squadNumber].Add(this);
            }
        }
    }
}
