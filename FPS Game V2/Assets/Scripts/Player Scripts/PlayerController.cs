using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEditor;
[RequireComponent(typeof(CharacterController))]



public class PlayerController : MonoBehaviour
{
    #region variables
    [SerializeField] private float playerSpeed = 10.0f;
    [SerializeField] private float jumpHeight = 1.0f;
    [SerializeField] private float gravityValue = -9.81f;
    [SerializeField] mouselook look;

    //public Canvas playerCanvas;

    public Slider staminaBar;
    public Slider healthBar;

    public Text ammo;
    public Text killText;
    public Text deathText;

    public Transform playerBody;
    public GameObject gunPosition;
    public GameObject camera_recoil;
    public Transform deathZone;
    public Animator animate;

    private CharacterController controller;

    private Vector3 playerVelocity;

    private Vector2 movementInput = Vector2.zero;
    private Vector2 pauseMovementInput = Vector2.zero;
    private Vector2 mouseInput;

    public GameObject gun1;
    public GameObject gun2;
    public GameObject gunClone;
    public GameObject gunClone2;
    public GameObject currentGun;
    private GameObject tempGun;
    private GameObject playerManager;
    private GameObject pistol;

    [SerializeField] public Camera cam;
    [SerializeField] public Camera gunCam;

    private WaitForSeconds regentick = new WaitForSeconds(0.1f);
    private WaitForSeconds staminaRegentick = new WaitForSeconds(0.05f);
    private WaitForSeconds staminaRegenWait = new WaitForSeconds(2f);
    private WaitForSeconds healthRegenWait = new WaitForSeconds(7f);

    private Coroutine staminaRegen;
    private Coroutine healthRegen;
    private Coroutine rapidFireCoroutine;
    private Coroutine burstFireCoroutine;
    private Coroutine weaponSwapCoroutine;
    private Coroutine holsterCoroutine;
    private Coroutine drawCoroutine;

    public float healthMax = 100f;
    private float currentHealth;
    private float tempSpeed = 0f;
    private float sprintSpeed = 0f;
    private float staminaMax = 100f;
    private float currentStamina;
    private float staminaDrain = 15f;
    public float pickupDistance = 4f;

    private int kills = 0;
    private int deaths = 0;
    private int equipGunsAmount = 1;

    private bool jumped = false;
    private bool fire = false;
    private bool reload = false;
    private bool is_sprinting = false;
    private bool is_aiming = false;
    private bool using_scope = false;
    private bool swapCheck = false;
    private bool pickupCheck = false;
    private bool canPickup = false;
    private bool is_paused = false;
    private bool is_holstering = false;
    private bool is_drawing = false;
    private bool is_joining = false;
    private bool groundedPlayer;
    private bool isAlive = true;

    float m_MySliderValue = 1.0f;//for aniamtion speed testing

    private int layerMaskForGun;
    private int id;

    private GameObject pickupObject;
    private Transform pickupTransform;

    Ray ray;
    RaycastHit hit;
    #endregion

    void Awake()
    {
        controller = gameObject.GetComponent<CharacterController>();
        controller.enabled = false;
        playerManager = GameObject.Find("Player Manager");
    }

    private void Start()
    {

        camera_recoil.GetComponent<Recoil>().setPlayer(gameObject.GetComponent<PlayerController>());
        pickupTransform = gameObject.transform.Find("PlayerCanvas/PickupText");
        pickupObject = pickupTransform.gameObject;
        id = gameObject.GetComponent<PlayerDetails>().playerID;
        GameObject timer = GameObject.Find("GameTimer");

        if (gameObject.GetComponent<PlayerDetails>().playerID == 1)
        {
            timer.GetComponent<Timer>().setPlayer1(gameObject);
            gunCam.GetComponent<Camera>().rect = new Rect(0f, 0f, 0.5f, 1.0f);
            layerMaskForGun = (1 << 7) | (1 << 5);
            //layerMaskForGun = (1 << 7);
            gunCam.GetComponent<Camera>().cullingMask = layerMaskForGun;
            cam.GetComponent<Camera>().cullingMask = ~layerMaskForGun;
            layerMaskForGun = 7;
            playerManager.GetComponent<PlayerSpawnManager>().players[0] = gameObject;
        }

        else if (gameObject.GetComponent<PlayerDetails>().playerID == 2)
        {
            timer.GetComponent<Timer>().setPlayer2(gameObject);
            gunCam.rect = cam.rect;
            layerMaskForGun = (1 << 11) | (1 << 5);
            gunCam.GetComponent<Camera>().cullingMask = layerMaskForGun;
            cam.GetComponent<Camera>().cullingMask = ~layerMaskForGun;
            layerMaskForGun = 11;
            playerManager.GetComponent<PlayerSpawnManager>().players[1] = gameObject;
        }
        tempSpeed = playerSpeed;
        gunPosition.layer = layerMaskForGun;
        gunClone = Instantiate(gun1, gunPosition.transform, false);
        gunClone.layer = layerMaskForGun;
        gunClone.transform.position = gunPosition.transform.position;
        gunClone.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        gunClone.GetComponent<Gun>().animator = animate;
        gunClone.GetComponent<Gun>().getCamera(cam);
        //gunClone.GetComponent<Gun>().setPosition(gunPosition);
        gunClone.GetComponent<Recoil>().player = gameObject.GetComponent<PlayerController>();
        gunClone.GetComponent<Gun>().equip();
        gunClone.SetActive(true);
        pistol = gunClone;
        currentGun = gunClone;
        //gunClone2 = Instantiate(gun2, gunPosition.transform, false);
        //gunClone2.layer = layerMaskForGun;
        //gunClone2.transform.position = gunPosition.transform.position;
        //gunClone2.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        //gunClone2.GetComponent<Gun>().animator = animate;
        //gunClone2.GetComponent<Gun>().getCamera(cam);
        ////gunClone2.GetComponent<Gun>().setPosition(gunPosition);
        //gunClone2.GetComponent<Recoil>().player = gameObject.GetComponent<PlayerController>();
        //gunClone2.GetComponent<Gun>().equip();
        //gunClone2.SetActive(false);

        
        currentGun.GetComponent<Gun>().animator = animate;

        ammo.text = currentGun.GetComponent<Gun>().updateAmmoText();
        killText.text = "Kills: ";
        deathText.text = "Deaths: ";
        currentStamina = staminaMax;
        staminaBar.maxValue = staminaMax;
        staminaBar.value = staminaMax;

        currentHealth = healthMax;
        healthBar.maxValue = healthMax;
        healthBar.value = healthMax;


        //camera_recoil = transform.Find("CameraRotation/CameraRecoil");
        //Debug.Log(transform.Find("CameraRotation/CameraRecoil"));
        kills = 0;
        deaths = 0;

        controller.enabled = true;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (gameObject.GetComponent<PauseMenu>().getGameIsPaused())
        {
            return;
        }

        movementInput = context.ReadValue<Vector2>();
        updateStamina();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (gameObject.GetComponent<PauseMenu>().getGameIsPaused())
            return;
        jumped = context.ReadValueAsButton();
        jumped = context.action.triggered;
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        if (gameObject.GetComponent<PauseMenu>().getGameIsPaused() || is_aiming)
            return;
        is_sprinting = context.ReadValueAsButton();
        is_sprinting = context.action.triggered;

        if (context.performed)
        {
            if (is_aiming || fire || reload || currentGun.GetComponent<Gun>().reloadingCheck() || movementInput == Vector2.zero)//not canceling function properly. will still drain if aiming
                return;

            is_sprinting = true;
            updateSprint();
        }
        if (context.canceled)
        {
            cancelSprint();
        }

    }

    public void OnFire(InputAction.CallbackContext context)//fires gun and checks if the gun has rapid fire
    {
        if (gameObject.GetComponent<PauseMenu>().getGameIsPaused() || isAlive == false)
        {
            return;
        }

        fire = context.ReadValueAsButton();
        fire = context.action.triggered;
        if (is_sprinting)
            cancelSprint();
        if (currentGun.GetComponent<Gun>().rapidFire)
        {
            currentGun.GetComponent<Gun>().getCamera(cam);
            if (context.performed || currentGun.GetComponent<Gun>().holsterCheck())//problem when player switches weapons while firing???
            {
                StartFiring();
            }
            if (context.canceled || reload || swapCheck)
            {
                StopFiring();
            }
        }
        else if (currentGun.GetComponent<Gun>().burstFire)
        {
            currentGun.GetComponent<Gun>().getCamera(cam);
            if (context.performed || currentGun.GetComponent<Gun>().holsterCheck())
            {
                BurstFiring();
            }
            if (context.canceled || reload || swapCheck)
            {
                StopFiring();
            }
        }
        else
        {
            currentGun.GetComponent<Gun>().getCamera(cam);
            currentGun.GetComponent<Gun>().Shoot(fire);
        }
    }

    public void OnReload(InputAction.CallbackContext context)//Reloads your current gun
    {
        if (gameObject.GetComponent<PauseMenu>().getGameIsPaused())
            return;
        reload = context.ReadValueAsButton();
        reload = context.action.triggered;
        if (currentGun.GetComponent<Gun>().maxAmmo <= 0)
            return;
        if (is_aiming && animate.GetBool("Aiming"))
        {
            animate.SetBool("Aiming", false);
            is_aiming = false;
        }
        if (reload)
        {
            if (currentGun.GetComponent<Gun>().getShotsFired() != 0)
            {
                playerSpeed /= 2;
                currentGun.GetComponent<Gun>().Load();
                cancelSprint();
            }
        }
    }

    public void OnSwap(InputAction.CallbackContext context)//deactivates current gun and activates the other
    {
        if (gameObject.GetComponent<PauseMenu>().getGameIsPaused() || equipGunsAmount == 1)
            return;
        swapCheck = context.ReadValueAsButton();
        swapCheck = context.action.triggered;
        if (is_sprinting)
            cancelSprint();
        if (swapCheck && equipGunsAmount == 2)
        {
            //Debug.Log(gunClone + " " + gunClone2);
            if (is_aiming)
            {
                animate.SetBool("Aiming", false);
                is_aiming = false;
            }
            StopFiring();
            if (gunClone.activeInHierarchy)
            {
                startHolster(gunClone);
                startDraw(gunClone2);
                currentGun = gunClone2;
            }
            else if (gunClone2.activeInHierarchy)
            {
                startHolster(gunClone2);
                startDraw(gunClone);
                currentGun = gunClone;

            }
        }
    }

    public void OnPickup(InputAction.CallbackContext context)//picks up new gun you are looking at and swaps the current gun with it
    {
        if (gameObject.GetComponent<PauseMenu>().getGameIsPaused())
            return;
        pickupCheck = context.ReadValueAsButton();
        pickupCheck = context.action.triggered;
        //Had a weird instance where I dropped a gun and was not able to pick it back up
        if (canPickup)
        {
            if (pickupCheck && equipGunsAmount == 1)
            {
                pickupGun();
            }
            else if (pickupCheck && equipGunsAmount == 2)
            {
                dropGun();
                pickupGun();
            }
        }
    }

    public void OnAim(InputAction.CallbackContext context)
    {
        if (gameObject.GetComponent<PauseMenu>().getGameIsPaused() || swapCheck)
            return;
        is_aiming = context.ReadValueAsButton();
        is_aiming = context.action.triggered;
        if (is_sprinting)
            cancelSprint();
        if (context.performed)
        {
            playerSpeed /= 2;
            animate.SetBool("Aiming", true);
            animate.GetCurrentAnimatorClipInfo(0);
        }
        if (context.canceled)
        {
            resetPlayerSpeed();
            animate.SetBool("Aiming", false);

            is_aiming = false;
        }
    }

    public void OnScope(InputAction.CallbackContext context)
    {
        if (gameObject.GetComponent<PauseMenu>().getGameIsPaused())
            return;
        using_scope = context.ReadValueAsButton();
        using_scope = context.action.triggered;
        if (is_sprinting)
            cancelSprint();
        if (context.performed)
        {
            if (currentGun.GetComponent<Gun>().scope)//goes to scope just on click and does not leave
            {
                //animate.SetBool("Scoped", true);
                StartCoroutine(gameObject.GetComponent<Scope>().OnScoped());
            }
        }
        if (context.canceled)
        {
            if (currentGun.GetComponent<Gun>().scope)
            {
                //animate.SetBool("Scoped", false);
                gameObject.GetComponent<Scope>().OnUnscoped();
            }
        }
        using_scope = false;
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        mouseInput = context.ReadValue<Vector2>();
        if (is_aiming)
            mouseInput /= 2;
    }

    public void MouseX(InputAction.CallbackContext context)
    {
        if (gameObject.GetComponent<PauseMenu>().getGameIsPaused())
            return;
        mouseInput.x = context.ReadValue<float>();
    }

    public void MouseY(InputAction.CallbackContext context)
    {
        if (gameObject.GetComponent<PauseMenu>().getGameIsPaused())
            return;
        mouseInput.y = context.ReadValue<float>();
    }

    public void OnPause(InputAction.CallbackContext context)
    {
        is_paused = context.ReadValueAsButton();
        is_paused = context.action.triggered;
        if (is_paused)
        {
            movementInput = pauseMovementInput;
            mouseInput = Vector2.zero;
            mouseInput.x = 0f;
            mouseInput.y = 0f;
            StopFiring();
            animate.SetBool("Aiming", false);
            animate.SetBool("Scoped", false);
            gameObject.GetComponent<Scope>().OnUnscoped();
            resetPlayerSpeed();
            gameObject.GetComponent<mouselook>().enabled = false;
            gameObject.GetComponent<PauseMenu>().checkIfPaused();
        }

    }

    public void OnJoin(InputAction.CallbackContext context)
    {
        is_joining = context.ReadValueAsButton();
        is_joining = context.action.triggered;

    }

    void Update()
    {
        ammo.text = currentGun.GetComponent<Gun>().updateAmmoText();
        currentGun.GetComponent<Gun>().animator = animate;

        setCameraRecoil();

        if (movementInput.x == 0 && movementInput.y == 0)
            cancelSprint();
        if (is_sprinting)
            updateStamina();
        if (currentGun.GetComponent<Gun>().reloadingCheck() == false && !is_aiming)
        {
            resetPlayerSpeed();
        }
        //cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFieldOfView, Time.deltaTime * 10f);
        groundedPlayer = controller.isGrounded;

        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }
        Vector3 move = (transform.right * movementInput.x + transform.forward * movementInput.y);
        controller.Move(move * Time.deltaTime * (playerSpeed + sprintSpeed));
        look.ReceiveInput(mouseInput);
        // Changes the height position of the player..
        if (jumped && groundedPlayer)
        {
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
        }

        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
        checkIfCanPickup();
        checkIfAlive();
    }

    void OnCollisionEnter(Collision collision)//if you collied with something
    {
        if (collision.gameObject.tag == "AmmoPickup")//if the collied object is ammo
        {
            Debug.Log(collision.collider.name);
            int current = currentGun.GetComponent<Gun>().maxAmmo;
            int pickup = collision.gameObject.GetComponent<AmmoPickup>().ammoAmount;
            //Debug.Log(pickup);
            int max = currentGun.GetComponent<Gun>().getTotalMaxAmmo();
            int num = 0;
            if (current == max)
                return;
            else if (current + pickup < max)
            {
                //Debug.Log(current + " + " + pickup + " < " + max);
                num = current + pickup;
            }
            else if (current + pickup >= max)
            {
                //Debug.Log(current + " + " + pickup + " >= " + max);
                num = max;
            }
            collision.gameObject.SetActive(false);
            GameObject location = GameObject.Find("AmmoPickupPool");
            location.GetComponent<AmmoCrateSpawnManager>().Spawning(collision.gameObject);
            currentGun.GetComponent<Gun>().maxAmmo = num;
            if (currentGun.GetComponent<Gun>().getCurrentAmmo() == 0)
            {
                currentGun.GetComponent<Gun>().Load();
            }
        }
        if (collision.gameObject.tag == "KillTag")
        {
            //Debug.Log("IN ZONE");
            currentHealth = 0;
        }
    }

    public void cancelSprint()//sets is_sprinting to false and calls updateSprint
    {
        is_sprinting = false;
        updateSprint();
    }

    public void updateSprint()
    {
        if (is_sprinting)
        {
            animate.SetBool("Sprinting", is_sprinting);
            sprintSpeed = 5;
        }
        else if (is_sprinting == false)
        {
            animate.SetBool("Sprinting", is_sprinting);
            sprintSpeed = 0;
        }
    }

    public void updateStamina()
    {
        if (is_sprinting)
        {
            if (currentStamina >= 0)
            {
                currentStamina -= staminaDrain * Time.deltaTime;
                staminaBar.value = currentStamina;
                if (staminaRegen != null)
                {
                    StopCoroutine(staminaRegen);
                }
                staminaRegen = StartCoroutine(RegenStamina());
            }
            else
            {
                Debug.Log("Not enough stamina");
                cancelSprint();
            }
        }
    }

    private IEnumerator RegenStamina()
    {
        yield return staminaRegenWait;

        while (currentStamina < staminaMax)
        {
            currentStamina += staminaMax / 100;
            staminaBar.value = currentStamina;
            yield return staminaRegentick;
        }
        staminaRegen = null;
    }

    public void updateHealth()//Will detect if your current health is lower than your max health and start the RegenHealth Coroutine
    {

        if (currentHealth < healthMax && isAlive == true)
        {
            healthBar.value = currentHealth;
            if (healthRegen != null)
            {
                StopCoroutine(healthRegen);
            }
            healthRegen = StartCoroutine(RegenHealth());
        }

    }

    public void takeDamage(float damage)//Will make player take damage
    {
        currentHealth -= damage;
        updateHealth();
    }

    private IEnumerator RegenHealth()//Health Regen Coroutine
    {

        yield return healthRegenWait;

        while (currentHealth < healthMax)
        {
            Debug.Log("Went into regen health");
            currentHealth += healthMax / 100;
            healthBar.value = currentHealth;
            yield return regentick;
        }
        healthRegen = null;
    }

    void StartFiring()//Starts rapid fire coroutine
    {
        rapidFireCoroutine = StartCoroutine(currentGun.GetComponent<Gun>().RapidFire());
    }

    void BurstFiring()//Starts burst fire coroutine
    {
        burstFireCoroutine = StartCoroutine(currentGun.GetComponent<Gun>().BurstFire());
    }

    void StopFiring()//Stops rapid fire coroutine or burst fire coroutine
    {
        if (rapidFireCoroutine != null)
        {
            StopCoroutine(rapidFireCoroutine);
        }
        if (burstFireCoroutine != null)
        {
            StopCoroutine(burstFireCoroutine);
        }
    }

    void startHolster(GameObject secondGun)//Starts holster coroutine
    {
        holsterCoroutine = StartCoroutine(currentGun.GetComponent<Gun>().Holster(secondGun));
    }

    IEnumerator holstering(GameObject secondGun)
    {
        StopFiring();
        is_holstering = true;
        animate.SetBool("Holster", true);

        yield return new WaitForSeconds(currentGun.GetComponent<Gun>().holsterTime);

        animate.SetBool("Holster", false);
        currentGun.SetActive(false);
        is_holstering = false;
    }

    void startDraw(GameObject secondGun)//Stars draw coroutine
    {
        drawCoroutine = StartCoroutine(currentGun.GetComponent<Gun>().Draw(secondGun));

    }

    IEnumerator drawing(GameObject secondGun)
    {
        StopFiring();
        while (is_holstering)
        {
            yield return currentGun.GetComponent<Gun>().getHolsterWait();
        }
        is_drawing = true;
        secondGun.SetActive(true);
        animate.SetBool("Draw", true);

        yield return new WaitForSeconds(secondGun.GetComponent<Gun>().drawTime);

        is_drawing = false;
        animate.SetBool("Draw", false);
        currentGun = secondGun;
    }

    void checkIfCanPickup()//Will check if a gun is in distance to pick up
    {
        int layerMask = 1 << 10;
        RaycastHit hit;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, pickupDistance, layerMask))
        {
            if (hit.transform.tag == "Gun" && hit.transform.root.tag != "Player")
            {
                //Debug.Log("Can pick up");
                pickupObject.SetActive(true);
                canPickup = true;
                tempGun = hit.transform.gameObject;
            }
            else if (hit.transform.tag != "Gun")
            {
                pickupObject.SetActive(false);
            }
            else
            {
                canPickup = false;

            }
        }
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit))
        {
            if (hit.transform.tag != "Gun")
            {
                pickupObject.SetActive(false);
                canPickup = false;
            }
        }

    }

    void pickupGun()//Will pick up gun and set it equal to current gun or you second gun if you have no second gun
    {
        if (equipGunsAmount == 1)
        {

            gunClone2 = tempGun;
            gunClone2.SetActive(false);
            startHolster(gunClone);
            startDraw(gunClone2);
            currentGun = gunClone2;
            gun2 = gunClone2;
            equipGunsAmount = 2;
        }
        if(equipGunsAmount == 2)
        {
            //Debug.Log(equipGunsAmount);
            if(currentGun == gunClone)
            {
                //Debug.Log(gunClone);
                gunClone = tempGun;
            }
            else if(currentGun == gunClone2)
            {
                //Debug.Log(gunClone2);
                gunClone2 = tempGun;
            }
        }
        currentGun = tempGun;
        currentGun.transform.position = gunPosition.transform.position;
        currentGun.transform.parent = gunPosition.transform;
        currentGun.GetComponent<Gun>().player = gameObject.GetComponent<PlayerController>();
        currentGun.GetComponent<Gun>().ammo = gameObject.GetComponent<PlayerController>().ammo;
        currentGun.GetComponent<Gun>().equip();
        currentGun.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
        currentGun.GetComponent<Rigidbody>().isKinematic = true;
        //currentGun.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        currentGun.GetComponent<Recoil>().player = gameObject.GetComponent<PlayerController>();
        currentGun.GetComponent<GunReset>().hasBeenPickedUp();
        currentGun.layer = layerMaskForGun;
    }

    void dropGun()//Will current gun if a gun in front of you is detected
    {
        currentGun.transform.parent = null;
        currentGun.GetComponent<Rigidbody>().isKinematic = false;
        currentGun.GetComponent<Rigidbody>().useGravity = true;
        currentGun.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        currentGun.layer = 10;
        currentGun.transform.localEulerAngles = new Vector3(0f, 0f, 90f);
        currentGun.GetComponent<Gun>().removePlayerScript();
        currentGun.GetComponent<Gun>().unequip();
        currentGun.GetComponent<Recoil>().removePlayerScript();
        if (currentGun == gunClone)
        {
            gunClone = null;
        }
        else if (currentGun == gunClone2)
        {
            gunClone2 = null;
        }
        currentGun.GetComponent<GunReset>().hasBeenDropped();
        //currentGun.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
        
        currentGun = null;
    }

    public void updateKills()//make this specific to current player
    {
        kills++;
        killText.text = "Kills: " + kills;
    }

    public void updateDeaths()//make this specific to current player
    {
        deaths++;
        deathText.text = "Deaths: " + deaths;
    }

    public bool getAiming()
    {
        return is_aiming;
    }

    public void resetPlayerSpeed()
    {
        playerSpeed = tempSpeed;
    }

    public float getCurrentHealth()
    {
        return currentHealth;
    }

    public void resetHealth()
    {
        currentHealth = healthMax;
        healthBar.value = currentHealth;
    }

    public void checkIfAlive()//checks if player is alive. If currentHealth is zero or below this will run
    {
        if (currentHealth <= 0 && isAlive)
        {
            isAlive = false;
            Vector3 temp = gameObject.transform.position;
            controller.enabled = false;

            gameObject.transform.position = deathZone.position;
            gun1 = null;
            gun1 = pistol;
            gun2 = null;
            gunClone.GetComponent<GunReset>().reset();
            gunClone = pistol;
            gunClone2.GetComponent<GunReset>().reset();
            gunClone2 = null;
            
            currentGun.GetComponent<GunReset>().reset();
            currentGun = null;
            currentGun = gun1;
            currentGun.layer = 7;
            currentGun.transform.position = gunPosition.transform.position;
            currentGun.transform.parent = gunPosition.transform;
            currentGun.GetComponent<Gun>().equip();
            currentGun.GetComponent<GunReset>().hasBeenPickedUp();
            currentGun.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
            currentGun.GetComponent<Rigidbody>().isKinematic = true;
            currentGun.GetComponent<Gun>().player = gameObject.GetComponent<PlayerController>();
            currentGun.GetComponent<Recoil>().player = gameObject.GetComponent<PlayerController>();
            currentGun.layer = layerMaskForGun;
            equipGunsAmount = 1;
        }
    }

    public void makeAlive()
    {
        isAlive = true;
    }

    public bool getAliveStatus()
    {
        return isAlive;
    }

    public void addKill()
    {
        kills++;
    }

    public int getKills()
    {
        return kills;
    }

    public void setCameraRecoil()
    {
        camera_recoil.GetComponent<Recoil>().recoilX = currentGun.GetComponent<Recoil>().recoilX;
        camera_recoil.GetComponent<Recoil>().recoilY = currentGun.GetComponent<Recoil>().recoilY;
        camera_recoil.GetComponent<Recoil>().recoilZ = currentGun.GetComponent<Recoil>().recoilZ;

        camera_recoil.GetComponent<Recoil>().aimRecoilX = currentGun.GetComponent<Recoil>().aimRecoilX;
        camera_recoil.GetComponent<Recoil>().aimRecoilY = currentGun.GetComponent<Recoil>().aimRecoilY;
        camera_recoil.GetComponent<Recoil>().aimRecoilZ = currentGun.GetComponent<Recoil>().aimRecoilZ;

        camera_recoil.GetComponent<Recoil>().setTargetRotation(currentGun.GetComponent<Recoil>().getTargetRotation());
        camera_recoil.GetComponent<Recoil>().setCurrentRotation(currentGun.GetComponent<Recoil>().getCurrentRotation());

        camera_recoil.GetComponent<Recoil>().snappiness = currentGun.GetComponent<Recoil>().snappiness;
        camera_recoil.GetComponent<Recoil>().returnSpeed = currentGun.GetComponent<Recoil>().returnSpeed;
    }

    public GameObject clone1()
    {
        return gunClone;
    }

    public GameObject clone2()
    {
        return gunClone2;
    }

    public GameObject getPistol()
    {
        return pistol;
    }
    //void OnGUI()
    //{
    //    GUI.Label(new Rect(0, 25, 40, 60), "Speed");
    //    m_MySliderValue = GUI.HorizontalSlider(new Rect(45, 25, 200, 60), m_MySliderValue, 0.0F, 1.0F);
    //    animate.speed = m_MySliderValue;
    //}
}