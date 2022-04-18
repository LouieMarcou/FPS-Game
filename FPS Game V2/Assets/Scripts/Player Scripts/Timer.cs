using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public float remaining = 600;
    float minutes;
    float seconds;
    public float timeToDisplay;

    private bool timerIsRunning = false;

    public Text timerObject;
    // Start is called before the first frame update
    void Start()
    {
        timerIsRunning = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(timerIsRunning)
        {
            if(remaining>0)
            {
                displayTime(remaining);
                remaining -= Time.deltaTime;
                
            }
            else
            {
                Debug.Log("Time has run out");
                remaining = 0;
                timerIsRunning = false;
            }
        }
        
    }

    void displayTime(float timeToDisplay)
    {
        timeToDisplay += 1;
        minutes = Mathf.FloorToInt(remaining / 60);
        seconds = Mathf.FloorToInt(remaining % 60);
        timerObject.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
