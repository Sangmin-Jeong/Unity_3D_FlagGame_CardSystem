using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class EnemyHandForP1 : MonoBehaviour
{
    [SerializeField] private CardManager enemyCM;
    [SerializeField] private GameObject P2;
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

        if(P2)
        {
            for (int i = 0; i < P2.transform.childCount; i++)
            {
                P2.transform.GetChild(i).gameObject.GetComponent<Card>().isfliped = false;
                P2.transform.GetChild(i).gameObject.SetActive(false);
            }
        }

    }
}
