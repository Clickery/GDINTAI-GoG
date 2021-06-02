using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour
{
    public GameObject winnerText;
    public GameObject mainMenuButton;
    public GameObject gameOverText;




    //Spawn piece
    public GameObject gamePiece;
    
    private BoardManager boardRef;
    public GameObject[] enemy = new GameObject[21];
    public GameObject[] player = new GameObject[21];

    // Start is called before the first frame update
    void Start()
    {
        GameObject temp = GameObject.FindGameObjectWithTag("Board");
        this.boardRef = temp.GetComponent<BoardManager>();
        if (this.boardRef == null)
        {
            Debug.LogError("Script: Game, variable 'boardRef' is null!");
        }

        //create pieces for "Player"
        this.SetPlayerPieces();

        /// setting enemy piece (blitzkrieg)
        this.SetEnemyPieces();

        //Hide
        this.HideEnemyPieces();

    }


    // Update is called once per frame
    void Update()
    {
       
        if(PersistentData.instance.GetState() == PersistentData.GameState.Versus)
        {
            if (!this.player[20].GetComponent<GamePiece>().IsPieceAlive())//check if player flag is dead
            {
                PersistentData.instance.SetWinner("Enemy");
                PersistentData.instance.GameOverState();
            }
            else if (!this.enemy[20].GetComponent<GamePiece>().IsPieceAlive())//check if enemy flag is dead
            {
                PersistentData.instance.SetWinner("Player");
                PersistentData.instance.GameOverState();
            }
            else if(this.player[20].GetComponent<GamePiece>().GetCoords().y == 7)//if player flag reaches enemy base
            {
                
                int x = (int)this.player[20].GetComponent<GamePiece>().GetCoords().x;
                int y = (int)this.player[20].GetComponent<GamePiece>().GetCoords().y;
                bool noLeft = true; bool noRight = true; bool noBack = true;

                if(x + 1 < 9)
                {
                    for(int i = 0; i < 21; i++)
                    {
                        int xTemp = (int)this.enemy[i].GetComponent<GamePiece>().GetCoords().x;
                        int yTemp = (int)this.enemy[i].GetComponent<GamePiece>().GetCoords().y;
                        if(xTemp == x + 1 && yTemp == y)
                        {
                            noRight = false;
                        }
                    }
                }
                if(x - 1 > -1)
                {
                    for (int i = 0; i < 21; i++)
                    {
                        int xTemp = (int)this.enemy[i].GetComponent<GamePiece>().GetCoords().x;
                        int yTemp = (int)this.enemy[i].GetComponent<GamePiece>().GetCoords().y;
                        if (xTemp == x - 1 && yTemp == y)
                        {
                            noLeft = false;
                        }
                    }
                }

                for (int i = 0; i < 21; i++)
                {
                    int xTemp = (int)this.enemy[i].GetComponent<GamePiece>().GetCoords().x;
                    int yTemp = (int)this.enemy[i].GetComponent<GamePiece>().GetCoords().y;
                    if (xTemp == x && yTemp == y - 1)
                    {
                        noBack = false;
                    }
                }

                if (noRight && noLeft && noBack)
                {
                    PersistentData.instance.SetWinner("Player");
                    PersistentData.instance.GameOverState();
                }


                Debug.Log("Noleft: " + noLeft + ", Noright: " + noRight + ", Noback: " + noBack);
            }
            else if(this.enemy[20].GetComponent<GamePiece>().GetCoords().y == 0)//if enemy flag reaches enemy base
            {
                int x = (int)this.enemy[20].GetComponent<GamePiece>().GetCoords().x;
                int y = (int)this.enemy[20].GetComponent<GamePiece>().GetCoords().y;
                bool noLeft = true; bool noRight = true; bool noBack = true;

                if (x + 1 < 9)
                {
                    for (int i = 0; i < 21; i++)
                    {
                        int xTemp = (int)this.player[i].GetComponent<GamePiece>().GetCoords().x;
                        int yTemp = (int)this.player[i].GetComponent<GamePiece>().GetCoords().y;
                        if (xTemp == x + 1 && yTemp == y)
                        {
                            noRight = false;
                        }
                    }
                }

                if (x - 1 > -1)
                {
                    for (int i = 0; i < 21; i++)
                    {
                        int xTemp = (int)this.player[i].GetComponent<GamePiece>().GetCoords().x;
                        int yTemp = (int)this.player[i].GetComponent<GamePiece>().GetCoords().y;
                        if (xTemp == x - 1 && yTemp == y)
                        {
                            noLeft = false;
                        }
                    }
                }

                for (int i = 0; i < 21; i++)
                {
                    int xTemp = (int)this.player[i].GetComponent<GamePiece>().GetCoords().x;
                    int yTemp = (int)this.player[i].GetComponent<GamePiece>().GetCoords().y;
                    if (xTemp == x && yTemp == y + 1)
                    {
                        noBack = false;
                    }
                }

                if (noRight && noLeft && noBack)
                {
                    PersistentData.instance.SetWinner("Player");
                    PersistentData.instance.GameOverState();
                }
            }
        
        }
        else if(PersistentData.instance.GetState() == PersistentData.GameState.GameOver)
        {
            this.gameOverText.SetActive(true);
            this.winnerText.SetActive(true);
            this.mainMenuButton.SetActive(true);
        }
    }
  

    private GameObject CreatePiece(string name)
    {
        GameObject obj = Instantiate(gamePiece, new Vector3(0, 0 - 2), Quaternion.identity);
        GamePiece gp = obj.GetComponent<GamePiece>();
        gp.name = name;
        gp.Activate();
        return obj;
    }

    private void SetPlayerPieces()
    {
        float xTemp = -9.0f;
        float yTemp = 2.5f;

        for (int i = 0; i < 8; i++)
        {
            if (i < 6)
                this.player[i] = this.CreatePiece("player_Private");// 6 privates
            else
                this.player[i] = this.CreatePiece("player_Spy");// 2 Spies

        }
        this.player[8] = this.CreatePiece("player_5star");
        this.player[9] = this.CreatePiece("player_4star");
        this.player[10] = this.CreatePiece("player_3star");
        this.player[11] = this.CreatePiece("player_2star");
        this.player[12] = this.CreatePiece("player_1star");
        this.player[13] = this.CreatePiece("player_Colonel");
        this.player[14] = this.CreatePiece("player_Lieutentant_Colonel");
        this.player[15] = this.CreatePiece("player_Major");
        this.player[16] = this.CreatePiece("player_Captain");
        this.player[17] = this.CreatePiece("player_1st_Lieutenant");
        this.player[18] = this.CreatePiece("player_2nd_Lieutenant");
        this.player[19] = this.CreatePiece("player_Sergeant");
        this.player[20] = this.CreatePiece("player_Flag");

        /////////////////////////////////////////
        for (int i = 0; i < 21; i++)
        {
            if (i < 6)
            {
                this.player[i].transform.position = new Vector3(-9.0f, 3.5f, -1);
            }
            else if(i < 8)
            {
                this.player[i].transform.position = new Vector3(-7.5f, 3.5f, -1);
            }
            else
            {
                this.player[i].transform.position = new Vector3(xTemp, yTemp, -1);
                xTemp += 1.5f;
                if(xTemp > -7.5f)
                {
                    xTemp = -9.0f;
                    yTemp--;
                }
            }
        }
        ////////////////////////////////////////

        for (int i = 0; i < player.Length; i++)
        {
            this.player[i].GetComponent<GamePiece>().SetOwner("Player");
        }
    }

    private void SetEnemyPieces()
    {
        //initializing pieces
        for (int i = 0; i < 8; i++)
        {
            if (i < 6)
                this.enemy[i] = this.CreatePiece("enemy_Private");// 6 privates
            else
                this.enemy[i] = this.CreatePiece("enemy_Spy");// 2 Spies

        }
        this.enemy[8] = this.CreatePiece("enemy_5star");
        this.enemy[9] = this.CreatePiece("enemy_4star");
        this.enemy[10] = this.CreatePiece("enemy_3star");
        this.enemy[11] = this.CreatePiece("enemy_2star");
        this.enemy[12] = this.CreatePiece("enemy_1star");
        this.enemy[13] = this.CreatePiece("enemy_Colonel");
        this.enemy[14] = this.CreatePiece("enemy_Lieutentant_Colonel");
        this.enemy[15] = this.CreatePiece("enemy_Major");
        this.enemy[16] = this.CreatePiece("enemy_Captain");
        this.enemy[17] = this.CreatePiece("enemy_1st_Lieutenant");
        this.enemy[18] = this.CreatePiece("enemy_2nd_Lieutenant");
        this.enemy[19] = this.CreatePiece("enemy_Sergeant");
        this.enemy[20] = this.CreatePiece("enemy_Flag");

        //////////////Set Enemy pos (preset)
        int[,] preset = new int[2, 21];
        preset[0, 0] = 0; preset[1, 0] = 6;//privates
        preset[0, 1] = 3; preset[1, 1] = 6;
        preset[0, 2] = 4; preset[1, 2] = 7;
        preset[0, 3] = 5; preset[1, 3] = 7;
        preset[0, 4] = 5; preset[1, 4] = 6;
        preset[0, 5] = 7; preset[1, 5] = 5;

        preset[0, 6] = 1; preset[1, 6] = 6;//spies
        preset[0, 7] = 3; preset[1, 7] = 5;

        preset[0, 8] = 0; preset[1, 8] = 5;//generals
        preset[0, 9] = 4; preset[1, 9] = 5;
        preset[0, 10] = 5; preset[1, 10] = 5;
        preset[0, 11] = 6; preset[1, 11] = 5;
        preset[0, 12] = 8; preset[1, 12] = 5;

        preset[0, 13] = 1; preset[1, 13] = 7;//colonel to major
        preset[0, 14] = 2; preset[1, 14] = 7;
        preset[0, 15] = 8; preset[1, 15] = 6;

        preset[0, 16] = 7; preset[1, 16] = 6;//captain to 2nd lieutenant
        preset[0, 17] = 4; preset[1, 17] = 6;
        preset[0, 18] = 6; preset[1, 18] = 6;

        preset[0, 19] = 7; preset[1, 19] = 7;//sergeant

        preset[0, 20] = 0; preset[1, 20] = 7;//flag

        for(int i = 0; i < 21; i++)
        {
            //set pos
            this.enemy[i].transform.position = new Vector3(this.boardRef.GetTile(preset[0, i], preset[1, i]).transform.position.x, this.boardRef.GetTile(preset[0, i], preset[1, i]).transform.position.y, -1);
            
            //set tile occupation to true
            this.boardRef.SetTileOccupation(preset[0, i], preset[1, i], true);


            GamePiece gp = this.enemy[i].GetComponent<GamePiece>();
            //set piece's coords on the board
            gp.SetCoords(preset[0, i], preset[1, i]);

            //Set owner of pieces
            gp.SetOwner("Enemy");
        }
    }

    public void HideEnemyPieces()
    {
        Color color = new Color(0,0,0);
        for(int i = 0; i < 21; i++)
        {
            this.enemy[i].transform.GetComponent<Renderer>().material.color = color;
        }
 
    }

    public void ShowEnemyPieces()
    {
        Color color = new Color(1, 1, 1);
        for (int i = 0; i < 21; i++)
        {
            this.enemy[i].transform.GetComponent<Renderer>().material.color = color;
        }
    }

}
