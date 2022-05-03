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
    private PlayerController player1;
    private PlayerController player2;

    // Start is called before the first frame update
    void Start()
    {
        playerManager = GameObject.Find("Player Manager");
        player1 = playerManager.GetComponent<PlayerSpawnManager>().players[0].GetComponent<PlayerController>();
        player2 = playerManager.GetComponent<PlayerSpawnManager>().players[1].GetComponent<PlayerController>();
        if (attachedToPlayer)
        {
            //Debug.Log(GameObject.Find("GameTimer").GetComponent<Timer>().getRemaining());
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
                    GameObject canvas = GameObject.Find("EndScreen");
                    GameObject victory = GameObject.Find("EndScreen/VictoryText");
                    GameObject kills = GameObject.Find("EndScreen/PlayerKills");
                    canvas.SetActive(true);
                    if(player1.getKills() > player2.getKills())
                    {
                        victory.GetComponent<Text>().text = "Player 1 wins!";
                        kills.GetComponent<Text>().text = "Player 1 kills: " + player1.getKills();
                    }
                    else if(player1.getKills() < player2.getKills())
                    {
                        victory.GetComponent<Text>().text = "Player 2 wins!";
                        kills.GetComponent<Text>().text = "Player 2 kills: " + player1.getKills();
                    }
                    else if(player1.getKills() == player2.getKills())
                    {
                        victory.GetComponent<Text>().text = "It is a draw!";
                    }
                }
            }
        }
        
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
