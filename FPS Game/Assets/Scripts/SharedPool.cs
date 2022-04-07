using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SharedPool : MonoBehaviour
{
    public static SharedPool SharedInstance;
    public List<GameObject> pooledObjects;
    public GameObject objectToPool;
    public int amountToPool;
    private RaycastHit hit;

    void Awake()
    {
        SharedInstance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        pooledObjects = new List<GameObject>();
        GameObject tmp;
        if (objectToPool.name == "bulletHole(Clone)")
        {
            tmp = Instantiate(objectToPool, hit.point, Quaternion.LookRotation(hit.normal));
            tmp.SetActive(false);
            pooledObjects.Add(tmp);
            //Debug.Log("yes");
        }
        else
        {
            for (int i = 0; i < amountToPool; i++)
            {
                tmp = Instantiate(objectToPool);
                tmp.SetActive(false);
                pooledObjects.Add(tmp);
            
            }
        }
        
    }

    // Update is called once per frame
    void Update()//use update to check if a bullet hole is active and deactivate it???
    {
        
    }

    public void setRaycastHit(RaycastHit tmp)
    {
        hit = tmp;
        //Debug.Log(hit.transform.name);
    }

    public void createBulletHole()
    {
        GameObject bulletHole = GetPooledObject();
        bulletHole.transform.position = hit.point;
        bulletHole.transform.rotation = Quaternion.LookRotation(hit.normal);
        bulletHole.SetActive(true);
    }


    public GameObject GetPooledObject()
    {
        for (int i = 0; i < amountToPool; i++)
        {
            if (!pooledObjects[i].activeInHierarchy)
            {
                return pooledObjects[i];
            }
        }
        return null;
    }

    public void destroy(GameObject gameobject)
    {
        gameobject.SetActive(false);
    }
}
