// Author - Ronnie Rawlings.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BattleValues
{
    #region Player Values

    // Tiles player can move each turn.
    public static int playerTileAmount = 5;

    // Starting health of the player.
    public static int basePlayerHealth = 250;

    // In cover evasion chance increase.
    public static int coverEvasionChance = 30;

    // Steady aim value changes.
    public static int steadyAimCritGain = 15, steadyAimHitGain = 30;

    // Base player accuracy.
    public static int playerAcc = 70;

    // Quick draw value changes.
    public static int quickDrawHitDecrease = 15;

    #endregion

    #region Enemy Values

    #endregion

    #region Weapon Values

    // SIX - SHOOTER //

    // Six-Shooter (Base Weapon) crit chance & multiplyer.
    public static int sixCritChance = 30, sixCritMultiplyer = 50;

    // Base damage of Six-Shooter (before multiplyer).
    public static int sixBaseDamage = 75;

    // Amount of tiles the Six-Shooter can hit + mag capacity.
    public static int sixRange = 3, sixMagSize = 6;

    // LASER RIFLE //

    // Laser Rifle crit chance % multiplyer.
    public static int laserCritChance = 10, laserCritMultiplyer = 30;

    // Base damage of Laser Rifle (before multiplyer).
    public static int laserBaseDamage = 15;

    // Tile distance the Laser Rifle can shoot + Infinite Mag Size.
    public static int laserRange = 4;

    #endregion

    #region Grid Values

    #endregion
}
