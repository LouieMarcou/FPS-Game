using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Gun : MonoBehaviour
{
    //NEED TO MAKE A RELOAD TIMER FOR EACH GUN
    //Add recoil to camera 

    public Text ammo;

    public Animator animator;

    private int shotsFired;
    public int magazineSize;
    public int maxAmmo;
    private int totalMaxAmmo;
    private int currentAmmo;

    #region floats
    public float reloadTime;
    public float swapTime;
    private float holsterTime;
    private float drawTime;
    public float damage = 10f;
    public float range = 100f;
    public float fireRate = 15f;
    private float nextTimeToFire = 0f;
    #endregion

    private bool isReloading = false;
    private bool isSwaping = false;
    [SerializeField] public bool rapidFire = false;

    private Camera cam;
    private WaitForSeconds rapidFireWait;
    private WaitForSeconds weaponSwapWait;

    public GameObject[] muzzelFlash;//array of muzzle flashes
    public GameObject muzzelFlashPool;//array of muzzle flashes
    
    public Transform muzzelSpawn;//where the muzzle flash will appear
    private GameObject holdFlash;
    public GameObject bulletHole;

    private Transform pos;
    private Recoil recoil_script;

    private void Awake()
    {
        rapidFireWait = new WaitForSeconds(1 / fireRate);
    }
    // Start is called before the first frame update
    void Start()
    {
        currentAmmo = magazineSize;
        totalMaxAmmo = maxAmmo;
        //recoil_script = cam.GetComponent<Recoil>();
        //recoil_script = pos.GetComponent<Recoil>();
        //recoil_script = pos.GetComponent<Recoil>();
        //recoil_script = GameObject.Find("Player/Main Camera/GunPosition").GetComponent<Recoil>();
        recoil_script = gameObject.GetComponent<Recoil>();
        //Debug.Log(recoil_script);
    }

    void OnEnable()
    {
        isReloading = false;
        animator.SetBool("Reloading", false);
    }

    // Update is called once per frame
    void Update()
    {
        transform.localRotation = pos.localRotation;
    }

    public void Shoot(bool check)//alternate script to shooting. May be better
    {
        //Debug.Log(check);
        if (isReloading || isSwaping)
        {
            return;
        }
        if (currentAmmo <= 0)
        {
            if (maxAmmo <= 0)
            {
                return;
            }
            StartCoroutine(Reload());
            return;
        }
        if(check && Time.time>=nextTimeToFire)
        {
           nextTimeToFire = Time.time + 1f / fireRate;
           Firing(); 
        }
    }

    public void Firing()
    {
        //Debug.Log(pos.rotation);
        recoil_script.RecoilFire();//muzzle flash positon does not sync up with the recoil
        //Debug.Log(pos.rotation);
        currentAmmo--;
        shotsFired++;
        int random = Random.Range(0, 5);
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;
        holdFlash = muzzelFlashPool.GetComponent<NewObjectPool>().releaseRandom(); //Object Pool atempt
        holdFlash.transform.position = muzzelSpawn.transform.position;
        holdFlash.transform.rotation = muzzelSpawn.transform.rotation * Quaternion.Euler(0, 0, 90);
        holdFlash.SetActive(true);
        if (Physics.Raycast(cam.transform.position,cam.transform.forward,out hit,range ))
        {
            //Debug.Log(hit.transform.name);
            
            //Target target = hit.transform.GetComponent<Target>();
            PlayerController player = hit.transform.GetComponent<PlayerController>();
            if(player != null)
            {
                player.takeDamage(damage);
                
            }
            SharedPool.SharedInstance.setRaycastHit(hit);
            SharedPool.SharedInstance.createBulletHole();
        }
    }

    public IEnumerator RapidFire()
    {
        if(rapidFire)
        {
            while (true)
            {
                Shoot(rapidFire);
                yield return rapidFireWait;
            }
        }
        else
        {
            Shoot(rapidFire);
            yield return null;
        }
    }

    IEnumerator Reload()
    {    
        isReloading = true;
        Debug.Log("Reloading...");
        animator.SetBool("Reloading", true);
        yield return new WaitForSeconds(reloadTime);
        
        //Debug.Log(maxAmmo);
        if (shotsFired > maxAmmo)
        {
            shotsFired = maxAmmo;
        }
        maxAmmo -= shotsFired;
        currentAmmo += shotsFired;


        shotsFired = 0;
        animator.SetBool("Reloading", false);
        Debug.Log("Done");

        isReloading = false;
    }

    /*public IEnumerator Holster()
    {

    }

    public IEnumerator Draw()
    {

    }*/

     public IEnumerator Swapping(GameObject secondGun)
    {
        isSwaping = true;
        Debug.Log("Swapping weapons");
        Debug.Log(isSwaping);
        
        yield return new WaitForSeconds(swapTime);
        gameObject.SetActive(false);
        Debug.Log("Swap completed");
        secondGun.SetActive(true);

        isSwaping = false;
    }

    public void Load()
    {
        if (maxAmmo < 0)
        {
            return;
        }
        StartCoroutine(Reload());
    }

    public void setPosition(Transform position)
    {
        pos = position;
    }

    public void checkIfSwaping(bool check)
    {
        isSwaping = check;
    }

    public void getCamera(Camera camera)
    {
        cam = camera;
    }
    
    public int getShotsFired()
    {
        return shotsFired;
    }

    public int getCurrentAmmo()
    {
        return currentAmmo;
    }

    public int getTotalMaxAmmo()
    {
        return totalMaxAmmo;
    }

    public string updateAmmoText()
    {
        return currentAmmo.ToString() + " / " + maxAmmo.ToString();
    }

}
