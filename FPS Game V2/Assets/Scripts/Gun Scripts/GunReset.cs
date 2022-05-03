using UnityEngine;

public class GunReset : MonoBehaviour
{
    public float remaining = 30;
    private float orignalTime;
    float seconds;

    private bool timerIsRunning = false;
    private GameObject SpawnPoint;

    void Awake()
    {
        orignalTime = remaining;
        SpawnPoint = GameObject.Find("Guns");
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
                //Debug.Log(remaining);
            }
            else
            {
                //Debug.Log("Gun will despawn");
                remaining = 0;
                timerIsRunning = false;
                if(gameObject.name != "pistol(Clone)")
                {
                    reset();
                    gameObject.SetActive(true);
                }
                else
                {
                    gameObject.SetActive(false);
                }
                
                
            }
        }
    }

    public void reset()
    {
        if (gameObject.GetComponent<Gun>().player != null && gameObject.GetComponent<Gun>().player.getAliveStatus() == false && gameObject.GetComponent<Gun>().getEquiped() == true)
        {
            if (gameObject.GetComponent<Gun>().getPickup() == true)
            {
                gameObject.transform.parent = null;
                gameObject.GetComponent<Gun>().unequip();
                gameObject.GetComponent<Gun>().returnToSpawn();

                gameObject.GetComponent<Gun>().resetAmmo();
                gameObject.SetActive(true);
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
