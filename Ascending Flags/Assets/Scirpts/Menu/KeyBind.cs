using System.Collections;
using System.Collections.Generic;
using Mirror.Examples.Basic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KeyBind : MonoBehaviour
{
    public Toggle FullScreenToggle;
    public static Dictionary<string, KeyCode> keys = new Dictionary<string, KeyCode>();
    public TMP_Text drawCards, flipCards;

    private GameObject currentKey;

    // Start is called before the first frame update
    void Start()
    {
        if (!keys.ContainsKey("1") && !keys.ContainsKey("H"))
        {
            keys.Add("1", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("1", "1")));
            keys.Add("H", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("H", "H")));
        }


        drawCards.text = keys["1"].ToString();
        flipCards.text = keys["H"].ToString();

        FullScreenToggle.isOn = Screen.fullScreen;
    }

    // Update is called once per frame
    void Update()
    {
        Screen.fullScreen = FullScreenToggle.isOn;

        if (Input.GetKeyDown(keys["1"]))
        {
            print("1");
        }

        if (Input.GetKeyDown(keys["H"]))
        {
            print("H");
        }
    }

    void OnGUI()
    {
        if (currentKey != null)
        {
            Event e = Event.current;

            if (e.isKey)
            {
                keys[currentKey.name] = e.keyCode;
                currentKey.transform.GetChild(0).GetComponent<TMP_Text>().text = e.keyCode.ToString();
                SaveKey();
                currentKey = null;
            }
        }
    }

    public void ChangeKey(GameObject clicked)
    {
        currentKey = clicked;
    }

    public void SaveKey()
    {
        foreach (var key in keys)
        {
            PlayerPrefs.SetString(key.Key, key.Value.ToString());
        }
        PlayerPrefs.Save();
    }
}