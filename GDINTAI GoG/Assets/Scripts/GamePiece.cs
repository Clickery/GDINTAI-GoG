using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePiece : MonoBehaviour
{
    //References
    public GameObject controller;

    //Positions
    private int xCoord;
    private int yCoord;

    private bool isAlive;


    private int point;

    //Enemy or Player
    private string owner;


    private string pieceName;

    //Sprite References
    public Sprite enemy_5star, enemy_4star, enemy_3star, enemy_2star, enemy_1star, enemy_Colonel,
        enemy_Lieutentant_Colonel, enemy_Major, enemy_Captain, enemy_1st_Lieutenant, enemy_2nd_Lieutenant,
        enemy_Sergeant, enemy_Spy, enemy_Private, enemy_Flag,
        player_5star, player_4star, player_3star, player_2star, player_1star, player_Colonel,
        player_Lieutentant_Colonel, player_Major, player_Captain, player_1st_Lieutenant, player_2nd_Lieutenant,
        player_Sergeant, player_Spy, player_Private, player_Flag;

   public void Activate()
   {
        this.controller = GameObject.FindGameObjectWithTag("GameController");

        switch(this.name)
        {
            case "enemy_5star": this.GetComponent<SpriteRenderer>().sprite = this.enemy_5star; this.point = 14; break;
            case "enemy_4star": this.GetComponent<SpriteRenderer>().sprite = this.enemy_4star; this.point = 13; break;
            case "enemy_3star": this.GetComponent<SpriteRenderer>().sprite = this.enemy_3star; this.point = 12; break;
            case "enemy_2star": this.GetComponent<SpriteRenderer>().sprite = this.enemy_2star; this.point = 11; break;
            case "enemy_1star": this.GetComponent<SpriteRenderer>().sprite = this.enemy_1star; this.point = 10; break;
            case "enemy_Colonel": this.GetComponent<SpriteRenderer>().sprite = this.enemy_Colonel; this.point = 9; break;
            case "enemy_Lieutentant_Colonel": this.GetComponent<SpriteRenderer>().sprite = this.enemy_Lieutentant_Colonel; this.point = 8; break;
            case "enemy_Major": this.GetComponent<SpriteRenderer>().sprite = this.enemy_Major; this.point = 7; break;
            case "enemy_Captain": this.GetComponent<SpriteRenderer>().sprite = this.enemy_Captain; this.point = 6; break;
            case "enemy_1st_Lieutenant": this.GetComponent<SpriteRenderer>().sprite = this.enemy_1st_Lieutenant; this.point = 5; break;
            case "enemy_2nd_Lieutenant": this.GetComponent<SpriteRenderer>().sprite = this.enemy_2nd_Lieutenant; this.point = 4; break;
            case "enemy_Sergeant": this.GetComponent<SpriteRenderer>().sprite = this.enemy_Sergeant; this.point = 3; break;
            case "enemy_Spy": this.GetComponent<SpriteRenderer>().sprite = this.enemy_Spy; this.point = 15; break;
            case "enemy_Private": this.GetComponent<SpriteRenderer>().sprite = this.enemy_Private; this.point = 2; break;
            case "enemy_Flag": this.GetComponent<SpriteRenderer>().sprite = this.enemy_Flag; this.point = 1; break;

            case "player_5star": this.GetComponent<SpriteRenderer>().sprite = this.player_5star; this.point = 14; break;
            case "player_4star": this.GetComponent<SpriteRenderer>().sprite = this.player_4star; this.point = 13; break;
            case "player_3star": this.GetComponent<SpriteRenderer>().sprite = this.player_3star; this.point = 12; break;
            case "player_2star": this.GetComponent<SpriteRenderer>().sprite = this.player_2star; this.point = 11; break;
            case "player_1star": this.GetComponent<SpriteRenderer>().sprite = this.player_1star; this.point = 10; break;
            case "player_Colonel": this.GetComponent<SpriteRenderer>().sprite = this.player_Colonel; this.point = 9; break;
            case "player_Lieutentant_Colonel": this.GetComponent<SpriteRenderer>().sprite = this.player_Lieutentant_Colonel; this.point = 8; break;
            case "player_Major": this.GetComponent<SpriteRenderer>().sprite = this.player_Major; this.point = 7; break;
            case "player_Captain": this.GetComponent<SpriteRenderer>().sprite = this.player_Captain; this.point = 6; break;
            case "player_1st_Lieutenant": this.GetComponent<SpriteRenderer>().sprite = this.player_1st_Lieutenant; this.point = 5; break;
            case "player_2nd_Lieutenant": this.GetComponent<SpriteRenderer>().sprite = this.player_2nd_Lieutenant; this.point = 4; break;
            case "player_Sergeant": this.GetComponent<SpriteRenderer>().sprite = this.player_Sergeant; this.point = 3; break;
            case "player_Spy": this.GetComponent<SpriteRenderer>().sprite = this.player_Spy; this.point = 15; break;
            case "player_Private": this.GetComponent<SpriteRenderer>().sprite = this.player_Private; this.point = 2; break;
            case "player_Flag": this.GetComponent<SpriteRenderer>().sprite = this.player_Flag; this.point = 1; break;
        }

        this.pieceName = this.name;

        this.xCoord = -1;
        this.yCoord = -1;

        this.isAlive = true;
   }


   public void SetCoords( int x, int y)
   {
        this.xCoord = x;
        this.yCoord = y;
   }

    public Vector2Int GetCoords()
    {
        return new Vector2Int(this.xCoord, this.yCoord);
    }

    public void SetOwner(string name)
    {
        this.owner = name;
    }

    public string GetOwner()
    {
        return this.owner;
    }

    public string GetName()
    {
        return this.pieceName;
    }

    public void GetKilled()
    {
        this.isAlive = false;
        this.transform.position = new Vector3(-9, -4.5f, -1);
        Destroy(this.gameObject.GetComponent<BoxCollider2D>()); 
    }

    public bool IsPieceAlive()
    {
        return this.isAlive;
    }

    public int GetPoint()
    {
        return this.point;
    }

    public void SetName(string name)
    {
        this.pieceName = name;
    }

    public void SetPoint(int value)
    {
        this.point = value;
    }


    public void SetLifeStatus(bool lifeStatus)
    {
        this.isAlive = lifeStatus;
    }


    public void EmptySprites()
    {
        this.player_5star = null;
        this.player_4star = null;
        this.player_3star = null;
        this.player_2star = null;
        this.player_1star = null;
        this.player_Colonel = null;
        this.player_Lieutentant_Colonel = null;
        this.player_Major = null;
        this.player_Captain = null;
        this.player_1st_Lieutenant = null;
        this.player_2nd_Lieutenant = null;
        this.player_Sergeant = null;
        this.player_Spy = null;
        this.player_Private = null;
        this.player_Flag = null;

        this.enemy_5star = null;
        this.enemy_4star = null;
        this.enemy_3star = null;
        this.enemy_2star = null;
        this.enemy_1star = null;
        this.enemy_Colonel = null;
        this.enemy_Lieutentant_Colonel = null;
        this.enemy_Major = null;
        this.enemy_Captain = null;
        this.enemy_1st_Lieutenant = null;
        this.enemy_2nd_Lieutenant = null;
        this.enemy_Sergeant = null;
        this.enemy_Spy = null;
        this.enemy_Private = null;
        this.enemy_Flag = null;

        this.controller = null;
    }
}
