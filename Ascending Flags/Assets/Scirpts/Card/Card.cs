using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System;
using UnityEngine.UIElements;
using Unity.VisualScripting;

public abstract class Card : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler, 
    IBeginDragHandler, IDragHandler, IEndDragHandler/*, IDropHandler*/
{
    [Header("Base Properties")]
    [SerializeField] public UnityEngine.UI.Image cardImage;
    [SerializeField] public TMP_Text nameText;
    [SerializeField] public TMP_Text hoverNameText;
    [SerializeField] public TMP_Text descriptionText;
    [SerializeField] public TMP_Text costText;
    [SerializeField] public TMP_Text hoverDescriptionText;

    [HideInInspector]
    public BoxCollider boxCollider;
    [HideInInspector]
    UnityEngine.UI.Image cardBG;

    [SerializeField] UnityEngine.UI.Image BackSideIMG;

    [SerializeField] public BaseCardObject cardObject = null;


    [HideInInspector]
    public bool isClicked = false;
    [HideInInspector]
    public Transform cardParent;
    [HideInInspector]
    public bool isfliped = false;

    //Drag and drop
    [HideInInspector]
    public Transform originalParent = null;
    [HideInInspector]
    public bool isReleased = false;
    [HideInInspector]
    public bool isTransforming = false;
    //public Vector2 dir = Vector2.zero;

    // Hover card
    public GameObject HoverObj;


    //References
    [HideInInspector]
    public CardManager cm;
    CameraController cc;

    public virtual void Start()
    {
        cm = FindObjectOfType<CardManager>();   
        cardBG = GetComponent<UnityEngine.UI.Image>();
        cc = GetComponentInParent<CameraController>();
        //cardParent = GameObject.Find("[CARDUNITS]").transform;
        boxCollider= GetComponent<BoxCollider>();

        name = nameText.text = hoverNameText.text = cardObject.cardName;
        if (descriptionText != null && hoverDescriptionText != null)
        {
            descriptionText.text = cardObject.description;
            hoverDescriptionText.text = cardObject.description;
        }
        costText.text = cardObject.cost.ToString();

        transform.localScale = new Vector2(1.0f, 1.0f);
    }

    void Update()
    {
        updateCostColor();

        if (BackSideIMG)
        {
            if(isfliped)
            {
                BackSideIMG.gameObject.SetActive(true);
            }
            else
            {
                BackSideIMG.gameObject.SetActive(false);
            }
        }  
    }
    private void updateCostColor()
    {
        if (cm.currentPlayerMana < cardObject.cost)
        {
            costText.color = Color.red;
            foreach (Transform child in HoverObj.transform)
            {
                if (child.name == "H_Cost")
                {
                    child.GetComponent<TMP_Text>().color = Color.red;
                }
            }
        }
        else
        {
            costText.color = Color.white;
            foreach (Transform child in HoverObj.transform)
            {
                if (child.name == "H_Cost")
                {
                    child.GetComponent<TMP_Text>().color = Color.white;
                }
            }
        }
    }

    private void FixedUpdate()
    {
        // Transforming animation for card when it is used
        if(isTransforming == true) 
        {       
            if(transform.localScale.x > 0 && transform.localScale.y > 0)
            {
                transform.localScale -= new Vector3(0.1f,0.1f,0);  
            }
            else
            {
                isTransforming = false;
            }
        }

    }

    public abstract GameObject Ability(Transform transform);

    public void OnPointerDown(PointerEventData eventData)
    {

        // Keep below to use in case that need to create click option

        //Debug.Log("down");
        //isClicked = !isClicked;

        //cm.SwitchClickedCard(this);

        //if (isClicked)
        //{
        //    cardImage.color = Color.yellow;
        //}
        //else
        //{
        //    cardImage.color = Color.red;
        //}
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //Debug.Log("enter");
        if (!isClicked)
        {
            if(!isfliped)
            cardBG.color = Color.red;
        }

        // Hover info
        if(!isfliped)
        {
            transform.localScale = new Vector2(1.2f, 1.2f);
            HoverObj.gameObject.SetActive(true);
        }


    }
    public void OnPointerExit(PointerEventData eventData)
    {
        //Debug.Log("exit");
        if (!isClicked)
        {
            if(!isfliped)
            cardBG.color = Color.white;
        }

        // Hover info
        if(!isfliped)
        {
            transform.localScale = new Vector2(1.0f, 1.0f);
            HoverObj.gameObject.SetActive(false);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if(!isfliped)
        { 
            // BeginDrag card
            isReleased = false;
            cc.Movement(false);
            cm.SwitchClickedCard(this);
            cardBG.color = Color.yellow;
            originalParent = transform.parent;
            transform.SetParent(transform.parent.parent);
            //GetComponent<CanvasGroup>().blocksRaycasts = true;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if(!isfliped)
        { 
            // EndDrag card
            isReleased = true;
            cc.Movement(true);
            cardBG.color = Color.white;
            transform.localScale = new Vector2(2.0f, 1.0f);

            StartCoroutine(MoveBackCard());
        }
        
    }

    IEnumerator MoveBackCard()
    {
        yield return new WaitForSecondsRealtime(0.1f);
        //If card is not used, move back to the hand panel
        if(cm.selectedCard.isTransforming == false)
        {
            cm.selectedCard.isClicked = false;
            transform.SetParent(originalParent);
        }
        //GetComponent<CanvasGroup>().blocksRaycasts = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if(!isfliped)
        { 
            //Draging Card
            transform.localScale = new Vector2(1.0f, 0.5f);
            HoverObj.gameObject.SetActive(false);
            this.transform.position = eventData.position;        
        }
    }

}
