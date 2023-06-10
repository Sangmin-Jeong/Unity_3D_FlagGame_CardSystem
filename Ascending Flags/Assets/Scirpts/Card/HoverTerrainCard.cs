using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HoverTerrainCard : MonoBehaviour
{
    public Card parentCard;

    //private Terrain terrainCard;

    [Header("Base Stats")]
    [SerializeField] public UnityEngine.UI.Image cardImage;
    [SerializeField] TMP_Text nameText;
    [SerializeField] TMP_Text TypeText;
    [SerializeField] TMP_Text abilityText;
    [SerializeField] TMP_Text costText;

    //[Header("Terrain Stats")]
    //[SerializeField] TMP_Text sth;

    // Start is called before the first frame update
    void Start()
    {
        cardImage.sprite = parentCard.cardImage.sprite;
        nameText.text = parentCard.nameText.text;
        TypeText.text = parentCard.cardObject.className;
        abilityText.text = parentCard.cardObject.abilityName;
        costText.text = parentCard.cardObject.cost.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
