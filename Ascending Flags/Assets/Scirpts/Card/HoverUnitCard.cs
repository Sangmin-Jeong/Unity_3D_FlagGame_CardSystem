using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HoverUnitCard : MonoBehaviour
{
    public Card parentCard;

    private UnitCard unitCard;

    [Header("Base Stats")]
    [SerializeField] public UnityEngine.UI.Image cardImage;
    [SerializeField] TMP_Text nameText;
    [SerializeField] TMP_Text TypeText;
    [SerializeField] TMP_Text abilityText;
    [SerializeField] TMP_Text costText;

    [Header("Unit Stats")]
    [SerializeField] TMP_Text defense_Text;
    [SerializeField] TMP_Text atk_Text;
    [SerializeField] TMP_Text range_Text;
    [SerializeField] TMP_Text move_Text;
    // Start is called before the first frame update

    private void Awake()
    {
        unitCard = parentCard.GetComponent<UnitCard>();

    }
    void Start()
    { 
        cardImage.sprite = parentCard.cardImage.sprite;
        nameText.text = parentCard.nameText.text;
        TypeText.text = parentCard.cardObject.className;
        abilityText.text = parentCard.cardObject.abilityName;
        costText.text = parentCard.cardObject.cost.ToString();

        defense_Text.text = unitCard.defense_Text.text;
        atk_Text.text = unitCard.atk_Text.text;
        range_Text.text = unitCard.range_Text.text;
        move_Text.text = unitCard.move_Text.text;

    }

    // Update is called once per frame
    void Update()
    {
    }
}
