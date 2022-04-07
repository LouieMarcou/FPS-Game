using System.Collections;
using UnityEngine;

public class Deactivate : MonoBehaviour
{
    public float timeToDeactivate = 0.8f;
    private Coroutine deactivateCoroutine;


    void Start()
    {
        
    }

    void Update()
    {
        if(gameObject.activeSelf==true)
        {
            deactivateCoroutine = StartCoroutine(turnOff(gameObject));
        }
    }

    IEnumerator turnOff(GameObject obj)
    {
        //Debug.Log("Went into method");
        yield return new WaitForSeconds(timeToDeactivate);
        obj.SetActive(false);
    }
}
