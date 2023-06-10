using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HoverUnit : MonoBehaviour
{

    [HideInInspector]
    public Card parentCard;

    public GameObject hoverObj;
    public Unit unit;

    private UnitCard unitCard;

    [Header("Base Stats")]
    [SerializeField] public UnityEngine.UI.Image cardImage;
    [SerializeField] TMP_Text nameText;
    [SerializeField] TMP_Text descriptionText;
    [SerializeField] TMP_Text TypeText;
    [SerializeField] TMP_Text abilityText;
    [SerializeField] TMP_Text costText;

    [Header("Unit Stats")]
    [SerializeField] public TMP_Text defense_Text;
    [SerializeField] public TMP_Text atk_Text;
    [SerializeField] public TMP_Text range_Text;
    [SerializeField] public TMP_Text move_Text;
    // Start is called before the first frame update

    private void Awake()
    {

    }
    void Start()
    {
        if(parentCard)
        {
            unitCard = parentCard.GetComponent<UnitCard>();

            cardImage.sprite = parentCard.cardImage.sprite;
            nameText.text = parentCard.nameText.text;
            //TypeText.text = parentCard.cardObject.className;
            //abilityText.text = parentCard.cardObject.abilityName;
            costText.text = parentCard.cardObject.cost.ToString();

            defense_Text.text = unitCard.defense_Text.text;
            atk_Text.text = unitCard.atk_Text.text;
            range_Text.text = unitCard.range_Text.text;
            move_Text.text = unitCard.move_Text.text;
        }


    }

    // Update is called once per frame
    void Update()
    {
        // Hovering Info tranformation
        if(hoverObj.activeSelf)
        {
            hoverObj.transform.position = new Vector3(Input.mousePosition.x + 150.0f, Input.mousePosition.y + 0.0f, Input.mousePosition.z);
        }

        // Update Card info
        defense_Text.text = unit.Health.ToString();
        atk_Text.text = unit.Atk.ToString();
        range_Text.text = unit.getCurrentRange().ToString();
        move_Text.text = unit.getCurrentMovement().ToString();

    }
}
