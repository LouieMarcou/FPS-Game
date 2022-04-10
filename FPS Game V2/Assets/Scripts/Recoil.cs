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
        //Debug.Log(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        isAiming = player.GetComponent<PlayerController>().getAiming();
        //isAiming = player.getAiming();


        targetRotation = Vector3.Lerp(targetRotation, Vector3.zero, returnSpeed * Time.deltaTime);
        currentRotation = Vector3.Slerp(currentRotation, targetRotation, snappiness * Time.fixedDeltaTime);
        transform.localRotation = Quaternion.Euler(currentRotation);
    }

    public void RecoilFire()
    {
        if(isAiming)
        {
            targetRotation += new Vector3(aimRecoilX, Random.Range(-aimRecoilY, aimRecoilY), Random.Range(-aimRecoilZ, aimRecoilZ));
        }
        else
        {
            targetRotation += new Vector3(recoilX, Random.Range(-recoilY, recoilY), Random.Range(-recoilZ, recoilZ));
        }
        
    }


    public Vector3 getTargetRotation()
    {
        return targetRotation;
    }
}
