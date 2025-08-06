using System.Collections.Generic;
using UnityEngine;

public static class WeaponSelectionTracker
{
    private static Dictionary<string, int> weaponSelections = new Dictionary<string, int>();

    public static void Increment(string weaponName)
    {
        if (weaponSelections.ContainsKey(weaponName))
        {
            weaponSelections[weaponName]++;
        }
        else
        {
            weaponSelections[weaponName] = 1;
        }
    }

    public static void Print()
    {
        List<string> entries = new List<string>();
        foreach (var kvp in weaponSelections)
        {
            entries.Add($"{kvp.Key}: {kvp.Value}");
        }

        Debug.Log("Weapon Selection Counts: " + string.Join(" | ", entries));
    }

    public static void Clear()
    {
        weaponSelections.Clear();
    }
}