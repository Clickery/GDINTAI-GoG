using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class DragDrop : MonoBehaviour
{
    private float startPosX;
    private float startPosY;
    private float snapSensitivity;

    //reference to board
    private BoardManager boardRef;

    private bool isBeingHeld = false;
    
    //anim purposes
    private bool startAnim = false;
    private float timePassed = 0.0f;


    private int placedPieces = 0;

    private void Start()
    {
        this.startPosX = this.transform.position.x;
        this.startPosY = this.transform.position.y;
        this.snapSensitivity = 2.2f;

        GameObject temp = GameObject.FindGameObjectWithTag("Board");
        this.boardRef = temp.GetComponent<BoardManager>();

        if(this.boardRef == null)
        {
            Debug.LogError("Script: DragDrop, variable 'boardRef' is null!");
        }
    }

    private void Update()
    {

        if(PersistentData.instance.GetState() == PersistentData.GameState.Placement)
        {
            if (isBeingHeld)
            {
                Vector3 mousePos = Input.mousePosition;
                mousePos = Camera.main.ScreenToWorldPoint(mousePos);

                this.gameObject.transform.localPosition = new Vector3(mousePos.x, mousePos.y, -2);
            }

            if (startAnim)
            {
                this.timePassed += Time.deltaTime;
                Vector3 temp = new Vector3(this.startPosX, this.startPosY, -1.0f);
                this.transform.position = Vector3.Lerp(this.transform.position, temp, this.timePassed);
                if (this.timePassed > 0.5f)
                {
                    this.startAnim = false;
                }
            }

           
        }
        else
        {
            Destroy(this);
        }
    }


    private void OnMouseDown()
    {
        if (Input.GetMouseButtonDown(0))
        {
            GamePiece gp = this.GetComponent<GamePiece>();

            if(gp.GetOwner() == "Player")
                isBeingHeld = true;

            //Debug.Log("clicked!, x: " + (int)gp.GetCoords().x + ", y: " + (int)gp.GetCoords().y);
        }
    }

    private void OnMouseUp()
    {
        this.placedPieces = 0;

        isBeingHeld = false;
        bool snapped = false;

        GamePiece gp = this.GetComponent<GamePiece>();

        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                if ((Vector3.Distance(this.boardRef.GetTile(j, i).transform.position, this.transform.position) < this.snapSensitivity)
                    && (this.boardRef.IsTileOccupied(j, i) == false))
                {

                    //set position on the tile
                    float x = this.boardRef.GetTile(j, i).transform.position.x;
                    float y = this.boardRef.GetTile(j, i).transform.position.y;
                    float z = -1.0f;
                    this.transform.position = new Vector3(x, y, z);

                    //set previous pos to false if it has been placed before
                    if ((int)gp.GetCoords().x != -1 && (int)gp.GetCoords().y != -1)
                    {
                        Debug.Log("previously placed!, x: " + (int)gp.GetCoords().x + ", y: " + (int)gp.GetCoords().y);
                        this.boardRef.SetTileOccupation((int)gp.GetCoords().x, (int)gp.GetCoords().y, false);
                    }

                    //set coordinates
                    gp.SetCoords(j, i);

                    //tell this part of the board is taken
                    this.boardRef.SetTileOccupation(j, i, true);

                    //Debug.Log("Snapped at: " + j + ", " + i);
                    snapped = true;

                    this.GetNumOfPlacedPieces();

                    return;
                }
            }
        }

        if (!snapped)
        {
            this.startAnim = true;
            this.timePassed = 0.0f;

            if ((int)gp.GetCoords().x != -1 && (int)gp.GetCoords().y != -1)
            {
                //set tile to false
                this.boardRef.SetTileOccupation((int)gp.GetCoords().x, (int)gp.GetCoords().y, false);
            }
            //set to null
            gp.SetCoords(-1, -1);


            this.GetNumOfPlacedPieces();

        }

    }

    private void GetNumOfPlacedPieces()
    {
        int numPlacedPiece = 0;
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                if (this.boardRef.IsTileOccupied(j, i))
                {
                    numPlacedPiece++;
                }

            }
        }
        this.placedPieces = numPlacedPiece;
        
        if(this.placedPieces == 21)
        {
            PersistentData.instance.ShowStartButton();
        }
        else
        {
            PersistentData.instance.HideStartButton();
        }

        //Debug.Log(this.placedPieces);
    }

}
