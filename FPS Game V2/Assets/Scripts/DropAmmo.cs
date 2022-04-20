using UnityEngine;

public class DropAmmo : MonoBehaviour
{
    private GameObject ammoPool;
    private GameObject ammoPickup;
    int drop = 0;
    // Start is called before the first frame update
    void Start()
    {
        ammoPool = GameObject.Find("AmmoPool");
    }

    // Update is called once per frame
    void Update()
    {
        if(gameObject.GetComponent<PlayerController>().getCurrentHealth() > 0)
        {
            drop = 0;
        }
        if(gameObject.GetComponent<PlayerController>().getCurrentHealth()<=0 && drop==0)
        {
            drop = 1;
            //dropAmmo();
        }
    }

    public void dropAmmo()
    {
        ammoPickup = ammoPool.GetComponent<SharedPool>().GetPooledObject();
        //ammoPickup.transform.position = gameObject.GetComponent<Respawn>().getAmmoDropPosition();
        ammoPickup.SetActive(true);
    }

    public GameObject getAmmoPickup()
    {
        return ammoPickup;
    }
}
