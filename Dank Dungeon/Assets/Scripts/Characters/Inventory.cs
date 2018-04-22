using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public List<CraftingMaterial> materials = new List<CraftingMaterial>();
    public Dictionary<Craftable, int> consumables = new Dictionary<Craftable, int>();

    public static Inventory Instance;

    private void Start()
    {
        Instance = this;
    }
}
