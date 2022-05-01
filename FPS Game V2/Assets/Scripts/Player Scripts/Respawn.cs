using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Respawn : MonoBehaviour
{
    public float respawnTime;
    private float currentTime;
    private float seconds;

    private bool timerIsRunning = false;
    private bool canRespawn = false;

    //public GameObject[] spawnPointArray;
    public Transform[] spawnPointArray;
    public GameObject respawnTextObject;
    public Text respawnTimeText;

    public Transform DeathZone;

    private Vector3 ammoDropHere;

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
        if (canRespawn)
        {
            Debug.Log("Respawning now");
            respawnPlayer();
            canRespawn = false;
        }
        if (timerIsRunning)
        {
            if (currentTime > 0)
            {
                displayTime();
                currentTime -= Time.deltaTime;

                //Debug.Log(currentTime);
            }
            if (currentTime <= 0)
            {
                //Debug.Log("Respawning now");

                //respawnPlayer();//does not always move player to spawn point
                currentTime = respawnTime;
                respawnTextObject.SetActive(false);
                timerIsRunning = false;
                canRespawn = true;
            }
        }
        else if (gameObject.GetComponent<PlayerController>().getAliveStatus() == false)//disable this???
        {
            gameObject.GetComponent<PlayerController>().updateDeaths();
            //ammoDropHere = DeathZone.transform.position;
            //ammoDropHere = gameObject.transform.position;
            //transform.position = DeathZone.position;
            respawnTextObject.SetActive(true);
            timerIsRunning = true;
            displayTime();
        }
    }

    void displayTime()
    {
        seconds = Mathf.FloorToInt(currentTime % 60);
        respawnTimeText.text = string.Format("{0:0}", seconds);
    }

    public Vector3 getAmmoDropPosition()
    {
        return ammoDropHere;
    }

    public void respawnPlayer()
    {
        gameObject.GetComponent<PlayerController>().resetHealth();
        gameObject.GetComponent<PlayerController>().clone1().GetComponent<Gun>().resetAmmo();
        gameObject.GetComponent<PlayerController>().clone2().GetComponent<Gun>().resetAmmo();
        int index = (int)Random.Range(0.0f, spawnPointArray.Length);
        //Debug.Log(spawnPointArray[index].position + " is "+spawnPointArray[index].name, spawnPointArray[index]);

        //gameObject.GetComponent<CharacterController>().enabled = false;

        //Debug.Log("Position before moved is " + transform.position,gameObject);

        transform.position = spawnPointArray[index].position;

        gameObject.GetComponent<CharacterController>().enabled = true;
        gameObject.GetComponent<PlayerController>().makeAlive();
        //Debug.Log("moved player");
        //Debug.Log("Position after moved is " + transform.position,gameObject);
        //gameObject.GetComponent<PlayerController>().makeAlive();

    }
}
