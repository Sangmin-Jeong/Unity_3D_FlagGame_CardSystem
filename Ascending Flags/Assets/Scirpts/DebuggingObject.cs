using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebuggingObject : MonoBehaviour
{
    public static DebuggingObject Instance { get; private set; }

    [Header("Match settings")]
    public int TurnLimit;
    public int MapSize;
    public float TurnTime;


    [Header("Terrain cards")]
    public int AmountOfDeepSnow;
    public int AmountOfFrozen;
    public int AmountOfSnowygrass;

    [Header("Hand Panel")]
    public int maxHandAmount;
    public int drawCardsPerTurn;
    public int startingCardAmount;

    [Header("Mana")]
    public int maxMana;
    public int startingMana;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        MapSize = PlayerPrefs.GetInt("gridSize");
    }

}
