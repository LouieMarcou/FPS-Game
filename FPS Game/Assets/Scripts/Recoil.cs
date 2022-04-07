using UnityEngine;

public class Recoil : MonoBehaviour
{
    //recoil script should be attached to guns for different recoils
    //Rotations
    private Vector3 currentRotation;
    private Vector3 targetRotation;

    //Hipfire Recoil
    [SerializeField] private float recoilX;
    [SerializeField] private float recoilY;
    [SerializeField] private float recoilZ;

    //Settings
    [SerializeField] private float snappiness;
    [SerializeField] private float returnSpeed;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(targetRotation);
        targetRotation = Vector3.Lerp(targetRotation, Vector3.zero, returnSpeed * Time.deltaTime);
        //Debug.Log(targetRotation);

        //Debug.Log(currentRotation);
        currentRotation = Vector3.Slerp(currentRotation, targetRotation, snappiness * Time.fixedDeltaTime);
        //Debug.Log(currentRotation);

        //Debug.Log(Quaternion.Euler(currentRotation));

        //Debug.Log(transform.localRotation);
        transform.localRotation = Quaternion.Euler(currentRotation);
        //Debug.Log(transform.localRotation);
    }

    public void RecoilFire()
    {
        //Debug.Log(targetRotation);
        targetRotation += new Vector3(recoilX, Random.Range(-recoilY, recoilY), Random.Range(-recoilZ, recoilZ));
        //Debug.Log(targetRotation);
    }
}
