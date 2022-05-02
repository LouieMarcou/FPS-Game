using UnityEngine;

public class Recoil : MonoBehaviour
{
    //recoil script should be attached to guns for different recoils

    //Scripts
    [SerializeField] public PlayerController player;

    //bool
    private bool isAiming = false;

    //Rotations
    private Vector3 currentRotation;
    private Vector3 targetRotation;

    //Hipfire Recoil
    [SerializeField] public float recoilX;
    [SerializeField] public float recoilY;
    [SerializeField] public float recoilZ;

    //ADS Recoil
    [SerializeField] public float aimRecoilX;
    [SerializeField] public float aimRecoilY;
    [SerializeField] public float aimRecoilZ;

    //Settings
    [SerializeField] public float snappiness;
    [SerializeField] public float returnSpeed;
    // Start is called before the first frame update
    void Start()
    {
        //if(gameObject.tag != "Gun")
        //{
        //    Debug.Log(gameObject + " is not a gun");
        //}
    }

    // Update is called once per frame
    void Update()
    {
        if (player != null)
        {
            isAiming = player.GetComponent<PlayerController>().getAiming();
        }
        //Debug.Log(recoilX);
        //Debug.Log(gameObject,gameObject);

        targetRotation = Vector3.Lerp(targetRotation, Vector3.zero, returnSpeed * Time.deltaTime);
        currentRotation = Vector3.Slerp(currentRotation, targetRotation, snappiness * Time.fixedDeltaTime);
        transform.localRotation = Quaternion.Euler(currentRotation);
        //Debug.Log(targetRotation + "  " + currentRotation);
    }

    public void RecoilFire()
    {
        if (isAiming)
        {
            targetRotation += new Vector3(aimRecoilX, Random.Range(-aimRecoilY, aimRecoilY), Random.Range(-aimRecoilZ, aimRecoilZ));
        }
        else
        {
            targetRotation += new Vector3(recoilX, Random.Range(-recoilY, recoilY), Random.Range(-recoilZ, recoilZ));
        }

    }

    public void setPlayer(PlayerController newPlayer)
    {
        player = newPlayer;
    }

    public void setTargetRotation(Vector3 target)
    {
        targetRotation = target;
    }

    public Vector3 getTargetRotation()
    {
        return targetRotation;
    }

    public void setCurrentRotation(Vector3 target)
    {
        currentRotation = target;
    }

    public Vector3 getCurrentRotation()
    {
        return currentRotation;
    }

    public void removePlayerScript()
    {
        player = null;
    }
}
