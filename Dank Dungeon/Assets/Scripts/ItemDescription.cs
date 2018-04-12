using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ItemDescription : MonoBehaviour {
    public GameObject[] description = new GameObject[6];

    public void ShortSwordDescription()
    {
        Rendering(0);
    }

    public void BroadSwordDescription()
    {
        Rendering(1);
    }

    public void KatanaDescription()
    {
        Rendering(2);
    }

    public void ArmorDescription()
    {
        Rendering(3);
    }

    public void LargeHealthPotionDescription()
    {
        Rendering(4);
    }

    public void LargeCurePotionDescription()
    {
        Rendering(5);
    }

    public void None()
    {
        Rendering(description.Length);
    }

    private void Rendering(int index)
    {
        for (int count = 0; count < description.Length; count++)
            if (count != index)
                description[count].SetActive(false);
            else
                description[count].SetActive(true);
    }

  }
