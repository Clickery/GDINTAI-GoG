using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    //Spawn piece
    public GameObject gamePiece;


    private GameObject[,] positions = new GameObject[8, 9];
    private GameObject[] enemy = new GameObject[21];
    private GameObject[] player = new GameObject[21];

    private string currentTurn = "Player";

    private bool gameOver = false;


    // Start is called before the first frame update
    void Start()
    {
        //create pieces for "Player"
        for(int i = 0; i < 8; i++)
        {
            if(i < 6)
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

        SetInitPos();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private GameObject CreatePiece(string name)
    {
        GameObject obj = Instantiate(gamePiece, new Vector3(0, 0 - 2), Quaternion.identity);
        GamePiece gp = obj.GetComponent<GamePiece>();
        gp.name = name;
        gp.Activate();
        return obj;
    }

    private void SetInitPos()
    {
        float xTemp = -9.0f;
        float yTemp = 2.5f;
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
    }
}
