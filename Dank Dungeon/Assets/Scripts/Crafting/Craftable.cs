using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Craftable : ScriptableObject
{
    [Header("Crafting")]
    public string displayName;
    public string description;
    public List<CraftingMaterial> materialsNeeded = new List<CraftingMaterial>();
    public float craftingTime;
    public bool isConsumable;
}