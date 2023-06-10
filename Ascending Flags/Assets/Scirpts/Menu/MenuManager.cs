using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using System;

public class MenuManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] int maxGridSize;
    [SerializeField] int minGridSize;
    [SerializeField] float maxBlur;
    [SerializeField] Material plr1Mat;
    [SerializeField] Material plr2Mat;

    private int currentSize;
    private int currentPlayer;
    private bool canMoveTile = false;

    [SerializeField] GameObject clickToPlayScreen;
    [SerializeField] GameObject buttons;
    [SerializeField] GameObject loadingScreen;

    [Header("Starting Screen")]
    [SerializeField] GameObject startingScreen;
    [SerializeField] TMP_Text startingText;
    [SerializeField] Image startingLogo;
    [SerializeField] float startingScreenTimerLength;

    [Header("Settings Screen")]
    [SerializeField] GameObject settingsScreen;
    [SerializeField] TMP_Dropdown gridSizeDropDown;
    [SerializeField] Transform selectionTile1;
    [SerializeField] Transform selectionTile2;
    [SerializeField] TMP_Text[] warning_texts;

    /* Private variables */
    private float timer;
    static bool firstTimeInMenu = true;
    bool hasEnteredStartScreen;
    bool playingAnimation;

    private void Awake()
    {
        hasEnteredStartScreen = false;
        playingAnimation = false;
        Time.timeScale = 1.0f;
        StopAllCoroutines();
        if (firstTimeInMenu)
        {
            PlayStartScreen();
        } else
        {
            SetupStartScreen();
        }
        SetupSettingsMenu();
    }

    private void SetupSettingsMenu()
    {
        currentSize = minGridSize;
        plr1Mat.color = new Color(1, 0, 1, 255);
        plr2Mat.color = Color.blue;
        /* Adding grid size options to the dropdown bar. */
        List<TMP_Dropdown.OptionData> sizes = new List<TMP_Dropdown.OptionData>();

        // For loop on a count of 2.
        for (int i = minGridSize; i <= maxGridSize; i+= 2)
        {
            TMP_Dropdown.OptionData temp = new TMP_Dropdown.OptionData();
            temp.text = i.ToString() + "x" + i.ToString();
            sizes.Add(temp);
        }
        gridSizeDropDown.AddOptions(sizes);
    }

    private void SetupStartScreen()
    {
        settingsScreen.SetActive(false);
        loadingScreen.SetActive(false);
        clickToPlayScreen.SetActive(true);
    }

    private void PlayStartScreen()
    {
        playingAnimation = true;
        firstTimeInMenu = false;
        settingsScreen.SetActive(false);
        startingScreen.SetActive(true);
        StartCoroutine(TypeText("A GAME BY", startingText));
        timer = startingScreenTimerLength;
    }

    private void Update()
    {
        MouseEvents();
        // If we are ready to fade the starting screen into the background art.
        if (timer <= 0)
        {
            StartCoroutine(FadeOutImage(startingScreen.GetComponent<Image>()));
            StartCoroutine(FadeOutImage(startingLogo));
            StartCoroutine(FadeOutText(startingText));
        }

        timer -= Time.deltaTime;
    }

    private void MouseEvents()
    {
        if (Input.GetMouseButtonUp(0))
        {
            if (!hasEnteredStartScreen && !playingAnimation)
            {
                hasEnteredStartScreen = true;
                buttons.SetActive(true);
                clickToPlayScreen.SetActive(false);
            }
        }
    }

    public void AskForSettings()
    {
        settingsScreen.SetActive(true);
    }

    public void SetSelectionTile(Transform transform)
    {
        if (canMoveTile)
        {
            Transform tempSelectionTile = (currentPlayer == 0 ? selectionTile1 : selectionTile2);

            tempSelectionTile.position = transform.position;
        }
    }

    public void SetCurrentPlayer(int player)
    {
       currentPlayer = player;
    }

    /* switch statements aren't the best, but works for the time being. */
    public void SetCurrentColor(GameObject currentButton)
    {
        // If player = 0
        Material tempMat = (currentPlayer == 0 ? plr1Mat : plr2Mat);
        Color tempColor = currentButton.GetComponent<Image>().color;

        foreach(TMP_Text text in warning_texts)
        {
            text.text = String.Empty;
        }
        canMoveTile = false;

        if (tempColor == plr2Mat.color)
        {
            warning_texts[0].text = "CANNOT BE SAME COLOR";
        } else if (tempColor == plr1Mat.color)
        {
            warning_texts[1].text = "CANNOT BE SAME COLOR";
        } else
        {
            canMoveTile = true;
            tempMat.color = tempColor;
            tempMat.SetVector("_EmissionColor", tempColor);
        }
    }

    public void SetCurrentGridSize()
    {
        currentSize = minGridSize + (gridSizeDropDown.value * 2);
        
    }

    public void PlayScene()
    {
        settingsScreen.SetActive(false);
        clickToPlayScreen.SetActive(false);
        loadingScreen.SetActive(true);

        PlayerPrefs.SetInt("gridSize", currentSize);
        PlayerPrefs.Save();
        SceneManager.LoadSceneAsync("Match_Scene");
        Time.timeScale = 1.0f;
    }
    public void MenuScene()
    {
        SceneManager.LoadSceneAsync("Menu");
        Time.timeScale = 0.0f;
        Unit.isP1 = false;
        Unit.isP2 = false;
    }
    public void QuitGame()
    {
        Application.Quit();
    }

    /* FINALLY COROUTINES WANT TO WORK WITH ME, YIPPEE */

    IEnumerator FadeOutImage(Image image)
    {
        while(image.color.a > 0)
        {
            yield return new WaitForSeconds(0.001f);
            image.color = new Color(image.color.r, 
                image.color.g, 
                image.color.b, 
                image.color.a - 0.001f);
        }

        StopCoroutine(FadeOutImage(image));
        image.gameObject.SetActive(false);
        playingAnimation = false;
        yield return null;
    }

    IEnumerator FadeInImage(Image image)
    {
        while (image.color.a < 1)
        {
            yield return new WaitForSeconds(0.001f);
            image.color = new Color(image.color.r,
                image.color.g,
                image.color.b,
                image.color.a + 0.01f);
        }

        StopCoroutine(FadeInImage(image));
        yield return null;
    }

    IEnumerator FadeOutText(TMP_Text text)
    {
        while (text.color.a > 0)
        {
            yield return new WaitForSeconds(0.001f);
            text.color = new Color(text.color.r,
                text.color.g,
                text.color.b,
                text.color.a - 0.001f);
        }

        StopCoroutine(FadeOutText(text));
        text.gameObject.SetActive(false);
        playingAnimation = false;
        yield return null;
    }

    IEnumerator TypeText(string text, TMP_Text text_object)
    {
        string temp = string.Empty;
        for(int i = 0; i < text.Length; i++)
        {
            yield return new WaitForSeconds(0.1f);
            temp += text[i];
            text_object.text = temp;
        }

        StopCoroutine(TypeText(text, text_object));
        /* Not the greatest, direct calling. Since this is only used once I will let it slide. */
        StartCoroutine(FadeInImage(startingLogo));
        yield return null;
    }

}