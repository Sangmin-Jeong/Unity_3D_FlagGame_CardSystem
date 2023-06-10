using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.XR;
using UnityEngine.UI;
using System;

public class Unit : MonoBehaviour
{
    [SerializeField] private UnitCardObject cardObject;

    [SerializeField]
    private CapsuleCollider capsuleCollider;
    private bool drag;
    private Tile CurrentTile;
    private int Player;

    

    public static bool isP1 = false;
    public static bool isP2 = false;


    // unit stats
    [HideInInspector]
    public UNITTYPE type;
    [HideInInspector]
    public int MaxHealth;
    [HideInInspector]
    public int Health;
    [HideInInspector]
    public int Atk;
    [HideInInspector]
    public int Defence;
    [HideInInspector]
    public int Range;
    [HideInInspector]
    private int CurrentRange;
    [HideInInspector]
    public int Movement;
    [HideInInspector]
    private int CurrentMovement;
    [HideInInspector]
    public int fogRange;

    private int Counter = 0;
    private bool dead = false;
    public bool attacking = false;
    public Animator animator;

    public GameObject flag;

    //floating damage text when taken damage
    public TMP_Text damageText;
    //Display damaged health on the card
    private HoverUnit hoverUnit;
    private int initialHealth;
    private int initialATK;
    private int initialRange;
    public int initialMovement;

    private int damageCounter;
    private void Awake()
    {
        isP1 = false;
        isP2 = false;
    }
    private void Start()
    {
        damageCounter = 0;
        capsuleCollider = GetComponent<CapsuleCollider>();
        // set stats
        type = cardObject.unitType;
        MaxHealth = cardObject.defense;
        Health = MaxHealth;
        Atk = cardObject.atk;
        Defence = cardObject.defense;
        Range = cardObject.range;
        CurrentRange = Range;
        Movement = cardObject.movement;
        CurrentMovement = Movement;
        fogRange = cardObject.fogRange;

        hoverUnit = FindObjectOfType<HoverUnit>();
        initialHealth = Health;
        initialATK = Atk;
        initialRange = Range;
        initialMovement = Movement;

    }
    private void Update()
    {
        checkIfTakingDamage();
        if (attacking)
        {
            animator.SetBool("Attacking", true);
            if (Counter < 80)
            {
                Counter++;
            }
            else
            {
                Counter = 0;
                animator.SetBool("Attacking", false);
                attacking = false;
            }
        }


        if (dead)
        {

            
            animator.SetBool("Dead", true);

            
            if (Counter < 80)
            {
                Counter++;
            }
            else
            {
                // change to drop on same tile
                if (flag != null)
                {
                    if (Player == 1)
                    {
                        //print("player 1 bakc");
                        flag = Instantiate(flag, CurrentTile.transform);
                        CurrentTile.GetComponent<Tile>().flag = flag;
                    }
                    else
                    {
                        //print("player 2 bakc");
                        flag = Instantiate(flag, CurrentTile.transform);
                        CurrentTile.GetComponent<Tile>().flag = flag;
                    }
                }
                Destroy(this.gameObject);

            }
        }


        draging();
        unitMouseEvents();


        if (this.gameObject.layer != CurrentTile.gameObject.layer)
            Match.Instance.setLayer(this.gameObject, CurrentTile.gameObject.layer);


        //Display updated stats on the card
        HealthIndicator();
        MovementIndicator();
        RangeIndicator();
        AtkIndicator();
    }
    private void FixedUpdate()
    {
        //Disappearing Floating damage text
        if(damageText.gameObject.activeSelf)
        {
            damageText.transform.position += new Vector3(0,5,0);
            Color textColor = damageText.color;
            //textColor.a -= 0.3f * Time.deltaTime;
            damageText.color = textColor;

            if(damageCounter < 120)
            {
                damageCounter++;
            }
            else
            {
                damageText.gameObject.SetActive(false);
                damageCounter = 0;
            }
            //StartCoroutine(DisappearingText());
        }
    }

    IEnumerator DisappearingText()
    {
        yield return new WaitForSeconds(1.5f);
       
    }
    private void Win()
    {


        if (Match.Instance.getCurrentPlayer() == 0)
        {
            Match.isP1= true;

            
            print("P1 Wins");
        }
        else if (Match.Instance.getCurrentPlayer() == 1)
        {
            // p2 wins
            Match.isP2 = true;
            
            print("P2 Wins");

        }
    }

    public void unitVision()
    {
        if (CurrentTile.GetComponent<Tile>().player != Player)
        {
            CurrentTile.GetComponent<Tile>().EnableNeighborsForUnits(fogRange, Player);
        }
    }
    private void unitMouseEvents()
    {
        RaycastHit hit = new RaycastHit();
        Ray ray = Match.Instance.currentCamera.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);


        if (Physics.Raycast(ray, out hit) && hit.collider == capsuleCollider
            && Input.GetMouseButtonDown(0))
        {
            UnitManager.Instance.DragUnit(this, CurrentTile.gameObject);

        }

        
    }
    private void draging()
    {
        
        if (drag == true)
        {
            


            animator.SetBool("Moving",true);
            int hoverHeight = 1;
            Vector3 ScreenPos = Input.mousePosition;
            Vector3 WorldPos = new Vector3(0, 0, 0);

            Plane plane = new Plane(Vector3.up, Vector3.zero);
            Ray ray = Match.Instance.currentCamera.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);

            float entry;

            if (plane.Raycast(ray, out entry))
            {
                WorldPos = ray.GetPoint(entry);
            }



            transform.position = WorldPos;
            transform.position = new Vector3(transform.position.x, hoverHeight, transform.position.z); ;
        }
        else
        {
            animator.SetBool("Moving", false);
        }
    }

    public void dragToggle(bool tog)
    {
        drag = tog;
        toggleMovementRangeIndicator(tog);
        toggleRangeIndicator(tog);
    }
    public bool getDragBool()
    {
        return drag;
    }

    // getters and setters
    public Tile getCurrentTile()
    {
        return CurrentTile;
    }
    public void setCurrentTile(GameObject curTile)
    {
        CurrentTile = curTile.GetComponent<Tile>();

        transform.position = curTile.transform.position;

        if (curTile.GetComponent<Tile>().flag != null)
        {
            if (Player == 0)
            {
                if (curTile.GetComponent<Tile>().flag.tag == "Player2")
                {
                    flag = Instantiate(curTile.GetComponent<Tile>().flag, this.transform);
                    Destroy(curTile.GetComponent<Tile>().flag);
                    flag.SetActive(true);
                    flag.transform.localScale = Vector3.one * 20;
                    Match.Instance.setLayer(flag, gameObject.layer);
                }
            }
            else if (Player == 1)
            {
                if (curTile.GetComponent<Tile>().flag.tag == "Player1")
                {
                    flag = Instantiate(curTile.GetComponent<Tile>().flag, this.transform);
                    Destroy(curTile.GetComponent<Tile>().flag);
                    flag.SetActive(true);
                    flag.transform.localScale = Vector3.one * 20;
                    Match.Instance.setLayer(flag, gameObject.layer);
                }
            }
        }
        if (curTile.tag == "Flag"
            && curTile.GetComponent<Tile>().player == Player
            && flag != null)
        {
            Win();
        }
    }

    public int getPlayer()
    {
        return Player;
    }

    public void setPlayer(int pla)
    {
        Player = pla;
    }

    public bool dealDamage(int dam)
    {
        //Floating Damage
        damageText.text = (-dam).ToString();
        Color textColor = damageText.color;
        textColor.a = 1.0f;
        damageText.color = textColor;
        damageText.transform.position = new Vector3(Input.mousePosition.x - 50.0f, Input.mousePosition.y + 0.0f, Input.mousePosition.z);
        damageText.gameObject.SetActive(true);

        Health -= dam;

        if (Health <= 0)
        {
            //die
            dead = true;

            //damageText.text = "Dead";
            return true;
            
            

           
        }
        return false;
    }

    public int getHealth()
    {
        return Health;
    }
    public int getAttack()
    {
        return Atk;
    }
    public bool useMovement(int amount)
    {
        if(amount <= CurrentMovement)
        {
            CurrentMovement -= amount;
            return true;
        }
        else
        {
            return false;
        }
    }

    public void ResetRange()
    {
        CurrentRange = Range;
        CurrentMovement = Movement;
    }

    public void checkIfTakingDamage()
    {
        RaycastHit hit = new RaycastHit();
        Ray ray = Match.Instance.currentCamera.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);

        // OnMouseOver for combat
        //hover
        if (Physics.Raycast(ray, out hit) )
        {
            if (Input.GetMouseButtonUp(0) 
                && UnitManager.Instance.getSelectedUnit() != null
                && UnitManager.Instance.getSelectedUnit() != this
                && UnitManager.Instance.getSelectedUnit().GetComponent<Unit>().Player != Player
                && hit.collider == capsuleCollider)
            {
                int dis = UnitManager.Instance.checkIfNeigborInRange(UnitManager.Instance.getSelectedUnit().CurrentRange, CurrentTile.GetComponent<Tile>());
                if ( 0 != dis)
                {
                    UnitManager.Instance.Combat(this);
                }
               
               

            }




        }

    }
    public int getCurrentRange()
    {
        return CurrentRange;
    }

    public int getCurrentMovement()
    {
        return CurrentMovement;
    }

    public void ZeroRange()
    {
        CurrentRange = 0;
        CurrentMovement = 0;
    }

    private void HealthIndicator()
    {
        if (initialHealth > Health)
        {
            hoverUnit.defense_Text.color = Color.red;
        }
        else if (initialHealth < Health)
        {
            hoverUnit.defense_Text.color = Color.green;
        }
        else
        {
            hoverUnit.defense_Text.color = Color.white;
        }
    }

    private void MovementIndicator()
    {
        if (initialMovement > Movement)
        {
            hoverUnit.move_Text.color = Color.red;
        }
        else if (initialMovement < Movement)
        {
            hoverUnit.move_Text.color = Color.green;
        }
        else
        {
            hoverUnit.move_Text.color = Color.white;
        }
    }

    private void RangeIndicator()
    {
        if (initialRange > Range)
        {
            hoverUnit.range_Text.color = Color.red;
        }
        else if (initialRange < Range)
        {
            hoverUnit.range_Text.color = Color.green;
        }
        else
        {
            hoverUnit.range_Text.color = Color.white;
        }
    }

    private void AtkIndicator()
    {
        if (initialATK > Atk)
        {
            hoverUnit.atk_Text.color = Color.red;
        }
        else if (initialATK < Atk)
        {
            hoverUnit.atk_Text.color = Color.green;
        }
        else
        {
            hoverUnit.atk_Text.color = Color.white;
        }
    }

    public void toggleRangeIndicator(bool toggle)
    {
        int checkRange = 0;

        if (toggle)
        {
            checkRange = CurrentRange;
        }
        else
        {
            checkRange = Range;
        }
        // turn the tiles with enemy units to red
        foreach (Tile tiles in CurrentTile.getAllNeighbours(checkRange, new Tile[1]))
        {
            if(tiles != null)
            {

           
            if (tiles.CurrentUnit != null)
            {
                if (tiles.CurrentUnit.getPlayer() != Player && tiles.CheckIfVisible())
                {
                    // change color of tile
                    if (toggle)
                    {
                        tiles.toggleTransparentOn(toggle, Player, TileManager.Instance.TransparentRed);
                    }
                    else
                    {
                        tiles.toggleTransparentOn(toggle, Player, TileManager.Instance.TransparentRed);
                    }
                }
            }
            }
        }
    }

    public void toggleMovementRangeIndicator(bool toggle)
    {
        // turn all tiles indicator to grean except tiles with units
        int checkRange = 0;

        if(toggle)
        {
            checkRange = CurrentMovement;
        }
        else
        {
            checkRange = Movement;
        }

        foreach (Tile tiles in CurrentTile.getAllNeighbours(checkRange, new Tile[1]))
        {
            if (tiles != null)
            {
                if (tiles.CurrentUnit == null && (tiles.filled || type == UNITTYPE.JET) && tiles.CheckIfVisible())
                {
                    // change color of tile
                    if (toggle)
                    {
                        tiles.toggleTransparentOn(toggle, Player, TileManager.Instance.TransparentGreen);
                    }
                    else
                    {
                        tiles.toggleTransparentOn(toggle, Player, TileManager.Instance.TransparentGreen);
                    }
                }
            }
        }
    }

}
