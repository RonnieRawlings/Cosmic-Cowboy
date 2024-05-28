// Author - Ronnie Rawlings.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SkillsAndClasses
{
    // Ref to each starting class & their stat changes.
    public static Dictionary<string, Dictionary<string, int>> playerClasses;

    // Ref to each stat & how it effects gameplay.
    public static Dictionary<string, int> statChanges;

    // Class chosen by player, at level 1 end.
    public static string playerChosenClass;

    // Current player stats.
    public static Dictionary<string, int> playerStats;

    // Ref to player's remaining points to distribuite.
    public static int remainingPoints = 14;

    /// <summary> constructor <c>SkillsAndClasses</c> initlizases & fills starting skill/class info & variables. </summary>
    static SkillsAndClasses()
    {
        // Sets data for each possible class choice.
        playerClasses = new Dictionary<string, Dictionary<string, int>>()
        {
            { "Mercenary", new Dictionary<string, int>()
                {
                    { "Endurance", 0 }, { "Fire-Power", 10 },
                    { "Perception", 0 }, { "Toughness", 10 }, { "Fitness", 0 }, 
                    { "Luck", 0 }
                } 
            },

            { "Sharpshooter", new Dictionary<string, int>()
                {
                    { "Endurance", 0 }, { "Fire-Power", 0 },
                    { "Perception", 10 }, { "Toughness", 0 }, { "Fitness", 0 },
                    { "Luck", 10 }
                }
            },

            { "Nomad", new Dictionary<string, int>()
                {
                    { "Endurance", 10 }, { "Fire-Power", 0 },
                    { "Perception", 0 }, { "Toughness", 0 }, { "Fitness", 10 },
                    { "Luck", 0 }
                }
            }
        };

        statChanges = new Dictionary<string, int>()
        {
            { "Endurance", 5 }, { "Fire-Power", 1 },
            { "Perception", 2 }, { "Toughness", 1 }, { "Fitness", 1 },
            { "Luck", 1 }
        };

        // Base player stats, all 1.
        playerStats = new Dictionary<string, int>()
        {
            { "Endurance", 50 }, { "Fire-Power", 50 },
            { "Perception", 50 }, { "Toughness", 50 }, { "Fitness", 50 },
            { "Luck", 50 }
        };
    }

    /// <summary> static method <c>ApplyStatIncrease</c> increases given stat by given increase (usually 1). </summary>
    public static void ApplyStatIncrease(string stat, int increase)
    {
        // Prevents negative stats.
        if ((playerStats[stat] + increase) < 50) { return; }

        // Finds stat in player dict, increases.
        playerStats[stat] += increase;

        Debug.Log(stat + " | " + playerStats[stat]);
    }

    /// <summary> static method <c>SetPlayerClass</c> assigns ref to chosen class, increases/decreases stats based on chosen class values. </summary>
    public static void SetPlayerClass(string chosenClass)
    {
        // Sets ref to chosen class.
        playerChosenClass = chosenClass;

        // Contains stat changes made in loop.
        Dictionary<string, int> updatedStats = new Dictionary<string, int>();

        int newStatValue = 0;
        // Changes player stas based on class stats.
        foreach (KeyValuePair<string, int> stat in playerStats)
        {
            // Prevents stat from going below minimum.
            if (stat.Value + playerClasses[chosenClass][stat.Key] < 50)
            {
                newStatValue = 50;
            }
            else
            {
                newStatValue = stat.Value + playerClasses[chosenClass][stat.Key];
            }

            // Sets new stats in dict.
            updatedStats.Add(stat.Key, newStatValue);
            Debug.Log(updatedStats[stat.Key].ToString());
        }

        // Assign the updated stats back to playerStats.
        playerStats = updatedStats;
    }

    /// <summary> static method <c>ReturnPlayerClassValues</c> works out what the class values would be, for display on skill overview. </summary>
    public static Dictionary<string, int> ReturnPlayerClassValues(string chosenClass)
    {
        // Contains stat changes made in loop.
        Dictionary<string, int> updatedStats = new Dictionary<string, int>();

        int newStatValue = 0;
        // Changes player stas based on class stats.
        foreach (KeyValuePair<string, int> stat in playerStats)
        {
            // Prevents stat from going below minimum.
            if (stat.Value + playerClasses[chosenClass][stat.Key] < 50) 
            {
                newStatValue = 50;
            }
            else
            {
                newStatValue = stat.Value + playerClasses[chosenClass][stat.Key];
            }

            // Sets new stats in dict.
            updatedStats.Add(stat.Key, newStatValue);
        }

        // Returns the updated dict, dosen't apply to actual stats.
        return updatedStats;
    }
}