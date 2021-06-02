using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private BoardManager boardRef;
    private Game gameRef;

    private GameObject clickedPiece;

    // Start is called before the first frame update
    void Start()
    {
        GameObject temp = GameObject.FindGameObjectWithTag("Board");
        this.boardRef = temp.GetComponent<BoardManager>();
        if (this.boardRef == null)
        {
            Debug.LogError("Script: PlayerController, variable 'boardRef' is null!");
        }

        temp = GameObject.FindGameObjectWithTag("GameController");
        this.gameRef = temp.GetComponent<Game>();
        if (this.gameRef == null)
        {
            Debug.LogError("Script: PlayerController, variable 'gameRef' is null!");
        }

        this.clickedPiece = null;
    }

    // Update is called once per frame
    void Update()
    {
        if((PersistentData.instance.GetState() == PersistentData.GameState.Versus) && (PersistentData.instance.GetTurn() == "Player"))
        {
            if(Input.GetMouseButtonDown(0))
            {
                this.PickPiece();
                this.PlacePiece();
            }
        }
    }

    private void PickPiece()
    {
        
        RaycastHit2D hit2d = Physics2D.GetRayIntersection(Camera.main.ScreenPointToRay(Input.mousePosition));
        if(hit2d.collider != null && hit2d.transform.gameObject.GetComponent<GamePiece>() != null)
        {
            if (hit2d.transform.gameObject.GetComponent<GamePiece>().GetOwner() == "Player")
            { 
                if (this.clickedPiece == null)
                {
                    this.clickedPiece = hit2d.transform.gameObject;
                    this.clickedPiece.transform.GetComponent<Renderer>().material.color = new Color(0.5f, 0.5f, 0.5f);
                }
                else
                {
                    this.clickedPiece.transform.GetComponent<Renderer>().material.color = new Color(1.0f, 1.0f, 1.0f);
                    //
                    this.clickedPiece = hit2d.transform.gameObject;
                    this.clickedPiece.transform.GetComponent<Renderer>().material.color = new Color(0.5f, 0.5f, 0.5f);
                }
            }
        }
    }

    private void PlacePiece()
    {
        if (this.clickedPiece != null)
        {
            RaycastHit2D[] hits2d = Physics2D.GetRayIntersectionAll(Camera.main.ScreenPointToRay(Input.mousePosition));
            GameObject clickedPiece = this.clickedPiece;
            GamePiece clickedGP = this.clickedPiece.GetComponent<GamePiece>();
            TileData td = null;
            GameObject tile = null;

            if (hits2d.Length > 1) //occupied space
            {
                GameObject piece = null;
                GamePiece gp = null;
                for(int i = 0; i < hits2d.Length; i++)
                {
                    if(hits2d[i].transform.gameObject.GetComponent<GamePiece>() != null)
                    {
                        piece = hits2d[i].transform.gameObject;
                        gp = hits2d[i].transform.gameObject.GetComponent<GamePiece>();
                    }
                    else
                    {
                        tile = hits2d[i].transform.gameObject;
                        td = hits2d[i].transform.gameObject.GetComponent<TileData>();
                    }
                }


                int xMove = -1;
                int yMove = -1;
                if (((td.x == clickedGP.GetCoords().x + 1) || (td.x == clickedGP.GetCoords().x - 1)) && (td.y == clickedGP.GetCoords().y))// move x wise
                {
                    xMove = td.x;
                    yMove = td.y;
                }
                if (((td.y == clickedGP.GetCoords().y + 1) || (td.y == clickedGP.GetCoords().y - 1)) && (td.x == clickedGP.GetCoords().x))// move y wise
                {
                    xMove = td.x;
                    yMove = td.y;
                }

                if (gp.GetOwner() == "Enemy" && xMove != -1 && yMove != -1)//arbiter
                {

                    //Debug.Log("Player point:" + clickedGP.GetPoint() + ", Enemy point: " + gp.GetPoint());

 
                    //arbiter
                    if (gp.GetPoint() == clickedGP.GetPoint())
                    {
                        if(gp.GetName() == "enemy_Flag" && clickedGP.GetName() == "player_Flag")
                            gp.GetKilled();
                        else
                        {
                            clickedGP.GetKilled();
                            gp.GetKilled();
                        } 
                    }
                    else if (gp.GetName() == "enemy_Spy")
                    {
                        if (clickedGP.GetName() == "player_Private")
                            gp.GetKilled();
                        else
                            clickedGP.GetKilled();
                    }
                    else if(gp.GetName() == "enemy_Private" && clickedGP.GetName() == "player_Spy")
                    {
                        clickedGP.GetKilled();
                    }
                    else if (gp.GetPoint() > clickedGP.GetPoint())
                    {
                        clickedGP.GetKilled();
                    }
                    else if (gp.GetPoint() < clickedGP.GetPoint())
                    {
                        gp.GetKilled();
                    }

                    if(clickedGP.IsPieceAlive() == true)//when our piece lives
                    {
                        this.clickedPiece.transform.position = new Vector3(tile.transform.position.x, tile.transform.position.y, -1);// set position
                        this.boardRef.SetTileOccupation((int)clickedGP.GetCoords().x, (int)clickedGP.GetCoords().y, false);//set current tile occupation to false
                        clickedGP.SetCoords(xMove, yMove);//update current piece coords
                        this.boardRef.SetTileOccupation(xMove, yMove, true);// current tile is occupied

                    }
                    else if(clickedGP.IsPieceAlive() == false)// when our piece dies
                    {
                        
                        this.boardRef.SetTileOccupation((int)clickedGP.GetCoords().x, (int)clickedGP.GetCoords().y, false);//set current tile occupation to false
                        clickedGP.SetCoords(-1, -1);//coords for dead pieces

                        
                    }
                    
                    if(gp.IsPieceAlive() == false)// when enemy piece dies
                    {
                        this.boardRef.SetTileOccupation((int)gp.GetCoords().x, (int)gp.GetCoords().y, false);//set current tile occupation to false
                        gp.SetCoords(-1, -1);//coords for dead pieces
                    }

                    

                    //reset color
                    this.clickedPiece.transform.GetComponent<Renderer>().material.color = new Color(1.0f, 1.0f, 1.0f);
                    this.clickedPiece = null;


                    PersistentData.instance.SetTurn("Enemy");
                }
            }
            else if(hits2d.Length == 1)//empty space
            {
                tile = hits2d[0].transform.gameObject;
                td = hits2d[0].transform.gameObject.GetComponent<TileData>();

                int xMove = -1;
                int yMove = -1;
                if (((td.x == clickedGP.GetCoords().x + 1) || (td.x == clickedGP.GetCoords().x - 1)) && (td.y == clickedGP.GetCoords().y))// move x wise
                {
                    xMove = td.x;
                    yMove = td.y;
                }
                if (((td.y == clickedGP.GetCoords().y + 1) || (td.y == clickedGP.GetCoords().y - 1)) && (td.x == clickedGP.GetCoords().x))// move y wise
                {
                    xMove = td.x;
                    yMove = td.y;
                }

                if(xMove != -1 && yMove != -1)
                {
                    this.clickedPiece.transform.position = new Vector3(tile.transform.position.x, tile.transform.position.y, -1);// set position
                    this.boardRef.SetTileOccupation((int)clickedGP.GetCoords().x, (int)clickedGP.GetCoords().y, false);//set current tile occupation to false
                    clickedGP.SetCoords(xMove, yMove);//update current piece coords
                    this.boardRef.SetTileOccupation(xMove, yMove, true);// current tile is occupied

                    //reset color
                    this.clickedPiece.transform.GetComponent<Renderer>().material.color = new Color(1.0f, 1.0f, 1.0f);
                    //Debug.Log(clickedGP.GetCoords());
                    this.clickedPiece = null;

                    PersistentData.instance.SetTurn("Enemy");
                    
                }
            }
        }
    }
}
