using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHandForP2 : MonoBehaviour
{
    [SerializeField] private CardManager enemyCM;
    [SerializeField] private GameObject P1;
    public int amount = 0;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        amount = enemyCM.amountHand;

        for (int i = 0; i < amount; i++)
        {
            transform.GetChild(i).gameObject.GetComponent<Card>().isfliped = true;
            transform.GetChild(i).gameObject.SetActive(true);
        }

        if(P1)
        {
            for (int i = 0; i < P1.transform.childCount; i++)
            {
                P1.transform.GetChild(i).gameObject.GetComponent<Card>().isfliped = false;
                P1.transform.GetChild(i).gameObject.SetActive(false);
            }
        }

    }
}
