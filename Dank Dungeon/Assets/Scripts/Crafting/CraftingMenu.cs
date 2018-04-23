using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CraftingMenu : MonoBehaviour
{
    public static CraftingMenu Instance;

    public Text blueMaterialHave_text;
    public Text redMaterialHave_text;
    public Text greenMaterialHave_text;
    public Text yellowMaterialHave_text;

    public CraftListItem selectedItem;
    public Image selectedIcon_image;
    public Text selectedName_text;
    public Text selectedDescription_text;
    public Text blueMaterialNeeded_text;
    public Text redMaterialNeeded_text;
    public Text greenMaterialNeeded_text;
    public Text yellowMaterialNeeded_text;

    public Text craftButton_text;
    public Text useButton_text;
    public Button useButton_button;
    public Button craftButton_button;

    public bool IsOpen
    {
        get { return isOpen; }
    }
    
    public int BlueMaterial
    {
        get { return blueMaterial; }
        set
        {
            blueMaterial = value;
            blueMaterialHave_text.text = blueMaterial.ToString();
            FormatMaterialNeeded(blueMaterialNeeded_text, selectedItem.blueMaterialNeeded, blueMaterial);
        }
    }

    public int RedMaterial
    {
        get { return redMaterial; }
        set
        {
            redMaterial = value;
            redMaterialHave_text.text = redMaterial.ToString();
            FormatMaterialNeeded(redMaterialNeeded_text, selectedItem.redMaterialNeeded, redMaterial);
        }
    }

    public int GreenMaterial
    {
        get { return greenMaterial; }
        set
        {
            greenMaterial = value;
            greenMaterialHave_text.text = greenMaterial.ToString();
            FormatMaterialNeeded(greenMaterialNeeded_text, selectedItem.greenMaterialNeeded, greenMaterial);
        }
    }

    public int YellowMaterial
    {
        get { return yellowMaterial; }
        set
        {
            yellowMaterial = value;
            yellowMaterialHave_text.text = yellowMaterial.ToString();
            FormatMaterialNeeded(yellowMaterialNeeded_text, selectedItem.yellowMaterialNeeded, yellowMaterial);
        }
    }

    private bool isOpen;
    private int blueMaterial;
    private int redMaterial;
    private int greenMaterial;
    private int yellowMaterial;
    
    [SerializeField]
    private GameObject[] immediateChildren;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleMenu();
        }

        //if (Input.GetKeyDown(KeyCode.Alpha1))
        //    BlueMaterial++;
        //if (Input.GetKeyDown(KeyCode.Alpha2))
        //    RedMaterial++;
        //if (Input.GetKeyDown(KeyCode.Alpha3))
        //    GreenMaterial++;
        //if (Input.GetKeyDown(KeyCode.Alpha4))
        //    YellowMaterial++;
        //if (Input.GetKeyDown(KeyCode.Alpha0))
        //    selectedItem.Number_inv++;
    }

    public void ToggleMenu()
    {
        isOpen = !isOpen;
        for (int i = 0; i < immediateChildren.Length; i++)
        {
            immediateChildren[i].SetActive(isOpen);
        }

        Time.timeScale = isOpen ? 0 : 1;
    }

    public void X_ButtonClicked()
    {
        ToggleMenu();
    }

    public void MainMenuButtonClicked()
    {
        SceneManager.LoadScene("StartMenu");
    }

    public void CraftButtonClicked()
    {
        selectedItem.Crafted();
    }

    public void UseButtonClicked()
    {
        selectedItem.Used();
    }

    public void ListItemSelected(CraftListItem selected)
    {
        selectedItem = selected;
        selectedIcon_image.sprite = selected.icon.sprite;
        selectedDescription_text.text = selected.itemDescription;
        selectedName_text.text = selected.ItemName;
        FormatMaterialNeeded(blueMaterialNeeded_text, selected.blueMaterialNeeded, blueMaterial);
        FormatMaterialNeeded(redMaterialNeeded_text, selected.redMaterialNeeded, redMaterial);
        FormatMaterialNeeded(greenMaterialNeeded_text, selected.greenMaterialNeeded, greenMaterial);
        FormatMaterialNeeded(yellowMaterialNeeded_text, selected.yellowMaterialNeeded, yellowMaterial);
        UpdateCraftButton();
        UpdateUseButton();
    }

    private void FormatMaterialNeeded(Text text, int needed, int have)
    {
        text.text = string.Format("{0}/{1}", needed, have);
    }

    public void UpdateCraftButton()
    {
        craftButton_button.interactable = CanCraft();
        craftButton_text.text = selectedItem.GetCraftButtonText();
    }

    public void UpdateUseButton()
    {
        useButton_button.interactable = selectedItem.canUseInMenu;
        useButton_text.text = selectedItem.GetUseButtonText();
    }

    void Start () {
        Instance = this;

        isOpen = false;
        ToggleMenu();

        if (selectedItem == null)
        {
            var firstItem = GetComponentInChildren<CraftListItem>();
            if (firstItem)
            {
                ListItemSelected(firstItem);
            }
        }
        
        ToggleMenu();
	}

    private bool CanCraft()
    {
        return selectedItem.blueMaterialNeeded <= blueMaterial
            && selectedItem.redMaterialNeeded <= redMaterial
            && selectedItem.greenMaterialNeeded <= greenMaterial
            && selectedItem.yellowMaterialNeeded <= yellowMaterial;
    }
}
