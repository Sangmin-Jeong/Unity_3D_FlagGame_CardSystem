
using UnityEngine;

public class CameraSwitch : MonoBehaviour
{
    // Probably easier to use an array, might be confusing to work with though
    //public GameObject[] cameras;
   // public GameObject MainCamera;
    public GameObject CameraOne;
    public GameObject CameraTwo;


    private int DiceRoll;

    // Start is called before the first frame update
    void Start()
    {
       // MainCamera.SetActive(true);

        CameraOne.SetActive(true);
        CameraTwo.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        // Dice Roll
        if (Input.GetKeyDown(KeyCode.Space))
        {
            DiceRoll = Random.Range(0, 6);
            print("Dice roll: " + DiceRoll);
        }

        // Switching between cameras using '1' and '2'
        {
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                DiceRoll = 1;
                CameraOne.SetActive(true);
                CameraTwo.SetActive(false);
            }
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                DiceRoll = 2;
                CameraOne.SetActive(false);
                CameraTwo.SetActive(true);
            }
        }

        //If dice roll is odd P1 goes, if dice roll is even P2 goes
        {
            if (DiceRoll is 1 or 3 or 5)
            {
                //print("Player 1 goes first");
                CameraOne.SetActive(true);
                CameraTwo.SetActive(false);
            }
            else if (DiceRoll is 2 or 4 or 6)
            {
                //print("Player 2 goes first");
                CameraTwo.SetActive(true);
                CameraOne.SetActive(false);
            }
        }
    }
}
