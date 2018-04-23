using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftListItem : MonoBehaviour {

    public Image icon;
    public string itemDescription;
    public int blueMaterialNeeded;
    public int redMaterialNeeded;
    public int greenMaterialNeeded;
    public int yellowMaterialNeeded;
    public int craftTime;
    public enum ItemType { Weapon, Armor, Consumable };
    public ItemType itemType;
    public bool canUseInMenu;

    public string ItemName
    {
        get
        {
            if (itemName == null)
            {
                var t = GetComponentInChildren<Text>();
                if (t)
                    itemName = t.text;
            }
            return itemName;
        }
    }

    public int Number_inv
    {
        get { return number_inv; }
        set
        {
            number_inv = value;
            if (CraftingMenu.Instance.selectedItem == this)
            {
                CraftingMenu.Instance.UpdateUseButton();
            }
        }
    }

    private string itemName;

    [SerializeField]
    private int number_inv;

    public string GetCraftButtonText()
    {
        switch (itemType)
        {
            case ItemType.Weapon:
                return string.Format("Upgrade ({0}s)", craftTime);
            case ItemType.Armor:
                return string.Format("Upgrade ({0}s)", craftTime);
            case ItemType.Consumable:
                return string.Format("Craft ({0}s)", craftTime);
        }
        return "Craft";
    }

    public string GetUseButtonText()
    {
        if (canUseInMenu)
        {
            switch (itemType)
            {
                case ItemType.Weapon:
                    return "Equip";
                case ItemType.Consumable:
                    return string.Format("Use ({0})", number_inv);
            }
        }
        return string.Format("Current ({0})", number_inv);
    }

    public void Used()
    {

    }

    public void Crafted()
    {

    }
}
