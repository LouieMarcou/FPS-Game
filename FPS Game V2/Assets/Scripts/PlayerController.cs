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
    public Slider staminaBar;
    public Slider healthBar;

    public Text ammo;

    public Transform playerBody;
    public Transform gunPosition;
    private Transform camera_recoil;

    public Animator animate;

    private CharacterController controller;

    private Vector3 playerVelocity;

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

    private Coroutine staminaRegen;
    private Coroutine healthRegen;
    private Coroutine rapidFireCoroutine;
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
    private float targetFieldOfView = 60f;

    private Vector2 movementInput = Vector2.zero;
    private Vector2 mouseInput;

    private bool jumped = false;
    private bool fire = false;
    private bool reload = false;
    private bool is_sprinting = false;
    private bool is_aiming = false;
    private bool swapCheck = false;
    private bool pickupCheck = false;
    private bool canPickup = false;
    private bool is_paused = false;
    private bool groundedPlayer;

    
    #endregion
    private void Start()
    {
        controller = gameObject.GetComponent<CharacterController>();

        tempSpeed = playerSpeed;
        gunClone = Instantiate(gun1, gunPosition.transform, false);
        gunClone.transform.position = gunPosition.transform.position;
        gunClone.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        gunClone.GetComponent<Gun>().animator = animate;
        gunClone.GetComponent<Gun>().getCamera(cam);
        gunClone.GetComponent<Gun>().setPosition(gunPosition);
        gunClone.GetComponent<Recoil>().player = gameObject.GetComponent<PlayerController>();

        gunClone2 = Instantiate(gun2, gunPosition.transform, false);
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

        currentStamina = staminaMax;
        staminaBar.maxValue = staminaMax;
        staminaBar.value = staminaMax;

        currentHealth = healthMax;
        healthBar.maxValue = healthMax;
        healthBar.value = healthMax;

        camera_recoil = transform.Find("CameraRotation/CameraRecoil");

    }

    public void OnMove(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>();
        updateStamina();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        jumped = context.ReadValueAsButton();
        jumped = context.action.triggered;
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        is_sprinting = context.ReadValueAsButton();
        is_sprinting = context.action.triggered;
        if (context.performed)
        {
            if (is_aiming || fire || reload || currentGun.GetComponent<Gun>().reloadingCheck())
                return;
            //if (fire)
            //    return;
            //if (reload || currentGun.GetComponent<Gun>().reloadingCheck())
            //    return;
            
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
        fire = context.ReadValueAsButton();
        fire = context.action.triggered;
        if (is_sprinting)
            cancelSprint();
        if (currentGun.GetComponent<Gun>().rapidFire)
        {
            currentGun.GetComponent<Gun>().getCamera(cam);
            if (context.performed)//problem when player switches weapons while firing???
            {
                StartFiring();
            }
            if (context.canceled)
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
        reload = context.ReadValueAsButton();
        reload = context.action.triggered;
        if (currentGun.GetComponent<Gun>().maxAmmo <= 0)
            return;
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
        swapCheck = context.ReadValueAsButton();
        swapCheck = context.action.triggered;
        if (swapCheck)
        {
            StopFiring();
            if (gunClone.activeInHierarchy)
            {
                //startSwapping(gunClone2);//ammo text does not update properly in time with the changing of guns
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
        pickupCheck = context.ReadValueAsButton();
        pickupCheck = context.action.triggered;
        //Had a weird instance where I dropped a gun and was not able to pick it back up
        if (canPickup)
        {
            if (pickupCheck)
            {
                dropGun();
                pickupGun();
            }
        }
    }

    public void OnAim(InputAction.CallbackContext context)
    {
        is_aiming = context.ReadValueAsButton();
        is_aiming = context.action.triggered;
        if (is_sprinting)
            cancelSprint();
        if (context.performed)
        {
            playerSpeed -= 5;

            animate.SetBool("Aiming", true);
            if (currentGun.GetComponent<Gun>().scope)
            {
                animate.SetBool("Scoped", true);
                StartCoroutine(gameObject.GetComponent<Scope>().OnScoped());

            }
        }
        if (context.canceled)
        {
            resetPlayerSpeed();
            if (currentGun.GetComponent<Gun>().scope)
            {
                animate.SetBool("Scoped", false);
                gameObject.GetComponent<Scope>().OnUnscoped();
            }
            animate.SetBool("Aiming", false);
            
            is_aiming = false;
        }
    }

    public void MouseX(InputAction.CallbackContext context)
    {
        mouseInput.x = context.ReadValue<float>();
    }

    public void MouseY(InputAction.CallbackContext context)
    {
        mouseInput.y = context.ReadValue<float>();
    }

    public void OnPause(InputAction.CallbackContext context)
    {
        is_paused = context.ReadValueAsButton();
        is_paused = context.action.triggered;
        if(is_paused)
        {
            gameObject.GetComponent<PauseMenu>().checkIfPaused();
        }
    }
    void Update()
    {
        ammo.text = currentGun.GetComponent<Gun>().updateAmmoText();
        currentGun.GetComponent<Gun>().animator = animate;
        camera_recoil.localRotation = currentGun.transform.localRotation;
        updateHealth();
        updateStamina();
        if (currentGun.GetComponent<Gun>().reloadingCheck() == false)
        {
            resetPlayerSpeed();
        }
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFieldOfView, Time.deltaTime * 10f);
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
        //Debug.Log(currentStamina);

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
        if (currentHealth < healthMax)
        {
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

    void StopFiring()//Stops rapid fire coroutine
    {
        if (rapidFireCoroutine != null)
        {
            StopCoroutine(rapidFireCoroutine);
        }
    }

    void startHolster(GameObject secondGun)//Starts holster coroutine
    {
        holsterCoroutine = StartCoroutine(currentGun.GetComponent<Gun>().Holster(secondGun));

    }

    void startDraw(GameObject secondGun)//Stars draw coroutine
    {
        drawCoroutine = StartCoroutine(currentGun.GetComponent<Gun>().Draw(secondGun));

    }

    /*void startSwapping(GameObject secondGun)
    {
        weaponSwapCoroutine = StartCoroutine(currentGun.GetComponent<Gun>().Swapping(secondGun));

    }*/

    void checkIfCanPickup()//Will check if a gun is in distance to pick up
    {
        RaycastHit hit;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, pickupDistance))
        {
            if (hit.transform.tag == "Gun")
            {
                //Debug.Log("Can grab it");
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
        currentGun.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        currentGun.GetComponent<Recoil>().player = gameObject.GetComponent<PlayerController>();
        //currentGun.layer = 2;
    }

    void dropGun()//Will current gun if a gun in front of you is detected
    {
        currentGun.transform.parent = null;
        currentGun.GetComponent<Rigidbody>().isKinematic = false;
        currentGun.GetComponent<Rigidbody>().useGravity = true;
        currentGun.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        //currentGun.layer = 7;
        currentGun = null;
    }

    public bool getAiming()
    {
        return is_aiming;
    }

    public void resetPlayerSpeed()
    {
        playerSpeed = tempSpeed;
    }
}