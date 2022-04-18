using UnityEngine;
using UnityEngine.UI;

public class Respawn : MonoBehaviour
{
    public float respawnTime;
    private float currentTime;
    private float seconds;

    private bool isAlive;
    private bool timerIsRunning = false;

    public GameObject spawnPoint;
    public GameObject respawnTextObject;
    public Text respawnTimeText;

    void Awake()
    {

    }
    // Start is called before the first frame update
    void Start()
    {
        currentTime = respawnTime;
    }

    // Update is called once per frame
    void Update()
    {
        if(timerIsRunning)
        {
            if(currentTime>0)
            {
                displayTime();
                currentTime -= Time.deltaTime;
                
                //Debug.Log(respawnTime);
            }
            else if(currentTime<=0)
            {
                Debug.Log("Respawning now");

                currentTime = respawnTime;
                gameObject.GetComponent<PlayerController>().resetHealth();
                gameObject.transform.position = spawnPoint.transform.position;
                respawnTextObject.SetActive(false);
                timerIsRunning = false;
            }
        }
        else if(gameObject.GetComponent<PlayerController>().getCurrentHealth()<=0)
        {
            respawnTextObject.SetActive(true);
            timerIsRunning = true;
            displayTime();
        }
    }

    void displayTime()
    {
        seconds = Mathf.FloorToInt(currentTime % 60);
        respawnTimeText.text = string.Format("{0:0}",seconds);
    }
}
