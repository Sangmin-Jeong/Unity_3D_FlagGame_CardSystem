using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Pause : MonoBehaviour
{
    public static bool isGamePaused = false;

    public GameObject PausemenuUI;

    // Only need 2, but it's easier to understand this way
    public GameObject Win;
    public GameObject Lose;
    public GameObject Win2;
    public GameObject Lose2;
    
    public GameObject Tie;
 

    public GameObject[] ObjectsToRemove;

    public GameObject QuitButton;
    public GameObject MenuButton;

    // If we want to display text for who wins
    //public TMP_Text WinnerText;
    //public TMP_Text LoserText;
    //public TMP_Text WinnerText2;
    //public TMP_Text LoserText2;

    // Private variables

    private int num;

    void Start()
    {
    }
    // Update is called once per frame
    void Update()
    {
       
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isGamePaused)
            {
                Resume();
            }
            else
            {
                PauseGame();
            }
        }

        //if (Unit.isP1 || Match.isP1)
        bool won = false;

        if (Match.Instance.getWhoWon() == 1 || Match.Instance.getWhoWon() == 2 || Match.Instance.getWhoWon() == 3)
            won = true;
            
        if(won)
        {

        
        if (Match.isP1)
        {
            Time.timeScale = 0.0f;

            Win.SetActive(true);
            Lose2.SetActive(true);

            QuitButton.SetActive(true);
            MenuButton.SetActive(true);

            foreach (var obj in ObjectsToRemove)
            {
                obj.gameObject.SetActive(false);
            }
        }
        else if (Match.isP2)
        {
            Time.timeScale = 0.0f;

            Win2.SetActive(true);
            Lose.SetActive(true);

            QuitButton.SetActive(true);
            MenuButton.SetActive(true);

            foreach (var obj in ObjectsToRemove)
            {
                obj.gameObject.SetActive(false);
            }
        }
        else if (Match.isTie)
        {
            Time.timeScale = 0.0f;

            Tie.SetActive(true);
            QuitButton.SetActive(true);
            MenuButton.SetActive(true);

            foreach (var obj in ObjectsToRemove)
            {
                obj.gameObject.SetActive(false);
            }
        }
        }
    }

    public void Resume()
    {
        PausemenuUI.SetActive(false);

        foreach (var obj in ObjectsToRemove)
        {
            obj.gameObject.SetActive(true);
        }

        Time.timeScale = 1.0f;
        isGamePaused = false;
    }

    void PauseGame()
    {
        PausemenuUI.SetActive(true);

        foreach (var obj in ObjectsToRemove)
        {
            obj.gameObject.SetActive(false);
        }

        Time.timeScale = 0.0f;
        isGamePaused = true;
    }

    public void Surrender()
    {
        Resume();
        if (CameraController.playerID == 0)
        {
            Time.timeScale = 0.0f;

            Lose.SetActive(true);
            Win2.SetActive(true);
        }
        else
        {
            Time.timeScale = 0.0f;

            Lose2.SetActive(true);
            Win.SetActive(true);
        }
        SceneManager.LoadScene("Menu");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}