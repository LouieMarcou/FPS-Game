using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public float remaining = 600;
    float minutes;
    float seconds;
    private float timeToDisplay;

    private bool timerIsRunning = false;
    public bool attachedToPlayer = false;
    public Text timerObject;
    private GameObject playerManager;
    private GameObject player1;
    private GameObject player2;

    public GameObject panel;
    public Text victory;
    public Text kills;

    // Start is called before the first frame update
    void Start()
    {
        playerManager = GameObject.Find("Player Manager");
        //player1 = playerManager.GetComponent<PlayerSpawnManager>().players[0].GetComponent<PlayerController>();
        //if (playerManager.GetComponent<PlayerSpawnManager>().players[1].GetComponent<PlayerController>() != null)
        //    player2 = playerManager.GetComponent<PlayerSpawnManager>().players[1].GetComponent<PlayerController>();
        //else
        //    player2 = null;
        if (attachedToPlayer)
        {
            remaining = GameObject.Find("GameTimer").GetComponent<Timer>().getRemaining();
            timerIsRunning = true;
        }
        else
            timerIsRunning = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(timerIsRunning)
        {
            if (remaining>0)
            {
                displayTime(remaining);
                remaining -= Time.deltaTime;
                
            }
            else
            {
                Debug.Log("Time has run out");
                remaining = 0;
                timerIsRunning = false;
                if(attachedToPlayer == false)
                {
                    endGameEvent();
                }
            }
        }
        
    }

    public void endGameEvent()
    {
        //GameObject canvas = GameObject.Find("Panel");
        //GameObject victory = GameObject.Find("Panel/VictoryText");
        //GameObject kills = GameObject.Find("Panel/PlayerKills");
        Debug.Log(victory);
        Debug.Log(player1);
        panel.SetActive(true);
        if (player2 != null)
        {
            if (player1.GetComponent<PlayerController>().getKills() > player2.GetComponent<PlayerController>().getKills())
            {
                //victory.GetComponent<Text>().text += "Player 1 wins!";
                victory.text = "Player 1 wins!";
                //kills.GetComponent<Text>().text += "Player 1 kills: " + player1.getKills();
                kills.text = "Player 1 kills: " + player1.GetComponent<PlayerController>().getKills();
            }
            else if (player1.GetComponent<PlayerController>().getKills() < player2.GetComponent<PlayerController>().getKills())
            {
                //victory.GetComponent<Text>().text += "Player 2 wins!";
                victory.text = "Player 2 wins!";
                //kills.GetComponent<Text>().text += "Player 2 kills: " + player2.getKills();
                kills.text = "Player 2 kills: " + player2.GetComponent<PlayerController>().getKills();
            }
            else if (player1.GetComponent<PlayerController>().getKills() == player2.GetComponent<PlayerController>().getKills())
            {
                victory.text = "It is a draw!";
            }
            Time.timeScale = 0;
        }
    }

    public void setPlayer1(GameObject player)
    {
        player1 = player;
    }

    public void setPlayer2(GameObject player)
    {
        player2 = player;
    }

    void displayTime(float timeToDisplay)
    {
        timeToDisplay += 1;
        minutes = Mathf.FloorToInt(remaining / 60);
        seconds = Mathf.FloorToInt(remaining % 60);
        if(attachedToPlayer)
            timerObject.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public float getRemaining()
    {
        return remaining;
    }
}
