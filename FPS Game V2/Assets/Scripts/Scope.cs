using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scope : MonoBehaviour
{
    public GameObject sniperScopeOverlay;
    public GameObject weaponCamera;
    public Camera mainCamera;

    public float scopedFOV = 0f;
    private float normalFOV = 60f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnUnscoped()
    {
        sniperScopeOverlay.SetActive(false);
        weaponCamera.SetActive(true);
        mainCamera.fieldOfView = normalFOV;
    }

    public IEnumerator OnScoped()
    {
        Debug.Log(sniperScopeOverlay);
        yield return new WaitForSeconds(0.25f);

        sniperScopeOverlay.SetActive(true);
        weaponCamera.SetActive(false);
        mainCamera.fieldOfView = scopedFOV;

    }
}
