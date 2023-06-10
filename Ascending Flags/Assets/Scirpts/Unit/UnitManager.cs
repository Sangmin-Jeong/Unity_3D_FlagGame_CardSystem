using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.VFX;
using static UnityEngine.UI.CanvasScaler;
using static UnityEngine.UIElements.UxmlAttributeDescription;

public class UnitManager : MonoBehaviour
{
    public static UnitManager Instance { get; private set; }

    [SerializeField]
    private Transform units1;
    [SerializeField]
    private Transform units2;
    
    
    [SerializeField]
    private Unit selectedUnit;

    public GameObject selectedStartTile;
    // [SerializeField]
    // private Transform units2;
    public Material player1UnitMaterial;
    public Material player2UnitMaterial;

    [SerializeField]
    private CameraController cam;
    [SerializeField]
    private CameraController cam2;

    [SerializeField]
    private VisualEffect effectAsset;
    
    [SerializeField]
    private VisualEffect combatVFX;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        
    }

    private void Start()
    {
        playVFX(effectAsset.transform);
        playCombatVFX(effectAsset.transform);
    }
    private void LateUpdate()
    {
        if (Input.GetMouseButtonUp(0))
            resetUnitPos();

    }
    public void enableNeighbors(int player)
    {
        if(player == 0)
        {
            foreach (Unit uni in units1.GetComponentsInChildren<Unit>())
            {
                uni.getCurrentTile().GetComponent<Tile>().EnableNeighborsForUnits(uni.fogRange, uni.getPlayer());
                uni.ResetRange();
            }

        }
        else
        {
            foreach (Unit uni in units2.GetComponentsInChildren<Unit>())
            {
                uni.getCurrentTile().GetComponent<Tile>().EnableNeighborsForUnits(uni.fogRange, uni.getPlayer());
                uni.ResetRange();
            }

        }
    }
    public void DragUnit(Unit uni, GameObject startTile)
    {
        
        if (uni.GetComponent<Unit>().getPlayer() != Match.Instance.getCurrentPlayer()
            || uni.GetComponent<Unit>().getCurrentMovement() ==0)
            return;

        


        selectedUnit = uni;
        selectedStartTile = startTile;

        uni.GetComponent<Unit>().dragToggle(true);

        
        if (Match.Instance.getCurrentPlayer() == 0)
            cam.Movement(false);
        else
            cam2.Movement(false);


    }
    public int checkIfNeigborInRange(int range, Tile newTile)
    {
        int distance = 0;

        foreach(Tile nei in getSelectedUnit().getCurrentTile().GetComponent<Tile>().getAllNeighbours(range, new Tile[1]))
        {
            if(nei == newTile)
            {
                distance = nei.currentDistance;
                break;
            }
        }

        return distance;
    }
    public void UnDragUnit(Tile newTile)
    {
        int dis = checkIfNeigborInRange(getSelectedUnit().getCurrentMovement(), newTile);

        if (dis == 0)
            return;


        selectedUnit.GetComponent<Unit>().dragToggle(false);
        if (!getSelectedUnit().GetComponent<Unit>().useMovement(dis))
            return;

        

        newTile.CurrentUnit = getSelectedUnit();
        selectedStartTile.GetComponent<Tile>().DisableNeighborsForUnits(getSelectedUnit().fogRange, getSelectedUnit().getPlayer());
        getSelectedUnit().GetComponent<Unit>().setCurrentTile(newTile.gameObject);
        getSelectedUnit().GetComponent<Unit>().unitVision();
        getSelectedUnit().GetComponent<Unit>().Movement -= 1;

        selectedStartTile.GetComponent<Tile>().CurrentUnit = null;
        selectedStartTile = null;

        //vfx
        playVFX(selectedUnit.transform);

        selectedUnit = null;

        if (Match.Instance.getCurrentPlayer() == 0)
            cam.Movement(true);
        else
            cam2.Movement(true);

       
    }
    public void Combat(Unit defender)
    {
        Unit Attacker = selectedUnit.GetComponent<Unit>();
        Attacker.attacking = true;


        playCombatVFX(defender.transform);
        if (defender.dealDamage(Attacker.getAttack()))
        {
            
            // move to new tile if killed unit
            if ((defender.getCurrentTile().GetComponent<Tile>().filled || Attacker.type == UNITTYPE.JET)
                && defender.getCurrentTile().currentDistance == 1)
            {
                Attacker.dragToggle(false);
                UnitManager.Instance.UnDragUnit(defender.getCurrentTile().GetComponent<Tile>());
            }
            Attacker.ZeroRange();
        }
        Attacker.ZeroRange();
    }


    public void resetUnitPos()
    {
 
        if (selectedUnit != null)
        {
            

            selectedUnit.transform.position = selectedStartTile.transform.position;
            selectedUnit.GetComponent<Unit>().dragToggle(false);

            playVFX(selectedUnit.transform);

            selectedUnit = null;
            if (Match.Instance.getCurrentPlayer() == 0)
                cam.Movement(true);
            else
                cam2.Movement(true);
        }
    }

    public void playVFX(Transform trans)
    {
        if(Match.Instance.getCurrentPlayer() == 0)
        {
            effectAsset.gameObject.layer = 7;
        }
        else
        {
            effectAsset.gameObject.layer = 6;
        }

        effectAsset.transform.position = trans.position;
        //ofset
        effectAsset.transform.position += new Vector3(0, 0.3f, 0);
        effectAsset.Play();
    } 
    
    public void playCombatVFX(Transform trans)
    {
        if(Match.Instance.getCurrentPlayer() == 0)
        {
            combatVFX.gameObject.layer = 7;
        }
        else
        {
            combatVFX.gameObject.layer = 6;
        }

        combatVFX.transform.position = trans.position;
        //ofset
        combatVFX.transform.position += new Vector3(0, 0.4f, 0);
        combatVFX.Play();
    }

    public Unit getSelectedUnit()
    {
        return selectedUnit;
    }

    public Unit PlaceUnit(GameObject uni, GameObject startTile, int player )
    {
        GameObject une;
        if (player == 0) //player 1
        {
            une = Instantiate(uni, units1);
            foreach (SkinnedMeshRenderer mat in une.GetComponentsInChildren<SkinnedMeshRenderer>())
            {
                Material[] materials = new Material[mat.materials.Length];
                for (int i = 0; i < materials.Length; i++)
                {
                    materials[i] = player1UnitMaterial;
                }
                mat.materials = materials;
            }
            foreach (MeshRenderer mesh in une.GetComponentsInChildren<MeshRenderer>())
            {
                mesh.material = player1UnitMaterial;
            }
        }
        else //player 2
        {
            une = Instantiate(uni, units2);
            foreach (SkinnedMeshRenderer mat in une.GetComponentsInChildren<SkinnedMeshRenderer>())
            {
                Material[] materials = new Material[mat.materials.Length];
                for (int i = 0; i < materials.Length; i++)
                {
                    materials[i] = player2UnitMaterial;
                }
                mat.materials = materials;
            }
            foreach (MeshRenderer mesh in une.GetComponentsInChildren<MeshRenderer>())
            {
                mesh.material = player2UnitMaterial;
            }
            une.transform.Rotate(0.0f, 180.0f, 0.0f, Space.World);
        }

        une.GetComponent<Unit>().setPlayer(player);
        une.GetComponent<Unit>().setCurrentTile(startTile);

        startTile.GetComponent<Tile>().EnableNeighborsForUnits(1, player);

        playVFX(une.transform);

        return une.GetComponent<Unit>();
    }
    public int countUnits()
    {
        int player1Count = 0;
        int player2Count = 0;

        foreach(Transform u in units1)
        {
            player1Count++;
        }
        foreach(Transform u in units2)
        {
            player2Count++;
        }

        // tie
        if (player1Count == player2Count)
            return 2;
        // player 1 has most
        if (player1Count > player2Count)
            return 0;
        // player 2 has most
        if (player1Count < player2Count)
            return 1;

        // error
        return 3;
        
    }

    public void RestroeUnitStats()
    {
        if(units1)
        foreach(Transform u in units1)
        {
            u.GetComponent<Unit>().Movement = u.GetComponent<Unit>().initialMovement;
        }

        if(units2)
        foreach(Transform u in units2)
        {
            u.GetComponent<Unit>().Movement = u.GetComponent<Unit>().initialMovement;
        }
    }

}


