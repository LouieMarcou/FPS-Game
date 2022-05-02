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
        if(gameObject.GetComponent<Gun>().player != null && gameObject.GetComponent<Gun>().player.getAliveStatus() == false && gameObject.GetComponent<Gun>().getEquiped() == true)//can return picked up gun to it's orginal position reset it's ammo.
        {
            if (gameObject.GetComponent<Gun>().getPickup() == true)
            {
                Debug.Log(gameObject.GetComponent<Gun>().getPickup());
                gameObject.transform.parent = null;
                gameObject.GetComponent<Gun>().unequip();
                gameObject.GetComponent<Gun>().returnToSpawn();
                
            }
            
            gameObject.GetComponent<Gun>().resetAmmo();
            gameObject.SetActive(true);
            
        }
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
                if(gameObject.name != "pistol(Clone)")
                {
                    gameObject.GetComponent<Gun>().returnToSpawn();
                    gameObject.GetComponent<Gun>().resetAmmo();
                    gameObject.SetActive(true);
                }
                else
                {
                    gameObject.SetActive(false);
                }
                
                
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
