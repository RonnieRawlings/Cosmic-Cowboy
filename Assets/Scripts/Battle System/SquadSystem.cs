// Author - Ronnie Rawlings.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SquadSystem
{
    // Set when level is first loaded.
    public static bool levelLoadedBefore = false;

    // All level squads.
    public static Dictionary<int, List<SquadMember>> levelSquads;

    static SquadSystem()
    {
        levelSquads = new Dictionary<int, List<SquadMember>>();
    }
}
