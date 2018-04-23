using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CraftingMenu : MonoBehaviour {

    public int blueMaterial;
    public int redMaterial;
    public int greenMaterial;
    public int yellowMaterial;
    public CraftListItem selectedItem;
    public Image selectedIcon;
    public Text selectedName;
    public Text selectedDescription;
    public Text blueMaterialNeeded;
    public Text redMaterialNeeded;
    public Text greenMaterialNeeded;
    public Text yellowMaterialNeeded;
    public Text craftButtonText;
    public Text useButtonText;

    public bool IsOpen
    {
        get { return isOpen; }
    }

    [SerializeField]
    private GameObject[] immediateChildren;
    private bool isOpen;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleMenu();
        }
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

    }

    public void UseButtonClicked()
    {

    }

    public void ListItemSelected(CraftListItem selected)
    {
        selectedItem = selected;
        selectedIcon.sprite = selected.icon.sprite;
        selectedDescription.text = selected.itemDescription;
        selectedName.text = selected.itemName;
        blueMaterialNeeded.text = FormatMaterialNeeded(selected.blueMaterialNeeded, blueMaterial);
        redMaterialNeeded.text = FormatMaterialNeeded(selected.redMaterialNeeded, redMaterial);
        greenMaterialNeeded.text = FormatMaterialNeeded(selected.greenMaterialNeeded, greenMaterial);
        yellowMaterialNeeded.text = FormatMaterialNeeded(selected.yellowMaterialNeeded, yellowMaterial);
    }

    private string FormatMaterialNeeded(int needed, int have)
    {
        return string.Format("{0}/{1}", needed, have);
    }
    
    void Start () {
        if (selectedItem == null)
        {
            var firstItem = GetComponentInChildren<CraftListItem>();
            if (firstItem)
            {
                ListItemSelected(firstItem);
            }
        }
	}
}
