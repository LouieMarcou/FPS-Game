using UnityEngine;

public class GunReset : MonoBehaviour
{
    public float remaining = 30;
    private float orignalTime;
    float seconds;

    private bool timerIsRunning = false;
    
    void Awake()
    {
        orignalTime = remaining;
    }

    // Start is called before the first frame update
    void Start()
    {
        
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
                Debug.Log(remaining);
            }
            else
            {
                //Debug.Log("Gun will despawn");
                remaining = 0;
                timerIsRunning = false;
                gameObject.SetActive(false);
            }
        }
    }

    public void hasBeenDropped()
    {
        timerIsRunning = true;
    }

    public void hasBeenPickedUp()
    {
        timerIsRunning = false;
        remaining = orignalTime;
    }

    void displayTime(float timeToDisplay)
    {
        timeToDisplay += 1;
        seconds = Mathf.FloorToInt(remaining % 60);
    }
}
