using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TerrainCard : Card
{
    [Header("Terrain Properties")]
    public string type;

    public override void Start()
    { 
        TerrainCardObject terrianCardObject;
        base.Start();

        // To set up the ability naming based on ability type
        terrianCardObject = (TerrainCardObject)cardObject;

        switch (terrianCardObject.terrianType)
        {
            case TERRAINTYPE.FROZEN_LAKE:
                cardObject.className = "Frozen Lake";
                break;
            case TERRAINTYPE.DEEP_SNOW:
                cardObject.className = "Deep Snow";
                break;
            case TERRAINTYPE.SNOWY_GRASSLAND:
                cardObject.className = "Snowy Grassland";
                break;
            default:
                break;
        }
    }


    public override GameObject Ability(Transform transform)
    {
        //TODO implement certain ablity for each cardImage
        Debug.Log(cardObject.cardName);
        
        // Transform card into tile
        isTransforming = true;
        boxCollider.enabled= false;

        // PlaceUnit(transform);

        return null;
    }

    // change to place terrain
    //public void PlaceUnit(Transform transform)
    //{
    //    Vector3 temp = transform.position;
    //    temp.y += 10;
    //    Instantiate(cardObject.unitPrefab, temp, transform.rotation);      
    //}

    private void FixedUpdate()
    {
        // Transform card into Tile
        if(isTransforming) 
        {
            //this.transform.position += new Vector3(dir.x * 2.0f, dir.y * 2.0f, 0.0f);
            
            if(this.transform.localScale.x > 0 && this.transform.localScale.y > 0)
            this.transform.localScale -= new Vector3(0.1f,0.1f,0);  

 
        }
    }
}
