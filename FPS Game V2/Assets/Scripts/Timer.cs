using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    float value;
    bool start;
    public Text timerObject;
    // Start is called before the first frame update
    void Start()
    {
        value = 0;
        start = false;
    }

    // Update is called once per frame
    void Update()
    {
        //int newValue = (int)value;
        //value = Mathf.Round(Time.time * 10f) * 0.1f;
        value = Mathf.FloorToInt(Time.time);
        timerObject.text = "Time: " + value;
        //Debug.Log(value);
    }
}
