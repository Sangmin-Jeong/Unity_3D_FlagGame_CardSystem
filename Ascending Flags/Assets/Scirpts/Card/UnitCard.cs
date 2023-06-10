using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
//using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class UnitCard : Card
{
    UnitCardObject unitCardObject;

    [Header("Unit Stats")]
    public TMP_Text defense_Text;
    public TMP_Text atk_Text;
    public TMP_Text range_Text;
    public TMP_Text move_Text;

    private void Awake()
    {
        // To set up the ability naming based on ability type
        unitCardObject = (UnitCardObject)cardObject;

        switch (unitCardObject.abilityType)
        {
            case ABILITYTYPE.NON:
                cardObject.abilityName = "";
                break;
            case ABILITYTYPE.RAMPAGE:
                cardObject.abilityName = "Rampage";
                break;
            case ABILITYTYPE.HANDY:
                cardObject.abilityName = "Handy";
                break;
            case ABILITYTYPE.SILENCE:
                cardObject.abilityName = "Silence";
                break;
            case ABILITYTYPE.TRAITOR:
                cardObject.abilityName = "Traitor";
                break;
            default:
                break;
        }

        switch (unitCardObject.abilityType2)
        {
            case ABILITYTYPE.NON:
                cardObject.abilityName += "";
                break;
            case ABILITYTYPE.RAMPAGE:
                cardObject.abilityName += "\nRampage";
                break;
            case ABILITYTYPE.HANDY:
                cardObject.abilityName += "\nHandy";
                break;
            case ABILITYTYPE.SILENCE:
                cardObject.abilityName += "\nSilence";
                break;
            case ABILITYTYPE.TRAITOR:
                cardObject.abilityName += "\nTraitor";
                break;
            default:
                break;
        }

        if(defense_Text)
        defense_Text.text = unitCardObject.defense.ToString();
        if(atk_Text)
        atk_Text.text = unitCardObject.atk.ToString();
        if(range_Text)
        range_Text.text = unitCardObject.range.ToString();
        if(move_Text)
        move_Text.text = unitCardObject.movement.ToString();

    }

    public override void Start()
    {
        base.Start();
    }

    public override GameObject Ability(Transform transform)
    {
        //TODO implement certain ablity for each card
        Debug.Log(cardObject.cardName);



        return PlaceUnit(transform);
       
    }

    public GameObject PlaceUnit(Transform transform)
    {
        Vector3 temp = transform.position;
        temp.y += 0.8f;

        GameObject unit = Instantiate(cardObject.CPrefab, temp, transform.rotation, cardParent);
        unit.name = cardObject.cardName;
        //BaseCardObject tempCardObj = GetComponent<BaseCardObject>();

        Unit tempUnit = unit.GetComponent<Unit>();
        //tempUnit.cardObject = cardObject;
        //tempUnit.baseCard = this.gameObject;

        return unit;

    }

}

