using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullets : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
    
    }
    
    void OnCollisionEnter(Collision collision)
    {
        //Debug.Log("collision", gameObject);
        //ObjectPool.SharedInstance.destroy(gameObject);
        //NewObjectPool.SharedInstance.destroy(gameObject);
        SharedPool.SharedInstance.destroy(gameObject);
    }
}
