using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UI;
using TMPro;
//using UnityEngine.WSA;

public class CardManager : MonoBehaviour
{
    private int MAX_HAND;
    public List<Card> deck = new List<Card>();
    public List<Card> hand = new List<Card>();
    public Card selectedCard = null;
    public TextMeshProUGUI deckSizeText;
    public TextMeshProUGUI manaText;
    public GameObject handPanel;

    //Draw
    private int startingCardNumber;
   // private static bool turnDrawForP1 = true;
    //private static bool turnDrawForP2 = true;
    private HorizontalLayout HL;

    //public TMP_Text KeybindText;

    //Set the amount of terrain cards
    public GameObject deepSnow;
    public GameObject frozen;
    public GameObject snowygrass;
    DebuggingObject DO;

    // Enemy's hand counter
    [HideInInspector]
    public int amountHand;
    [HideInInspector]
    private int tempInt;

    //Mana
    ManaManager mm;
    [HideInInspector]
    public int currentPlayerMana;
    [HideInInspector]
    public bool hasEnoughMana = false;
    void Start()
    {
        //// Not sure if this is needs - couldn't get from the menu -> game scene due to networking stuff
        //KeyBind.keys.Add("1", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("1", "1")));
        //KeyBind.keys.Add("H", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("H", "H")));

        // Layout for cards
        HL = handPanel.GetComponentInChildren<HorizontalLayout>();

        //Debugging Object
        DO = FindObjectOfType<DebuggingObject>();

        //Mana
        mm = FindObjectOfType<ManaManager>();

        //Set up values by Debugging Object
        MAX_HAND = DO.maxHandAmount;
        startingCardNumber = DO.startingCardAmount;

        // To set up the deck list automatically from the Hand Panel
        for (int i = 0; i < handPanel.transform.childCount; i++)
        {
            Card temp = handPanel.transform.GetChild(i).gameObject.GetComponent<Card>();
            
            deck.Add(temp);
        }

        //Set the amount of terrain cards into deck
        for (int i = 0; i < DO.AmountOfDeepSnow; i++)
        {
            GameObject temp = Instantiate(deepSnow, deepSnow.transform.position, deepSnow.transform.rotation);
            temp.transform.SetParent(handPanel.transform);
            Card CTemp = temp.GetComponent<Card>();

            deck.Add(CTemp);
        }

        for (int i = 0; i < DO.AmountOfFrozen; i++)
        {
            GameObject temp = Instantiate(frozen, frozen.transform.position, frozen.transform.rotation);
            temp.transform.SetParent(handPanel.transform);
            Card CTemp = temp.GetComponent<Card>();

            deck.Add(CTemp);
        }

        for (int i = 0; i < DO.AmountOfSnowygrass; i++)
        {
            GameObject temp = Instantiate(snowygrass, snowygrass.transform.position, snowygrass.transform.rotation);
            temp.transform.SetParent(handPanel.transform);
            Card CTemp = temp.GetComponent<Card>();

            deck.Add(CTemp);
        }

        for (int i = 0; i < startingCardNumber; i++)
        {
            DrawCard();
        }
    }

    void Update()
    {
        deckSizeText.text = deck.Count.ToString();
        manaText.text = currentPlayerMana.ToString();

        //KeybindText.text = "(" + KeyBind.keys["1"] + ") - Draw\n(" + KeyBind.keys["H"] + ") - Flip cards on hand";

        //DebugInput();

        //Mana and Turn Draw
        if (Match.Instance.getCurrentPlayer() == 0)
        {
            //Debug.Log(turnDrawForP1 + " / " + turnDrawForP2);
            currentPlayerMana = mm.p1Mana;
            //Turn Draw
            //if(turnDrawForP1)
            //{
            //    turnDrawForP1 = false;
            //    turnDrawForP2 = true;
            //    DrawCard();
            //}
            
        }
        else if (Match.Instance.getCurrentPlayer() == 1)
        {
            //Debug.Log(turnDrawForP1 + " / " + turnDrawForP2);
            currentPlayerMana = mm.p2Mana;
            //Turn Draw
            //if(turnDrawForP2)
            //{
            //    turnDrawForP2 = false;
            //    turnDrawForP1 = true;
            //    DrawCard();

            //}
        }
        //Check if player has enough mana
        if(selectedCard)
        {
            if (selectedCard.cardObject.cost <= currentPlayerMana)
            {
                hasEnoughMana = true;
            }
            else
            {
                hasEnoughMana = false;
            }
        }

        // Enemy's hand counter
        if(gameObject.activeSelf)
        {
            foreach (var card in hand)
            {
                if (card != null)
                {
                    if (card.gameObject.activeSelf)
                    {
                        tempInt++;
                    }
                } 
            }
        }
        amountHand = tempInt;
        tempInt = 0;

        //if (selectedCard != null)
        //{
        //    Debug.Log(selectedCard.name);
        //}

        if (TileManager.Instance.getIfAllTilesFilled())
        {
            foreach(var card in hand)
            {
                if (card != null) 
                {
                    if (card.cardObject.cardType == CARDTYPE.TERRAIN)
                    {
                        hand.Remove(card);
                        Destroy(card.gameObject);
                        
                    }
                }
                
            }
        }


    }

    private void FixedUpdate()
    {

    }

    // For dubug
    private void DebugInput()
    {
        //if (Pause.isGamePaused == false)
        //{
        //    if (Input.GetKeyDown(KeyBind.keys["1"]))
        //    {
        //        DrawCard();
        //    }
        //    else if (Input.GetKeyDown(KeyBind.keys["H"]))
        //    {
        //        foreach (var card in hand)
        //        {
        //            card.isfliped = !card.isfliped;
        //        }
        //    }
        //}
    }

    public void SwitchClickedCard(Card card)
    {
        if (selectedCard != null)
        {
            if (selectedCard != card)
            {
                selectedCard.isClicked = false;
            }
            UnityEngine.UI.Image image;
            image = selectedCard.GetComponent<UnityEngine.UI.Image>();
            image.color = Color.white;
            
        }
        selectedCard = card;
        selectedCard.isClicked = true;
    }

    public void UseCard(Transform transform)
    {
        if (hasEnoughMana)
        {
            if (selectedCard != null && selectedCard.isClicked == true)
            {
                // Tranform card into Tile
                selectedCard.boxCollider.enabled = false;
                selectedCard.GetComponent<CanvasGroup>().blocksRaycasts = false;
                selectedCard.isTransforming = true;

                //GameObject unit = selectedCard.Ability(transform);

                //Mana
                currentPlayerMana -= selectedCard.cardObject.cost;
                if (Match.Instance.getCurrentPlayer() == 0)
                {
                    mm.p1Mana = currentPlayerMana;
                }
                else if (Match.Instance.getCurrentPlayer() == 1)
                {
                    mm.p2Mana = currentPlayerMana;
                }


                hand.Remove(selectedCard);
                Destroy(selectedCard.gameObject, 0.4f);

            }
            else
            {
                Debug.Log("Must Select A Card!");
            }
        }
        else
        {
            Debug.Log("You do NOT have enough Mana!");
        }
    }

    public void DrawCard()
    {
        if (hand.Count >= MAX_HAND) {return;}

        if (deck.Count >= 1)
        {
            Card randCard = deck[Random.Range(0, deck.Count)];


            // no more terrain cards if board is full
            bool loop = true;
            if (TileManager.Instance.getIfAllTilesFilled())
            {
                DebuggingObject.Instance.drawCardsPerTurn = 1;
                while (loop)
                {
                    loop = false;

                    if(randCard.cardObject.cardType == CARDTYPE.TERRAIN)
                    {
                        loop = true;
                        deck.Remove(randCard);
                        randCard = deck[Random.Range(0, deck.Count)];
                    }
                    


                }
            }
           
            if (hand.Count < MAX_HAND)
            {
                randCard.gameObject.SetActive(true);
                hand.Add(randCard);
                deck.Remove(randCard);

                //Layout
                HL.check = true;
            }
        }
    }
}
