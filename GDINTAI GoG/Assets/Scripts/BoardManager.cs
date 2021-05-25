using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    //board
    private int rows = 8;
    private int cols = 9;
    private float tileSizeX = 1.4f;
    private float tileSizeY = 1.0f;

    //tells if the board at x,y is occupied by a piece
    private bool[,] isOccupied;

    //reference to tiles, for position purposes
    GameObject[,] tiles;



    // Start is called before the first frame update
    void Awake()
    {
        this.tiles = new GameObject[this.cols, this.rows];
        GenerateBoard();

        this.isOccupied = new bool[this.cols, this.rows];
        for (int i = 0; i < this.rows; i++)
        {
            for (int j = 0; j < this.cols; j++)
            {
                this.isOccupied[j, i] = false;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void GenerateBoard()
    {
        GameObject referenceTile = (GameObject)Instantiate(Resources.Load("tile"));
        if(referenceTile == null)
        {
            Debug.LogError("Script: BoardManager, referenceTile is Null!");
            return;
        }


        for (int i = 0; i < this.rows; i++)
        {
            for(int j = 0; j < this.cols; j++)
            {
                this.tiles[j, i] = (GameObject)Instantiate(referenceTile, this.transform);
                float posX = j * this.tileSizeX;
                float posY = i * this.tileSizeY;
                this.tiles[j, i].transform.position = new Vector2(posX, posY);

                this.tiles[j, i].GetComponent<TileData>().x = j;
                this.tiles[j, i].GetComponent<TileData>().y = i;


                if (this.tiles[j, i] == null)
                {
                    Debug.LogError("Script: BoardManager, this.tiles[" + j + ", " + i + "] is NULL");
                    return;
                }
            }
        }
        Destroy(referenceTile);

        float boardW = this.cols * this.tileSizeX;
        float boardH = this.rows * this.tileSizeY;

        transform.position = new Vector2((-boardW / 2) + this.tileSizeX / 2, -boardH / 2 + this.tileSizeY / 2);
    }

    public GameObject GetTile(int x, int y)
    {
        return this.tiles[x, y];
    }

    public bool IsTileOccupied(int x , int y)
    {
        return this.isOccupied[x, y];
    }

    public void SetTileOccupation(int x, int y, bool setOccupation)
    {
        this.isOccupied[x, y] = setOccupation;
    }
}
