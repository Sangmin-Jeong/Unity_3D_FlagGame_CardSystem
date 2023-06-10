using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Unity.VisualScripting;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.EventSystems;
using System.Linq;
using UnityEngine.SceneManagement;

public class MatchSystem : MonoBehaviour
{
    const float diceTimer = 3.0f;
    const float diceDelayTimer = 1.0f;

    // UI-related match data
    // note: better way to do this for sure, I will update it later though
    [Header("Match UI Settings")]
    [SerializeField] Image plr1TurnImage;
    [SerializeField] Image plr2TurnImage;
    [SerializeField] GameObject diceRollPanel;
    [SerializeField] TMP_Text plr1DiceText;
    [SerializeField] TMP_Text plr2DiceText;
    [SerializeField] TMP_Text plrTurnTimerText;
    [SerializeField] Image plrTurnTimerImage;
    [SerializeField] TMP_Text plr1TurnText;
    [SerializeField] TMP_Text plr2TurnText;
    [SerializeField] Button endButton;
    [SerializeField] GameObject TurnNumImage;
    [SerializeField] TMP_Text TurnNum;
    [SerializeField] GameObject PlayerIndicatorImage;
    [SerializeField] TMP_Text PlayerIndicator;
    [SerializeField] GameObject PlayerIndicatorImage2;
    [SerializeField] TMP_Text PlayerIndicator2;

    [Header("Camera Settings")]
    [SerializeField] GameObject Player1Object;
    [SerializeField] GameObject Player2Object;
    [SerializeField] GameObject SpectatorObject;
    [SerializeField] float maxFocalLength = 60.0f;
    [SerializeField] float minFocalLength = 0.0f;

    [Header("Match Settings")]
    [SerializeField] private float turnLengthSeconds;
    private bool plr1Turn;
    private bool isStartOfMatch;
    private bool isEndOfMatch;

    // Other private variables
    private float diceRollTimer;
    private float turnLengthTimer;
    [SerializeField] float buttonCooldown = 3.0f;
    bool isButtonInteractable;

    private bool playerTurn;

    // Events
    public static event Action<int> OnPlayerSwitch;

    private void Awake()
    {
        plr1TurnImage.gameObject.SetActive(true);
        plr2TurnImage.gameObject.SetActive(true);
        SpectatorObject.SetActive(true);
        Player1Object.SetActive(false);
        Player2Object.SetActive(false);
        diceRollPanel.SetActive(true);
        plrTurnTimerImage.gameObject.SetActive(false);
        OnPlayerSwitch = delegate { };
    }

    private void Start()
    {
        ActivateUIs(false);
        endButton.enabled = false;
        turnLengthSeconds = DebuggingObject.Instance.TurnTime;
        diceRollTimer = diceTimer;
        turnLengthTimer = 0.0f;

        isStartOfMatch = true;
        isEndOfMatch = false;

        //Turn Text
        TurnNum.text = Match.Instance.TurnNumber.ToString();
        PlayerIndicatorImage.gameObject.SetActive(false);
        PlayerIndicatorImage2.gameObject.SetActive(false);
    }

    void Update()
    {
        CheckForMatchWinner();

        MouseEvents();


        if (!isEndOfMatch)
        {
           

            if (isStartOfMatch) // If we are just beginning the match
            {
                StartMatch();
                TurnNum.text = Match.Instance.TurnNumber.ToString();
            }
            else
            {
                if (diceRollTimer <= 0.0f)
                {
                    diceRollPanel.SetActive(false);
                    if (turnLengthTimer <= 0.0f)
                    {
                        turnLengthTimer = turnLengthSeconds;
                        StartTurn(plr1Turn);
                        plr1Turn = !plr1Turn; // Set it to the next player!

                    } else if (turnLengthTimer <= 11.0f) // 11 or else the 10 seconds won't be red.
                    {
                        plrTurnTimerText.color = Color.red;
                    } else
                    {
                        plrTurnTimerText.color = Color.white;
                    }
                    plrTurnTimerImage.gameObject.SetActive(true);

                    plrTurnTimerText.text = ((int)turnLengthTimer).ToString();
                    plrTurnTimerImage.fillAmount = turnLengthTimer / turnLengthSeconds;
                    turnLengthTimer -= Time.deltaTime;

                }
                diceRollTimer -= Time.deltaTime;
            }
            CheckForEndButtonCooldown();
        }
       
    }

    void ActivateUIs(bool set)
    {
        endButton.gameObject.SetActive(set);
        TurnNumImage.gameObject.SetActive(set);
        //TurnNum.gameObject.SetActive(set);
        //PlayerIndicatorImage.gameObject.SetActive(set);
        //PlayerIndicatorImage2.gameObject.SetActive(set);
        //PlayerIndicator.gameObject.SetActive(set);
    }
    void CheckForMatchWinner()
    {
        if (Unit.isP1 || Match.isP1)
        {
            isEndOfMatch = true;
            StartCoroutine(TurnUpFocalLength(Player1Object));
            StartCoroutine(TurnUpFocalLength(Player2Object));
        }
        else if (Unit.isP2 || Match.isP2)
        {
            isEndOfMatch = true;
            StartCoroutine(TurnUpFocalLength(Player1Object));
            StartCoroutine(TurnUpFocalLength(Player2Object));
        }
        else if (Match.isTie)
        {
            isEndOfMatch = true;
            StartCoroutine(TurnUpFocalLength(Player1Object));
            StartCoroutine(TurnUpFocalLength(Player2Object));
        }
    }


    void MouseEvents()
    {
        PointerEventData pData = new PointerEventData(EventSystem.current);
        pData.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pData, results);


        foreach (RaycastResult result in results)
        {
            if (results.Where(r => r.gameObject.layer == 5).Count() > 0)
            {
                // If we are looking at the end button.
                if (!isEndOfMatch)
                {
                    if (result.gameObject.CompareTag("endButton"))
                    {
                        if (Input.GetMouseButtonUp(0))
                        {
                            EndTurn();
                        }
                    }
                }
                // If we are looking at the menu button.
                if (result.gameObject.CompareTag("menuButton"))
                {
                    if (Input.GetMouseButtonUp(0))
                    {
                        SceneManager.LoadSceneAsync("Menu");
                    }
                }
                // If we are looking at the quit button.
                if (result.gameObject.CompareTag("quitButton"))
                {
                    if (Input.GetMouseButtonUp(0))
                    {
                        Application.Quit();
                    }
                }
            }
            
        }

    }

    void CheckForEndButtonCooldown()
    {
        if (!isStartOfMatch)
        {
            if (buttonCooldown <= 0)
            {
                buttonCooldown = 4.0f;
                isButtonInteractable = true;
            }
            else
            {
                buttonCooldown -= Time.deltaTime;
            }
            endButton.interactable = isButtonInteractable;
        }
       
    }

    void StartMatch()
    {

        int pl1DiceRoll = DiceRoll();
        int pl2DiceRoll = DiceRoll();

        plr1DiceText.text = pl1DiceRoll.ToString();
        plr2DiceText.text = pl2DiceRoll.ToString();
        if (diceRollTimer < 0.0f) // If we are done dice rolling
        {
            isStartOfMatch = false;
            if (pl1DiceRoll > pl2DiceRoll) // If player 1 is the higher number
            {
                plr1Turn = true;
                diceRollTimer = diceDelayTimer;
                print("Player 1 goes first!");
                Match.Instance.StartGame();

            }
            else if (pl1DiceRoll == pl2DiceRoll) // If dice rolls are equal
            {
                isStartOfMatch = true;
                diceRollTimer = diceTimer;
            } 
            else // If player 2 is the higher number
            {
                plr1Turn = false;
                diceRollTimer = diceDelayTimer;
                print("Player 2 goes first!");
                Match.Instance.StartGame();
            }
        }
        diceRollTimer -= Time.deltaTime;
    }

    public void EndTurn()
    {
        if (isButtonInteractable)
        {
            isButtonInteractable = false;
            turnLengthTimer = 0.0f;
        }
    }
    void StartTurn(bool whoseTurn)
    {
        playerTurn = whoseTurn;
        if(whoseTurn)
        {
            PlayerIndicatorImage2.gameObject.SetActive(false);
            PlayerIndicatorImage.gameObject.SetActive(true);
            PlayerIndicator.text = "P1";
            PlayerIndicatorImage.GetComponent<Image>().color = UnitManager.Instance.player1UnitMaterial.color;
            //PlayerIndicator.color = UnitManager.Instance.player1UnitMaterial.color;
        }
        else
        {
            PlayerIndicatorImage.gameObject.SetActive(false);
            PlayerIndicatorImage2.gameObject.SetActive(true);
            PlayerIndicator2.text = "P2";
            PlayerIndicatorImage2.GetComponent<Image>().color = UnitManager.Instance.player2UnitMaterial.color;
            //PlayerIndicator2.color = UnitManager.Instance.player2UnitMaterial.color;
        }
        
        Match.Instance.whoseTurn(whoseTurn);
        SpectatorObject.SetActive(false);

        Player1Object.SetActive(true);
        Player2Object.SetActive(true);

        ActivateUIs(true);

        if (whoseTurn) // Is player 1's turn
        {
            OnPlayerSwitch?.Invoke(0);

            //disable
            Match.Instance.currentCamera = Player2Object.transform.GetChild(1).gameObject;
            Match.Instance.setPostProces(true);
            
            Player2Object.transform.GetChild(0).gameObject.SetActive(false);
            Player2Object.GetComponent<CameraController>().enabled = false;

            //enable
            Player1Object.transform.GetChild(0).gameObject.SetActive(true);
            Match.Instance.currentCamera = Player1Object.transform.GetChild(1).gameObject;
            Match.Instance.setPostProces(false);
            Player1Object.GetComponent<CameraController>().enabled = true;
            Player1Object.GetComponent<CameraController>().MoveToFlag();
            

            // StartCoroutine(TurnDownFocalLength(Player1Object));
            //StartCoroutine(TurnUpFocalLength(Player2Object));

            //StartCoroutine(StartPlayerTurnScreen(plr1TurnImage));
            //StartCoroutine(StartPlayerTurnScreenText(plr1TurnText));

        } else // Is player 2's turn
        {
            OnPlayerSwitch?.Invoke(1);
            //disable
            Match.Instance.currentCamera = Player1Object.transform.GetChild(1).gameObject;
            Match.Instance.setPostProces(true);
            
            Player1Object.transform.GetChild(0).gameObject.SetActive(false);
            Player1Object.GetComponent<CameraController>().enabled = false;

            //enable
            Player2Object.transform.GetChild(0).gameObject.SetActive(true);
            Match.Instance.currentCamera = Player2Object.transform.GetChild(1).gameObject;
            Match.Instance.setPostProces(false);
            Player2Object.GetComponent<CameraController>().enabled = true;
            Player2Object.GetComponent<CameraController>().MoveToFlag();
            



            //StartCoroutine(TurnDownFocalLength(Player2Object));
            // StartCoroutine(TurnUpFocalLength(Player1Object));
            // StartCoroutine(StartPlayerTurnScreen(plr2TurnImage));
            //StartCoroutine(StartPlayerTurnScreenText(plr2TurnText));
        }

        UnitManager.Instance.RestroeUnitStats();

    }

    // Coroutine to maximize the start player bar
    IEnumerator StartPlayerTurnScreen(Image plrImageToStart)
    {
        float counter = 1.5f;
        while (plrImageToStart.rectTransform.sizeDelta.y < 200)
        {
            plrImageToStart.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, counter);
            counter += 1.5f;
            yield return new WaitForSeconds(0.001f);
        }

        StopCoroutine(StartPlayerTurnScreen(plrImageToStart));

        yield return new WaitForSeconds(1.0f);
        StartCoroutine(ClosePlayerTurnScreen(plrImageToStart));

        yield return null;
    }

    // Coroutine to minimize the start player bar
    IEnumerator ClosePlayerTurnScreen(Image plrImageToClose)
    {
        float counter = plrImageToClose.rectTransform.sizeDelta.y;
        while (plrImageToClose.rectTransform.sizeDelta.y > 0)
        {
            plrImageToClose.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, counter);
            counter -= 1.5f;
            yield return new WaitForSeconds(0.001f);
        }
        StopCoroutine(ClosePlayerTurnScreen(plrImageToClose));
        yield return null;
    }

    IEnumerator StartPlayerTurnScreenText(TMP_Text textToStart)
    {
        while (textToStart.fontSize < 100)
        {
            textToStart.fontSize += 2;
            yield return new WaitForSeconds(0.001f);
        }
        StopCoroutine(StartPlayerTurnScreenText(textToStart));
        yield return new WaitForSeconds(1.0f);
        StartCoroutine(ClosePlayerTurnScreenText(textToStart));
        yield return null;
    }

    IEnumerator ClosePlayerTurnScreenText(TMP_Text textToClose)
    {
        while (textToClose.fontSize > 0)
        {
            textToClose.fontSize -= 2;
            yield return new WaitForSeconds(0.001f);
        }
        StopCoroutine(ClosePlayerTurnScreenText(textToClose));
        yield return null;
    }

    IEnumerator TurnUpFocalLength(GameObject playerObj)
    {
        playerObj.transform.GetChild(0).gameObject.SetActive(false);
        playerObj.GetComponent<CameraController>().enabled = false;
        DepthOfField tempField;
        GameObject cameraToFocus = playerObj.transform.GetChild(1).gameObject;
        cameraToFocus.GetComponent<PostProcessVolume>().profile.TryGetSettings<DepthOfField>(out tempField);
        float currentLength = 0.0f;
        while (currentLength < maxFocalLength)
        {
            currentLength += 2.0f;
            tempField.focalLength.value = currentLength;
            yield return new WaitForSeconds(0.001f);
        }
        
        //StopCoroutine(TurnUpFocalLength(cameraToFocus));
        yield return null;
    }

    IEnumerator TurnDownFocalLength(GameObject playerObj)
    {
        GameObject cameraToFocus = playerObj.transform.GetChild(1).gameObject;
        playerObj.transform.GetChild(0).gameObject.SetActive(true);
        DepthOfField tempField;
        cameraToFocus.GetComponent<PostProcessVolume>().profile.TryGetSettings<DepthOfField>(out tempField);
        float currentLength = tempField.focalLength.value;
        while (currentLength > minFocalLength)
        {
            currentLength -= 2.0f;
            tempField.focalLength.value = currentLength;
            yield return new WaitForSeconds(0.001f);
        }
        //StopCoroutine(TurnDownFocalLength(cameraToFocus));
        playerObj.GetComponent<CameraController>().MoveToFlag();
        playerObj.GetComponent<CameraController>().enabled = true;

        Match.Instance.setPostProces(true);
        Match.Instance.currentCamera = cameraToFocus;
        Match.Instance.setPostProces(false);
        yield return null;
    }

    int DiceRoll()
    {
        return UnityEngine.Random.Range(1, 7);
    }

    public void MainMenu()
    {
        SceneManager.LoadSceneAsync("Menu");
    }

    public void Quit()
    {
        Application.Quit();
    }
}
