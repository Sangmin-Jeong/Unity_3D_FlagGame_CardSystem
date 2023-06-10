using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TERRAINTYPE
{
    FROZEN_LAKE,
    DEEP_SNOW,
    SNOWY_GRASSLAND,
    FLAG
}

[CreateAssetMenu(fileName = "New Terrain Card", menuName = "Card/New Terrain Card")]
public class TerrainCardObject : BaseCardObject
{
    public TERRAINTYPE terrianType;

    private void Awake()
    {
        cardType = CARDTYPE.TERRAIN;
    }
}
