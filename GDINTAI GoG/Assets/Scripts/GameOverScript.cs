using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverScript : MonoBehaviour
{

    public Text winnerText;

    private void Start()
    {
        
    }

    private void Update()
    {
        winnerText.text = PersistentData.instance.GetWinner() + " Wins";
    }


    public void MainMenu()
    {
        Debug.Log("Back to Main Menu");
        SceneManager.LoadScene(0);
    }
}
