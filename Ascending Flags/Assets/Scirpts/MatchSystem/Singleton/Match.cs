using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEngine.UI.CanvasScaler;


public class Match : MonoBehaviour
{
    public int maxTurns = 10;
    public static Match Instance { get; private set; }

    private int currentPlayer = 0;

    public CardManager player1Cards;

    public CardManager player2Cards;

    public int TurnNumber = 1;
    public int ManaNumber = 1;

    [HideInInspector]
    public Tile flagTile1;
    [HideInInspector]
    public Tile flagTile2;
    [SerializeField]
    private TextMeshProUGUI turnCounter;
    [SerializeField]
    private UnitManager unitManager;

    [HideInInspector]
    public GameObject currentCamera;


    public static bool isP1 = false;
    public static bool isP2 = false;
    public static bool isTie = false;

    private void Awake()
    {

        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
        isP1 = false;
        isP2 = false;
        isTie = false;
    }

    public int getWhoWon()
    {
        if (isP1)
        {
            return 1;
        }
        else if (isP2)
        {
            return 2;
        }
        else if (isTie)
        {
            return 3;
        }
        else return 4;
    }

    private void Start()
    {
        Time.timeScale = 1.0f;
        
        maxTurns = DebuggingObject.Instance.TurnLimit;
    }

    public void whoseTurn(bool who)
    {
        turnCounter.text = TurnNumber.ToString();//"Turn: " + TurnNumber +"/"+maxTurns;
        if (TurnNumber >= maxTurns)
        {
            // end game
            //count units of each player
            int winner = unitManager.countUnits();
            if (winner == 0)
            {
                isP1 = true;
                print("player 1 wins");
                //player 1 wins
            }
            else if (winner == 1)
            {
                isP2 = true;
                print("player 2 wins");
                //player 2 wins
            }
            else if(winner == 2) 
            {
                print("Tie");
                // tie
                isTie =true;
            }
        }
        else if (who)
        {
            if(ManaNumber < 10)
                ManaNumber++;
            TurnNumber++;


            currentPlayer = 0;
            for(int i = 0; i < DebuggingObject.Instance.drawCardsPerTurn; i++)
            {
                player1Cards.DrawCard();
            }
            
        }
        else
        {
            currentPlayer = 1;
            for (int i = 0; i < DebuggingObject.Instance.drawCardsPerTurn; i++)
            {
                player2Cards.DrawCard();
            }
        }
        UnitManager.Instance.enableNeighbors(currentPlayer);

        
    }

    public void setPostProces(bool toggle)
    {
        if(currentCamera != null)
            currentCamera.GetComponent<UniversalAdditionalCameraData>().renderPostProcessing = toggle;
  
    }

    public int getCurrentPlayer()
    {
        return currentPlayer;
    }

    public void setLayer(GameObject gam, int layer)
    {
        gam.layer= layer;
        foreach (Transform tan in gam.GetComponentsInChildren<Transform>())
        {
            if (!tan.gameObject.CompareTag("transparent"))
            {
                tan.gameObject.layer = layer;
            }

           
        }
    }
    public void StartGame()
    {
    }
}
