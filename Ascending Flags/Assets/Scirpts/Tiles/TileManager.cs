using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.VFX;
using static UnityEngine.UIElements.UxmlAttributeDescription;

public class TileManager : MonoBehaviour
{

    public static TileManager Instance { get; private set; }

    public HexGridLayout grid;
    public CardManager cardManager;
    private Tile[] m_allTiles;
    private GameObject[] m_transparentTiles;

    public GameObject Flag;
    public GameObject FlagPole;
    public GameObject Hover;
    private GameObject m_hover;
    public GameObject transparentFlagTile;

    public Material TransparentNorm;
    public Material TransparentRed;
    public Material TransparentGreen;

    private int currentPlayer = 3;

    public Vector2Int gridSize;

    private int emptyTilesLeft = 0;

    [SerializeField]
    private VisualEffect vfxPlacedTile;

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
        // hover object
        m_hover = Instantiate(Hover, transform);
        m_hover.SetActive(false);
    }


    // Start is called before the first frame update
    void Start()
    {
        gridSize = new Vector2Int(DebuggingObject.Instance.MapSize, DebuggingObject.Instance.MapSize);
        layGrid(gridSize.x, gridSize.y);
        setFlags();

        
        MatchSystem.OnPlayerSwitch += SetHoverLayer;

        playVfx(vfxPlacedTile.transform);
    }

    public void playVfx(Transform trans)
    {
        if (Match.Instance.getCurrentPlayer() == 0)
        {
            vfxPlacedTile.gameObject.layer = 7;
        }
        else
        {
            vfxPlacedTile.gameObject.layer = 6;
        }

        vfxPlacedTile.transform.position = trans.position;
        vfxPlacedTile.transform.position += new Vector3(0, 0.0f, 0);
        vfxPlacedTile.Play();
    }

    void SetHoverLayer(int playerNum)
    {
        if (playerNum == 0)
        {
            foreach (Transform u in m_hover.transform.GetComponentsInChildren<Transform>())
            {
                u.gameObject.layer = 7;
            }
        } else if (playerNum == 1)
        {
            foreach (Transform u in m_hover.transform.GetComponentsInChildren<Transform>())
            {
                u.gameObject.layer = 6;
            }
        }

        currentPlayer = playerNum;
    }

    // Update is called once per frame
    void Update()
    {
        cardManager = FindObjectOfType<CardManager>();
        checkHover();
    }
    public void setFlags()
    {
        emptyTilesLeft = gridSize.x * gridSize.x;
        //player 1

        m_allTiles[0].tag = "Flag";
        m_allTiles[0].ChangeTile(Flag, 0, 1);
        
        //m_allTiles[0].flag = FlagPole;
        m_allTiles[0].flag = Instantiate(FlagPole, m_allTiles[0].transform);
        m_allTiles[0].flag.transform.localScale = Vector3.one * 20; ;
        m_allTiles[0].flag.transform.position += new Vector3(0,0.3f,0);
        m_allTiles[0].flag.GetComponentInChildren<MeshRenderer>().material = m_allTiles[0].plr1Mat;
        m_allTiles[0].flag.tag = "Player1";
        m_allTiles[0].SetTileOwner(0);


        Match.Instance.flagTile1 = m_allTiles[0];
        //player 2
        int finalTile = gridSize.x * gridSize.x - 1;
        
        m_allTiles[finalTile].tag = "Flag";
        m_allTiles[finalTile].ChangeTile(Flag, 1,1);
        
        //m_allTiles[finalTile].flag = FlagPole;
        m_allTiles[finalTile].flag = Instantiate(FlagPole, m_allTiles[finalTile].transform);
        m_allTiles[finalTile].flag.transform.localScale = Vector3.one *20; 
        m_allTiles[finalTile].flag.transform.position += new Vector3(0, 0.3f, 0); 
        m_allTiles[finalTile].flag.transform.transform.Rotate(0.0f, 180.0f, 0.0f, Space.World);
        m_allTiles[finalTile].flag.GetComponentInChildren<MeshRenderer>().material = m_allTiles[finalTile].plr2Mat;
        m_allTiles[finalTile].flag.tag = "Player2";
        m_allTiles[finalTile].SetTileOwner(1);
        Match.Instance.flagTile2 = m_allTiles[finalTile];

        
    }

    public void TileCounter()
    {
        emptyTilesLeft--;
    }

    public bool getIfAllTilesFilled()
    {
        if(emptyTilesLeft == 0)
        {
            return true;
        }
        else
        return false;
    }

    public void layGrid(int sizeX, int sizeY)
    {
        // layout grid
        m_allTiles = grid.LayoutGrid(sizeX, sizeY);
        gridSize = new Vector2Int(sizeX, sizeY);
        EnableTransparentTilePath(sizeX, sizeY);
    }

    private void EnableTransparentTilePath(int sizeX, int sizeY)
    {
        int finalTile = gridSize.x * gridSize.x - 1;
        m_transparentTiles = grid.LayoutTransparentGrid(sizeX, sizeY);

        /* I have no clue why but I have to manually set the flags here instead of it doing it for me in HexGridLayout :/ */
        Destroy(m_transparentTiles[0]);
        m_transparentTiles[0] = Instantiate(transparentFlagTile);
        m_transparentTiles[0].transform.position = m_allTiles[0].transform.position;
        m_transparentTiles[0].transform.SetParent(m_allTiles[0].transform);
        m_transparentTiles[0].SetActive(true);

        Destroy(m_transparentTiles[finalTile]);
        m_transparentTiles[finalTile] = Instantiate(transparentFlagTile);
        m_transparentTiles[finalTile].transform.position = m_allTiles[finalTile].transform.position;
        m_transparentTiles[finalTile].transform.SetParent(m_allTiles[finalTile].transform);
        m_transparentTiles[finalTile].SetActive(true);
    }

    private void checkHover()
    {
        m_hover.SetActive(false);
        if (cardManager == null)
        {
            return;
        }
        if (cardManager.selectedCard != null)
        {
            RaycastHit hit = new RaycastHit();
        Ray ray = Match.Instance.currentCamera.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);

            
            //check which player is playing
            if (Match.Instance.getCurrentPlayer() == 0)
                Match.Instance.setLayer(m_hover, 7);
            else
                Match.Instance.setLayer(m_hover, 6);
         

            for (int i = 0; i < m_allTiles.Length; i++)
        {
                
                if (Physics.Raycast(ray, out hit) && hit.collider == m_allTiles[i].GetComponent<BoxCollider>())
            {
                    // hover over things you can place
                    // terrain
                    if (m_allTiles[i].CheckIfFillable() && m_allTiles[i].CheckIfVisible() && cardManager.selectedCard.cardObject.cardType == CARDTYPE.TERRAIN)
                    {

                        m_hover.transform.position = grid.GetPositionForHexFromCoordinate(m_allTiles[i].hexCoordinate);
                        m_hover.SetActive(true);
                    }
                    //unit
                    else if (!m_allTiles[i].CheckIfFillable() && m_allTiles[i].CheckIfVisible() && cardManager.selectedCard.cardObject.cardType == CARDTYPE.UNIT
                        && m_allTiles[i].player == Match.Instance.getCurrentPlayer() && m_allTiles[i].CurrentUnit == null)
                    {

                        m_hover.transform.position = grid.GetPositionForHexFromCoordinate(m_allTiles[i].hexCoordinate);
                        m_hover.SetActive(true);
                    }

                }
        }
        }
    }

}
