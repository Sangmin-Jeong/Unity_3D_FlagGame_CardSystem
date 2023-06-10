using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class HexGridLayout : MonoBehaviour
{
    [Header("Grid Settings")]
    public int 
        OfWar_Distance;
    
    [Header("Tile")]
    public HexRenderer EmptyTile;
    public GameObject transparentTile;
    public GameObject transparentFlagTile;

    [Header("Empty Tile Generation Settings")]
    public float outerSize;
    private float innerSize;
    private float height;
    private Material material;
  

    // private variables
    private Tile[] m_allTiles;
    private Vector2Int Player1StartPos;
    private Vector2Int Player2StartPos;

    [Header("Materials")]
    public Material plr1Mat;
    public Material plr2Mat;
    public Material bothMat;

    public GameObject[] LayoutTransparentGrid(int sizeX, int sizeY)
    {
        GameObject[] transparentTiles = new GameObject[sizeX * sizeY];
        int num = 0;
        if (m_allTiles[0] != null)
        {
            for (int y = 0; y < sizeY; y++)
            {
                for (int x = 0; x < sizeX; x++)
                {
                    transparentTiles[num] = Instantiate(transparentTile);

                    transparentTiles[num].transform.position = m_allTiles[num].transform.position;
                    transparentTiles[num].transform.SetParent(m_allTiles[num].transform, true);
                    transparentTiles[num].name = "Transparent " + m_allTiles[num].name;
                    transparentTiles[num].gameObject.SetActive(true);

                    num++;
                }
            }
        }
        return transparentTiles;
    }

    public Tile[] LayoutGrid(int sizeX, int sizeY)
    {
        m_allTiles= new Tile[sizeX * sizeY];
        // create empty tilesbase
        int num = 0;
        for(int y = 0; y < sizeY; y++)
        { 
            for(int x = 0; x < sizeX; x++)
            { 
                // tile object generation
                GameObject tile = new GameObject($"Tile {x}, {y}", typeof(Tile));
                tile.transform.position = GetPositionForHexFromCoordinate(new Vector2Int(x, y));
                tile.GetComponent<Tile>().hexCoordinate = new Vector2Int(x,y);
                //tile.GetComponent<Tile>().fogRange = FogOfWar_Distance;
                tile.GetComponent<Tile>().SetMaterials(plr1Mat, plr2Mat, bothMat);
                tile.transform.SetParent(transform, true);

                // empty tile generation
                HexRenderer hexRenderer = Instantiate(EmptyTile, tile.transform);
                hexRenderer.transform.position = GetPositionForHexFromCoordinate(new Vector2Int(x, y));
                hexRenderer.start();
                hexRenderer.DrawMesh();
                hexRenderer.tag = "empty";

                //hexRenderer.gameObject.layer = 8;
                hexRenderer.SetMaterial(bothMat);

                // for neighbors
                m_allTiles[num] = tile.GetComponent<Tile>();
                num++;
            }
        }


        // Neigherbors
        for (int i = 0; i < m_allTiles.Length; i++)
        {
            int neiNum = 0;
            for (int j = 1; j < m_allTiles.Length; j ++)
            {
                Vector2 difference =m_allTiles[j].GetComponent<Tile>().hexCoordinate - m_allTiles[i].GetComponent<Tile>().hexCoordinate;
                float evenOrOdd = m_allTiles[i].GetComponent<Tile>().hexCoordinate.y;

                if (evenOrOdd % 2 == 0)
                {
                    if (difference == new Vector2(1, 0) ||
                        difference == new Vector2(1, -1) ||
                        difference == new Vector2(0, -1) ||
                        difference == new Vector2(-1, 0) ||
                        difference == new Vector2(0, 1) ||
                        difference == new Vector2(1, 1))
                    {
                        m_allTiles[i].GetComponent<Tile>().Neighbors[neiNum] = m_allTiles[j];
                        neiNum++;
                    }
                }
                if (evenOrOdd % 2 == 1)
                {
                    if (difference == new Vector2(1, 0) ||
                        difference == new Vector2(0, -1) ||
                        difference == new Vector2(-1, -1) ||
                        difference == new Vector2(-1, 0) ||
                        difference == new Vector2(-1, 1) ||
                        difference == new Vector2(0, 1) )
                    {
                        m_allTiles[i].GetComponent<Tile>().Neighbors[neiNum] = m_allTiles[j];
                        neiNum++;
                    }
                }
            }

            
        }

        foreach (Tile obj in m_allTiles[0].GetComponent<Tile>().Neighbors)
        {
            if(obj != null) 
            obj.Neighbors[5] = m_allTiles[0];
        }
        


        return m_allTiles;
    }





    public Vector3 GetPositionForHexFromCoordinate(Vector2Int coordinate)
    {
        int column = coordinate.x;
        int row = coordinate.y;

        float width, height, xPosition, yPosition;
        bool shouldOffset;
        float horizontalDistance, verticalDistance, offset, size = outerSize;

        
            shouldOffset = (row % 2) == 0;
            width = Mathf.Sqrt(3) * size;
            height = 2f * size;
            
            horizontalDistance = width;
            verticalDistance = height * (3f/4f);

            offset = (shouldOffset) ? width/2 : 0;

            xPosition = (column * (horizontalDistance)) + offset;
            yPosition = (row * (verticalDistance));

        return new Vector3(xPosition, 0 , -yPosition);
    }
    // Hard-coded values, since we aren't looking to expand to 2+ players.
    public Vector3 GetStartingPosition(int playerNum)
    {
        foreach (Tile tile in m_allTiles)
        {
            if (tile.player == playerNum)
            {
                return tile.transform.position;
            }
        }
       
        print("Tile is not owned by anyone.");
        return new Vector3(-1, -1, -1); // Just means tile failed, and is not owned.
    }


}
