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
    public Transform gunPosition;
    public Transform holsterPosition;
    private Transform camera_recoil;
    public Transform deathZone;
    public Animator animate;

    private CharacterController controller;

    private Vector3 playerVelocity;

    private Vector2 movementInput = Vector2.zero;
    private Vector2 pauseMovementInput = Vector2.zero;
    private Vector2 mouseInput;

    public GameObject gun1;
    public GameObject gun2;
    private GameObject gunClone;
    private GameObject gunClone2;
    private GameObject currentGun;
    private GameObject tempGun;


    [SerializeField] public Camera cam;

    private WaitForSeconds regentick = new WaitForSeconds(0.1f);
    private WaitForSeconds staminaRegenWait = new WaitForSeconds(2f);
    private WaitForSeconds healthRegenWait = new WaitForSeconds(7f);
    //private WaitForSeconds holsterWait = new WaitForSeconds(1f);

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
    //private float targetFieldOfView = 60f;

    private int kills = 0;
    private int deaths = 0;

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

    float m_MySliderValue = 1.0f;


    Ray ray;
    RaycastHit hit;
    #endregion

    private void Start()
    {
        //transform.localPosition = spawnPoint.transform.position;//makes player spawn at spawn point. Currently not working
        controller = gameObject.GetComponent<CharacterController>();

        tempSpeed = playerSpeed;
        gunClone = Instantiate(gun1, gunPosition.transform, false);
        //gunClone.layer = 7;
        gunClone.transform.position = gunPosition.transform.position;
        gunClone.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        gunClone.GetComponent<Gun>().animator = animate;
        gunClone.GetComponent<Gun>().getCamera(cam);
        gunClone.GetComponent<Gun>().setPosition(gunPosition);
        gunClone.GetComponent<Recoil>().player = gameObject.GetComponent<PlayerController>();

        gunClone2 = Instantiate(gun2, gunPosition.transform, false);
        //gunClone2.layer = 7;
        gunClone2.transform.position = gunPosition.transform.position;
        gunClone2.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        gunClone2.GetComponent<Gun>().animator = animate;
        gunClone2.GetComponent<Gun>().getCamera(cam);
        gunClone2.GetComponent<Gun>().setPosition(gunPosition);
        gunClone2.GetComponent<Recoil>().player = gameObject.GetComponent<PlayerController>();
        gunClone2.SetActive(false);

        currentGun = gunClone;
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

        camera_recoil = transform.Find("CameraRotation/CameraRecoil");
        kills = 0;
        deaths = 0;
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
        if (gameObject.GetComponent<PauseMenu>().getGameIsPaused())
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
        if (gameObject.GetComponent<PauseMenu>().getGameIsPaused())
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
                playerSpeed -= 5;
                currentGun.GetComponent<Gun>().Load();
                cancelSprint();
            }
        }
    }

    public void OnSwap(InputAction.CallbackContext context)//deactivates current gun and activates the other
    {
        if (gameObject.GetComponent<PauseMenu>().getGameIsPaused())
            return;
        swapCheck = context.ReadValueAsButton();
        swapCheck = context.action.triggered;
        if (swapCheck)
        {
            if (is_aiming)
            {
                animate.SetBool("Aiming", false);
                is_aiming = false;
            }
            StopFiring();
            if (gunClone.activeInHierarchy)
            {
                //startSwapping(gunClone2);//ammo text does not update properly in time with the changing of guns
                //gunClone.GetComponent<Gun>().moveToHolster(gunPosition,holsterPosition);
                //startHolster(gunPosition, holsterPosition);
                startHolster(gunClone);
                startDraw(gunClone2);
                currentGun = gunClone2;
            }
            else if (gunClone2.activeInHierarchy)
            {
                //startSwapping(gunClone);
                startHolster(gunClone2);
                startDraw(gunClone);
                currentGun = gunClone;

            }
        }
    }

    public void OnPickup(InputAction.CallbackContext context)//picks up new gun you are looking at and swaps the current gun with it
    {
        //if gun is attached to a player then return
        if (gameObject.GetComponent<PauseMenu>().getGameIsPaused())
            return;
        pickupCheck = context.ReadValueAsButton();
        pickupCheck = context.action.triggered;
        //Had a weird instance where I dropped a gun and was not able to pick it back up
        if (canPickup)
        {
            if (pickupCheck)//can't pick up new guns
            {
                dropGun();
                pickupGun();
            }
        }
    }

    public void OnAim(InputAction.CallbackContext context)
    {
        if (gameObject.GetComponent<PauseMenu>().getGameIsPaused())
            return;
        is_aiming = context.ReadValueAsButton();
        is_aiming = context.action.triggered;
        if (is_sprinting)
            cancelSprint();
        if (context.performed)
        {
            playerSpeed -= 5;
            animate.SetBool("Aiming", true);
            animate.GetCurrentAnimatorClipInfo(0);
            //if (currentGun.GetComponent<Gun>().scope && animate.GetBool("Aiming"))//goes to scope just on click and does not leave
            //{
            //    animate.SetBool("Scoped", true);
            //    StartCoroutine(gameObject.GetComponent<Scope>().OnScoped());
            //}
        }
        if (context.canceled)
        {
            resetPlayerSpeed();
            //if (currentGun.GetComponent<Gun>().scope)
            //{
            //    animate.SetBool("Scoped", false);
            //    gameObject.GetComponent<Scope>().OnUnscoped();
            //}
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
            //look.SetActive(false);
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
        if (is_joining)
        {
            Debug.Log("Joined?");
        }

    }

    void Update()
    {
        ammo.text = currentGun.GetComponent<Gun>().updateAmmoText();
        currentGun.GetComponent<Gun>().animator = animate;
        camera_recoil.rotation = currentGun.transform.rotation;
        if (movementInput.x == 0 && movementInput.y == 0)
        {
            cancelSprint();
        }
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
        //updateScore();
    }

    void OnCollisionEnter(Collision collision)//if you collied with something
    {

        if (collision.gameObject.tag == "AmmoPickup")//if the collied object is ammo
        {
            //Debug.Log(collision.collider.name);
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
            currentGun.GetComponent<Gun>().maxAmmo = num;
            if (currentGun.GetComponent<Gun>().getCurrentAmmo() == 0)
            {
                currentGun.GetComponent<Gun>().Load();
            }
        }
        if (collision.gameObject.tag == "KillTag")
        {
            Debug.Log("IN ZONE");
            currentHealth = 0;
        }
    }

    public void cancelSprint()
    {
        is_sprinting = false;
        updateSprint();
    }

    public void updateSprint()
    {
        if (is_sprinting)
        {
            animate.SetBool("Sprinting", is_sprinting);
            sprintSpeed = 10;
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
            yield return regentick;
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
        //holsterCoroutine = StartCoroutine(holstering(secondGun));

    }

    //void startHolster(Transform gunPosition, Transform holsterPosition)//Starts holster coroutine
    //{
    //    holsterCoroutine = StartCoroutine(currentGun.GetComponent<Gun>().moveToHolster(gunPosition, holsterPosition));
    //    //holsterCoroutine = StartCoroutine(holstering(secondGun));

    //}

    IEnumerator holstering(GameObject secondGun)
    {
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
        //drawCoroutine = StartCoroutine(drawing(secondGun));

    }

    IEnumerator drawing(GameObject secondGun)
    {
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

    /*void startSwapping(GameObject secondGun)
    {
        weaponSwapCoroutine = StartCoroutine(currentGun.GetComponent<Gun>().Swapping(secondGun));

    }*/

    void checkIfCanPickup()//Will check if a gun is in distance to pick up
    {
        int layerMask = 1 << 10;
        RaycastHit hit;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, pickupDistance, layerMask))
        {
            if (hit.transform.tag == "Gun" && hit.transform.parent == null)
            {
                Debug.Log("Can grab it");
                //display controll to pick up
                canPickup = true;
                tempGun = hit.transform.gameObject;
            }
            else
            {
                canPickup = false;
            }
        }
    }

    void pickupGun()//Will pick up gun and set it equal to current gun
    {
        currentGun = tempGun;
        currentGun.transform.position = gunPosition.position;
        currentGun.transform.parent = gunPosition;
        currentGun.GetComponent<Gun>().player = gameObject.GetComponent<PlayerController>();
        currentGun.GetComponent<Gun>().ammo = gameObject.GetComponent<PlayerController>().ammo;
        currentGun.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
        currentGun.GetComponent<Rigidbody>().isKinematic = true;
        //currentGun.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        currentGun.GetComponent<Recoil>().player = gameObject.GetComponent<PlayerController>();
        //currentGun.layer = 7;
    }

    void dropGun()//Will current gun if a gun in front of you is detected
    {
        currentGun.transform.parent = null;
        currentGun.GetComponent<Rigidbody>().isKinematic = false;
        currentGun.GetComponent<Rigidbody>().useGravity = true;
        currentGun.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        currentGun.layer = 10;
        currentGun = null;
    }

    public void updateKills()//make this specific to current player
    {
        Debug.Log(kills);
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
        Debug.Log("Is Alive");
        //Debug.Log("Health is " + currentHealth);
    }

    public void checkIfAlive()
    {
        if (currentHealth <= 0)
        {
            //Debug.Log("Is Dead");
            isAlive = false;
            controller.enabled = false;
            Vector3 temp = gameObject.transform.position;
            //gameObject.GetComponent<DropAmmo>().getAmmoPickup().transform.position = temp;
            gameObject.transform.position = deathZone.position;


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

    public void updateScore()
    {
        killText.text = "Kills: " + kills;
    }

    public void addKill()
    {
        kills++;
    }

    //void OnGUI()
    //{
    //    GUI.Label(new Rect(0, 25, 40, 60), "Speed");
    //    m_MySliderValue = GUI.HorizontalSlider(new Rect(45, 25, 200, 60), m_MySliderValue, 0.0F, 1.0F);
    //    animate.speed = m_MySliderValue;
    //}
}