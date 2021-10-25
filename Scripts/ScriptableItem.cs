using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Default Weapon", menuName = "Scriptable Objects/Weapon", order = 2)]
public class ScriptableItem : ScriptableObject
{
    public float KnockbackMultiplier = 1.15f;
    public int Durability = 10;
    public GameObject weaponPrefab;
}