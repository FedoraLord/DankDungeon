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
        PlayerController player = GameController.PlayerCtrl;
        switch (itemName)
        {
            case "Short Sword":
                TryEquipSword(typeof(ShortSword));
                CraftingMenu.Instance.UpdateUseButton();
                break;
            case "Broad Sword":
                TryEquipSword(typeof(BroadSword));
                CraftingMenu.Instance.UpdateUseButton();
                break;
            case "Katana":
                TryEquipSword(typeof(Katana));
                CraftingMenu.Instance.UpdateUseButton();
                break;
            case "Red Potion":
                if (player.DrinkRedPotion())
                    Number_inv--;
                break;
            case "Blue Potion":
                if (player.DrinkBluePotion())
                    Number_inv--;
                break;
            case "Green Potion":
                if (player.DrinkGreenPotion())
                    Number_inv--;
                break;
            case "Gold Potion":
                if (player.DrinkYellowPotion())
                    Number_inv--;
                break;
        }
    }

    private void TryEquipSword(Type t)
    {
        PlayerController player = GameController.PlayerCtrl;
        PlayerController.ActiveWeapon w = player.GetWeapon(t);
        if (w.weapon.Sheathe())
        {
            player.SetWeapon(w.weapon);
        }
    }

    public void Crafted()
    {
        PlayerController player = GameController.PlayerCtrl;
        switch (itemName)
        {
            case "Short Sword":
                var a = player.GetWeapon(typeof(ShortSword));
                var b = a.weapon;
                b.ApplyUpgrade();
                CraftingMenu.Instance.UpdateCraftButton();
                CraftingMenu.Instance.UpdateUseButton();
                break;
            case "Broad Sword":
                PlayerController.ActiveWeapon broadSword = player.GetWeapon(typeof(BroadSword));
                if (broadSword.isUnlocked)
                    broadSword.weapon.ApplyUpgrade();
                else
                    broadSword.isUnlocked = true;
                CraftingMenu.Instance.UpdateCraftButton();
                CraftingMenu.Instance.UpdateUseButton();
                break;
            case "Katana":
                PlayerController.ActiveWeapon katana = player.GetWeapon(typeof(Katana));
                if (katana.isUnlocked)
                    katana.weapon.ApplyUpgrade();
                else
                    katana.isUnlocked = true;
                CraftingMenu.Instance.UpdateCraftButton();
                CraftingMenu.Instance.UpdateUseButton();
                break;
            case "Armor":
            case "Red Potion":
            case "Blue Potion":
            case "Green Potion":
            case "Yellow Potion":
            case "Dagger":
            case "Bomb":
                Number_inv++;
                break;
        }
    }
}
