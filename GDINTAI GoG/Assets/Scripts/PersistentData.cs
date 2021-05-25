using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistentData : MonoBehaviour
{
    public static PersistentData instance = null;

    public enum GameState { Placement = 1, Versus = 2, GameOver = 3 };

    private GameState state;

    private GameObject button;

    private string winner;

    private string turn;


    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }


        this.state = GameState.Placement;

        this.button = GameObject.Find("StartButton");

        this.button.SetActive(false);

        this.winner = null;

        this.turn = "Player";

        DontDestroyOnLoad(this.gameObject);
    }


    public void VersusState()
    {
        this.state = GameState.Versus;
    }

    public void GameOverState()
    {
        this.state = GameState.GameOver;
    }

    public void PlacementState()
    {
        this.state = GameState.Placement;
    }

    public GameState GetState()
    {
        return this.state;
    }


    public void ShowStartButton()
    {
        this.button.SetActive(true);
    }

    public void HideStartButton()
    {
        this.button.SetActive(false);
    }

    public void DestroyButton()
    {
        Destroy(this.button);
    }

    public string GetWinner()
    {
        return this.winner;
    }

    public void SetWinner(string winner)
    {
        this.winner = winner;
    }

    public string GetTurn()
    {
        return this.turn;
    }

    public void SetTurn(string turn)
    {
        this.turn = turn;
    }
}


