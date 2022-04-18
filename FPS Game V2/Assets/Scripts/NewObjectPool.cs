using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class NewObjectPool : MonoBehaviour
{
    public static NewObjectPool SharedInstance;
    //public List<GameObject> pooledObjects;
    public GameObject[] pool;
    public GameObject[] secretPool;
    //public GameObject objectToPool;
    //public int amountToPool;

    void Awake()
    {
        SharedInstance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        //Debug.Log(pooledObjects.Count);
        //pooledObjects = new List<GameObject>();
        secretPool = new GameObject[pool.Length];
        GameObject tmp;
        for (int i = 0; i < pool.Length; i++)
        {
            tmp = Instantiate(pool[i],gameObject.transform);
            //Debug.Log(tmp);
            secretPool[i] = tmp;
            tmp.SetActive(false);
            //pooledObjects.Add(tmp);
            
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public GameObject releaseRandom()
    {
        int random = Random.Range(0, 5);
        GameObject flash = NewObjectPool.SharedInstance.secretPool[random];
        return flash;
    }

    public GameObject GetPooledObject()
    {
        for (int i = 0; i < pool.Length; i++)
        {
            if (!pool[i].activeInHierarchy)
            {
                return pool[i];
            }
        }
        return null;
    }

    public void destroy(GameObject gameobject)
    {
        gameobject.SetActive(false);
    }
}
