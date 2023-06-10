using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaManager : MonoBehaviour
{
    // Put 1 for P1Mana because when the game gets start, currentPlayer always starts as Player1 before the diceRoll
    [HideInInspector]
    public int p1Mana = 1;
    [HideInInspector]
    public int p2Mana = 2;
    [HideInInspector]
    private int maxMana;

    public GameObject p1ManaImages;
    public GameObject p2ManaImages;
    
    private bool isIncreasedForP1 = false;
    private bool isIncreasedForP2 = false;

    private bool isP1Turn = false;

    DebuggingObject DO;

    void Start()
    {
        DO = FindObjectOfType<DebuggingObject>();
        maxMana = DO.maxMana;

        p1Mana = DO.startingMana;
        p2Mana = DO.startingMana;
        //Debug.Log("P1: " + p1Mana);


        for(int i = 0; i < p1ManaImages.transform.childCount; i++)
        {
            p1ManaImages.transform.GetChild(i).gameObject.SetActive(false);
        }
        for(int i = 0; i < p2ManaImages.transform.childCount; i++)
        {
            p2ManaImages.transform.GetChild(i).gameObject.SetActive(false);
        }

    }

    void Update()
    {
        // Check who's turn
        if (Match.Instance.getCurrentPlayer() == 0)
        {
            isP1Turn = true;
        }
        else if (Match.Instance.getCurrentPlayer() == 1)
        {
            isP1Turn = false;
        }

        //if (Match.Instance.getCurrentPlayer() == 0)
        //{
        //    Debug.Log("P1: " + p1Mana);
        //}
        //else
        //{
        //    Debug.Log("P2: " + p2Mana);
        //}

        // Player 1's turn
        if (isP1Turn)
        {
            // Increase mana by 1 for each turn
            isIncreasedForP2 = false;
            if(isIncreasedForP1 == false)
            {
                isIncreasedForP1 = true;
                if (p1Mana < maxMana)
                p1Mana=Match.Instance.ManaNumber;

            }
        }

        // Player 2's turn
        else
        {
            isIncreasedForP1 = false;
            if(isIncreasedForP2 == false)
            {
                isIncreasedForP2 = true;
                if (p2Mana < maxMana)
                p2Mana= Match.Instance.ManaNumber;

            }


        }

        // Mana Indicator
        if(p1ManaImages.gameObject.activeSelf)
        {
            for(int i = 0; i < p1ManaImages.transform.childCount; i++)
            {
                p1ManaImages.transform.GetChild(i).gameObject.SetActive(false);
            }
            for (int i = 0; i < p1Mana; i++)
            {
                p1ManaImages.transform.GetChild(i).gameObject.SetActive(true);
            }
        }

        if(p2ManaImages.gameObject.activeSelf)
        {
            for(int i = 0; i < p2ManaImages.transform.childCount; i++)
            {
                p2ManaImages.transform.GetChild(i).gameObject.SetActive(false);
            }
            for(int i = 0; i < p2Mana; i++)
            {
                p2ManaImages.transform.GetChild(i).gameObject.SetActive(true);
            }
        }
    }
}
