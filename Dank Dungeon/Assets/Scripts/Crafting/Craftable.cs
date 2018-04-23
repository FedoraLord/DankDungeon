using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class Craftable : ScriptableObject
{
    [Header("Crafting")]
    public string displayName;
    public string description;
    public List<CraftingMaterial> materialsNeeded = new List<CraftingMaterial>();
    public float craftingTime;
    public bool isConsumable;

    public bool CanCraft()
    {
        for (int i = 0; i < materialsNeeded.Count; i++)
        {
            CraftingMaterial onHand = Inventory.Instance.materials.Where(x => x.material == materialsNeeded[i].material).First();
            CraftingMaterial needed = materialsNeeded[i];

            if (needed.number > onHand.number)
                return false;
        }

        return true;
    }

    public void Craft()
    {
        GameController.PlayerCtrl.craftingSound.Play();
        for (int i = 0; i < materialsNeeded.Count; i++)
        {
            CraftingMaterial onHand = Inventory.Instance.materials.Where(x => x.material == materialsNeeded[i].material).First();
            CraftingMaterial needed = materialsNeeded[i];
            onHand.number -= needed.number;
        }
    }

    protected abstract void OnCraft();
}