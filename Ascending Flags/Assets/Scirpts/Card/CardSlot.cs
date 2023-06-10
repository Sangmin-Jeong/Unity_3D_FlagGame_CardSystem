using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class CardSlot : MonoBehaviour
{
    private Renderer renderer;

    CardManager cm;

    public Transform slotObject;
    // Start is called before the first frame update
    void Start()
    {
        renderer = GetComponent<Renderer>();    
        cm = FindObjectOfType<CardManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseEnter()
    {
        renderer.material.color = Color.red;
    }

    private void OnMouseExit()
    {
        renderer.material.color= Color.white; 
    }

    private void OnMouseDown()
    {
        //slotObject = gameObject.transform;
        //cm.UseCard(slotObject);
    }
}
