using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public enum UNITTYPE
{
    NO_TYPE,
    SNOWY,
    SNOWY_MECHA,
    JET,
    TURRET,
    FIGHTER,
}

public enum ABILITYTYPE
{
    RAMPAGE,
    HANDY,
    NON,
    SILENCE,
    TRAITOR,

}

[CreateAssetMenu(fileName = "New Unit Card", menuName = "Card/New Unit Card")]
public class UnitCardObject : BaseCardObject
{
    [Header("Unit Properties")]
    public UNITTYPE unitType;
    public ABILITYTYPE abilityType;
    public ABILITYTYPE abilityType2;
    
    public int atk;
    public int defense;
    public int range;
    public int movement;

    [HideInInspector]
    public bool isTakeDamage = false;

    private void Awake()
    {
        cardType = CARDTYPE.UNIT;

    }
}
