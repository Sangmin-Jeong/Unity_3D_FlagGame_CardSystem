using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HorizontalLayout : MonoBehaviour
{
    private float spacing;
    private HorizontalLayoutGroup hLayoutGroup;
    public CardManager cm;
    public GameObject handPanel;

    private int activatedCardAmount;

    public bool check = true;

    void Start()
    {
        hLayoutGroup = GetComponent<HorizontalLayoutGroup>();
        //cm = FindObjectOfType<CardManager>();

        spacing = -40.0f;
        hLayoutGroup.spacing = spacing;

    }

   
    void Update()
    {
        if(check)
        {
            activatedCardAmount = 0;

            for (int i = 0; i < handPanel.transform.childCount; i++)
            {
                if(handPanel.transform.GetChild(i).gameObject.activeSelf)
                    activatedCardAmount++;

                check = false;
            }
        }


        if(activatedCardAmount >= 7 && activatedCardAmount <= 8)
        {
            spacing = -60.0f; 
        }
        else if(activatedCardAmount > 8 && activatedCardAmount <= 10)
        {
            spacing = -80.0f; 
        }
        else if(activatedCardAmount > 10)
        {
            spacing = -100.0f; 
        }
        else
        {
            spacing = -40.0f;
        }

        hLayoutGroup.spacing = spacing;
    }
}
