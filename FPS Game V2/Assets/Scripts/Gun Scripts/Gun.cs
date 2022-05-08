using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Gun : MonoBehaviour
{
    //Note: Player can accidently shoot and injure themseleves
    #region variables
    //Scripts
    [SerializeField] public PlayerController player;

    public Text ammo;

    public Animator animator;

    private int shotsFired;
    public int magazineSize;
    public int maxAmmo;//ammo on the right
    private int totalMaxAmmo;//absoulute max ammo. can not go above this
    private int currentAmmo;//ammo on the left
    private int originalMax;

    public float reloadTime;
    //public float swapTime;
    public float holsterTime;
    public float drawTime;
    public float damage = 10f;
    public float range = 100f;
    public float fireRate = 15f;
    private float nextTimeToFire = 0f;

    private bool isReloading = false;
    private bool isSwaping = false;
    [SerializeField] public bool rapidFire = false;
    [SerializeField] public bool burstFire = false;
    [SerializeField] public bool scope = false;
    private bool hasHolstered = false;
    private bool isDrawing = false;
    public bool isEquiped = false;
    public bool isPickup = false;

    private Camera cam;
    private WaitForSeconds rapidFireWait;
    private WaitForSeconds burstFireWait;
    private WaitForSeconds weaponSwapWait;
    private WaitForSeconds holsterWait;

    public GameObject[] muzzelFlash;//array of muzzle flashes
    public GameObject muzzelFlashPool;//array of muzzle flashes

    public Transform muzzelSpawn;//where the muzzle flash will appear
    private GameObject holdFlash;
    public GameObject bulletHole;

    private Transform pos;
    private Recoil recoil_script;

    public AudioSource shoot_sound_source, reloadSound_source;//sounds
    public AudioSource hitMarker;

    private GameObject spawnPosition;

    #endregion
    private void Awake()
    {
        rapidFireWait = new WaitForSeconds(1 / fireRate);
        burstFireWait = new WaitForSeconds(1 / fireRate);//change this value to fix the burst fire problem?
        holsterWait = new WaitForSeconds(0.01f);
        player = GetComponentInParent<PlayerController>();
        currentAmmo = magazineSize;
        totalMaxAmmo = maxAmmo;
        originalMax = maxAmmo;
        //Debug.Log(player);

    }
    // Start is called before the first frame update
    void Start()
    {
        recoil_script = gameObject.GetComponent<Recoil>();
        shoot_sound_source = transform.Find("AudioSourceShoot").GetComponent<AudioSource>();
        reloadSound_source = transform.Find("AudioSourceReload").GetComponent<AudioSource>();
        hitMarker = transform.Find("AudioSourceHitMarker").GetComponent<AudioSource>();
        //animator = null;
    }

    void OnEnable()
    {
        isReloading = false;
        if (animator != null)
        {
            animator.SetBool("Reloading", false);
        }

    }

    // Update is called once per frame
    void Update()
    {
        //if (pos != null)
        //{
        //    transform.localRotation = pos.localRotation;
        //}
        //Debug.Log(transform.localRotation);
    }

    public void Shoot(bool check)
    {
        if (isReloading || isSwaping)
        {
            //Debug.Log(isSwaping);
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
        if (check && Time.time >= nextTimeToFire)
        {
            nextTimeToFire = Time.time + 1f / fireRate;
            Firing();
        }
    }

    public void Firing()
    {
        recoil_script.RecoilFire();//muzzle flash positon does not sync up with the recoil
        player.GetComponent<mouselook>().didFire();
        currentAmmo--;
        shotsFired++;
        int random = Random.Range(0, 5);
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;
        holdFlash = muzzelFlashPool.GetComponent<NewObjectPool>().releaseRandom(); //Object Pool attempt
        holdFlash.transform.position = muzzelSpawn.transform.position;
        holdFlash.transform.rotation = muzzelSpawn.transform.rotation * Quaternion.Euler(0, 0, 90);
        holdFlash.SetActive(true);
        int layerMask = (1 << 7) | (1 << 11 | (1 << 10));
        if (shoot_sound_source)
            shoot_sound_source.Play();
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, range, ~layerMask))
        {
            PlayerController target = hit.transform.GetComponent<PlayerController>();
            SharedPool.SharedInstance.setRaycastHit(hit);
            if (target != null && target != player)
            {

                target.takeDamage(damage);

                //SharedPool.SharedInstance.createBlood();
                //GameObject.Find("BloodEffectPool").GetComponent<SharedPool>().createBlood();
                if (target.getCurrentHealth() <= 0)
                {
                    Debug.Log("Player has died");
                    player.GetComponent<PlayerController>().updateKills();
                }

            }
            if (target == null)
            {
                GameObject.Find("BulletHoleObjectPool").GetComponent<SharedPool>().createBulletHole();
            }

        }
    }

    public IEnumerator RapidFire()
    {
        if (rapidFire)
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

    public IEnumerator BurstFire()//alter so that you have to wait for the burst to finish to fire again
    {
        int count = 0;
        if (burstFire)
        {
            while (count < 3)
            {
                Shoot(burstFire);
                count++;
                yield return burstFireWait;//change this value to fix the problem?
            }
        }
        else
        {
            Shoot(burstFire);
            yield return null;
        }
    }

    IEnumerator Reload()
    {
        isReloading = true;
        animator.SetBool("Reloading", true);
        if (reloadSound_source)
            reloadSound_source.Play();
        yield return new WaitForSeconds(reloadTime);

        if (shotsFired > maxAmmo)
        {
            Debug.Log("shotsFired is greater than max ammo: " + shotsFired + " > " + maxAmmo);
            shotsFired = maxAmmo;
        }
        if(shotsFired<maxAmmo)
        {
            Debug.Log("shotsFired is less than max ammo: " + shotsFired + " < " + maxAmmo);
            Debug.Log("shots fired becomes: " + (magazineSize - currentAmmo));
            shotsFired = magazineSize - currentAmmo;

        }
        maxAmmo -= shotsFired;
        //Debug.Log(shotsFired);
        currentAmmo += shotsFired;
        //Debug.Log(shotsFired);

        shotsFired = 0;
        animator.SetBool("Reloading", false);
        isReloading = false;
    }

    public IEnumerator Holster(GameObject secondGun)
    {
        isSwaping = true;
        hasHolstered = true;
        animator.SetBool("Holster", true);
        //animator.Play("GunHolster 1", 0, 0);
        //Debug.Log("Swapping weapons");
        //Debug.Log(isSwaping);

        yield return new WaitForSeconds(holsterTime);

        animator.SetBool("Holster", false);
        gameObject.SetActive(false);
        hasHolstered = false;

    }

    public IEnumerator Draw(GameObject secondGun)
    {
        while (hasHolstered)
        {
            // Debug.Log("waiting");
            yield return holsterWait;
        }
        isDrawing = true;
        secondGun.SetActive(true);
        float temp = secondGun.GetComponent<Gun>().drawTime / 2;
        //animator.SetFloat("DrawSpeed", temp);

        animator.SetBool("Draw", true);
        //animator.Play("GunDraw", 0, 0);
        //Debug.Log(temp);
        yield return new WaitForSeconds(temp);

        //if (secondGun.GetComponent<Gun>().drawTime >= 1.0f)
        //    yield return new WaitForSeconds(secondGun.GetComponent<Gun>().drawTime);
        //else if (secondGun.GetComponent<Gun>().drawTime <= 0.5f)
        //    yield return new WaitForSeconds(secondGun.GetComponent<Gun>().drawTime);

        if (player.GetComponent<PlayerController>().getAiming())
            yield return null;
        isDrawing = false;
        animator.SetBool("Draw", false);
        //Debug.Log("Done");
        isSwaping = false;
        //Debug.Log(isSwaping);
    }

    /*public IEnumerator Swapping(GameObject secondGun)
    {
        isSwaping = true;
        Debug.Log("Swapping weapons");
        //Debug.Log(isSwaping);
        
        yield return new WaitForSeconds(swapTime/2);
        gameObject.SetActive(false);
        //Debug.Log("Swap completed");
        secondGun.SetActive(true);
        player.ammo.text = secondGun.GetComponent<Gun>().updateAmmoText();//Check this
        isSwaping = false;
    }*/

    public void Load()
    {
        if (maxAmmo < 0)
        {
            return;
        }
        StartCoroutine(Reload());
    }

    public void resetAmmo()
    {
        currentAmmo = magazineSize;
        maxAmmo = originalMax;
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

    public bool holsterCheck()
    {
        return hasHolstered;
    }

    public bool reloadingCheck()
    {
        return isReloading;
    }

    public string updateAmmoText()
    {
        return currentAmmo.ToString() + " / " + maxAmmo.ToString();
    }

    public WaitForSeconds getHolsterWait()
    {
        return holsterWait;
    }

    public void removePlayerScript()
    {
        player = null;
    }

    public bool getEquiped()
    {
        return isEquiped;
    }

    public void equip()
    {
        isEquiped = true;
    }

    public void unequip()
    {
        isEquiped = false;
        removePlayerScript();
    }

    public bool getPickup()
    {
        return isPickup;
    }

    public void setPickup()
    {
        isPickup = true;
    }

    public void returnToSpawn()
    {
        gameObject.transform.parent = spawnPosition.transform;
        transform.position = spawnPosition.transform.position;
        gameObject.layer = 10;
        //Debug.Log("Has returned to spawn");
        //setPosition(spawnPosition.transform);
    }

    public Transform getSpawnPositionTransform()
    {
        return spawnPosition.transform;
    }

    public GameObject getSpawnPositionObject()
    {
        return spawnPosition;
    }

    public void setSpawnPosition(GameObject position)
    {
        //Debug.Log(position);
        spawnPosition = position;
        spawnPosition.transform.position = position.transform.position;
    }

}
