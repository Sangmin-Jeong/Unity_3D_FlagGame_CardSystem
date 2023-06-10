using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
//using UnityEditor.Experimental.GraphView;
//using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using UnityEngine.UIElements;
//using static UnityEditor.Experimental.GraphView.GraphView;
//using UnityEngine.WSA;
//using UnityEngine.WSA;

[RequireComponent(typeof(BoxCollider))]
public class Tile : MonoBehaviour
{
    [Header("Tile Settings")]
    public bool filled = false;
    public int fogRange = 1;


    [Header("Tile Info")]
    public int player = 3; // Set to no player
    public Vector2Int hexCoordinate;
    public Tile[] Neighbors = new Tile[6];
    public bool Both = false;


    [Header("Basic Tile Stuff")]
    public BoxCollider m_boxCollider;


    [Header("Card Stuf")]
    public CardManager cardManager;
    public UnitCardObject placedUnit;
    public GameObject placedCardGameObject;


    [Header("Unit")]
    public Unit CurrentUnit;


    [Header("Materials")]
    public Material plr1Mat;
    public Material plr2Mat;
    public Material bothMat;


    // System variables
    private bool isMouseOver = false;
    
    public int lastLayer = 0;
    public GameObject flag;

    //[HideInInspector]
    public int currentDistance = 0;

    void Start()
    {
        //components
        m_boxCollider = GetComponent<BoxCollider>();


        //Managers
        
        cardManager = FindObjectOfType<CardManager>();

        // set defaults
        m_boxCollider.size = Vector3.one;

    }
    private void Update()
    {
        // NOTE: TMRW GONNA THROW THIS IN A ACTUAL FUNCTION, JUST FOR NOW TEMP
        //components
       // m_boxCollider = GetComponent<BoxCollider>();


        //Managers why because there are 2
        
        cardManager = FindObjectOfType<CardManager>();
        placeCards();
        mouseEvents();
        if(Both)
        {
            Match.Instance.setLayer(this.gameObject,8);
        }

    }


    public void SetMaterials(Material plr1, Material plr2, Material both)
    {
        plr1Mat = plr1;
        plr2Mat = plr2;
        bothMat = both;
    }

    private void placeCards()
    {
        // placing cards
        if (isMouseOver)
        {
            if (cardManager != null && cardManager.selectedCard != null && cardManager.selectedCard.isClicked && cardManager.selectedCard.isReleased && cardManager.hasEnoughMana)
            {
                cardManager.selectedCard.isReleased = false;

                switch (cardManager.selectedCard.cardObject.cardType)
                {
                    // unit placement
                    case CARDTYPE.UNIT:
                        if (filled && CurrentUnit == null && player == Match.Instance.getCurrentPlayer())
                        {
                            Card temp = cardManager.selectedCard.GetComponent<Card>();
                            CurrentUnit = UnitManager.Instance.PlaceUnit(cardManager.selectedCard.cardObject.CPrefab, this.gameObject, Match.Instance.getCurrentPlayer());
                            
                            //To put the card info into the unit for hovering
                            CurrentUnit.transform.Find("Canvas").transform.Find("Hover").GetComponent<HoverUnit>().parentCard = temp;
                           
                            cardManager.UseCard(transform);
                        }
                        break;

                    // Terrain placement
                    case CARDTYPE.TERRAIN:
                        if (CheckIfFillable())
                        {
                            ChangeTile(cardManager.selectedCard.cardObject.CPrefab, Match.Instance.getCurrentPlayer(), cardManager.selectedCard.cardObject.fogRange);
                            SetTileOwner(player);

                            cardManager.UseCard(transform);
                        }
                        break;
                }
            }
        }
    }
    public void ChangeTile(GameObject newTile, int pla, int fogRange)
    {
        
        // first layer click
        if (CheckIfFillable())
        {
            this.fogRange = fogRange;
            player = pla;

            // destroy empty tile
            Destroy(transform.GetChild(0).gameObject);

            // create new filled tile
            GameObject hey = Instantiate(newTile, transform);
            hey.transform.position= transform.position;
           transform.position -= new Vector3(0, 0.1f, 0);
            // setting state of tile
            filled = true;

            if (pla == 0)
            {
                hey.GetComponentInChildren<MeshRenderer>().material = plr1Mat;
                // to change whole tile to color
                //if(tag == "Flag")
                //{
                //    foreach(MeshRenderer rend in hey.GetComponentsInChildren<MeshRenderer>())
                //    {
                //        rend.material = plr1Mat;
                //        rend.material.color = new UnityEngine.Color(rend.material.color.r, rend.material.color.g, rend.material.color.b, rend.material.color.a / 2);

                //    }

                //}
            }
            else if (pla == 1)
            {
                hey.GetComponentInChildren<MeshRenderer>().material = plr2Mat;


                //if (tag == "Flag")
                //{
                //    foreach (MeshRenderer rend in hey.GetComponentsInChildren<MeshRenderer>())
                //    {
                //        rend.material = plr2Mat;
                //        rend.material.color = new UnityEngine.Color(rend.material.color.r, rend.material.color.g, rend.material.color.b, rend.material.color.a /2);
                //    }

                //}
            }
            EnableNeighbors(fogRange, player);

            TileManager.Instance.playVfx(transform);
            TileManager.Instance.TileCounter();
        }
    }

    public void EnableNeighbors(int range, int thisPlayer)
    {
    // fogrange need to implement
     
       foreach (Tile neighbor_tile in getAllNeighbours(range, new Tile[1]))
       {
           if (neighbor_tile != null)
           {

                if ((neighbor_tile.player == 0 && thisPlayer == 1)
                || (neighbor_tile.player == 1 && thisPlayer == 0)
                )
                {
                    Match.Instance.setLayer(this.gameObject, 8);
                    Both = true;
                }

                // both players layer 8
                if ((neighbor_tile.gameObject.layer == 7 && neighbor_tile.player == 0 && thisPlayer == 1)
                        || (neighbor_tile.gameObject.layer == 6 && neighbor_tile.player == 1 && thisPlayer == 0)
                        || (neighbor_tile.gameObject.layer == 7 && neighbor_tile.player == 3 && thisPlayer == 1)
                        || (neighbor_tile.gameObject.layer == 6 && neighbor_tile.player == 3 && thisPlayer == 0))
                    {

                        Match.Instance.setLayer(neighbor_tile.gameObject, 8);
                        neighbor_tile.Both = true;

                    }
                    // player 1 alone layer 7
                    else if (thisPlayer == 0 && neighbor_tile.gameObject.layer != 8)
                    {
                        Match.Instance.setLayer(neighbor_tile.gameObject, 7);
                        neighbor_tile.lastLayer = 7;
                    }
                    // player 2 alone layer 6
                    else if (thisPlayer == 1 && neighbor_tile.gameObject.layer != 8)
                    {
                        Match.Instance.setLayer(neighbor_tile.gameObject, 6);
                        neighbor_tile.lastLayer = 6;
                    }
                    

            }
       }
    }
    public void EnableNeighborsForUnits(int range, int thisPlayer)
    {
        // fogrange need to implement

        foreach (Tile neighbor_tile in getAllNeighbours(range, new Tile[1]))
        {
            if (neighbor_tile != null)
            {
               

                if ((neighbor_tile.player == 0 && thisPlayer == 1)
                || (neighbor_tile.player == 1 && thisPlayer == 0)
                )
                {
                    Match.Instance.setLayer(this.gameObject, 8);
                    
                }

                // both players layer 8
                if ((neighbor_tile.gameObject.layer == 7 && neighbor_tile.player == 0 && thisPlayer == 1)
                        || (neighbor_tile.gameObject.layer == 6 && neighbor_tile.player == 1 && thisPlayer == 0)
                        || (neighbor_tile.gameObject.layer == 7 && neighbor_tile.player == 3 && thisPlayer == 1)
                        || (neighbor_tile.gameObject.layer == 6 && neighbor_tile.player == 3 && thisPlayer == 0))
                {

                    Match.Instance.setLayer(neighbor_tile.gameObject, 8);
                    

                }
                // player 1 alone layer 7
                else if (thisPlayer == 0 && neighbor_tile.gameObject.layer != 8)
                {
                    Match.Instance.setLayer(neighbor_tile.gameObject, 7);
                    neighbor_tile.lastLayer = 7;
                }
                // player 2 alone layer 6
                else if (thisPlayer == 1 && neighbor_tile.gameObject.layer != 8)
                {
                    Match.Instance.setLayer(neighbor_tile.gameObject, 6);
                    neighbor_tile.lastLayer = 6;
                }


            }
        }
    }
    public void DisableNeighborsForUnits(int range, int thisPlayer)
    {
        //disabled for now figure out a different way of doing this
        ////this tile
        //if(!Both)
        //{


        //    if (this.player == 0 )
        //    {
        //        Match.Instance.setLayer(this.gameObject, 7);
        //    }
        //    else if (this.player == 1 ) 
        //    {
        //        Match.Instance.setLayer(this.gameObject, 6);
        //    }
        //}


        //// neighbors
        //foreach (Tile neighbor_tile in getAllNeighbours(range, new Tile[1]))
        //{
        //    if (neighbor_tile != null) 
        //    {
        //        if (!neighbor_tile.Both)
        //        {
        //            //placed tiles
        //            if (neighbor_tile.player == 0 )
        //            {
        //                Match.Instance.setLayer(neighbor_tile.gameObject, 7);
        //            }
        //            else if (neighbor_tile.player == 1 )
        //            {
        //                Match.Instance.setLayer(neighbor_tile.gameObject, 6);
        //            }
        //            //empty tiles
        //            else if (neighbor_tile.player == 3 )
        //            {
        //                Match.Instance.setLayer(neighbor_tile.gameObject, neighbor_tile.lastLayer);
        //            }
        //        }
        //    }
        //}


    }


    public Tile[] getAllNeighbours(int range, Tile[] emptyArray)
    {
        Tile[] allNeighbours = new Tile[48];
        int pos = 0;

        foreach(Tile nei in Neighbors)
        {
            if(nei != null)
            {

                bool inside = false;
                foreach (Tile curNei in emptyArray)
                {
                    if (curNei == nei)
                        inside = true;
                }
                if (!inside)
                {
                    nei.currentDistance = 1;
                    allNeighbours[pos++] = nei;
                }

                // second neibours
                if (range>=2)
                {
                    
                    foreach(Tile nei2 in nei.getAllNeighbours(1, allNeighbours))
                    {
                        if(nei2!= null)
                        {
                           
                            inside = false;
                            foreach(Tile curNei in allNeighbours)
                            {
                                if(curNei == nei2)
                                    inside = true;
                            }
                            if(!inside)
                            {
                                nei2.currentDistance= 2;
                                allNeighbours[pos++] = nei2;
                            }
                            

                            //third neibours
                            if (range == 3)
                            {

                                foreach (Tile nei3 in nei2.getAllNeighbours(1, allNeighbours))
                                {
                                    if (nei3 != null)
                                    {
                                        inside = false;
                                        foreach (Tile curNei in allNeighbours)
                                        {
                                            if (curNei == nei3)
                                                inside = true;
                                        }
                                        if (!inside)
                                        {
                                            nei3.currentDistance= 3;
                                            allNeighbours[pos++] = nei3;
                                        }
                                            
                                    }

                                }
                            }
                        }
                        
                    }

                    


                }

            }
        }



        return allNeighbours;
    }


    public bool CheckIfFillable()
    {
        if (!filled &&
            ((gameObject.layer == 7 && Match.Instance.getCurrentPlayer() == 0)
            ||
            (gameObject.layer == 6 && Match.Instance.getCurrentPlayer() == 1)
            ||
            (gameObject.layer == 8)
            ||
            tag == "Flag"))
        {
        return true;
        }
        else
        {
            return false;
        }

    }
    public bool CheckIfVisible()
    {
        if (
    (gameObject.layer == 7 && Match.Instance.getCurrentPlayer() == 0)
    ||
    (gameObject.layer == 6 && Match.Instance.getCurrentPlayer() == 1)
    ||
    (gameObject.layer == 8))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public void SetTileOwner(int playerNum)
    {
        if(playerNum == 0)
        {
            foreach (Transform u in transform.GetComponentsInChildren<Transform>())
            {
                if (!u.gameObject.CompareTag("transparent"))
                {
                    u.gameObject.layer = 7;
                } else
                {
                    u.gameObject.layer = 6;
                }
            }
            player = 0;
        }
        else if(playerNum == 1)
        {
            foreach (Transform u in transform.GetComponentsInChildren<Transform>())
            {
                if (!u.gameObject.CompareTag("transparent"))
                {
                    u.gameObject.layer = 6;
                }
                else
                {
                    u.gameObject.layer = 7;
                }
            }
            player = 1;
        }
            
    }
    private void mouseEvents()
    {
        if (Match.Instance.currentCamera != null)
        {
            RaycastHit hit = new RaycastHit();
            Ray ray = Match.Instance.currentCamera.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);

            // OnMouseOver for combat
            //hover
            if (Physics.Raycast(ray, out hit) && hit.collider == m_boxCollider)
            {
                isMouseOver = true;

                unitMouseEvents(hit);
            }
            else
            {
                isMouseOver = false;
            }
        }
    }
    private void unitMouseEvents(RaycastHit hitData)
    {
        if (Input.GetMouseButtonDown(0) && CurrentUnit != null && CurrentUnit.getCurrentMovement() != 0)
        {
            UnitManager.Instance.DragUnit(CurrentUnit, this.gameObject);

        }
        // place unit
        else if ((Input.GetMouseButtonUp(0) 
            && UnitManager.Instance.getSelectedUnit() != null 
            && (filled || UnitManager.Instance.getSelectedUnit().type == UNITTYPE.JET || CurrentUnit!=null)
            && CheckIfVisible())
            )
        {
            if (CurrentUnit == null ||(CurrentUnit == null&& UnitManager.Instance.getSelectedUnit().type == UNITTYPE.JET))
            {
                UnitManager.Instance.UnDragUnit(this);
                
            }
            //damage
            else if(CurrentUnit != null)
            {
                
                
                if (UnitManager.Instance.getSelectedUnit().GetComponent<Unit>().getCurrentTile().GetComponent<Tile>() != this.GetComponent<Tile>()
                    && UnitManager.Instance.getSelectedUnit().GetComponent<Unit>().getPlayer() != CurrentUnit.GetComponent<Unit>().getPlayer()
                    && UnitManager.Instance.getSelectedUnit().GetComponent<Unit>().getCurrentRange() > 0)
                {
                    int dis = UnitManager.Instance.checkIfNeigborInRange(UnitManager.Instance.getSelectedUnit().getCurrentRange(), this);
                    if (0 != dis)
                    {
                        UnitManager.Instance.Combat(CurrentUnit.GetComponent<Unit>());
                    }
                    
                }
                    

            }
            
           
            

        }

    }
    public void addFlag()
    {
        flag.SetActive(true);
    }


    public void toggleTransparentOn(bool toggle,int player,Material mat)
    {
        foreach (Transform u in transform.GetComponentsInChildren<Transform>())
        {
            if (u.gameObject.CompareTag("transparent"))
            {
                if(!toggle)
                {
                    
                    if(this.player == 0)
                    {
                        Match.Instance.setLayer(u.gameObject, 6);
                    }
                    else if(this.player == 1)
                    {
                        Match.Instance.setLayer(u.gameObject, 7);
                    }
                    else
                    {
                        Match.Instance.setLayer(u.gameObject, 0);
                    }
                    foreach (MeshRenderer rend in u.gameObject.GetComponentsInChildren<MeshRenderer>())
                    {
                        rend.material = TileManager.Instance.TransparentNorm;
                    }
                }
                else if(player == 0 )
                {
                    Match.Instance.setLayer(u.gameObject, 7);
                    foreach(MeshRenderer rend in u.gameObject.GetComponentsInChildren<MeshRenderer>())
                    {
                        rend.material = mat;
                    }
                    


                }
                else if (player == 1)
                {
                    Match.Instance.setLayer(u.gameObject, 6);
                    foreach (MeshRenderer rend in u.gameObject.GetComponentsInChildren<MeshRenderer>())
                    {
                        rend.material = mat;
                    }
                }
                
            }

        }
    }
   
}



