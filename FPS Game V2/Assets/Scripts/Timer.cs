using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public float remaining = 100;
    float minutes;
    float seconds;
    public Text timerObject;
    // Start is called before the first frame update
    void Start()
    {
        minutes = Mathf.FloorToInt(remaining / 60);
        seconds = Mathf.FloorToInt(remaining % 60);
    }

    // Update is called once per frame
    void Update()
    {
        remaining -= Time.deltaTime;
        minutes = Mathf.FloorToInt(remaining / 60);
        seconds = Mathf.FloorToInt(remaining % 60);
        timerObject.text = "Time: " +minutes + ":"+seconds;

    }
}
