using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ObjectPool : MonoBehaviour
{
    public GameObject objectToPool;
    public static ObjectPool SharedInstance;
    public List<GameObject> thePool = new List<GameObject>();
    public int amountToPool;
    private float shotSpeed = 100f;

    void Awake()
    {
        SharedInstance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        GameObject tmp;
        for (int i = 0; i < amountToPool; i++)
        {
            tmp = Instantiate(objectToPool, transform, false);
            tmp.SetActive(false);
            thePool.Add(tmp);

        }
    }
    

    // Update is called once per frame
    void Update()
    {
        
    }

    public void release(Vector3 direction, Transform shootLocation)
    {
        Debug.Log("went into release method");
        GameObject bullet = ObjectPool.SharedInstance.GetPooledObject();
        Debug.Log(bullet);
        if (bullet != null)
        {
            Debug.Log("bullet is not null");
            //bullet is not actually changing position
            //bullet is not actually active. A parent or grandparent must be inactive somehow.

            bullet.transform.position = shootLocation.transform.position;
            bullet.transform.rotation = shootLocation.transform.rotation;

            bullet.transform.forward = direction.normalized;
            Debug.Log(bullet.transform.position);
            bullet.SetActive(true);
            if(bullet.activeSelf==true)
            {
                Debug.Log("bullet is active");
                Debug.Log("bullet was released");
            }
            bullet.GetComponent<Rigidbody>().AddForce(direction.normalized * shotSpeed, ForceMode.Impulse);
            //Debug.Log(bullet.transform.position);
            
        }
    }

    public void clear()
    {
        for(int i=0;i<amountToPool;i++)
        {
            thePool[i].SetActive(false);
        }
        Debug.Log("Reloaded");
    }

    public GameObject GetPooledObject()
    {
        for(int i=0;i<amountToPool;i++)
        {
            if(!thePool[i].activeInHierarchy)
            {
                //Debug.Log(thePool[i].transform.position);
                return thePool[i];
            }
        }
        return null;
    }

    public void destroy(GameObject gameobject)
    {
        gameobject.SetActive(false);
    }
}
