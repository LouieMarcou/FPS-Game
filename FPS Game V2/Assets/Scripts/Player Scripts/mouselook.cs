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
        transform.Rotate(Vector3.up, mouseX * Time.deltaTime);
        
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -xClamp, xClamp);

        regularLook();
    }

    public void regularLook()
    {
        targetRotation = transform.eulerAngles;
        targetRotation.x = xRotation;
        playerCamera.eulerAngles = targetRotation;
    }

    public void didFire()
    {
        hasFired = true;
    }

    public void assignRecoilScript(Recoil re)
    {
        recoil_script = re;
    }
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
