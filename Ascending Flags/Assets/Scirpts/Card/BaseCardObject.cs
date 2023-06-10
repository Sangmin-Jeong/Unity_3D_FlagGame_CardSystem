using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CARDTYPE
{
    UNIT,
    TERRAIN
}

public class BaseCardObject : ScriptableObject
{
    [Header("Base Properties")]
    public string abilityName;
    public string cardName;
    public string description;
    public int cost;
    public string className;
    public int fogRange;
    public GameObject CPrefab;

    [HideInInspector]
    public CARDTYPE cardType;
}
