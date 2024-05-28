// Author - Ronnie Rawlings.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponValues : MonoBehaviour
{
    // Damage, range, & mag size for prefab.
    [SerializeField] public int baseDamage, range, magSize;

    // Chance of critical hit & damage multiplyer.
    [SerializeField] public int critChance, critMultiplyer;

    // Base accuracy of weapon.
    [SerializeField] public int baseAccuracy;

    /// <summary> method <c>AssignWeapon</c> assigns weapon to player ref for this level. </summary>
    public void AssignWeapon()
    {
        // Assigns player weapon ref, prevents enemy weapon being set.
        if (transform.parent.name == "Player") { BattleInfo.playerWeapon = this; }
    }

    private void OnEnable()
    {
        AssignWeapon();
    }
}
