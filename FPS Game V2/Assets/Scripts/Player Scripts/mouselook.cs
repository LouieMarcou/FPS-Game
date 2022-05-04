using UnityEngine;
using UnityEngine.InputSystem;

public class mouselook : MonoBehaviour
{
    [SerializeField] float sensitivityX = 8f;
    [SerializeField] float sensitivityY = 0.5f;
    float mouseX, mouseY;

    [SerializeField] Transform playerCamera;
    [SerializeField] float xClamp = 85f;
    float xRotation = 0f;
    public Vector3 targetRotation;
    public Recoil recoil_script;
    bool hasFired = false;
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;        
    }
    private void Update()
    {
        if (recoil_script != null)
        {
            recoil_script = gameObject.GetComponent<PlayerController>().currentGun.GetComponent<Recoil>();
            Debug.Log(recoil_script.getTargetRotation());
        }
        transform.Rotate(Vector3.up, mouseX * Time.deltaTime);
        
        xRotation -= mouseY;
        //Debug.Log(xRotation);
        xRotation = Mathf.Clamp(xRotation, -xClamp, xClamp);
        //Debug.Log(xRotation);
        if (hasFired)
        {
            targetRotation = recoil_script.targetRotation;
            targetRotation.x = xRotation;
            playerCamera.eulerAngles = targetRotation;
            hasFired = false;
        }
        else
        {
            targetRotation = transform.eulerAngles;
            //Debug.Log(targetRotation.x);
            targetRotation.x = xRotation;
            //Debug.Log(targetRotation.x);
            playerCamera.eulerAngles = targetRotation;
        }
    }

    public void didFire()
    {
        hasFired = true;
    }

    //public void setRotation(Vector3 rot)
    //{

    //}
    public void ReceiveInput (Vector2 mouseInput)
    {
        mouseX = mouseInput.x * sensitivityX;
        mouseY = mouseInput.y * sensitivityY;
        //Debug.Log(mouseY);
    }

    public void OnDisable()
    {
        Cursor.lockState = CursorLockMode.None;
    }
}
