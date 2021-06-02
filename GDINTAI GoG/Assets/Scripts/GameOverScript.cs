using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class GameOverScript : MonoBehaviour
{
    private Text winnerText = null;

    private void Start()
    {
        
    }

    private void Update()
    {
        GameObject temp = GameObject.FindGameObjectWithTag("WinnerText");
        this.winnerText = temp.GetComponent<Text>();
        this.winnerText.text = PersistentData.instance.GetWinner() + " Wins";
        Destroy(PersistentData.instance.gameObject);
    }


    public void MainMenu()
    {
        Debug.Log("Back to Main Menu");
        SceneManager.LoadScene(0);
    }
}
