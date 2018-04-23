﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftListItem : MonoBehaviour {

    public string itemDescription;
    public int blueMaterialNeeded;
    public int redMaterialNeeded;
    public int greenMaterialNeeded;
    public int yellowMaterialNeeded;
    public int craftTime;
    public int numInInventory;

    [NonSerialized]
    public Sprite icon;
    [NonSerialized]
    public string itemName;
    
    void Start () {
        var i = GetComponentInChildren<Image>();
        if (i)
            icon = i.sprite;

        var t = GetComponentInChildren<Text>();
        if (t)
            itemName = t.text;
    }
}
